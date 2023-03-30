using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTypeAttribute : PropertyAttribute
{
    public bool Transform;
    public float TransformDuration;

    public Vector3 FromPosition;
    public Vector3 FromRotation;
    public Vector3 FromScale;

    public Vector3 ToPosition;
    public Vector3 ToRotation;
    public Vector3 ToScale;

    // =======================
    public bool Color;
    public float ColorDuration;
    public Color FromColor;
    public Color ToColor;


    // =======================
    public bool Sound;
    public float SoundTime;
    public string SoundName;

    public ActionTypeAttribute()
    {
    }

    public override bool Match(object obj)
    {
        return base.Match(obj);
    }
}
