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

    private AuthViewModel viewModel;

    private void Start()
    {

        viewModel = new AuthViewModel();

        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private async void OnLoginClicked()
    {

        viewModel.Email = emailInput.text;
        viewModel.Password = passwordInput.text;

        bool success = await viewModel.TryLogin();

        ShowFeedback(viewModel.FeedbackMessage);

        if (success)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
        }
    }

    private async void OnRegisterClicked()
    {

        viewModel.Email = emailInput.text;
        viewModel.Password = passwordInput.text;

        bool success = await viewModel.TryRegister();

        ShowFeedback(viewModel.FeedbackMessage);
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
