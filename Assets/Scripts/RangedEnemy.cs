using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using DG.Tweening;

public class RangedEnemy : MonoBehaviour
{
    public GameObject _projectile;
    public GameObject _muzzle;
    public CharacterController _target;
    public float ReloadTime = 2f;
    public float ChargeTime = 500f;
    public float ProjectileSpeed = 10f;
    public float Range = 1000;
    public float TurnRate = 0.8f;
    public float Damage = 10f;

    private Color _projectileColor;
    private Material _muzzleMaterial;
    private Stopwatch _stopWatch;
    private Animator _animator;
    private AudioManager _audioManager;
    private AudioSource _chargingAudio;
    private Damagable _damagable;
    private enum State { Reloading, Idle, Charging }
    private State _state = State.Idle;

    // Start is called before the first frame update
    void Start()
    {
        _projectileColor = _projectile.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        _animator = GetComponent<Animator>();
        _damagable = GetComponent<Damagable>();
        _audioManager = FindObjectOfType<AudioManager>();

        _muzzleMaterial = _muzzle.GetComponent<MeshRenderer>().material;
        _muzzleMaterial.SetColor("_EmissionColor", Color.black);
        _muzzleMaterial.color = Color.black;
        _target = FindObjectOfType<JediCharacter>().GetComponent<CharacterController>();
        _stopWatch = new Stopwatch();
        _state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        bool inRange = (_target.center + _target.transform.position - transform.position).magnitude <= Range;
        _animator.SetBool("IsActive", inRange);

        if (!inRange)
            return;

        _damagable.HealthBarActive = true;

        if (_state == State.Idle)
        {
            _stopWatch.Start();
            _state = State.Reloading;
            return;
        }

        if (_state == State.Reloading && _stopWatch.ElapsedMilliseconds >= ReloadTime * 1000)
        {
            _state = State.Charging;

            _chargingAudio = _audioManager.Play("Laser Hum", loop: true, position: transform.position);

            _chargingAudio.DOPitch(10, ChargeTime).SetEase(Ease.InCubic).OnComplete(() =>
            {
                _chargingAudio.Stop();
                _chargingAudio = null;
                _audioManager.Play("Laser Shot", position: transform.position);
            });

            DOTween.To(
                () => _muzzleMaterial.GetColor("_EmissionColor"),
                (Color color) => _muzzleMaterial.SetColor("_EmissionColor", color),
                _projectileColor,
                ChargeTime
                ).SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    _state = State.Idle;
                    Shoot();
                    _muzzleMaterial.SetColor("_EmissionColor", Color.black);
                    _muzzleMaterial.color = Color.black;
                });
        }

        Vector3 targetDir = (_target.center + _target.transform.position - transform.position).normalized;
        Vector3 forward = Vector3.Lerp(transform.forward, targetDir, TurnRate);
        transform.forward = forward;
    }

    private void Shoot()
    {
        var projectile = Instantiate(_projectile, transform).GetComponent<Projectile>();
        projectile.transform.position = _muzzle.transform.position;
        projectile.gameObject.SetActive(true);
        projectile.Velocity = transform.forward * ProjectileSpeed;
        projectile.TargetLayer = 7;
        projectile.Damage = Damage;
        projectile.from = gameObject;

        //projectile.transform.DOMove(position, ProjectileFlightTIme).OnComplete(() => Destroy(projectile));
    }

    private void OnDestroy()
    {
        if (_chargingAudio != null)
            _chargingAudio.Stop();
    }
}
