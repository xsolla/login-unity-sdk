/// <summary>
/// Social providers list for Login Social Auth.
/// See full list at <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
/// </summary>
public enum SocialProvider
{
    None,
    Facebook,
    GitHub,
    Google,
    Twitch,
    Twitter,
    Steam,
    Xbox
}

public static class SocialProviderConverter
{
    public static string GetParameter(this SocialProvider provider)
    {
        switch (provider)
        {
            case SocialProvider.Facebook: return "facebook";
            case SocialProvider.GitHub: return "github";
            case SocialProvider.Google: return "google";
            case SocialProvider.Twitch: return "twitch";
            case SocialProvider.Twitter: return "twitter";
            case SocialProvider.Steam: return "steam";
            case SocialProvider.Xbox: return "xbox";
            default: return string.Empty;
        }
    }
}