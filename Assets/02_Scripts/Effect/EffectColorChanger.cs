using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectColorChanger : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;

    public void ChangeColor(ElementType elementType)
    {
        Color baseColor = EffectManager.Instance.BaseColor[(int)elementType];

        foreach (var particle in particles)
        {
            var main = particle.main;
            float alpha = main.startColor.color.a;
            main.startColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
    }
}
