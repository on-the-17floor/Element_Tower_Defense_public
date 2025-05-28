using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHpBar : MonoBehaviour
{

    [Header("===UI===")]
    [SerializeField] public GameObject panel;
    [SerializeField] private Image hpImage;

    public Quaternion panelRotation
    { 
        get => panel.transform.localRotation;
        set { panel.transform.localRotation = value; }
    }

    private void Start()
    {
        // panel null 일때 (다 할당해놔서 실행할 일 없음)
        if (panel == null)
        {
            Debug.LogError($"{this.name} does not have panel");
            // 게임오브젝트 하위에 canvas있음
            Transform can = transform.GetChild(0);
            panel = can.GetChild(0).gameObject;
        }
        if (hpImage == null)
        {
            Debug.LogError($"{this.name} does not have HpImage");
            // panel 하위에 Hp이미지 있음
            hpImage = transform.GetChild(1).GetComponent<Image>();
        }
    }

    public void UpdateHpBar(float value) 
    {
        // Debug.Log(this.unitHp / unitData.maxHp);
        hpImage.fillAmount = value;
    }
}
