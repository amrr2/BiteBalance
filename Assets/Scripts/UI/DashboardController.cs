using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardController : MonoBehaviour
{
    [Header("Calorie Display")]
    public TextMeshProUGUI calorieNumberText;
    public TextMeshProUGUI foodValueText;
    
    [Header("Action Buttons")]
    public Button logMealButton;
    public Button scanFoodButton;
    
    [Header("Bottom Navigation")]
    public Button dashboardBtn;
    public Button diaryBtn;
    public Button progressBtn;
    public Button settingsBtn;
    
    // ViewModel handles all the logic
    private DashboardViewModel viewModel;

    void Start()
    {
        // Create the ViewModel
        viewModel = new DashboardViewModel();
        
        // Load data through ViewModel
        viewModel.LoadTodayData();
        
        // Update UI
        UpdateDisplay();
        
        // Action button listeners
        logMealButton.onClick.AddListener(OnLogMealClicked);
        scanFoodButton.onClick.AddListener(OnScanFoodClicked);
        
        // Bottom navigation listeners
        diaryBtn.onClick.AddListener(GoToDiary);
        progressBtn.onClick.AddListener(GoToProgress);
        settingsBtn.onClick.AddListener(GoToSettings);
    }

    void UpdateDisplay()
    {
        if (calorieNumberText != null)
        {
            calorieNumberText.text = viewModel.RemainingCalories.ToString("N0");
        }
        
        if (foodValueText != null)
        {
            foodValueText.text = viewModel.EatenCalories.ToString("N0");
        }
    }

    void OnLogMealClicked()
    {
        viewModel.GoToLogMeal();
    }
    
    void OnScanFoodClicked()
    {
        viewModel.GoToScanFood();
    }
    
    void GoToDiary()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Diary");
    }
    
    void GoToProgress()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Progress");
    }
    
    void GoToSettings()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Settings");
    }
    
    // Call this when returning from LogMeal scene
    void OnEnable()
    {
        if (viewModel != null)
        {
            viewModel.LoadTodayData();
            UpdateDisplay();
        }
    }
}