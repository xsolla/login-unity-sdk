using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla;

public class AuthController : MonoBehaviour
{
    [SerializeField] private GameObject changePassword_Panel;
    [SerializeField] private GameObject loginSignUp_Panel;
    [SerializeField] private GameObject signUp_Panel;
    [SerializeField] private GameObject login_Panel;
    [SerializeField] private GameObject message_Panel;
    [SerializeField] private GameObject popUp_Controller;

    [SerializeField] private Button openChangePassw_Btn;
    [SerializeField] private Button openSignUp_Btn;
    [SerializeField] private Button openLogin_Btn;
    [SerializeField] private Button closeChangePassword_Btn;
    [SerializeField] private Button closeSuccessChangePassword_Btn;

    private void Awake()
    {
        changePassword_Panel.GetComponent<IPage>().Close();
        signUp_Panel.GetComponent<IPage>().Close();
        login_Panel.GetComponent<IPage>().Open();
        loginSignUp_Panel.GetComponent<IPage>().Open();
        openLogin_Btn.GetComponent<IPanelVisualElement>().Select();
        var returnToTheMain = new UnityAction(()=>
        {
            CloseAll();
            loginSignUp_Panel.GetComponent<IPage>().Open();
            login_Panel.GetComponent<IPage>().Open();
        });
        closeSuccessChangePassword_Btn.onClick.AddListener(returnToTheMain);
        closeChangePassword_Btn.onClick.AddListener(returnToTheMain);
        openChangePassw_Btn.onClick.AddListener(() => { CloseAll(); changePassword_Panel.GetComponent<IPage>().Open(); });
        openSignUp_Btn.onClick.AddListener(() => {
            CloseAll();
            signUp_Panel.GetComponent<IPage>().Open();
            loginSignUp_Panel.GetComponent<IPage>().Open();
            openSignUp_Btn.GetComponent<IPanelVisualElement>().Select();
            openLogin_Btn.GetComponent<IPanelVisualElement>().Deselect();
        });
        openLogin_Btn.onClick.AddListener(() =>
        {
            CloseAll();
            loginSignUp_Panel.GetComponent<IPage>().Open();
            login_Panel.GetComponent<IPage>().Open();
            openLogin_Btn.GetComponent<IPanelVisualElement>().Select();
            openSignUp_Btn.GetComponent<IPanelVisualElement>().Deselect();
        });
    }
    private void CloseAll()
    {
        loginSignUp_Panel.GetComponent<IPage>().Close();
        changePassword_Panel.GetComponent<IPage>().Close();
        signUp_Panel.GetComponent<IPage>().Close();
        login_Panel.GetComponent<IPage>().Close();
    }
    private void Start()
    {
        if (XsollaAuthentication.Instance.LoginID == "")
        {
            message_Panel.SetActive(true);
            Debug.Log("Please register on the Xsolla website, and fill the Login ID form. For more details read documentation.\nhttps://github.com/xsolla/login-unity-sdk/blob/master/README.md");
        }
        else if (XsollaAuthentication.Instance.IsTokenValid)
        {
            Debug.Log("Your token "+XsollaAuthentication.Instance.Token+" is active");
        }

        XsollaAuthentication.Instance.OnSuccessfulRegistration += () => OpenPopUp("OnSuccessfulRegistration", true);
        XsollaAuthentication.Instance.OnSuccessfulSignIn += (user) => OpenPopUp("OnSuccessfulSignIn", true);
        XsollaAuthentication.Instance.OnSuccessfulResetPassword += () => OpenPopUp("OnSuccessfulResetPassword", true);
        XsollaAuthentication.Instance.OnSuccessfulSignOut += () => OpenPopUp("OnSuccessfulSignOut", true);

        XsollaAuthentication.Instance.OnInvalidLoginOrPassword += (error) => OpenPopUp("Invalid Login Or Password", false);
        XsollaAuthentication.Instance.OnUserIsNotActivated += (obj) => OpenPopUp("User is not activated" + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnPasswordResetingNotAllowedForProject += (obj) => OpenPopUp("Password Reseting Not Allowed For Project " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnCaptchaRequiredException += (obj) => OpenPopUp("Captcha Required Exception: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException += (obj) => OpenPopUp("Registration Not Allowed Exception: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnUsernameIsTaken += (obj) => OpenPopUp("Username Is Taken: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnEmailIsTaken += (obj) => OpenPopUp("Email Is Taken: " + obj.code + ", " + obj.description + ".", false); ;
        XsollaAuthentication.Instance.OnInvalidProjectSettings += (obj) => OpenPopUp("OnInvalidProjectSettings: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnMultipleLoginUrlsException += (obj) => OpenPopUp("OnMultipleLoginUrlsException: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnSubmittedLoginUrlNotFoundException += (obj) => OpenPopUp("SubmittedLoginUrlNotFoundException: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnIdentifiedError += (obj) => OpenPopUp("Identified error: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnNetworkError += () => OpenPopUp("NETWORK ERROR", false);
    }
    
    private void OpenPopUp(string message, bool isSuccess)
    {
        popUp_Controller.GetComponent<IPopUpController>().ShowPopUp(message, isSuccess);
        Debug.Log(message);
    }
    
    private void OnDestroy()
    {
        XsollaAuthentication.Instance.OnSuccessfulRegistration -= () => OpenPopUp("OnSuccessfulRegistration", true);
        XsollaAuthentication.Instance.OnSuccessfulSignIn -= (user) => OpenPopUp("OnSuccessfulSignIn", true);
        XsollaAuthentication.Instance.OnSuccessfulResetPassword -= () => OpenPopUp("OnSuccessfulResetPassword", true);
        XsollaAuthentication.Instance.OnSuccessfulSignOut -= () => OpenPopUp("OnSuccessfulSignOut", true);

        XsollaAuthentication.Instance.OnInvalidLoginOrPassword -= (error) => OpenPopUp("Invalid Login Or Password", false);
        XsollaAuthentication.Instance.OnUserIsNotActivated -= (obj) => OpenPopUp("User is not activated" + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnPasswordResetingNotAllowedForProject -= (obj) => OpenPopUp("Password Reseting Not Allowed For Project " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnCaptchaRequiredException -= (obj) => OpenPopUp("Captcha Required Exception: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException -= (obj) => OpenPopUp("Registration Not Allowed Exception: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnUsernameIsTaken -= (obj) => OpenPopUp("Username Is Taken: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnEmailIsTaken -= (obj) => OpenPopUp("Email Is Taken: " + obj.code + ", " + obj.description + ".", false); ;
        XsollaAuthentication.Instance.OnInvalidProjectSettings -= (obj) => OpenPopUp("OnInvalidProjectSettings: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnMultipleLoginUrlsException -= (obj) => OpenPopUp("OnMultipleLoginUrlsException: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnSubmittedLoginUrlNotFoundException -= (obj) => OpenPopUp("SubmittedLoginUrlNotFoundException: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnIdentifiedError -= (obj) => OpenPopUp("Identified error: " + obj.code + ", " + obj.description + ".", false);
        XsollaAuthentication.Instance.OnNetworkError -= () => OpenPopUp("NETWORK ERROR", false);
    }
}
