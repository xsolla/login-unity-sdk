using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public interface IChangePassword
{
    void ChangePassword();
}
public class ChangePasswordPage : Page, IChangePassword
{
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Button change_Btn;
    [SerializeField] private GameObject success_Panel;
    [Header("Swap Button Images")]
    [SerializeField] private Image change_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    private void Awake()
    {
        change_Btn.onClick.AddListener(ChangePassword);
        email_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (email_InputField.text != "")
        {
            if (change_Image.sprite != enabled_Sprite)
                change_Image.sprite = enabled_Sprite;
        }
        else if (change_Image.sprite == enabled_Sprite)
            change_Image.sprite = disabled_Sprite;
    }
    private void Start()
    {
        XsollaAuthentication.Instance.OnSuccessfulResetPassword += OnSuccesfullResetPassword;
    }


    private void OnSuccesfullResetPassword()
    {
        Debug.Log("Successfull reseted password");
        success_Panel.GetComponent<IPage>().Open();
        success_Panel.GetComponent<ISuccessPage>().SetEmail(email_InputField.text);
    }
    private void OnDestroy()
    {
        XsollaAuthentication.Instance.OnSuccessfulResetPassword -= OnSuccesfullResetPassword;
    }
    public void ChangePassword()
    {
        if (email_InputField.text != "")
            XsollaAuthentication.Instance.ResetPassword(email_InputField.text);
        else
            Debug.Log("Fill all fields");
    }
}
