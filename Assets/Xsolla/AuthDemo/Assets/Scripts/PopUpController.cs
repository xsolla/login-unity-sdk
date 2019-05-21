using System;
using UnityEngine;
using UnityEngine.Events;

public interface IPopUpController
{
    void ShowPopUp(string message, PopUpWindows popUp);
    UnityAction OnClosePopUp { set; }
}
public enum PopUpWindows
{
    Success,
    Error,
    Warning
}
public class PopUpController : MonoBehaviour, IPopUpController
{
    [SerializeField] private GameObject success_Panel;
    [SerializeField] private GameObject error_Panel;
    [SerializeField] private GameObject warning_Panel;

    public UnityAction OnClosePopUp
    {
        set
        {
            success_Panel.GetComponent<IPopUp>().OnClose = value;
            error_Panel.GetComponent<IPopUp>().OnClose = value;
            warning_Panel.GetComponent<IPopUp>().OnClose = value;
        }
    }

    public void ShowPopUp(string message, PopUpWindows popUp)
    {
        switch (popUp)
        {
            case PopUpWindows.Success:
                success_Panel.GetComponent<IPopUp>().ShowPopUp(message);
                break;

            case PopUpWindows.Error:
                error_Panel.GetComponent<IPopUp>().ShowPopUp(message);
                break;

            case PopUpWindows.Warning:
                warning_Panel.GetComponent<IPopUp>().ShowPopUp(message);
                break;
        }
    }
}
