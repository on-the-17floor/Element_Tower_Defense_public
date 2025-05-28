using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [SerializeField]
    private Transform[] wayPoint;

    private void OnValidate()
    {
        //본인을 제외하고 자식의 Transform만 가져온다.
        wayPoint = GetComponentsInChildren<Transform>(true)
                   .Where(t => t != transform)
                   .ToArray();
    }

    public Transform[] GetWayPoint()
    {
        return wayPoint;
    }

}
