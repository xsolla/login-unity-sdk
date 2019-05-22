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

    // Does it make sense to expose separate actions for all success and error events?
    // Its quite an overhead to subscribe for all this stuff and easy to miss something.
    // Instead you can pass two callbacks (actions) to api methods (i.e. SignIn) - onComplete and onError.
    // This will allow to provide all logic that is required to deal with method call results right in place where it is used.
    // onError callback will receive ErrorDescription as a parameter and corresponding handler decides what to do with it.

    // Also, ErrorDescription class should be enhanced by introducing enumeration for error types - it is way more convenient than using strings.

    public class XsollaAuthentication : MonoSingleton<XsollaAuthentication>
    {
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
                return PlayerPrefs.HasKey("Xsolla_Token") ? PlayerPrefs.GetString("Xsolla_Token") : string.Empty;
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
	            if (PlayerPrefs.HasKey("Xsolla_User_Login") && !string.IsNullOrEmpty(_loginId))
                    return PlayerPrefs.GetString("Xsolla_User_Login");
                else
                    return string.Empty;
            }
        }
        public string LastUserPassword
        {
            get
            {
                try
                {
                    if (PlayerPrefs.HasKey("Xsolla_User_Password") && !string.IsNullOrEmpty(_loginId))
                        return Crypto.Decrypt(Encoding.ASCII.GetBytes(LoginID.Replace("-", string.Empty).Substring(0, 16)), PlayerPrefs.GetString("Xsolla_User_Password"));
                    else
                        return string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
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
        }
        private void SaveLoginPassword(string username, string password)
        {
            if (!string.IsNullOrEmpty(_loginId))
            {
                PlayerPrefs.SetString("Xsolla_User_Login", username);
                PlayerPrefs.SetString("Xsolla_User_Password", Crypto.Encrypt(Encoding.ASCII.GetBytes(LoginID.Replace("-", string.Empty).Substring(0, 16)), password));
            }
        }

        /// <summary>
        /// Sending Reset Password Message to email by login.
        /// </summary>
        public void ResetPassword(string email, Action onSuccessfulResetPassword, Action<ErrorDescription> onError)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", email);

            string proxy = _isProxy ? "proxy/registration/password/reset" : "password/reset/request";

            StartCoroutine(WebRequests.PostRequest(string.Format("https://login.xsolla.com/api/{0}?projectId={1}&engine=unity&engine_v={2}&sdk=login&sdk_v={3}", proxy, _loginId, Application.unityVersion, sdk_v),
                form,
                (status, message) =>
                {
                    ErrorDescription error = CheckForErrors(status, message, CheckResetPasswordError);

                    if (error == null && onSuccessfulResetPassword != null)
                        onSuccessfulResetPassword.Invoke();
                    else if (error != null && onError != null)
                        onError.Invoke(error);
                }));
        }

        /// <summary>
        /// Login
        /// </summary>
        public void SignIn(string username, string password, bool remember_user, Action<XsollaUser> onSuccessfulSignIn, Action<ErrorDescription> onError)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            form.AddField("remember_me", remember_user.ToString());
            
            string proxy = _isProxy ? "proxy/" : string.Empty;

            StartCoroutine(WebRequests.PostRequest(
                string.Format("https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}&engine=unity&engine_v={3}&sdk=login&sdk_v={4}", proxy, _loginId, _callbackURL, Application.unityVersion, sdk_v),
                form,
                (status, message) =>
                {
                    ErrorDescription error = CheckForErrors(status, message, CheckSignInError);
                    if (error != null)
                    {
                        if (onError != null)
                            onError.Invoke(error);
                        return;
                    }

                    Action<XsollaUser> onSuccess = (xsollaUser) =>
                    {
                        if (onSuccessfulSignIn != null)
                            onSuccessfulSignIn.Invoke(xsollaUser);
                        if (remember_user)
                            SaveLoginPassword(username, password);
                    };

                    if (_isJWTValidationToken)
                        JWTValidation(message,
                        (xsollaUser, errorDescription) =>
                        {
                            if (errorDescription != null && onError != null)
                                onError.Invoke(errorDescription);
                            else
                                onSuccess.Invoke(xsollaUser);
                        });
                    else
                        onSuccess.Invoke(new XsollaUser());
                }
                ));
        }


        /// <summary>
        /// Registration
        /// </summary>
        public void Registration(string username, string password, string email, Action onSuccessfulRegistration, Action<ErrorDescription> onError)
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
                    ErrorDescription error = CheckForErrors(status, message, CheckRegistrationError);

                    if (error == null && onSuccessfulRegistration != null)
                        onSuccessfulRegistration.Invoke();
                    else if (error != null && onError != null)
                        onError.Invoke(error);
                }));
        }

        #region Exceptions
        private ErrorDescription CheckForErrors(bool status, string message, Func<string, Error> checkError)
        {
            //status == false == network error
            if (!status)
                return new ErrorDescription(string.Empty, message, Error.NetworkError);

            //else try to deserialize mistake
            MessageJson messageJson = DeserializeError(message);
            //if request got an error
            if (messageJson != null && messageJson.error != null && !string.IsNullOrEmpty(messageJson.error.code))
            {
                messageJson.error.error = checkError(messageJson.error.code);
                if (messageJson.error.error == Error.IdentifiedError)
                    messageJson.error.error = CheckGeneralErrors(messageJson.error.code);

                return messageJson.error;
            }
            //else if success
            return null;
        }
        #region ErrorsCheck
        private Error CheckSignInError(string code)
        {
            switch (code)
            {
                case "003-007":
                    return Error.UserIsNotActivated;
                case "010-007":
                    return Error.CaptchaRequiredException;
                default:
                    return Error.IdentifiedError;
            }
        }

        private Error CheckRegistrationError(string code)
        {
            switch (code)
            {
                case "010-003":
                    return Error.RegistrationNotAllowedException;
                case "003-003":
                    return Error.UsernameIsTaken;
                case "003-004":
                    return Error.EmailIsTaken;
                default:
                    return Error.IdentifiedError;
            }
        }
        private Error CheckResetPasswordError(string code)
        {
            switch (code)
            {
                case "003-007":
                    return Error.PasswordResetingNotAllowedForProject;
                default:
                    return Error.IdentifiedError;
            }
        }
        private Error CheckTokenError(string code)
        {
            switch (code)
            {
                case "422":
                    return Error.TokenVerificationException;
                default:
                    return Error.IdentifiedError;
            }
        }

        private Error CheckGeneralErrors(string code)
        {
            switch (code)
            {
                case "0":
                    return Error.InvalidProjectSettings;
                case "003-001":
                    return Error.InvalidLoginOrPassword;
                case "003-061":
                    return Error.InvalidProjectSettings;
                case "010-011":
                    return Error.MultipleLoginUrlsException;
                case "010-012":
                    return Error.SubmittedLoginUrlNotFoundException;
                default:
                    return Error.IdentifiedError;
            }
        }
        #endregion
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
        private void JWTValidation(string message, Action<XsollaUser, ErrorDescription> onFinishValidate = null)
        {
            string token = ParseToken(message);
            if (!string.IsNullOrEmpty(token))
                ValidateToken(token, (status, recievedMessage) =>
                {
                    ErrorDescription error = CheckForErrors(status, recievedMessage, CheckTokenError);
                    XsollaUser xsollaUser = new XsollaUser();
                    if (error != null)
                    {
                        xsollaUser = JsonUtility.FromJson<TokenJson>(recievedMessage).token_payload;
                        PlayerPrefs.SetString("Xsolla_Token_Exp", xsollaUser.exp);
                    }
                    if (onFinishValidate != null)
                        onFinishValidate.Invoke(xsollaUser, error);
                });
            else if (onFinishValidate != null)
                onFinishValidate.Invoke(new XsollaUser(), new ErrorDescription(string.Empty, "Can't parse token", Error.InvalidToken));
        }
        private string ParseToken(string message)
        {
            Regex regex = new Regex(@"token=\S*[&#]");
            try
            {
                var match = regex.Match(message).Value.Replace("token=", string.Empty);
                match = match.Remove(match.Length - 1);

                var token = match;
                PlayerPrefs.SetString("Xsolla_Token", token);
                return token;
            }
            catch (Exception)
            {
                return string.Empty;
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
}