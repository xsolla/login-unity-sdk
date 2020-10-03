using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public partial class DemoImplementation : MonoBehaviour, IDemoImplementation
{
#region Token
	public Token Token
	{
		get => XsollaLogin.Instance.Token;
		set => XsollaLogin.Instance.Token = value;
	}

	public Token GetDemoUserToken()
	{
		XsollaSettings.JwtTokenInvalidationEnabled = false;
		return "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI";
	}
	public void SaveToken(string key, string token) => XsollaLogin.Instance.SaveToken(key, token);
	public bool LoadToken(string key, out string token) => XsollaLogin.Instance.LoadToken(key, out token);
	public void DeleteToken(string key) => XsollaLogin.Instance.DeleteToken(key);
	public void ValidateToken(string token, Action<string> onSuccess = null, Action<Error> onError = null)
	{
		GetUserInfo(token, useCache: false, onSuccess:info => onSuccess?.Invoke(token), onError:onError);
	}
#endregion

#region User

	public void GetUserInfo(string token, Action<UserInfo> onSuccess = null, Action<Error> onError = null)
	{
		GetUserInfo(token, useCache: true, onSuccess, onError);
	}

	private readonly Dictionary<string, UserInfo> _userCache = new Dictionary<string, UserInfo>();
	public void GetUserInfo(string token, bool useCache = true, Action<UserInfo> onSuccess = null, Action<Error> onError = null)
	{
		if (useCache && _userCache.ContainsKey(token))
			onSuccess?.Invoke(_userCache[token]);
		else
			XsollaLogin.Instance.GetUserInfo(token, info =>
			{
				_userCache[token] = info;
				onSuccess?.Invoke(info);
			}, onError);
	}

	public void Registration(string username, string password, string email, Action onSuccess, Action<Error> onError = null)
	{
		XsollaLogin.Instance.Registration(username, password, email, onSuccess, onError);
	}

	public void SignIn(string username, string password, bool rememberUser, Action onSuccess, Action<Error> onError = null)
	{
		XsollaLogin.Instance.SignIn(username, password, rememberUser, onSuccess, onError);
	}

	public void ResetPassword(string username, Action onSuccess, Action<Error> onError = null)
	{
		XsollaLogin.Instance.ResetPassword(username, onSuccess, onError);
	}
#endregion

#region Social
	public void SteamAuth(string appId, string sessionTicket, Action<string> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.SteamAuth(appId, sessionTicket, onSuccess, onError);
	}

	public string GetSocialNetworkAuthUrl(SocialProvider socialProvider)
	{
		return XsollaLogin.Instance.GetSocialNetworkAuthUrl(socialProvider);
	}
#endregion

#region AccountLinking
	public void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase)
	{
		XsollaLogin.Instance.SignInConsoleAccount(userId, platform, successCase, failedCase);
	}

	public void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
	{
		XsollaLogin.Instance.RequestLinkingCode(onSuccess, onError);
	}

	public void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
	{
		XsollaLogin.Instance.LinkConsoleAccount(userId, platform, confirmationCode, onSuccess, onError);
	}
#endregion

#region Attributes
	public void GetUserAttributes(string token, string projectId, UserAttributeType attributeType,
		List<string> attributeKeys, string userId, Action<List<UserAttribute>> onSuccess, Action<Error> onError)
	{
		XsollaLogin.Instance.GetUserAttributes(token, projectId, attributeType, attributeKeys, userId, onSuccess, onError);
	}

	public void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
	{
		XsollaLogin.Instance.UpdateUserAttributes(token, projectId, attributes, onSuccess, onError);
	}

	public void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError)
	{
		XsollaLogin.Instance.RemoveUserAttributes(token, projectId, attributeKeys, onSuccess, onError);
	}
#endregion

#region OAuth2.0
	public bool IsOAuthTokenRefreshInProgress => XsollaLogin.Instance.IsOAuthTokenRefreshInProgress;

	public void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.ExchangeCodeToToken(code, onSuccessExchange, onError);
	}
#endregion
}
