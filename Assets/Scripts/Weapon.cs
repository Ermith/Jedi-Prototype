using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float Damage = 50;
    public bool Active => _state != State.Idle;
    public Transform RecallPoint;
    public AudioClip SwordSwing;
    public ParticleSystem Sparks;

    enum State { Throwing, Recalling, Idle };

    private State _state = State.Idle;
    private Vector3 _throwVelocity;
    private float _rotationSpeed;
    private float _recallTime;
    private float _recallDuration;
    private Vector3 _recallFrom;
    private Vector3 _recallTo;
    private Quaternion _originalRotation;
    private bool _attacking;
    private AudioManager _audioManager;
    private LightsaberTrail _trail;
    private Transform _originalParent;
    private Action _onRecall = null;
    private AudioSource _spinAudio;

    public void Attack(float damage)
    {
        _attacking = true;
        Damage = damage;
        _trail.EnableTrail = true;
        
        //float mult = 2;
        //float time = 0.09f;

        //TweenBell((float val) => _trail.Width = val, _trail.Width, _trail.Width * mult, time);
        //TweenBell(
        //    (float val) =>
        //    {
        //        Vector3 sc = transform.localScale;
        //        sc.y = val;
        //        transform.localScale = sc;
        //    },
        //    transform.localScale.y, transform.localScale.y * mult, time);
    }

    private void TweenBell(DOSetter<float> setter, float defaultVal, float amplitude, float time)
    {
        DOTween.To(setter, defaultVal, amplitude, time / 2).OnComplete(
            () => DOTween.To(setter, amplitude, defaultVal, time / 2));
    }


    public void StopAttack()
    {
        _attacking = false;
        Damage = 0;
        _trail.EnableTrail = false;
    }

    public void SwordSwingAudio()
    {
        _audioManager.Play("Sword Swing", position: transform.position);
    }
    
    public void SwordSpinAudioLoopPlay()
    {
        if (_spinAudio != null)
            SwordSpinAudioLoopStop();

        _spinAudio = _audioManager.PlayOnTarget("Sword Spin", gameObject, loop: true);
    }

    public void SwordSpinAudioLoopStop()
    {
        if (_spinAudio != null)
            _audioManager.FadeOut(_spinAudio, 0.1f);

        _spinAudio = null;
    }

    public void Throw(Vector3 velocity, float rotationSpeed, Vector3 from, Quaternion originalRotation)
    {
        if (_state != State.Idle)
            return;

        _state = State.Throwing;
        _throwVelocity = velocity;
        _rotationSpeed = rotationSpeed;
        _recallTo = from;
        _originalRotation = originalRotation;
        _originalParent = transform.parent;
        transform.eulerAngles = new Vector3(90, 0, 0);
        transform.SetParent(null);
    }

    public void Recall(float duration, Action onRecall = null)
    {
        if (_state != State.Throwing)
            return;

        _state = State.Recalling;
        _recallDuration = duration;
        _recallFrom = transform.position;
        _onRecall = onRecall;

        transform.rotation = Quaternion.LookRotation(Vector3.down, (transform.position - _recallTo).normalized);
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _trail = GetComponentInChildren<LightsaberTrail>();
        Sparks = Instantiate(Sparks);
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == State.Idle)
            return;

        if (_state == State.Throwing)
        {
            transform.position += _throwVelocity * Time.deltaTime;
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y += _rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rotation);
        }

        if (_state == State.Recalling)
        {
            _recallTime += Time.deltaTime;
            transform.position = Vector3.Lerp(_recallFrom, RecallPoint.position, _recallTime / _recallDuration);

            if (_recallTime >= _recallDuration)
            {
                transform.SetParent(_originalParent);
                transform.localRotation = _originalRotation;
                _state = State.Idle;
                _recallTime = 0;
                _onRecall?.Invoke();
                _onRecall = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_attacking)
            return;

       if (other.gameObject.layer != 6)
            return;

        Vector3 pointOfImpact = other.ClosestPoint(transform.position);

        other.gameObject.GetComponent<Damagable>().Damage(Damage);
        var particleSystem = Instantiate(Sparks);
        particleSystem.transform.position = pointOfImpact;
        particleSystem.transform.up = Vector3.Lerp(-transform.up, Vector3.up, 0.5f);
        particleSystem.Play();
        _audioManager.Play("Sword Hit", position: pointOfImpact);
    }
}
