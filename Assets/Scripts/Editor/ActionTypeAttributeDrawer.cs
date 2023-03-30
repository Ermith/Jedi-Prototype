using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

[CustomPropertyDrawer(typeof(ActionTypeAttribute))]
public class NewBehaviourScript : PropertyDrawer
{
    private float _height = 0f;
    private bool _transformRollout = false;
    private bool _colorRollout;
    private bool _soundRollout;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return _height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //if (property.propertyType != SerializedPropertyType.)
        //{
        //    EditorGUI.LabelField(position, "Wrong type mate!");
        //    return;
        //}

        var parentObject = property.serializedObject.targetObject;
        var parent = parentObject as SmartObject;
        object targetObject = fieldInfo.GetValue(parentObject);
        TweenWrapper wrapper = (targetObject as TweenWrapper);
        ActionTypeAttribute actionType = (ActionTypeAttribute)attribute;

        float padding = 20f;
        float start = position.x;
        position.height = padding;

        EditorGUI.BeginProperty(position, label, property);

        Rect registerPosition = new Rect(position.x, position.y, position.width / 2, position.height);
        Rect setEndPosition = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

        bool setStart = GUI.Button(registerPosition, "Set Start");
        bool setEnd = GUI.Button(setEndPosition, "Set End");
        position.y += position.height;

        Rect playPosition = new Rect(position.x, position.y, position.width / 2, position.height);
        Rect resetPosition = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
        bool play = GUI.Button(playPosition, "Play");
        bool reset = GUI.Button(resetPosition, "Reset");
        position.y += position.height;

        // Transform
        //=====================================
        Section(ref position, ref _transformRollout, ref wrapper.Transform, "Transform", () =>
        {
            FloatField(ref position, ref wrapper.TransformDuration, "Duration");
            EaseField(ref position, ref wrapper.TransformEase, "Ease");
            LabelField(ref position, "To");
            EditorGUI.indentLevel++;

            Vector3Field(ref position, ref wrapper.ToPosition, "Position");
            Vector3Field(ref position, ref wrapper.ToRotation, "Rotation");
            Vector3Field(ref position, ref wrapper.ToScale, "Scale");
            EditorGUI.indentLevel--;

            LabelField(ref position, "From");
            EditorGUI.indentLevel++;

            Vector3Field(ref position, ref wrapper.FromPosition, "Position");
            Vector3Field(ref position, ref wrapper.FromRotation, "Rotation");
            Vector3Field(ref position, ref wrapper.FromScale, "Scale");
            EditorGUI.indentLevel--;
        });

        // Color
        //=====================================
        Section(ref position, ref _colorRollout, ref wrapper.Color, "Color", () =>
        {
            FloatField(ref position, ref wrapper.ColorDuration, "Duration");
            EaseField(ref position, ref wrapper.ColorEase, "Ease");
            ColorField(ref position, ref wrapper.FromColor, "From");
            ColorField(ref position, ref wrapper.ToColor, "To");
        });

        // Sound
        //=====================================
        Section(ref position, ref _soundRollout, ref wrapper.Sound, "Sound", () =>
        {
            wrapper.SoundName = EditorGUI.TextField(position, "Name", wrapper.SoundName);
            position.y += position.height;

            FloatField(ref position, ref wrapper.SoundTime, "Start");
        });

        EditorGUI.EndProperty();

        /**/
        if (setStart)
        {
            //var parent = parentObject as SmartObject;

            if (wrapper.Transform)
            {
                wrapper.FromPosition = parent.transform.position;
                wrapper.FromRotation = parent.transform.eulerAngles;
                wrapper.FromScale = parent.transform.localScale;
            }

            if (wrapper.Color)
                wrapper.FromColor = parent.GetComponent<Renderer>().material.color;
        }


        if (reset)
        {
            DG.DOTweenEditor.DOTweenEditorPreview.Stop();
            if (wrapper.Transform)
            {
                parent.transform.position = wrapper.FromPosition;
                parent.transform.eulerAngles = wrapper.FromRotation;
                parent.transform.localScale = wrapper.FromScale;
            }

            if (wrapper.Color)
                parent.GetComponent<Renderer>().material.color = wrapper.FromColor;
        }

        if (play)
        {
            wrapper.Tween.Kill();
            wrapper.Tween = DOTween.Sequence();

            if (wrapper.Transform)
            {
                parent.transform.position = wrapper.FromPosition;
                parent.transform.eulerAngles = wrapper.FromRotation;
                parent.transform.localScale = wrapper.FromScale;

                wrapper.Tween.Join(parent.transform.DOMove(wrapper.ToPosition, wrapper.TransformDuration).SetEase(wrapper.TransformEase));
                wrapper.Tween.Join(parent.transform.DORotate(wrapper.ToRotation, wrapper.TransformDuration).SetEase(wrapper.TransformEase));
                wrapper.Tween.Join(parent.transform.DOScale(wrapper.ToScale, wrapper.TransformDuration).SetEase(wrapper.TransformEase));
            }

            if (actionType.Color)
                wrapper.Tween.Join(parent.GetComponent<Renderer>().material.DOColor(actionType.ToColor, actionType.ColorDuration).SetEase(wrapper.ColorEase));

            //if (actionType.Sound)
            //    wrapper.Tween.Join(DOTween.To(() => 5, (float f) => { }, 0, actionType.SoundTime).OnComplete(() => parent.PlaySound(actionType.SoundName)).Pause());

            Debug.Log("Registering");
            DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(wrapper.Tween, preventAutoKill: false);
            DG.DOTweenEditor.DOTweenEditorPreview.Start();
        }
        //*/
        if (setEnd)
        {
            //var parent = parentObject as SmartObject;
            if (wrapper.Transform)
            {
                wrapper.ToPosition = parent.transform.position;
                wrapper.ToRotation = parent.transform.rotation.eulerAngles;
                wrapper.ToScale = parent.transform.localScale;
            }

            if (wrapper.Color)
                wrapper.ToColor = parent.GetComponent<Renderer>().material.color;
        }
        //*/

        _height = position.y - start;
    }

    private void Section(ref Rect position, ref bool rollout, ref bool enabled, string label, Action action)
    {
        rollout = EditorGUI.BeginFoldoutHeaderGroup(position, rollout, label);
        position.y += position.height;
        EditorGUI.indentLevel++;

        if (rollout)
        {
            enabled = EditorGUI.Toggle(position, "Enabled", enabled);
            position.y += position.height;

            EditorGUI.BeginDisabledGroup(!enabled);

            action();

            EditorGUI.EndDisabledGroup();
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndFoldoutHeaderGroup();
    }

    private void Vector3Field(ref Rect position, ref Vector3 vector, string label)
    {
        vector = EditorGUI.Vector3Field(position, label, vector);
        position.y += position.height;
    }

    private void ColorField(ref Rect position, ref Color color, string label)
    {
        color = EditorGUI.ColorField(position, label, color);
        position.y += position.height;
    }

    private void LabelField(ref Rect position, string label)
    {
        EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
        position.y += position.height;
    }

    private void FloatField(ref Rect position, ref float value, string label)
    {
        value = EditorGUI.FloatField(position, label, value);
        position.y += position.height;
    }

    private void EaseField(ref Rect position, ref Ease value, string label)
    {
        value = (Ease)EditorGUI.EnumPopup(position, label, value);
        position.y += position.height;
    }
}
