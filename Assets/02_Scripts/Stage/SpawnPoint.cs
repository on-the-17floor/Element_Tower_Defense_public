using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private int count;

    public void SpawnUnit(int _index, int _count)
    {
        //index = _index;
        // count = _count;

        int tempIndex = _index;
        int tempCount = _count;

        StartCoroutine(StartSpawn(tempIndex , tempCount));
    }

    private IEnumerator StartSpawn(int idx, int cnt)
    {
        Debug.Log(idx + " ===================== " + cnt );
        for (int i = 0; i < cnt; i++)
        {
            yield return new WaitForSeconds(1f);
            EnemyManger.Instance.GetEnemy(idx);
        }
    }
}
