using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static BCrypt.Net.BCrypt;

public class LoginController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isRegistration = false;
    public CustomTCP TCP;
    public Button loginButton;
    public Button registerButton;
    /* public Button returnToLoginButton; */
    public TextMeshProUGUI statusText;
    public TMP_InputField accountName;
    public TMP_InputField password;
    public string statusTextUpdate = "";
    public bool statusTextNeedsUpdate = false;

    void Start()
    {
        loginButton.onClick.AddListener(Login);
        registerButton.onClick.AddListener(SwitchToRegistration);
        /* returnToLoginButton.onClick.AddListener(SwitchToLogin); */
        /* string hash = HashPassword("fuck");
        print(hash);
        print(Verify("fuck", hash)); */
    }

    // Update is called once per frame
    void Update()
    {
        if (statusTextNeedsUpdate)
        {
            statusText.text = statusTextUpdate;
            statusTextNeedsUpdate = false;
        } // Clumsy, may need a specialized JoinHandle class to check if needs update of multiple items first.
    }

    public void HandleLoginResponse(string cmd, string[] args)
    {
        switch (cmd)
        {
            case "login_fail":
                statusTextUpdate = "Failed to connect.";
                statusTextNeedsUpdate = true;
                break;
            case "login_success":
                statusTextUpdate = "Success!";
                statusTextNeedsUpdate = true;
                break;
            case "register_success":
                statusTextUpdate = "Registration complete.";
                statusTextNeedsUpdate = true;
                break;
            case "register_fail":
                statusTextUpdate = "Registration error, try different credentials.";
                statusTextNeedsUpdate = true;
                break;
        }
        TCP.Disconnect();
    }

    void Login()
    {
        // !!!!!! TLS SSL REQUIRED HERE !!!!!!!!!
        /* string pass = BCrypt.Net.BCrypt.EnhancedHashPassword(password.text); */
        string pass = HashPassword(password.text);
        switch (isRegistration)
        {
            case false:
                statusText.text = "Attempting connection...";
                Debug.Log("Logging in with hash " + pass);
                TCP.SendCmd("login", accountName.text + ":" + pass);
                break;
            case true:
                statusText.text = "Attempting to register...";
                TCP.SendCmd("register", accountName.text + ":" + pass);
                Debug.Log("Sent hash " + pass);
                break;
        }
    }

    void SwitchToRegistration()
    {
        isRegistration = true;
        loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Register";
        registerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
        registerButton.onClick.RemoveListener(SwitchToRegistration);
        registerButton.onClick.AddListener(SwitchToLogin);

    }
    void SwitchToLogin()
    {
        isRegistration = false;
        loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Launch";
        registerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Register";
        registerButton.onClick.AddListener(SwitchToRegistration);
        registerButton.onClick.RemoveListener(SwitchToLogin);
    }
}
