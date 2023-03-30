using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TweenWrapper
{

    public bool Transform;
    public float TransformDuration;
    public Ease TransformEase;

    public Vector3 FromPosition;
    public Vector3 FromRotation;
    public Vector3 FromScale;

    public Vector3 ToPosition;
    public Vector3 ToRotation;
    public Vector3 ToScale;

    // =======================
    public bool Color;
    public float ColorDuration;
    public Ease ColorEase;
    public Color FromColor;
    public Color ToColor;


    // =======================
    public bool Sound;
    public float SoundTime;
    public string SoundName;

    public Sequence Tween { get; set; }
}


public class SmartObject : MonoBehaviour
{
    [ActionType]
    public TweenWrapper wrapper;

    private AudioManager _audioManager;


    public void PlaySound(string name)
    {
        _audioManager.PlayOnTarget(name, gameObject);
    }

    public void Activate()
    {
        wrapper.Tween.PlayForward();
    }

    public void Deactivate()
    {
        wrapper.Tween.PlayBackwards();
    }

    // Start is called before the first frame update
    void Start()
    {
        wrapper.Tween = DOTween.Sequence();
        wrapper.Tween.SetAutoKill(false);
        
        if (wrapper.Transform)
        {
            transform.position = wrapper.FromPosition;
            transform.eulerAngles = wrapper.FromRotation;
            transform.localScale = wrapper.FromScale;

            wrapper.Tween.Join(transform.DOMove(wrapper.ToPosition, wrapper.TransformDuration).SetEase(wrapper.TransformEase).Pause());
            wrapper.Tween.Join(transform.DORotate(wrapper.ToRotation, wrapper.TransformDuration).SetEase(wrapper.TransformEase).Pause());
            wrapper.Tween.Join(transform.DOScale(wrapper.ToScale, wrapper.TransformDuration).SetEase(wrapper.TransformEase).Pause());
        }

        if (wrapper.Color)
        {
            GetComponent<Renderer>().material.color = wrapper.FromColor;
            wrapper.Tween.Join(GetComponent<Renderer>().material.DOColor(wrapper.ToColor, wrapper.ColorDuration).SetEase(wrapper.ColorEase).Pause());
        }

        if (wrapper.Sound)
            wrapper.Tween.Join(DOTween.To(() => 5, (float f) => { }, 0, wrapper.SoundTime).OnComplete(() => PlaySound(wrapper.SoundName)).Pause());

        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
