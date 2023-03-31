using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField, Tooltip("The green bar.")]
    public Image Health;

    [SerializeField, Tooltip("The red bar that follows the green bar on damage.")]
    public Image Damage;

    public void TakeDamage(float percentage)
    {
        if (Health != null)
            Health.DOFillAmount(percentage, 0.2f);

        if (Damage != null)
            Damage.DOFillAmount(percentage, 0.3f).SetDelay(0.2f);
    }
}
