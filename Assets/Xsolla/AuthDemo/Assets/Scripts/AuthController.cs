using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla;

public class AuthController : MonoBehaviour
{
    [SerializeField] GameObject _changePassword_Panel;
    [SerializeField] GameObject _createAccount_Panel;
    [SerializeField] GameObject _signIn_Panel;
    [SerializeField] GameObject _message_Panel;

    [SerializeField] Button _openChangePassw_Btn;
    [SerializeField] Button _openCreateAccount_Btn;

    private void Start()
    {
        if (XsollaAuthentication.Instance.LoginID == "")
        {
            _message_Panel.SetActive(true);
            Debug.Log("Please register on the Xsolla website, and fill the Login ID form. For more details read documentation.\nhttps://github.com/xsolla/login-unity-sdk/blob/master/README.md");
        }
        else if (XsollaAuthentication.Instance.IsTokenValid)
        {
            Debug.Log("Your token "+XsollaAuthentication.Instance.Token+" is active");
            SceneManager.LoadScene("Game");
        }

        XsollaAuthentication.Instance.OnInvalidProjectSettings += OnInvalidProjectSettings;
        XsollaAuthentication.Instance.OnMultipleLoginUrlsException += OnMultipleLoginUrlsException;
        XsollaAuthentication.Instance.OnSubmittedLoginUrlNotFoundException += OnSubmittedLoginUrlNotFoundException;
        XsollaAuthentication.Instance.OnIdentifiedError += OnIdentifiedError;
        XsollaAuthentication.Instance.OnNetworkError += OnNetworkError;
    }

    private void OnNetworkError()
    {
        Debug.Log("NETWORK ERROR");
    }

    private void OnIdentifiedError(ErrorDescription obj)
    {
        Debug.Log("Identified error: "+obj.code+", "+obj.description+".");
    }

    private void OnSubmittedLoginUrlNotFoundException(ErrorDescription obj)
    {
        Debug.Log("SubmittedLoginUrlNotFoundException: "+obj.code+", "+obj.description+".");
    }

    private void OnMultipleLoginUrlsException(ErrorDescription obj)
    {
        Debug.Log("OnMultipleLoginUrlsException: "+obj.code+", "+obj.description+".");
    }

    private void OnInvalidProjectSettings(ErrorDescription obj)
    {
        Debug.Log("OnInvalidProjectSettings: "+obj.code+", "+obj.description+".");
    }
    private void OnDestroy()
    {
        XsollaAuthentication.Instance.OnInvalidProjectSettings -= OnInvalidProjectSettings;
        XsollaAuthentication.Instance.OnMultipleLoginUrlsException -= OnMultipleLoginUrlsException;
        XsollaAuthentication.Instance.OnSubmittedLoginUrlNotFoundException -= OnSubmittedLoginUrlNotFoundException;
        XsollaAuthentication.Instance.OnIdentifiedError -= OnIdentifiedError;
        XsollaAuthentication.Instance.OnNetworkError -= OnNetworkError;
    }
    private void Awake()
    {
        _changePassword_Panel.GetComponent<IPage>().Close();
        _createAccount_Panel.GetComponent<IPage>().Close();

        _openChangePassw_Btn.onClick.AddListener(() => { _changePassword_Panel.GetComponent<IPage>().Open(); });
        _openCreateAccount_Btn.onClick.AddListener(() => { _createAccount_Panel.GetComponent<IPage>().Open(); });
    }
}
