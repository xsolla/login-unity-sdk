using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla;

public interface ISignIn
{
    void SignIn();
}
public class SignInPage : Page, ISignIn
{
    [SerializeField] InputField _login_Text;
    [SerializeField] InputField _password_Text;
    [SerializeField] Button _signIn_Btn;
    [SerializeField] Toggle _saveMe_ChkBox;

    private void Awake()
    {
        _signIn_Btn.onClick.AddListener(SignIn);
        _login_Text.text = XsollaAuthentication.Instance.LastUserLogin;
        _password_Text.text = XsollaAuthentication.Instance.LastUserPassword;
    }


    private void Start()
    {
        XsollaAuthentication.Instance.OnSuccesfulSignIn += OnSignIn;
        XsollaAuthentication.Instance.OnValidToken += OnValidToken;
        XsollaAuthentication.Instance.OnInvalidToken += OnInvalidToken;
        XsollaAuthentication.Instance.OnCaptchaRequiredException += OnCaptchaRequiredException;
        XsollaAuthentication.Instance.OnUserIsNotActivated += OnUserIsNotActivated;
        XsollaAuthentication.Instance.OnInvalidLoginOrPassword += OnInvalidLoginOrPassword;
    }

    private void OnInvalidToken()
    {
        Debug.Log("Invalid token");
    }

    private void OnValidToken(string obj)
    {
        Debug.Log("Valid token");
    }

    private void OnInvalidLoginOrPassword(ErrorDescription error)
    {
        Debug.Log("Invalid Login Or Password");
    }

    private void OnUserIsNotActivated(ErrorDescription error)
    {
        Debug.Log("User is not activated");
    }

    private void OnCaptchaRequiredException(ErrorDescription error)
    {
        Debug.Log("Captcha exception");
    }

    private void OnSignIn(XsollaUser user)
    {
        if (XsollaAuthentication.Instance.IsTokenValid && XsollaAuthentication.Instance.IsJWTvalidationToken)
        {
            Debug.Log("Your token " + XsollaAuthentication.Instance.Token + " is active");
            SceneManager.LoadScene("Game");
        }
        else if (!XsollaAuthentication.Instance.IsJWTvalidationToken)
        {
            Debug.Log("Unsafe signed in");
            SceneManager.LoadScene("Game");
        }
    }

    public void SignIn()
    {
        if (_login_Text.text != "" && _password_Text.text != null)
        {
            XsollaAuthentication.Instance.SignIn(_login_Text.text, _password_Text.text, _saveMe_ChkBox.isOn);
        }
        else
            Debug.Log("Fill all fields");
    }
    void OnDestroy()
    {
        XsollaAuthentication.Instance.OnSuccesfulSignIn -= OnSignIn;
        XsollaAuthentication.Instance.OnValidToken -= OnValidToken;
        XsollaAuthentication.Instance.OnInvalidToken -= OnInvalidToken;
        XsollaAuthentication.Instance.OnCaptchaRequiredException -= OnCaptchaRequiredException;
        XsollaAuthentication.Instance.OnUserIsNotActivated -= OnUserIsNotActivated;
        XsollaAuthentication.Instance.OnInvalidLoginOrPassword -= OnInvalidLoginOrPassword;
    }
}
