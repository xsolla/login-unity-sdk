using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public interface IChangePassword
{
    void ChangePassword();
}
public class ChangePasswordPage : Page, IChangePassword
{
    [SerializeField] InputField _login_Text;
    [SerializeField] Button _close_Btn;
    [SerializeField] Button _change_Btn;

    private void Awake()
    {
        _close_Btn.onClick.AddListener(Close);
        _change_Btn.onClick.AddListener(ChangePassword);
    }
    private void Start()
    {
        XsollaAuthentication.Instance.OnSuccessfulResetPassword += OnSuccesfullResetPassword;
        XsollaAuthentication.Instance.OnPassworResetingNotAllowedForProject += OnPassworResetingNotAllowedForProject;
    }

    private void OnPassworResetingNotAllowedForProject(ErrorDescription error)
    {
        Debug.Log("Passwor Reseting Not Allowed For Project "+error.code+", "+error.description);
    }

    private void OnSuccesfullResetPassword()
    {
        Debug.Log("Successfull reseted password");
    }
    private void OnDestroy()
    {
        XsollaAuthentication.Instance.OnSuccessfulResetPassword -= OnSuccesfullResetPassword;
        XsollaAuthentication.Instance.OnPassworResetingNotAllowedForProject -= OnPassworResetingNotAllowedForProject;
    }
    public void ChangePassword()
    {
        if (_login_Text.text != "")
            XsollaAuthentication.Instance.ResetPassword(_login_Text.text);
        else
            Debug.Log("Fill all fields");
    }
}
