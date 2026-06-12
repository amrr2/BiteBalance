using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardController : MonoBehaviour
{
    [Header("Calorie Display")]
    public TextMeshProUGUI calorieNumberText;
    public TextMeshProUGUI foodValueText;

    [Header("Macros Display")]
    public TextMeshProUGUI proteinText;
    public TextMeshProUGUI carbsText;
    public TextMeshProUGUI fatText;

    [Header("Action Buttons")]
    public Button logMealButton;
    public Button scanFoodButton;
    public Button bmiButton;

    [Header("Bottom Navigation")]
    public Button dashboardBtn;
    public Button diaryBtn;
    public Button progressBtn;
    public Button settingsBtn;

    private DashboardViewModel viewModel;

    void Start()
    {

        viewModel = new DashboardViewModel();

        viewModel.LoadTodayData();

        UpdateDisplay();

        logMealButton.onClick.AddListener(OnLogMealClicked);
        scanFoodButton.onClick.AddListener(OnScanFoodClicked);

        if (bmiButton != null)
        {
            bmiButton.onClick.AddListener(GoToBMI);
        }

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

        UpdateMacros();
    }

    void UpdateMacros()
    {

        float protein = PlayerPrefs.GetFloat("TodayProtein", 0);
        float carbs = PlayerPrefs.GetFloat("TodayCarbs", 0);
        float fat = PlayerPrefs.GetFloat("TodayFat", 0);

        if (proteinText != null)
        {
            proteinText.text = "Protein\n" + protein.ToString("F0") + "g";
        }

        if (carbsText != null)
        {
            carbsText.text = "Carbs\n" + carbs.ToString("F0") + "g";
        }

        if (fatText != null)
        {
            fatText.text = "Fat\n" + fat.ToString("F0") + "g";
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

    void GoToBMI()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BMICalculator");
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

    void OnEnable()
    {
        if (viewModel != null)
        {
            viewModel.LoadTodayData();
            UpdateDisplay();
        }
    }
}
