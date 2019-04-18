﻿using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.Security;
using System.Text;
using System.Security.Cryptography;
using System.IO;

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
        private const string sdk_v = "0.1";
        #endregion
        /// <summary>
        /// Required. You can find it in your project settings. See xsolla.com
        /// </summary>
        public string LoginProjectID
        {
            get
            {
                return _loginProjectId;
            }
            set
            {
                _loginProjectId = value;
            }
        }
        /// <summary>
        /// Required if you have many LoginURLs. You can find it in your project settings. See xsolla.com
        /// </summary>
        public string CallbackURL
        {
            get
            {
                return _callbackUrl;
            }
            set
            {
                _callbackUrl = value;
            }
        }
        public string JWTvalidationURL
        {
            get
            {
                return _JWTvalidationURL;
            }
            set
            {
                _JWTvalidationURL = value;
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
                return PlayerPrefs.HasKey("Xsolla_Token_Exp") ? PlayerPrefs.GetString("Xsolla_Token_Exp") != "" ?
                    (long.Parse(PlayerPrefs.GetString("Xsolla_Token_Exp"))) >= unixTime : false : false;
            }
        }

        public bool IsJWTvalidationToken
        {
            get
            {
                return _isJWTvalidationToken;
            }

            set
            {
                _isJWTvalidationToken = value;
            }
        }
        public string LastUserLogin
        {
            get
            {
                return PlayerPrefs.HasKey("Xsolla_User_Login") && (_loginProjectId != null && _loginProjectId.Length > 0) 
                    ? PlayerPrefs.GetString("Xsolla_User_Login") 
                    : "";
            }
        }
        public string LastUserPassword
        {
            get
            {
                return PlayerPrefs.HasKey("Xsolla_User_Password") && (_loginProjectId != null && _loginProjectId.Length > 0) 
                    ? Crypto.Decrypt(Encoding.ASCII.GetBytes(LoginProjectID.Replace("-", "").Substring(0, 16)), PlayerPrefs.GetString("Xsolla_User_Password")) 
                    : "";
            }
        }
        [SerializeField]
        private string _loginProjectId;
        [SerializeField]
        private bool _isJWTvalidationToken;
        [SerializeField]
        private string _JWTvalidationURL;
        [SerializeField]
        private string _callbackUrl;

        public static XsollaAuthentication Instance = null;

        private void Awake()
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
        /// Clear Token 
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

            StartCoroutine(PostRequest("https://login.xsolla.com/api/password/reset/request?projectId="+_loginProjectId+"&engine=unity&engine_v="+Application.version+"&sdk=login&sdk_v="+sdk_v, form,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckResetPasswordError) && OnSuccesfulResetPassword != null)
                        OnSuccesfulResetPassword.Invoke();
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
            form.AddField("remember_me", "true");
            StartCoroutine(PostRequest("https://login.xsolla.com/api/login?projectId="+_loginProjectId+"&login_url="+_callbackUrl+"&engine=unity&engine_v="+Application.version+"&sdk=login&sdk_v="+sdk_v, form,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckSignInError))
                    {
                        if (_isJWTvalidationToken)
                            JWTValidation(message, () => { if (remember_user) SaveLoginPassword(username, password); });
                        else if (OnSuccesfulSignIn != null)
                        {
                            OnSuccesfulSignIn.Invoke(new XsollaUser());
                            if (remember_user)
                                SaveLoginPassword(username, password);
                        }
                    }
                }
                ));
        }

        private void SaveLoginPassword(string username, string password)
        {
            if (_loginProjectId != null && _loginProjectId.Length > 0)
            {
                PlayerPrefs.SetString("Xsolla_User_Login", username);
                PlayerPrefs.SetString("Xsolla_User_Password", Crypto.Encrypt(Encoding.ASCII.GetBytes(LoginProjectID.Replace("-", "").Substring(0, 16)), password));
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

            StartCoroutine(PostRequest("https://login.xsolla.com/api/user?projectId="+_loginProjectId+"&login_url="+_callbackUrl+"&engine=unity&engine_v="+Application.version+"&sdk=login&sdk_v="+sdk_v, registrationForm,
                (status, message) =>
                {
                    if (!CheckForErrors(status, message, CheckRegistrationError) && OnSuccesfulRegistration != null)
                        OnSuccesfulRegistration.Invoke();
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
                if (OnPassworResetingNotAllowedForProject != null)
                    OnPassworResetingNotAllowedForProject.Invoke(errorDescription);
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
            if (token != "")
                ValidateToken(token, (status, recievedMessage) =>
                {
                    if (!CheckForErrors(status, recievedMessage, CheckTokenError))
                    {
                        XsollaUser xsollaUser = JsonUtility.FromJson<TokenJson>(recievedMessage).token_payload;
                        PlayerPrefs.SetString("Xsolla_Token_Exp", xsollaUser.exp);
                        if (OnValidToken != null)
                            OnValidToken.Invoke(token);
                        if (OnSuccesfulSignIn != null)
                            OnSuccesfulSignIn.Invoke(xsollaUser);
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
            StartCoroutine(PostRequest(_JWTvalidationURL, form, onRecievedToken));
        }
        #endregion
        #region WebRequest
        private IEnumerator PostRequest(string url, WWWForm form, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, form);

#if UNITY_2018_1_OR_NEWER
            request.SendWebRequest();
#else
            request.Send();
#endif

            while (!request.isDone)
            {
                //wait 
                yield return new WaitForEndOfFrame();
            }
            if (request.isNetworkError && callback != null)
            {
                callback.Invoke(false, "");
            }
            else if (callback != null)
            {
                callback.Invoke(true, request.downloadHandler.text);
            }
        }
        private IEnumerator GetRequest(string uri, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(uri);

#if UNITY_2018_1_OR_NEWER
            request.SendWebRequest();
#else
            request.Send();
#endif
            while (!request.isDone)
            {
                //wait 
                yield return new WaitForEndOfFrame();
            }
            if (request.isNetworkError && callback != null)
            {
                callback.Invoke(false, "");
            }
            else if (callback != null)
            {
                callback.Invoke(true, request.downloadHandler.text);
            }
        }
    }
    #endregion
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
    class Crypto
    {
        public static string Encrypt(byte[] key, string dataToEncrypt)
        {
            try
            {
                // Initialize
                AesManaged encryptor = new AesManaged();
                // Set the key
                encryptor.Key = key;
                encryptor.IV = key;
                // create a memory stream
                using (MemoryStream encryptionStream = new MemoryStream())
                {
                    // Create the crypto stream
                    using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // Encrypt
                        byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(dataToEncrypt);
                        encrypt.Write(utfD1, 0, utfD1.Length);
                        encrypt.FlushFinalBlock();
                        encrypt.Close();
                        // Return the encrypted data
                        return Convert.ToBase64String(encryptionStream.ToArray());
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string Decrypt(byte[] key, string encryptedString)
        {
            try
            {
                // Initialize
                AesManaged decryptor = new AesManaged();
                byte[] encryptedData = Convert.FromBase64String(encryptedString);
                // Set the key
                decryptor.Key = key;
                decryptor.IV = key;
                // create a memory stream
                using (MemoryStream decryptionStream = new MemoryStream())
                {
                    // Create the crypto stream
                    using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        // Encrypt
                        decrypt.Write(encryptedData, 0, encryptedData.Length);
                        decrypt.Flush();
                        decrypt.Close();
                        // Return the unencrypted data
                        byte[] decryptedData = decryptionStream.ToArray();
                        return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}