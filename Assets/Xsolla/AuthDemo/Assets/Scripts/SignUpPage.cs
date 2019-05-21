using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public interface ISignUp
{
    void SignUp();
    string SignUpEmail { get; }
}
public class SignUpPage :  Page, ISignUp
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Toggle showPassword_Toggle;
    [SerializeField] private Button create_Btn;
    [Header("Swap Button Images")]
    [SerializeField] private Image signUp_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    public string SignUpEmail
    {
        get
        {
            return email_InputField.text;
        }
    }

    private void Awake()
    {
        create_Btn.onClick.AddListener(SignUp);
        showPassword_Toggle.onValueChanged.AddListener((mood) => {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        login_InputField.onValueChanged.AddListener(ChangeButtonImage);
        password_InputField.onValueChanged.AddListener(ChangeButtonImage);
        email_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (login_InputField.text != "" && email_InputField.text != "" && password_InputField.text.Length > 5)
        {
            if (signUp_Image.sprite != enabled_Sprite)
                signUp_Image.sprite = enabled_Sprite;
        }
        else if (signUp_Image.sprite == enabled_Sprite)
            signUp_Image.sprite = disabled_Sprite;
    }

    private void Start()
    {
        XsollaAuthentication.Instance.OnSuccessfulRegistration += OnSuccesfulRegistration;
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException += OnRegistrationNotAllowed;
        XsollaAuthentication.Instance.OnUsernameIsTaken += OnUsernameIsTaken;
        XsollaAuthentication.Instance.OnEmailIsTaken += OnEmailIsTaken;
    }

    private void OnSuccesfulRegistration()
    {
        Debug.Log("Succesfully registrated");
    }

    private void OnEmailIsTaken(ErrorDescription error)
    {
        Debug.Log("The email is already taken");
    }

    private void OnUsernameIsTaken(ErrorDescription error)
    {
        Debug.Log("The username is already taken");
    }

    private void OnRegistrationNotAllowed(ErrorDescription error)
    {
        Debug.Log("Password Registration Not Allowed "+error.code+", "+error.description);
    }
    private void OnDestroy()
    {
        XsollaAuthentication.Instance.OnSuccessfulRegistration -= OnSuccesfulRegistration;
        XsollaAuthentication.Instance.OnRegistrationNotAllowedException -= OnRegistrationNotAllowed;
        XsollaAuthentication.Instance.OnUsernameIsTaken -= OnUsernameIsTaken;
        XsollaAuthentication.Instance.OnEmailIsTaken -= OnEmailIsTaken;
    }
    public void SignUp()
    {
        if (login_InputField.text != "" && password_InputField.text.Length >= 6 && email_InputField.text != "")
        {
            XsollaAuthentication.Instance.Registration(login_InputField.text, password_InputField.text, email_InputField.text);
        }
        else
            Debug.Log("Fill all fields");
    }

}
