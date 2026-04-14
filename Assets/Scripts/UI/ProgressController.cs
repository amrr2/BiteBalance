using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI averageCaloriesText;
    public TextMeshProUGUI totalCaloriesText;
    public TextMeshProUGUI streakText;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        LoadProgress();
    }

    void LoadProgress()
    {
        // Get today's calories
        int todayCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        
        // For now, show simple stats
        averageCaloriesText.text = "Today: " + todayCalories + " kcal";
        totalCaloriesText.text = "Goal: 2000 kcal";
        
        // Calculate if under or over goal
        int remaining = 2000 - todayCalories;
        if (remaining >= 0)
        {
            streakText.text = remaining + " kcal remaining";
        }
        else
        {
            streakText.text = Mathf.Abs(remaining) + " kcal over goal";
        }
        
        Debug.Log("Progress loaded");
    }

    void OnBackClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
}
