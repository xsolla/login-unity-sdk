using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningPopUp : PopUp
{
    private void Awake()
    {
        close_Button.onClick.AddListener(() => Application.OpenURL("https://github.com/xsolla/login-unity-sdk/blob/master/README.md"));
    }
}
