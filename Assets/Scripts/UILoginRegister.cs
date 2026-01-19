using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // si fas servir TextMeshPro

public class UILoginRegister : MonoBehaviour
{
    [Header("Components UI")]
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private TextMeshProUGUI messageText;     // ← per mostrar errors/èxits

    private DatabaseManager dbManager;

    private void Start()
    {
        dbManager = FindObjectOfType<DatabaseManager>();

        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);

        ClearMessage();
    }

    private void OnRegister()
    {
        string user = usernameField.text.Trim();
        string pass = passwordField.text;

        string result = dbManager.RegisterUser(user, pass);

        if (result == "OK")
        {
            ShowMessage("Registre completat! Pots iniciar sessió.", Color.green);
        }
        else
        {
            ShowMessage(result, Color.red);
        }
    }

    private void OnLogin()
    {
        string user = usernameField.text.Trim();
        string pass = passwordField.text;

        var (success, message, userId) = dbManager.LoginUser(user, pass);

        if (success)
        {
            ShowMessage("Benvingut!", Color.green);
            // Guardem qui ha iniciat sessió (opció molt recomanable)
            PlayerPrefs.SetInt("CurrentUserID", userId);
            PlayerPrefs.SetString("CurrentUsername", user);
            PlayerPrefs.Save();

            // Canvi d'escena després d'un petit retard (més professional)
            Invoke(nameof(LoadMainScene), 1.2f);
        }
        else
        {
            ShowMessage(message, Color.red);
        }
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowMessage(string msg, Color color)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.color = color;
        }
        else
        {
            Debug.Log(msg);
        }
    }

    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = "";
    }
}