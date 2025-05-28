using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRangeCircle : MonoBehaviour
{
    [SerializeField] private Transform rangeSphere;

    public GameObject RangeSphere => rangeSphere.gameObject;

    public void OnRange(float range)
    {
        rangeSphere.localScale = Vector3.zero;

        float tmp = range * 2;
        Vector3 size = new Vector3(tmp, 0.01f, tmp);

        rangeSphere.DOScale(size, 0.25f).SetEase(Ease.OutQuad);
    }
}
