using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xsolla;

public class AuthController : MonoBehaviour
{
    [SerializeField] private GameObject resetPassword_Panel;
    [SerializeField] private GameObject loginSignUp_Panel;
    [SerializeField] private GameObject signUp_Panel;
    [SerializeField] private GameObject login_Panel;
    [SerializeField] private GameObject popUp_Controller;

    [SerializeField] private Button openChangePassw_Btn;
    [SerializeField] private Button openSignUp_Btn;
    [SerializeField] private Button openLogin_Btn;
    [SerializeField] private Button closeChangePassword_Btn;
    [SerializeField] private Button closeSuccessChangePassword_Btn;

    private List<GameObject> opened_Pages = new List<GameObject>();

    private void Awake()
    {
        PagesController();
        PagesEvents();
    }

    private void PagesEvents()
    {
        signUp_Panel.GetComponent<ISignUp>().OnSuccessfulSignUp = () => OpenPopUp(string.Format("User registration is successful. Check and confirm {0} now!", signUp_Panel.GetComponent<ISignUp>().SignUpEmail), PopUpWindows.Success);
        signUp_Panel.GetComponent<ISignUp>().OnUnsuccessfulSignUp = OnError;
        resetPassword_Panel.GetComponent<IResetPassword>().OnUnsuccessfulResetPassword = OnError;
        login_Panel.GetComponent<ILogin>().OnUnsuccessfulLogin = OnError;
        login_Panel.GetComponent<ILogin>().OnSuccessfulLogin = (user) => OpenPopUp("User authentication is completed! You can use Auth JWT token now!", PopUpWindows.Success);
    }
    
    private void PagesController()
    {
        OpenPage(loginSignUp_Panel);
        openLogin_Btn.GetComponent<IPanelVisualElement>().Select();
        OpenPage(login_Panel);
        popUp_Controller.GetComponent<IPopUpController>().OnClosePopUp = OpenSaved;

        var returnToTheMain = new UnityAction(() =>
        {
            CloseAll();
            OpenPage(loginSignUp_Panel);
            OpenPage(login_Panel);
        });
        closeSuccessChangePassword_Btn.onClick.AddListener(returnToTheMain);
        closeChangePassword_Btn.onClick.AddListener(returnToTheMain);
        openChangePassw_Btn.onClick.AddListener(() => { CloseAll(); OpenPage(resetPassword_Panel); });
        openSignUp_Btn.onClick.AddListener(() =>
        {
            CloseAll();
            OpenPage(signUp_Panel);
            OpenPage(loginSignUp_Panel);
            openSignUp_Btn.GetComponent<IPanelVisualElement>().Select();
            openLogin_Btn.GetComponent<IPanelVisualElement>().Deselect();
        });
        openLogin_Btn.onClick.AddListener(() =>
        {
            CloseAll();
            OpenPage(login_Panel);
            OpenPage(loginSignUp_Panel);
            openLogin_Btn.GetComponent<IPanelVisualElement>().Select();
            openSignUp_Btn.GetComponent<IPanelVisualElement>().Deselect();
        });
    }

    private void OpenPage(GameObject page)
    {
        page.GetComponent<IPage>().Open();
        opened_Pages.Add(page);
    }
    private void OpenSaved()
    {
        foreach (var page in opened_Pages)
        {
            page.GetComponent<IPage>().Open();
        }
    }

    private void CloseAndSave()
    {
        foreach (var page in opened_Pages)
        {
            page.GetComponent<IPage>().Close();
        }
    }

    private void CloseAll()
    {
        foreach (var page in opened_Pages)
        {
            page.GetComponent<IPage>().Close();
        }
        opened_Pages = new List<GameObject>();
    }

    private void Start()
    {
        if (XsollaAuthentication.Instance.LoginID == "")
        {
            OpenPopUp("Please register Xsolla Publisher Account, and fill the Login ID form.", PopUpWindows.Warning);
            Debug.Log("Please register Xsolla Publisher Account, and fill the Login ID form. For more details read documentation.\nhttps://github.com/xsolla/login-unity-sdk/blob/master/README.md");
        }
        else if (XsollaAuthentication.Instance.IsTokenValid)
        {
            Debug.Log("Your token " + XsollaAuthentication.Instance.Token + " is active");
        }
    }

    private void OpenPopUp(string message, PopUpWindows popUp)
    {
        CloseAndSave();
        popUp_Controller.GetComponent<IPopUpController>().ShowPopUp(message, popUp);
        Debug.Log(message);
    }
    private void OnError(ErrorDescription errorDescription)
    {
        switch (errorDescription.error)
        {
            case Error.PasswordResetingNotAllowedForProject:
                OpenPopUp(string.Format("Password Reseting Not Allowed For Project {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.TokenVerificationException:
                OpenPopUp(string.Format("Token Verification Exception: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.RegistrationNotAllowedException:
                OpenPopUp(string.Format("Registration Not Allowed Exception: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.UsernameIsTaken:
                OpenPopUp(string.Format("Username Is Taken: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.EmailIsTaken:
                OpenPopUp(string.Format("Email Is Taken: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.UserIsNotActivated:
                OpenPopUp(string.Format("User is not activated: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.CaptchaRequiredException:
                OpenPopUp(string.Format("Captcha Required Exception: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.InvalidProjectSettings:
                OpenPopUp(string.Format("OnInvalidProjectSettings: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.InvalidLoginOrPassword:
                OpenPopUp("Wrong username or password", PopUpWindows.Error);
                break;
            case Error.MultipleLoginUrlsException:
                OpenPopUp(string.Format("OnMultipleLoginUrlsException: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.SubmittedLoginUrlNotFoundException:
                OpenPopUp(string.Format("SubmittedLoginUrlNotFoundException: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
            case Error.InvalidToken:
                OpenPopUp("Invalid Token", PopUpWindows.Error);
                break;
            case Error.NetworkError:
                OpenPopUp(string.Format("Network Error: {0}", errorDescription.description), PopUpWindows.Error);
                break;
            case Error.IdentifiedError:
                OpenPopUp(string.Format("Identified error: {0}, {1}.", errorDescription.code, errorDescription.description), PopUpWindows.Error);
                break;
        }
    }
}
