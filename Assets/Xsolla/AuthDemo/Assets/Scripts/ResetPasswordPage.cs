using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public interface IResetPassword
{
    void ResetPassword();
    Action OnSuccessfulResetPassword { get; set; }
    Action<ErrorDescription> OnUnsuccessfulResetPassword { get; set; }
}
public class ResetPasswordPage : Page, IResetPassword
{
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Button change_Btn;
    [SerializeField] private GameObject success_Panel;
    [Header("Swap Button Images")]
    [SerializeField] private Image change_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    public Action OnSuccessfulResetPassword
    {
        get
        {
            return onSuccessfulResetPassword;
        }

        set
        {
            onSuccessfulResetPassword = value;
        }
    }

    public Action<ErrorDescription> OnUnsuccessfulResetPassword
    {
        get
        {
            return onUnsuccessfulResetPassword;
        }

        set
        {
            onUnsuccessfulResetPassword = value;
        }
    }

    private Action onSuccessfulResetPassword;
    private Action<ErrorDescription> onUnsuccessfulResetPassword;

    private void Awake()
    {
        change_Btn.onClick.AddListener(ResetPassword);
        email_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (!string.IsNullOrEmpty(email_InputField.text))
        {
            if (change_Image.sprite != enabled_Sprite)
                change_Image.sprite = enabled_Sprite;
        }
        else if (change_Image.sprite == enabled_Sprite)
            change_Image.sprite = disabled_Sprite;
    }

    private void SuccessfulResetPassword()
    {
        Debug.Log("Successfull reseted password");
        success_Panel.GetComponent<IPage>().Open();
        success_Panel.GetComponent<ISuccessPage>().SetEmail(email_InputField.text);
        if (onSuccessfulResetPassword != null)
            onSuccessfulResetPassword.Invoke();
    }
    public void ResetPassword()
    {
        if (!string.IsNullOrEmpty(email_InputField.text))
            XsollaAuthentication.Instance.ResetPassword(email_InputField.text, SuccessfulResetPassword, onUnsuccessfulResetPassword);
        else
            Debug.Log("Fill all fields");
    }
}
