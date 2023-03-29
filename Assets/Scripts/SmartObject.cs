using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObject : MonoBehaviour
{
    public bool Transform = false;
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
            _rotTween.PlayForward();
            _sclTween.PlayForward();
        }
    }

    public void Deactivate()
    {
        if (Transform)
        {
            _posTween.PlayBackwards();
            _rotTween.PlayBackwards();
            _sclTween.PlayBackwards();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Transform)
        {
            transform.position = From.position;
            transform.rotation = From.rotation;
            transform.localScale = From.localScale;

            _posTween = transform.DOMove(To.position, Duration).Pause();
            _rotTween = transform.DORotate(To.rotation.eulerAngles, Duration).Pause();
            _sclTween = transform.DOScale(To.localScale, Duration).Pause();

            _posTween.SetAutoKill(false);
            _rotTween.SetAutoKill(false);
            _sclTween.SetAutoKill(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
