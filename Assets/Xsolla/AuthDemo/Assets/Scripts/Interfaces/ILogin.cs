using System;
using Xsolla;

public interface ILogin
{
    void Login();
    Action<XsollaUser> OnSuccessfulLogin { get; set; }
    Action<ErrorDescription> OnUnsuccessfulLogin { get; set; }
}
