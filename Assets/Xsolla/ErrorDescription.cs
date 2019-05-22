using System;

namespace Xsolla
{
    public enum Error
    {
        PasswordResetingNotAllowedForProject,
        TokenVerificationException,
        RegistrationNotAllowedException,
        UsernameIsTaken,
        EmailIsTaken,
        UserIsNotActivated,
        CaptchaRequiredException,
        InvalidProjectSettings,
        InvalidLoginOrPassword,
        MultipleLoginUrlsException,
        SubmittedLoginUrlNotFoundException,
        InvalidToken,
        NetworkError,
        IdentifiedError
    }
    [Serializable]
    public class ErrorDescription
    {
        public string code;
        public string description;
        public Error error;
        public ErrorDescription(string _code, string _description, Error _error)
        {
            code = _code;
            description = _description;
            error = _error;
        }
    }
}