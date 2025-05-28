using DG.Tweening;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HyperBullet : TowerBullet
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private TrailRenderer trailRenderer;

    public override void InitBullet(TowerTier tier)
    {
        base.InitBullet(tier);

        OnBullet += ChangeBulletColor;
        OnSFX += () => SoundManager.Instance.SFXPlay(SFXType.TowerHyperAttack, transform.position);
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

    public override void Shoot(Transform start, Transform target, Action onHit)
    {
        //차징
        Charging(start, target);

        // 발사
        base.Shoot(start, target, onHit);
    }

    private float Charging(Transform start, Transform target)
    {
        // 예외처리
        if (start == null || target == null)
        {
            hitEffectPlayed = true;
            ReturnAsPool();
            return 0;
        }

        transform.position = start.position;

        // 목표 바라보기
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        // 차징 가져오기
        effectManager.PlayNormalEffect(EffectName.Charging, start.position, out var charging);

        if (charging.TryGetComponent(out EffectColorChanger colorChanger))
            colorChanger.ChangeColor(currentType);

        // 이펙트 실행 예상시간 반환
        return charging.duration;
    }
}
