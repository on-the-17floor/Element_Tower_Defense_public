using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Define.ENEMY))
        {
            //TODO: 오브젝트 풀 나오기 전까지 쓸 임시 코드
            Destroy(other.gameObject);
            ExitUnit();
        }
    }

    public void ExitUnit()
    {
        //Enemy Dead Event

        //적 삭제
        //currentCount와 Life 감소
    }
}
