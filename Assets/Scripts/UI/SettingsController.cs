using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;

public class SettingsController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI emailText;
    public TMP_InputField goalInput;
    public Button saveButton;
    public Button logoutButton;
    public Button backButton;

    private int calorieGoal = 2000;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        saveButton.onClick.AddListener(OnSaveClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);
        
        LoadSettings();
    }

    void LoadSettings()
    {
        // Show user email
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            emailText.text = user.Email;
        }
        else
        {
            emailText.text = "Not logged in";
        }
        
        // Load calorie goal
        calorieGoal = PlayerPrefs.GetInt("CalorieGoal", 2000);
        goalInput.text = calorieGoal.ToString();
        
        Debug.Log("Settings loaded");
    }

    void OnSaveClicked()
    {
        // Save calorie goal
        if (int.TryParse(goalInput.text, out int newGoal))
        {
            calorieGoal = newGoal;
            PlayerPrefs.SetInt("CalorieGoal", calorieGoal);
            PlayerPrefs.Save();
            Debug.Log("Calorie goal saved: " + calorieGoal);
        }
    }

    void OnLogoutClicked()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        Debug.Log("User logged out");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void OnBackClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
}
