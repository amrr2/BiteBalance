using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardController : MonoBehaviour
{
    public Button logMealButton;
    
    public TextMeshProUGUI calorieNumberText;
    
    public TextMeshProUGUI foodValueText;
    
    public Button scanFoodButton;
    private int goalCalories = 2000;

    void Start()
    {
        logMealButton.onClick.AddListener(OpenLogMeal);
        UpdateCalorieDisplay();
        scanFoodButton.onClick.AddListener(OpenScanFood);
    }

    void UpdateCalorieDisplay()
    {
        int eatenCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        int remaining = goalCalories - eatenCalories;
        
        if (calorieNumberText != null)
        {
            calorieNumberText.text = remaining.ToString("N0");
        }
        
        if (foodValueText != null)
        {
            foodValueText.text = eatenCalories.ToString("N0");
        }
    }

    void OpenLogMeal()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LogMeals");
    }
    void OpenScanFood()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ScanFood");
    }
}
