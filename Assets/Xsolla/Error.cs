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
}