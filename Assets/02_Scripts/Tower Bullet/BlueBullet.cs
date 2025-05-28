using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBullet : TowerBullet
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private TrailRenderer trailRenderer;

    public override void InitBullet(TowerTier tier)
    {
        base.InitBullet(tier);

        OnBullet += ChangeBulletColor;
        OnSFX += () => SoundManager.Instance.SFXPlay(SFXType.TowerRedAttack, transform.position);
    }

    private void ChangeBulletColor(ElementType elementType)
    {
        currentType = elementType;

        Color baseColor = projectileManager.Colors[(int)elementType];

        foreach (var particle in particles)
        {
            var main = particle.main;
            float alpha = main.startColor.color.a;
            main.startColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }

        trailRenderer.colorGradient = GradientGenerator.CreateGradient(baseColor);
    }
}
