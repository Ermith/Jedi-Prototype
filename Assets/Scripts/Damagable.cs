using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Damagable : MonoBehaviour
{
    public float MaxHealth = 100;
    public Image HealthBar;
    public Image DamageBar;
    public bool CameraShake = false;
    public bool CharacterShake = false;
    public bool ScaleShake = false;
    public bool Sound = false;
    public string SoundName = "";

    private float _health;
    public Camera Camera;
    private Material _material;
    private AudioManager _audioManager;

    public void Damage(float damage)
    {
        _health -= damage;

        if (HealthBar != null)
            HealthBar.DOFillAmount(_health / MaxHealth, 0.2f);

        if (DamageBar != null)
            DamageBar.DOFillAmount(_health / MaxHealth, 0.3f).SetDelay(0.2f);

        if (_health <= 0)
            return;

        if (CameraShake)
        {
            Camera.DOShakeRotation(0.2f, strength: 1.7f);
        }

        if (CharacterShake)
            transform.DOShakePosition(0.3f, strength: 0.2f, vibrato: 100);

        if (ScaleShake)
            transform.DOShakeScale(0.3f, strength: 0.2f, vibrato: 100);

        if (Sound)
            _audioManager.Play(SoundName, position: transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        //_camera = FindObjectOfType<Camera>();
        _audioManager = FindObjectOfType<AudioManager>();
        _health = MaxHealth;
    }

    private void Awake()
    {
        //_camera = FindObjectOfType<Camera>();
        _audioManager = FindObjectOfType<AudioManager>();
        _health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (_health <= 0)
            Destroy(gameObject);
    }
}
