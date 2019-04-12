using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
    public class XsollaAuthentication : MonoBehaviour
    {
        #region SuccessEvents

        public event Action OnSuccesfulRegistration;
        public event Action<XsollaUser> OnSuccesfulSignIn;
        public event Action OnSuccesfulResetPassword;
        public event Action<string> OnValidToken;

        #endregion

        #region ExceptionsEvents

        #region Token

        public event Action OnInvalidToken;

        //422
        public event Action<ErrorDescription> OnVerificationException;

        #endregion

        #region SignIN

        //010-007
        public event Action<ErrorDescription> OnCaptchaRequiredException;

        //003-007
        public event Action<ErrorDescription> OnUserIsNotActivated;

        #endregion

        #region ResetPassword

        //0 or 003-061
        public event Action<ErrorDescription> OnPassworResetingNotAllowedForProject;

        #endregion

        #region Registration

        //010-003
        public event Action<ErrorDescription> OnRegistrationNotAllowedException;

        //003-003
        public event Action<ErrorDescription> OnUsernameIsTaken;

        //003-004
        public event Action<ErrorDescription> OnEmailIsTaken;

        #endregion

        #region General

        //0 or 003-061
        public event Action<ErrorDescription> OnInvalidProjectSettings;

        //003-001 --- Sign In, Reset Password
        public event Action<ErrorDescription> OnInvalidLoginOrPassword;

        //010-011
        public event Action<ErrorDescription> OnMultipleLoginUrlsException;

        //010-012
        public event Action<ErrorDescription> OnSubmittedLoginUrlNotFoundException;

        #endregion

        public event Action<ErrorDescription> OnIdentifiedError;
        public event Action OnNetworkError;

        #endregion

        #region ApiHeaders

        const string sdk_v = "0.1";

        #endregion

        /// <summary>
        /// Required. You can find it in your project settings. See xsolla.com
        /// </summary>
        public string ProjectId
        {
            get { return _projectId; }
            set { _projectId = value; }
        }

        /// <summary>
        /// Required if you have many LoginURLs. You can find it in your project settings. See xsolla.com
        /// </summary>
        public string LoginURL
        {
            get { return _loginUrl; }
            set { _loginUrl = value; }
        }

        public string ValidationJWTTokenURL
        {
            get => _validationJWTTokenURL;
            set => _validationJWTTokenURL = value;
        }

        public string Token
        {
            get { return PlayerPrefs.HasKey("Xsolla_Token") ? PlayerPrefs.GetString("Xsolla_Token") : ""; }
        }

        public bool IsTokenValid
        {
            get
            {
                long epochTicks = new DateTime(1970, 1, 1).Ticks;
                long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
                return PlayerPrefs.HasKey("Xsolla_Token_Exp")
                    ? PlayerPrefs.GetString("Xsolla_Token_Exp") != "" ? (long.Parse(PlayerPrefs.GetString("Xsolla_Token_Exp"))) >= unixTime : false
                    : false;
            }
        }

        [SerializeField] private string _projectId;
        [SerializeField] private string _validationJWTTokenURL;
        [SerializeField] private string _loginUrl;

        public static XsollaAuthentication Instance = null;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Log Out
        /// </summary>
        public void SignOut()
        {
            if (PlayerPrefs.HasKey("Xsolla_Token"))
                PlayerPrefs.DeleteKey("Xsolla_Token");
            if (PlayerPrefs.HasKey("Xsolla_Token_Exp"))
                PlayerPrefs.DeleteKey("Xsolla_Token_Exp");
            Debug.Log("Succesfully log out");
        }

        /// <summary>
        /// Sending Reset Password Message to email by login.
        /// </summary>
        public void ResetPassword(string login)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", login);

            StartCoroutine(PostRequest($"https://login.xsolla.com/api/password/reset/request?" +
                                       $"projectId={_projectId}&engine=unity&engine_v={Application.version}&sdk=login&sdk_v={sdk_v}", form,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckResetPasswordError))
                        OnSuccesfulResetPassword?.Invoke();
                }));
        }

        /// <summary>
        /// Login
        /// </summary>
        public void SignIn(string username, string password)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            form.AddField("remember_me", "true");
            StartCoroutine(PostRequest($"https://login.xsolla.com/api/login?projectId={_projectId}&login_url={_loginUrl}" +
                                       $"&engine=unity&engine_v={Application.version}&sdk=login&sdk_v={sdk_v}", form,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckSignInError))
                        CheckToken(message);
                }
            ));
        }

        /// <summary>
        /// Registration
        /// </summary>
        public void Registration(string username, string password, string email)
        {
            WWWForm registrationForm = new WWWForm();
            registrationForm.AddField("username", username);
            registrationForm.AddField("password", password);
            registrationForm.AddField("email", email);

            StartCoroutine(PostRequest($"https://login.xsolla.com/api/user?projectId={_projectId}&login_url={_loginUrl}" +
                                       $"&engine=unity&engine_v={Application.version}&sdk=login&sdk_v={sdk_v}", registrationForm,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckRegistrationError))
                        OnSuccesfulRegistration?.Invoke();
                }));
        }

        /// <summary>
        /// Get all enabled social media links
        /// </summary>
        public void GetEnabledSocial(Action<SocialServices> callback)
        {
            StartCoroutine(GetRequest($"https://login.xsolla.com/api/social/login_urls?projectId={_projectId}" +
                                      $"&engine=unity&engine_v={Application.version}&sdk=login&sdk_v={sdk_v}",
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, null))
                    {
                        callback?.Invoke(ParceSocial(message));
                    }
                }));
        }

        private SocialServices ParceSocial(string message)
        {
            SocialServices social = new SocialServices();
            try
            {
                social = JsonUtility.FromJson<SocialServices>(message);
            }
            catch (Exception)
            {
            }

            return social;
        }

        #region Exceptions

        private bool CheckForErrors(bool status, string message, Func<ErrorDescription, bool> checkError)
        {
            //if it is not a network error
            if (status)
            {
                //try to deserialize mistake
                MessageJson messageJson = DeserializeError(message);
                bool errorShowStatus = false;
                //if postRequest got an error
                if (messageJson != null && messageJson.error != null && messageJson.error.code != null)
                {
                    //check for general errors
                    errorShowStatus = CheckGeneralErrors(messageJson.error);
                    //if it is not a general error check for registration error
                    if (!errorShowStatus)
                        errorShowStatus = checkError(messageJson.error);
                    //else if it is not a general and not a registration error generate indentified error
                    if (!errorShowStatus)
                        OnIdentifiedError?.Invoke(messageJson.error);
                    return true;
                }

                //else if success
                return false;
            }
            else
            {
                OnNetworkError?.Invoke();
                return true;
            }
        }

        private bool CheckResetPasswordError(ErrorDescription errorDescription)
        {
            switch (errorDescription.code)
            {
                case "003-007":
                    OnPassworResetingNotAllowedForProject?.Invoke(errorDescription);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool CheckTokenError(ErrorDescription errorDescription)
        {
            switch (errorDescription.code)
            {
                case "422":
                    OnVerificationException?.Invoke(errorDescription);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool CheckRegistrationError(ErrorDescription errorDescription)
        {
            switch (errorDescription.code)
            {
                case "010-003":
                    OnRegistrationNotAllowedException?.Invoke(errorDescription);
                    break;
                case "003-003":
                    OnUsernameIsTaken?.Invoke(errorDescription);
                    break;
                case "003-004":
                    OnEmailIsTaken?.Invoke(errorDescription);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool CheckSignInError(ErrorDescription errorDescription)
        {
            switch (errorDescription.code)
            {
                case "003-007":
                    OnUserIsNotActivated?.Invoke(errorDescription);
                    break;
                case "010-007":
                    OnCaptchaRequiredException?.Invoke(errorDescription);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool CheckGeneralErrors(ErrorDescription errorDescription)
        {
            switch (errorDescription.code)
            {
                case "0":
                    OnInvalidProjectSettings?.Invoke(errorDescription);
                    break;
                case "003-001":
                    OnInvalidLoginOrPassword?.Invoke(errorDescription);
                    break;
                case "003-061":
                    OnInvalidProjectSettings?.Invoke(errorDescription);
                    break;
                case "010-011":
                    OnMultipleLoginUrlsException?.Invoke(errorDescription);
                    break;
                case "010-012":
                    OnSubmittedLoginUrlNotFoundException?.Invoke(errorDescription);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private MessageJson DeserializeError(string recievedMessage)
        {
            MessageJson message = new MessageJson();

            try
            {
                message = JsonUtility.FromJson<MessageJson>(recievedMessage);
            }
            catch (Exception)
            {
            }

            return message;
        }

        #endregion

        #region TOKEN

        private void CheckToken(string message, Action onFinishValidate = null)
        {
            string token = ParseToken(message);
            if (token != "")
                ValidateToken(token, (status, recievedMessage) =>
                {
                    if (!CheckForErrors(status, recievedMessage, CheckTokenError))
                    {
                        XsollaUser xsollaUser = JsonUtility.FromJson<TokenJson>(recievedMessage).token_payload;
                        PlayerPrefs.SetString("Xsolla_Token_Exp", xsollaUser.exp);
                        OnValidToken?.Invoke(token);
                        OnSuccesfulSignIn?.Invoke(xsollaUser);
                        onFinishValidate?.Invoke();
                    }
                });
        }

        private string ParseToken(string message)
        {
            Regex regex = new Regex(@"token=\S*[&#]");
            try
            {
                var match = regex.Match(message).Value.Replace("token=", "");
                match = match.Remove(match.Length - 1);

                var token = match;
                PlayerPrefs.SetString("Xsolla_Token", token);
                return token;
            }
            catch (Exception e)
            {
                OnInvalidToken?.Invoke();
                return "";
            }
        }

        private void ValidateToken(string token, Action<bool, string> onRecievedToken)
        {
            WWWForm form = new WWWForm();
            form.AddField("token", token);
            StartCoroutine(PostRequest(_validationJWTTokenURL, form, onRecievedToken));
        }

        #endregion

        #region URL

        private string ParseUrl(string message)
        {
            try
            {
                string url = (JsonUtility.FromJson<URLJson>(message)).url;
                return url;
            }
            catch (Exception)
            {
                OnInvalidToken?.Invoke();
                return "";
            }
        }

        private void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        #endregion

        #region WebRequest

        private IEnumerator PostRequest(string url, WWWForm form, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SendWebRequest();

            while (!request.isDone)
            {
                //wait 
                yield return new WaitForEndOfFrame();
            }

            if (request.isNetworkError)
            {
                callback?.Invoke(false, "");
            }
            else
            {
                string recievedMessage = request.downloadHandler.text;
                callback?.Invoke(true, recievedMessage);
            }
        }

        private IEnumerator GetRequest(string uri, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(uri);
            request.SendWebRequest();

            while (!request.isDone)
            {
                //wait 
                yield return new WaitForEndOfFrame();
            }

            if (request.isNetworkError)
            {
                callback?.Invoke(false, "");
            }
            else
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
        }

        #endregion
    }

    public enum Social
    {
        amazon,
        baidu,
        battlenet,
        china_telecom,
        discord,
        facebook,
        github,
        google,
        instagram,
        kakao,
        linkedin,
        mailru,
        microsoft,
        msn,
        naver,
        odnoklassniki,
        paypal,
        pinterest,
        qq,
        reddit,
        steam,
        twitchtv,
        twitter,
        vimeo,
        vk,
        wechat,
        weibo,
        yahoo,
        yandex,
        youtube
    }

    #region JsonClasses

    [Serializable]
    public struct XsollaUser
    {
        public string exp;
        public string iss;
        public string iat;
        public string username;
        public string xsolla_login_access_key;
        public string sub;
        public string email;
        public string xsolla_login_project_id;
        public string publisher_id;
        public string provider;
        public string name;
        public bool is_linked;
    }

    [Serializable]
    class MessageJson
    {
        public ErrorDescription error;
    }

    [Serializable]
    public class ErrorDescription
    {
        public string code;
        public string description;
    }

    [Serializable]
    class URLJson
    {
        public string url;
    }

    [Serializable]
    class TokenJson
    {
        public XsollaUser token_payload;
    }

    [Serializable]
    public class SocialServices
    {
        public string amazon;
        public string baidu;
        public string battlenet;
        public string china_telecom;
        public string discord;
        public string facebook;
        public string github;
        public string google;
        public string instagram;
        public string kakao;
        public string linkedin;
        public string mailru;
        public string microsoft;
        public string msn;
        public string naver;
        public string odnoklassniki;
        public string paypal;
        public string pinterest;
        public string qq;
        public string reddit;
        public string steam;
        public string twitchtv;
        public string twitter;
        public string vimeo;
        public string vk;
        public string wechat;
        public string weibo;
        public string yahoo;
        public string yandex;
        public string youtube;
    }

    #endregion
}