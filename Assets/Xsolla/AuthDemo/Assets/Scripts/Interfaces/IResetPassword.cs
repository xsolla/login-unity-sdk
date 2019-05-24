using System;
using Xsolla;

public interface IResetPassword
{
    void ResetPassword();
    Action OnSuccessfulResetPassword { get; set; }
    Action<ErrorDescription> OnUnsuccessfulResetPassword { get; set; }
}
