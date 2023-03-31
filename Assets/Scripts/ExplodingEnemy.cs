using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    [SerializeField, Tooltip("The target they respond to.")]
    public Transform target;

    [SerializeField, Tooltip("Speed at which the enemy moves to the target.")]
    public float MoveSpeed = 1;

    [SerializeField, Tooltip("How close does target have to be for them to start exploding?")]
    public float ExplosionTriggerRange = 1.5f;

    [SerializeField, Tooltip("How big the explosion?")]
    public float ExplosionRadius = 5;

    [SerializeField, Tooltip("How much damage the explosion?")]
    public float ExplosionDamage = 100;

    [SerializeField, Tooltip("How close the target has to be for the enemy to start moving?")]
    public float AggretionRadius = 10f;

    [SerializeField, Tooltip("The visual of the explosion.")]
    public ParticleSystem Explosion;


    private CharacterController _controller;
    private Animator _animator;
    private AudioManager _audioManager;
    private AudioSource _audioSource;

    private void Explode()
    {
        ParticleSystem explosion = Instantiate(Explosion);
        explosion.transform.position = transform.position;
        explosion.transform.localScale = transform.localScale;
        explosion.Play();
        _audioManager.Play("Explosion", position: transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (Collider collider in colliders)
            collider.GetComponent<Damagable>()?.TakeDamage(ExplosionDamage);

        //_audioSource.Play();
        //_audioManager.Play("Explosion", source: _audioSource);

        Destroy(gameObject);
    }

    private void PlayBleep(float pitch)
    {
        _audioManager.Play("Bleep", position: transform.position, pitch: pitch);
    }

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _audioManager = FindObjectOfType<AudioManager>();
        target = FindObjectOfType<JediCharacter>().transform;
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("Moving", false);

        if (target == null)
            return;

        Vector3 diff = target.position - transform.position;
        float distance = diff.magnitude;
        Vector3 direction = diff.normalized;

        if (distance > AggretionRadius)
            return;

        if (distance < ExplosionTriggerRange)
        {
            _animator.SetTrigger("Explode");
            target = null;
            return;
        }

        _animator.SetBool("Moving", true);

        _controller.SimpleMove(direction * MoveSpeed);
    }
}
