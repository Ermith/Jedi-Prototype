using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField, Tooltip("Layer to hit.")]
    public int TargetLayer;

    [SerializeField, Tooltip("How long before get's destroyed on it's own.")]
    public float LifeTime = 100000f;

    [SerializeField, Tooltip("Visual on hitting something.")]
    public ParticleSystem ParticleSystem;

    [HideInInspector] public float Damage;
    [HideInInspector] public GameObject from;
    [HideInInspector] public Vector3 Velocity;

    private Stopwatch _stopwatch;
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _stopwatch = Stopwatch.StartNew();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Velocity * Time.deltaTime;
        if (_stopwatch.ElapsedMilliseconds > LifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == from || other.gameObject.layer == 2)
            return;

        other.GetComponent<Damagable>()?.TakeDamage(Damage);
        var particleRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        Vector3 pointOfImpact = other.ClosestPoint(transform.position);
        Instantiate(ParticleSystem, pointOfImpact, particleRotation).Play();
        
        Destroy(gameObject);
    }
}
