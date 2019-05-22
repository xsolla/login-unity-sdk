using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;

namespace Xsolla
{
    // REVIEW 
    
    // Keys for player preferences should be moved to separate class that holds constants ---- Cant do it cause they use constants like login id which was declared here
    // -- Why we can't encapsulate strings like "Xsolla_Token", "Xsolla_Token_Exp" etc.?
    
    // What about creating class that manages all request url formatting stuff? --- useless
    // -- Ok, after refactoring code with strings concatenation it looks much better

    // Does it make sense to expose separate actions for all success and error events?
    // Its quite an overhead to subscribe for all this stuff and easy to miss something.
    // Instead you can pass two callbacks (actions) to api methods (i.e. SignIn) - onComplete and onError.
    // This will allow to provide all logic that is required to deal with method call results right in place where it is used.
    // onError callback will receive ErrorDescription as a parameter and corresponding handler decides what to do with it.
    
    // Also, ErrorDescription class should be enhanced by introducing enumeration for error types - it is way more convenient than using strings.

    public class XsollaAuthentication : MonoSingleton<XsollaAuthentication>
    {
        #region SuccessEvents
        public event Action OnSuccessfulRegistration;
        public event Action<XsollaUser> OnSuccessfulSignIn;
        public event Action OnSuccessfulResetPassword;
        public event Action OnSuccessfulSignOut;
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
        public event Action<ErrorDescription> OnPasswordResetingNotAllowedForProject;
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
        private const string sdk_v = "0.1";
        #endregion
        /// <summary>
        /// Required. You can find it in your project settings. See xsolla.com
        /// </summary>
        public string LoginID
        {
            get
            {
                return _loginId;
            }
            set
            {
                _loginId = value;
            }
        }
        /// <summary>
        /// Required if you have many LoginURLs. You can find it in your project settings. See xsolla.com
        /// </summary>
        public string CallbackURL
        {
            get
            {
                return _callbackURL;
            }
            set
            {
                _callbackURL = value;
            }
        }
        public string JWTValidationURL
        {
            get
            {
                return _JWTValidationURL;
            }
            set
            {
                _JWTValidationURL = value;
            }
        }
        public string Token
        {
            get
            {
                return PlayerPrefs.HasKey("Xsolla_Token") ? PlayerPrefs.GetString("Xsolla_Token") : "";
            }
        }
        public bool IsTokenValid
        {
            get
            {
                long epochTicks = new DateTime(1970, 1, 1).Ticks;
                long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);

                if (PlayerPrefs.HasKey("Xsolla_Token_Exp") && !string.IsNullOrEmpty(PlayerPrefs.GetString("Xsolla_Token_Exp")))
                    return long.Parse(PlayerPrefs.GetString("Xsolla_Token_Exp")) >= unixTime;
                else
                    return false;
            }
        }

        public bool IsJWTValidationToken
        {
            get
            {
                return _isJWTValidationToken;
            }

            set
            {
                _isJWTValidationToken = value;
            }
        }
        public bool IsProxy
        {
            get
            {
                return _isProxy;
            }

            set
            {
                _isProxy = value;
            }
        }
        public string LastUserLogin
        {
            get
            {
	            // REVIEW Please use string.Empty instead of "" literal - it makes code look a bit cleaner :)
	            if (PlayerPrefs.HasKey("Xsolla_User_Login") && !string.IsNullOrEmpty(_loginId))
                    return PlayerPrefs.GetString("Xsolla_User_Login");
                else
                    return "";
            }
        }
        public string LastUserPassword
        {
            get
            {
                try
                {
                    if (PlayerPrefs.HasKey("Xsolla_User_Password") && !string.IsNullOrEmpty(_loginId))
                        return Crypto.Decrypt(Encoding.ASCII.GetBytes(LoginID.Replace("-", "").Substring(0, 16)), PlayerPrefs.GetString("Xsolla_User_Password"));
                    else
                        return "";
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        [SerializeField]
        private string _loginId;
        [SerializeField]
        private bool _isJWTValidationToken;
        [SerializeField]
        private string _JWTValidationURL;
        [SerializeField]
        private bool _isProxy;
        [SerializeField]
        private string _callbackURL;

        /// <summary>
        /// Clear Token 
        /// </summary>
        public void SignOut()
        {
            if (PlayerPrefs.HasKey("Xsolla_Token"))
                PlayerPrefs.DeleteKey("Xsolla_Token");
            if (PlayerPrefs.HasKey("Xsolla_Token_Exp"))
                PlayerPrefs.DeleteKey("Xsolla_Token_Exp");
            if (OnSuccessfulSignOut != null)
                OnSuccessfulSignOut.Invoke();
        }

        /// <summary>
        /// Sending Reset Password Message to email by login.
        /// </summary>
        public void ResetPassword(string email)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", email);

            string proxy = _isProxy ? "proxy/registration/password/reset" : "password/reset/request";

            StartCoroutine(WebRequests.PostRequest(string.Format("https://login.xsolla.com/api/{0}?projectId={1}&engine=unity&engine_v={2}&sdk=login&sdk_v={3}", proxy, _loginId, Application.unityVersion, sdk_v),
                form,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckResetPasswordError) && OnSuccessfulResetPassword != null)
                        OnSuccessfulResetPassword.Invoke();
                }));
        }

        /// <summary>
        /// Login
        /// </summary>
        public void SignIn(string username, string password, bool remember_user)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            form.AddField("remember_me", remember_user.ToString());
            
            string proxy = _isProxy ? "proxy/" : "";

            StartCoroutine(WebRequests.PostRequest(
                string.Format("https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}&engine=unity&engine_v={3}&sdk=login&sdk_v={4}", proxy, _loginId, _callbackURL, Application.unityVersion, sdk_v),
                form,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckSignInError))
                    {
                        if (_isJWTValidationToken)
                            JWTValidation(message, () => { if (remember_user) SaveLoginPassword(username, password); });
                        else if (OnSuccessfulSignIn != null)
                        {
                            OnSuccessfulSignIn.Invoke(new XsollaUser());
                            if (remember_user)
                                SaveLoginPassword(username, password);
                        }
                    }
                }
                ));
        }

        private void SaveLoginPassword(string username, string password)
        {
            if (!string.IsNullOrEmpty(_loginId))
            {
                PlayerPrefs.SetString("Xsolla_User_Login", username);
                PlayerPrefs.SetString("Xsolla_User_Password", Crypto.Encrypt(Encoding.ASCII.GetBytes(LoginID.Replace("-", "").Substring(0, 16)), password));
            }
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
            
            string proxy = _isProxy ? "proxy/registration" : "user";

            StartCoroutine(WebRequests.PostRequest(
                string.Format("https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}&engine=unity&engine_v={3}&sdk=login&sdk_v={4}", proxy, _loginId, _callbackURL, Application.unityVersion, sdk_v),
                registrationForm,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckRegistrationError) && OnSuccessfulRegistration != null)
                        OnSuccessfulRegistration.Invoke();
                }));
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
                    if (!errorShowStatus && OnIdentifiedError != null)
                        OnIdentifiedError.Invoke(messageJson.error);
                    return true;
                }
                //else if success
                return false;
            }
            else 
            {
                if (OnNetworkError != null)
                    OnNetworkError.Invoke();
                return true;
            }
        }

        private bool CheckResetPasswordError(ErrorDescription errorDescription)
        {
            switch (errorDescription.code)
            {
                case "003-007":
                if (OnPasswordResetingNotAllowedForProject != null)
                    OnPasswordResetingNotAllowedForProject.Invoke(errorDescription);
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
                if (OnVerificationException != null)
                    OnVerificationException.Invoke(errorDescription);
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
                if (OnRegistrationNotAllowedException != null)
                    OnRegistrationNotAllowedException.Invoke(errorDescription);
                    break;
                case "003-003":
                if (OnUsernameIsTaken != null)
                    OnUsernameIsTaken.Invoke(errorDescription);
                    break;
                case "003-004":
                if (OnEmailIsTaken != null)
                    OnEmailIsTaken.Invoke(errorDescription);
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
                if (OnUserIsNotActivated != null)
                    OnUserIsNotActivated.Invoke(errorDescription);
                    break;
                case "010-007":
                if (OnCaptchaRequiredException != null)
                        OnCaptchaRequiredException.Invoke(errorDescription);
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
                if (OnInvalidProjectSettings != null)
                    OnInvalidProjectSettings.Invoke(errorDescription);
                    break;
                case "003-001":
                if (OnInvalidLoginOrPassword != null)
                    OnInvalidLoginOrPassword.Invoke(errorDescription);
                    break;
                case "003-061":
                if (OnInvalidProjectSettings != null)
                    OnInvalidProjectSettings.Invoke(errorDescription);
                    break;
                case "010-011":
                if (OnMultipleLoginUrlsException != null)
                    OnMultipleLoginUrlsException.Invoke(errorDescription);
                    break;
                case "010-012":
                if (OnSubmittedLoginUrlNotFoundException != null)
                    OnSubmittedLoginUrlNotFoundException.Invoke(errorDescription);
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
            catch (Exception) { }
            return message;
        }
        #endregion
        #region TOKEN
        private void JWTValidation(string message, Action onFinishValidate = null)
        {
            string token = ParseToken(message);
            if (!string.IsNullOrEmpty(token))
                ValidateToken(token, (status, recievedMessage) =>
                {
                    if (!CheckForErrors(status, recievedMessage, CheckTokenError))
                    {
                        XsollaUser xsollaUser = JsonUtility.FromJson<TokenJson>(recievedMessage).token_payload;
                        PlayerPrefs.SetString("Xsolla_Token_Exp", xsollaUser.exp);
                        if (OnValidToken != null)
                            OnValidToken.Invoke(token);
                        if (OnSuccessfulSignIn != null)
                            OnSuccessfulSignIn.Invoke(xsollaUser);
                        if (onFinishValidate != null)
                            onFinishValidate.Invoke();
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
            catch (Exception)
            {
                if (OnInvalidToken != null)
                    OnInvalidToken.Invoke();
                return "";
            }
        }

        private void ValidateToken(string token, Action<bool, string> onRecievedToken)
        {
            WWWForm form = new WWWForm();
            form.AddField("token", token);
            StartCoroutine(WebRequests.PostRequest(_JWTValidationURL, form, onRecievedToken));
        }

        #endregion
    }
    
    // REVIEW Json classes can be moved to separate files as well - its a good practice to have separate files for different classes.
    
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
    internal class MessageJson
    {
        public ErrorDescription error;
    }
    [Serializable]
    public class ErrorDescription
    {
        public string code;
        public string description;
    }
    
    // REVIEW Do we need URLJson class at all?
    [Serializable]
    internal class URLJson
    {
        public string url;
    }
    [Serializable]
    internal class TokenJson
    {
        public XsollaUser token_payload;
    }
#endregion
}