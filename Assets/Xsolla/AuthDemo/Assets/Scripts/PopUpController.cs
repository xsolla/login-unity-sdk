using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPopUpController
{
    void ShowPopUp(string message, bool isSuccess);
}

public class PopUpController : MonoBehaviour, IPopUpController
{
    [SerializeField] private GameObject success_Panel;
    [SerializeField] private GameObject error_Panel;

    public void ShowPopUp(string message, bool isSuccess)
    {
        if (isSuccess)
            success_Panel.GetComponent<IPopUp>().ShowPopUp(message);
        else
            error_Panel.GetComponent<IPopUp>().ShowPopUp(message);
    }
}
