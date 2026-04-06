using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthController : MonoBehaviour
{
    [Header("Login UI")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;
    public TextMeshProUGUI feedbackText;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject dashboardPanel;

    private void Start()
    {
        // Add button listeners
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private async void OnLoginClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("Please enter email and password");
            return;
        }

        ShowFeedback("Logging in...");

        bool success = await FirebaseManager.Instance.SignInUser(email, password);

        if (success)
        {
            ShowFeedback("Login successful!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
            // Load dashboard or next screen
            if (dashboardPanel != null)
            {
                loginPanel.SetActive(false);
                dashboardPanel.SetActive(true);
            }
        }
        else
        {
            ShowFeedback("Login failed. Check your credentials.");
        }
    }

    private async void OnRegisterClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("Please enter email and password");
            return;
        }

        if (password.Length < 6)
        {
            ShowFeedback("Password must be at least 6 characters");
            return;
        }

        ShowFeedback("Creating account...");

        bool success = await FirebaseManager.Instance.RegisterUser(email, password);

        if (success)
        {
            ShowFeedback("Account created! You can now login.");
        }
        else
        {
            ShowFeedback("Registration failed. Try a different email.");
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        Debug.Log(message);
    }
}