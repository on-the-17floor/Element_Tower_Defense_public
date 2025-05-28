using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayBullet : TowerBullet
{
    [SerializeField] private ParticleSystem sparkle;
    [SerializeField] private ParticleSystem trail;
    public override void InitBullet(TowerTier tier)
    {
        base.InitBullet(tier);

        OnBullet += ChangeBulletColor;
        OnSFX += () => SoundManager.Instance.SFXPlay(SFXType.TowerWhiteAttack, transform.position);
    }

    private void ChangeBulletColor(ElementType elementType)
    {
        currentType = elementType;

        Color baseColor = projectileManager.Colors[(int)elementType];

        // 투사체 색변경
        var sparkleMain = sparkle.main;
        var trailMain = trail.main;

        sparkleMain.startColor = baseColor;
        trailMain.startColor = GradientGenerator.CreateGradient(baseColor);

    }
}
