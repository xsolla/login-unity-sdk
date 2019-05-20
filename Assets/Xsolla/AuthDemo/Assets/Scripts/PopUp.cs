using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPopUp
{
    void ShowPopUp(string message);
}

public class PopUp : Page, IPopUp
{
    [SerializeField] private Text message_Text;
    [SerializeField] private Button close_Button;
    private void Awake()
    {
        close_Button.onClick.AddListener(() => { Close(); });
    }
    public void ShowPopUp(string message)
    {
        message_Text.text = message;
        Open();
    }
}
