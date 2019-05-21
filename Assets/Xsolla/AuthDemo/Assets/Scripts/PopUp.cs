using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IPopUp
{
    void ShowPopUp(string message);
    UnityAction OnClose { set; }
}

public class PopUp : Page, IPopUp
{
    [SerializeField] private Text message_Text;
    [SerializeField] protected Button close_Button;

    public UnityAction OnClose
    {
        set
        {
            close_Button.onClick.AddListener(value);
        }
    }

    private void Awake()
    {
        close_Button.onClick.AddListener(() => Close());
    }
    public void ShowPopUp(string message)
    {
        message_Text.text = message;
        Open();
    }
}
