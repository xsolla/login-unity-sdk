using UnityEngine;
using UnityEngine.UI;

public interface ISuccessPage
{
    void SetEmail(string email);
}

public class ResetPswSuccessPage : Page, ISuccessPage
{
    [SerializeField] private Button submit_Btn;
    [SerializeField] private Button return_Btn;
    [SerializeField] private Text message_Text;
    private string start_Message;

    private void Awake()
    {
        start_Message = message_Text.text;
        submit_Btn.onClick.AddListener(() => { Close(); });
        return_Btn.onClick.AddListener(() => { Close(); });
    }
    public void SetEmail(string email)
    {
        message_Text.text = string.Format("{0} {1}", start_Message, email);
    }
}
