using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI totalCaloriesText;
    public Transform mealsContent;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        LoadTodaysMeals();
    }

    void LoadTodaysMeals()
    {
        // Show today's date
        dateText.text = System.DateTime.Now.ToString("dddd, MMMM d");
        
        // Get calories from PlayerPrefs
        int todayCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        totalCaloriesText.text = "Total: " + todayCalories + " kcal";
        
        Debug.Log("Diary loaded. Total calories: " + todayCalories);
    }

    void OnBackClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
}
