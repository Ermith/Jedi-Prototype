using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObject : MonoBehaviour
{
    public bool Transform = false;
    public bool Rotation = false;
    public bool Scale = false;
    public Transform From;
    public Transform To;
    public float Duration;
    private Tween _posTween;
    private Tween _rotTween;
    private Tween _sclTween;

    public bool DestroyOnFinish = false;
    public bool ColorChange = false;
    public bool Special = false;

    public void Activate()
    {
        if (Transform)
        {
            _posTween.PlayForward();
            if (Rotation) _rotTween.PlayForward();
            if (Scale) _sclTween.PlayForward();
        }
    }

    public void Deactivate()
    {
        if (Transform)
        {
            _posTween.PlayBackwards();
            if (Rotation) _rotTween.PlayBackwards();
            if (Scale) _sclTween.PlayBackwards();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Transform)
        {
            transform.position = From.position;
            if (Rotation) transform.rotation = From.rotation;
            if (Scale) transform.localScale = From.localScale;

            _posTween = transform.DOMove(To.position, Duration).Pause();
            if (Rotation) _rotTween = transform.DORotate(To.rotation.eulerAngles, Duration).Pause();
            if (Scale) _sclTween = transform.DOScale(To.localScale, Duration).Pause();

            _posTween.SetAutoKill(false);
            if (Rotation) _rotTween.SetAutoKill(false);
            if (Scale) _sclTween.SetAutoKill(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
