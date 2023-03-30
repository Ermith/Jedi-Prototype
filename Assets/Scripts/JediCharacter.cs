using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class JediCharacter : MonoBehaviour
{
    public float Speed = 3;
    public float RotationSpeed = 0.5f;
    public float JediPushStrength = 100;
    public float WeaponThrowSpeed = 10;
    public float WeaponThrowRotationSpeed = 1500;
    public float WeaponRecallDuration = 0.2f;
    public float DashStrength = 2f;

    [HideInInspector]
    public float CameraRotation;

    private CharacterController _controller;
    private Animator _animator;
    private Weapon _weapon;
    private AudioManager _audioManager;

    private bool _canMove = true;
    private Vector2 _movementInput;
    private Vector3 _center => _controller.center + transform.position;
    private Vector3 _dashVelocity = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        float correctHeight = _controller.center.y + _controller.skinWidth;
        // set the controller center vector:
        _controller.center = new Vector3(0, correctHeight, 0);

        _animator = GetComponent<Animator>();
        _weapon = GetComponentInChildren<Weapon>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void FixedUpdate()
    {
        if (_animator.GetBool("Dash") && _dashVelocity.magnitude > 0)
        {
            _controller.SimpleMove(_dashVelocity);
            return;
        }

        Vector3 input = _movementInput;
        float horizontal = input.x;
        float vertical = input.y;

        if (horizontal == 0 && vertical == 0)
        {
            _animator.SetBool("Running", false);
            return;
        }

        Vector3 fwd = Quaternion.Euler(0, CameraRotation, 0) * Vector3.forward;
        Vector3 right = -Vector3.Cross(fwd, Vector3.up);
        Vector3 finalDir = horizontal * right + vertical * fwd;

        transform.forward = Vector3.Lerp(transform.forward, finalDir, 0.5f);

        // Movement
        if (finalDir.magnitude > 1) finalDir.Normalize();

        if (_canMove)
        {
            _controller.SimpleMove(finalDir * Speed);
            _animator.SetBool("Running", true);
        }
    }

    #region Animation Events
    public void CanMove(int i)
    {
        _canMove = i != 0;
    }

    public void CanAttack(int i)
    {
        _animator.SetBool("CanAttack", i != 0);
    }

    public void SetRootMotion(int i)
    {
        _animator.applyRootMotion = (i != 0);
    }

    public void CanExit(int i)
    {
        if (i == 0) _animator.ResetTrigger("CanExit");
        else _animator.SetTrigger("CanExit");
    }

    public void Throw(float damage)
    {
        _weapon.Throw(WeaponThrowSpeed * transform.forward, WeaponThrowRotationSpeed, _weapon.transform.position, _weapon.transform.localRotation);
        _weapon.SwordSpinAudioLoopPlay();
        _weapon.Attack(damage);
        _animator.SetBool("CanAttack", false);
    }

    public void Recall()
    {
        _weapon.Recall(WeaponRecallDuration, () => { _animator.SetTrigger("Recalled"); _animator.SetBool("CanAttack", true); });
        _weapon.SwordSpinAudioLoopStop();
        _weapon.StopAttack();
    }

    public void JediForce(float forceDir)
    {
        Physics.Raycast(_center, transform.forward, out RaycastHit hitInfo, float.MaxValue);
        Debug.Log(hitInfo.collider);
        Debug.DrawRay(_center, transform.forward);

        if (hitInfo.transform == null) return;
        var pushable = hitInfo.transform.gameObject.GetComponent<JediInteractable>();
        pushable?.JediPush(transform.position, forceDir * JediPushStrength);

        if (forceDir > 0) _audioManager.Play("Jedi Push", position: pushable?.transform.position);
        else _audioManager.Play("Jedi Pull", position: pushable?.transform.position);
    }

    public void SetAttack(float damage)
    {
        if (damage > 0)
        {
            _weapon.Attack(damage);
            _weapon.SwordSwingAudio();

            return;
        }

        _weapon.StopAttack();
    }
    #endregion

    #region Input
    public void OnMovement(InputValue val)
    {
        _movementInput = val.Get<Vector2>();
    }

    private void OnAttack(InputValue val)
    {
        if (!_animator.GetBool("CanAttack"))
            return;

        _animator.SetTrigger("Attack");
        //_animator.ResetTrigger("CanExit");
        _animator.SetBool("Running", false);
        _canMove = false;

    }

    private void OnPush(InputValue val)
    {
        _animator.SetTrigger("Push");
    }

    private void OnPull(InputValue val)
    {
        _animator.SetTrigger("Pull");
    }

    private void OnThrow(InputValue val)
    {
        bool canAttack = _animator.GetBool("CanAttack");

        if (val.isPressed && canAttack)
        {
            _animator.SetTrigger("Throw");
        } else _animator.SetTrigger("Recall");
    }

    private void OnLook(InputValue val)
    {
        CameraRotation += val.Get<float>() * RotationSpeed; // * Time.deltaTime * 1000;
    }

    public void OnDash(InputValue val)
    {
        _canMove = false;
        _dashVelocity = transform.forward * DashStrength;
        _animator.SetBool("Dash", true);
        _audioManager.Play("Jedi Push");
        DOTween.To(() => _dashVelocity, (Vector3 vel) => _dashVelocity = vel, Vector3.zero, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _animator.SetBool("Dash", false);
                _canMove = true;
            });
    }

    #endregion
}
