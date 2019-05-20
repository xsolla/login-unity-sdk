using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla;

public interface ILogin
{
    void Login();
}
public class LoginPage : Page, ILogin
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private Button login_Btn;
    [SerializeField] private Toggle rememberMe_ChkBox;
    [SerializeField] private Toggle showPassword_Toggle;
    [Header("Swap Button Images")]
    [SerializeField] private Image login_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    private void Awake()
    {
        showPassword_Toggle.onValueChanged.AddListener((mood) => {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        login_Btn.onClick.AddListener(Login);
        login_InputField.onValueChanged.AddListener(ChangeButtonImage);
        password_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (login_InputField.text != "" && password_InputField.text.Length > 5)
        {
            if (login_Image.sprite != enabled_Sprite)
                login_Image.sprite = enabled_Sprite;
        }
        else if (login_Image.sprite == enabled_Sprite)
                login_Image.sprite = disabled_Sprite;
    }

    private void Start()
    {
        login_InputField.text = XsollaAuthentication.Instance.LastUserLogin;
        password_InputField.text = XsollaAuthentication.Instance.LastUserPassword;
        XsollaAuthentication.Instance.OnSuccessfulSignIn += OnSignIn;
        XsollaAuthentication.Instance.OnValidToken += OnValidToken;
        XsollaAuthentication.Instance.OnInvalidToken += OnInvalidToken;
    }

    private void OnInvalidToken()
    {
        Debug.Log("Invalid token");
    }

    private void OnValidToken(string obj)
    {
        Debug.Log("Valid token");
    }

    private void OnSignIn(XsollaUser user)
    {
        if (XsollaAuthentication.Instance.IsTokenValid && XsollaAuthentication.Instance.IsJWTValidationToken)
        {
            Debug.Log("Your token " + XsollaAuthentication.Instance.Token + " is active");
        }
        else if (!XsollaAuthentication.Instance.IsJWTValidationToken)
        {
            Debug.Log("Unsafe signed in");
        }
    }

    public void Login()
    {
        if (login_InputField.text != "" && password_InputField.text.Length > 5)
        {
            XsollaAuthentication.Instance.SignIn(login_InputField.text, password_InputField.text, rememberMe_ChkBox.isOn);
        }
        else
            Debug.Log("Fill all fields");
    }
    void OnDestroy()
    {
        XsollaAuthentication.Instance.OnSuccessfulSignIn -= OnSignIn;
        XsollaAuthentication.Instance.OnValidToken -= OnValidToken;
        XsollaAuthentication.Instance.OnInvalidToken -= OnInvalidToken;
    }
}