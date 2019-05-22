using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenDocumentation : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
	    // REVIEW This url is used in several places. Can be encapsulated in constant as well
        Application.OpenURL("https://github.com/xsolla/login-unity-sdk/blob/master/README.md");
    }
}
