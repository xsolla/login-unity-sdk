using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public interface ICreateAccount
{
    void CreateAccount();
}

public class CreateAccountPage : Page, ICreateAccount
{
    [SerializeField] InputField _login_Text;
    [SerializeField] InputField _password_Text;
    [SerializeField] InputField _email_Text;
    [SerializeField] Button _close_Btn;
    [SerializeField] Button _create_Btn;

    private void Awake()
    {
        _close_Btn.onClick.AddListener(Close);
        _create_Btn.onClick.AddListener(CreateAccount);
    }

    private void Start()
    {
        XsollaAuthentication.Instance.OnSuccesfulRegistration += OnSuccesfulRegistration;
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException += OnRegistrationNotAllowed;
        XsollaAuthentication.Instance.OnUsernameIsTaken += OnUsernameIsTaken;
        XsollaAuthentication.Instance.OnEmailIsTaken += OnEmailIsTaken;
    }

    private void OnSuccesfulRegistration()
    {
        Debug.Log("Succesfully registrated");
    }

    private void OnEmailIsTaken(ErrorDescription error)
    {
        Debug.Log("The email is already taken");
    }

    private void OnUsernameIsTaken(ErrorDescription error)
    {
        Debug.Log("The username is already taken");
    }

    private void OnRegistrationNotAllowed(ErrorDescription error)
    {
        Debug.Log($"Password Registration Not Allowed {error.code}, {error.description}");
    }

    private void OnDestroy()
    {
        XsollaAuthentication.Instance.OnSuccesfulRegistration -= OnSuccesfulRegistration;
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException -= OnRegistrationNotAllowed;
        XsollaAuthentication.Instance.OnUsernameIsTaken -= OnUsernameIsTaken;
        XsollaAuthentication.Instance.OnEmailIsTaken -= OnEmailIsTaken;
    }

    public void CreateAccount()
    {
        if (_login_Text.text != "" && _password_Text.text != "" && _email_Text.text != "")
        {
            XsollaAuthentication.Instance.Registration(_login_Text.text, _password_Text.text, _email_Text.text);
        }
        else
            Debug.Log("Fill all fields");
    }
}