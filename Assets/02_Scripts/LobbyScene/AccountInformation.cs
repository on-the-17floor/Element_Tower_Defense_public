using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AccountInformation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userAccountText;
    [SerializeField] private TextMeshProUGUI userUIDText;

    void Start()
    {
        UserData data = DataStore.userData;

        if (data == null)
            return;

        userAccountText.text = $"{data.email} ( {data.name} )";
        userUIDText.text = $"{data.uid}";
    }
}
