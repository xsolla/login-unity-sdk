using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public interface ILogin
{
    void Login();
    Action<XsollaUser> OnSuccessfulLogin { get; set; }
    Action<ErrorDescription> OnUnsuccessfulLogin { get; set; }
}
public class LoginPage : Page, ILogin
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private Button login_Btn;
    [SerializeField] private Toggle rememberMe_ChkBox;
    [SerializeField] private Toggle showPassword_Toggle;
    [Header("Swap Button Images")]
    [SerializeField] private Image login_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    public Action<XsollaUser> OnSuccessfulLogin
    {
        get
        {
            return onSuccessfulLogin;
        }

        set
        {
            onSuccessfulLogin = value;
        }
    }

    public Action<ErrorDescription> OnUnsuccessfulLogin
    {
        get
        {
            return onUnsuccessfulLogin;
        }

        set
        {
            onUnsuccessfulLogin = value;
        }
    }

    private Action<XsollaUser> onSuccessfulLogin;
    private Action<ErrorDescription> onUnsuccessfulLogin;

    private void Awake()
    {
        showPassword_Toggle.onValueChanged.AddListener((mood) => {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        login_Btn.onClick.AddListener(Login);
        login_InputField.onValueChanged.AddListener(ChangeButtonImage);
        password_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5)
        {
            if (login_Image.sprite != enabled_Sprite)
                login_Image.sprite = enabled_Sprite;
        }
        else if (login_Image.sprite == enabled_Sprite)
                login_Image.sprite = disabled_Sprite;
    }

    private void Start()
    {
        login_InputField.text = XsollaAuthentication.Instance.LastUserLogin;
        password_InputField.text = XsollaAuthentication.Instance.LastUserPassword;
    }

    private void OnLogin(XsollaUser user)
    {
        if (XsollaAuthentication.Instance.IsTokenValid && XsollaAuthentication.Instance.IsJWTValidationToken)
        {
            Debug.Log(string.Format("Your token {0} is active", XsollaAuthentication.Instance.Token));
        }
        else if (!XsollaAuthentication.Instance.IsJWTValidationToken)
        {
            Debug.Log("Unsafe signed in");
        }
        if (onSuccessfulLogin != null)
            onSuccessfulLogin.Invoke(user);
    }

    public void Login()
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5)
        {
            XsollaAuthentication.Instance.SignIn(login_InputField.text, password_InputField.text, rememberMe_ChkBox.isOn, OnLogin, onUnsuccessfulLogin);
        }
        else
            Debug.Log("Fill all fields");
    }
}