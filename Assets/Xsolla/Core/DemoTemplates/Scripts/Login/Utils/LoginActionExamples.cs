﻿using System;

public static class LoginProxyActions
{
	public static Action<LoginPageEnterController, object> RunDemoUserAuthDelegate => RunDemoUserAuthMethod;
	public static Action<LoginPageEnterController, object> RunSocialAuthDelegate => RunSocialAuthMethod;
	public static Action<LoginPageEnterController, object> RunSteamAuthDelegate => RunSteamAuth;

	private static void RunDemoUserAuthMethod(LoginPageEnterController loginController, object arg)
	{
		loginController.RunBasicAuth(username: "xsolla", password: "xsolla", rememberMe: true);
	}

	private static void RunSocialAuthMethod(LoginPageEnterController loginController, object arg)
	{
		SocialProvider provider = (SocialProvider)arg;
		loginController.RunSocialAuth(provider);
	}

	private static void RunSteamAuth(LoginPageEnterController loginController, object arg)
	{
		loginController.RunManualSteamAuth();
	}
}
