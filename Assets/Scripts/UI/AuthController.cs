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
    
    // ViewModel handles all the logic
    private AuthViewModel viewModel;

    private void Start()
    {
        // Create the ViewModel
        viewModel = new AuthViewModel();
        
        // Add button listeners
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private async void OnLoginClicked()
    {
        // Pass UI values to ViewModel
        viewModel.Email = emailInput.text;
        viewModel.Password = passwordInput.text;
        
        // ViewModel handles the logic
        bool success = await viewModel.TryLogin();
        
        // Update UI with result from ViewModel
        ShowFeedback(viewModel.FeedbackMessage);

        if (success)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
        }
    }

    private async void OnRegisterClicked()
    {
        // Pass UI values to ViewModel
        viewModel.Email = emailInput.text;
        viewModel.Password = passwordInput.text;
        
        // ViewModel handles the logic
        bool success = await viewModel.TryRegister();
        
        // Update UI with result from ViewModel
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
