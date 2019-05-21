using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xsolla;

public class AuthController : MonoBehaviour
{
    [SerializeField] private GameObject changePassword_Panel;
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
        OpenPage(loginSignUp_Panel);
        OpenPage(login_Panel);
        popUp_Controller.GetComponent<IPopUpController>().OnClosePopUp = OpenSaved;

        var returnToTheMain = new UnityAction(()=>
        {
            CloseAll();
            OpenPage(loginSignUp_Panel);
            OpenPage(login_Panel);
        });
        closeSuccessChangePassword_Btn.onClick.AddListener(returnToTheMain);
        closeChangePassword_Btn.onClick.AddListener(returnToTheMain);
        openChangePassw_Btn.onClick.AddListener(() => { CloseAll(); OpenPage(changePassword_Panel); });
        openSignUp_Btn.onClick.AddListener(() => {
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

        SubscribeOnXsollaEvents();
    }

    private void OpenPopUp(string message, PopUpWindows popUp)
    {
        CloseAndSave();
        popUp_Controller.GetComponent<IPopUpController>().ShowPopUp(message, popUp);
        Debug.Log(message);
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromXsollaEvents();
    }

    #region XsollaEvents
    private void SubscribeOnXsollaEvents()
    {
        XsollaAuthentication.Instance.OnSuccessfulRegistration += () => OpenPopUp("User registration is successful. Check and confirm " + signUp_Panel.GetComponent<ISignUp>().SignUpEmail + " now!", PopUpWindows.Success);
        XsollaAuthentication.Instance.OnSuccessfulSignIn += (user) => OpenPopUp("User authentication is completed! You can use Auth JWT token now!", PopUpWindows.Success);
        XsollaAuthentication.Instance.OnSuccessfulResetPassword += () => Debug.Log("OnSuccessfulResetPassword");
        XsollaAuthentication.Instance.OnSuccessfulSignOut += () => OpenPopUp("OnSuccessfulSignOut", PopUpWindows.Success);

        XsollaAuthentication.Instance.OnInvalidLoginOrPassword += (error) => OpenPopUp("Wrong username or password", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnUserIsNotActivated += (obj) => OpenPopUp("User is not activated" + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnPasswordResetingNotAllowedForProject += (obj) => OpenPopUp("Password Reseting Not Allowed For Project " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnCaptchaRequiredException += (obj) => OpenPopUp("Captcha Required Exception: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException += (obj) => OpenPopUp("Registration Not Allowed Exception: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnUsernameIsTaken += (obj) => OpenPopUp("Username Is Taken: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnEmailIsTaken += (obj) => OpenPopUp("Email Is Taken: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error); ;
        XsollaAuthentication.Instance.OnInvalidProjectSettings += (obj) => OpenPopUp("OnInvalidProjectSettings: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnMultipleLoginUrlsException += (obj) => OpenPopUp("OnMultipleLoginUrlsException: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnSubmittedLoginUrlNotFoundException += (obj) => OpenPopUp("SubmittedLoginUrlNotFoundException: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnIdentifiedError += (obj) => OpenPopUp("Identified error: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnNetworkError += () => OpenPopUp("NETWORK ERROR", PopUpWindows.Error);
    }
    private void UnsubscribeFromXsollaEvents()
    {
        XsollaAuthentication.Instance.OnSuccessfulRegistration -= () => OpenPopUp("User registration is successful. Check and confirm " + signUp_Panel.GetComponent<ISignUp>().SignUpEmail + " now!", PopUpWindows.Success);
        XsollaAuthentication.Instance.OnSuccessfulSignIn -= (user) => OpenPopUp("User authentication is completed! You can use Auth JWT token now!", PopUpWindows.Success);
        XsollaAuthentication.Instance.OnSuccessfulResetPassword -= () => Debug.Log("OnSuccessfulResetPassword");
        XsollaAuthentication.Instance.OnSuccessfulSignOut -= () => OpenPopUp("OnSuccessfulSignOut", PopUpWindows.Success);

        XsollaAuthentication.Instance.OnInvalidLoginOrPassword -= (error) => OpenPopUp("Wrong username or password", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnUserIsNotActivated -= (obj) => OpenPopUp("User is not activated" + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnPasswordResetingNotAllowedForProject -= (obj) => OpenPopUp("Password Reseting Not Allowed For Project " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnCaptchaRequiredException -= (obj) => OpenPopUp("Captcha Required Exception: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException -= (obj) => OpenPopUp("Registration Not Allowed Exception: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnUsernameIsTaken -= (obj) => OpenPopUp("Username Is Taken: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnEmailIsTaken -= (obj) => OpenPopUp("Email Is Taken: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error); ;
        XsollaAuthentication.Instance.OnInvalidProjectSettings -= (obj) => OpenPopUp("OnInvalidProjectSettings: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnMultipleLoginUrlsException -= (obj) => OpenPopUp("OnMultipleLoginUrlsException: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnSubmittedLoginUrlNotFoundException -= (obj) => OpenPopUp("SubmittedLoginUrlNotFoundException: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnIdentifiedError -= (obj) => OpenPopUp("Identified error: " + obj.code + ", " + obj.description + ".", PopUpWindows.Error);
        XsollaAuthentication.Instance.OnNetworkError -= () => OpenPopUp("NETWORK ERROR", PopUpWindows.Error);
    }
    #endregion
}
