using UnityEngine;

// Handles dashboard calorie calculations and data
// UI (DashboardController) uses this for display values
public class DashboardViewModel
{
    public int GoalCalories { get; set; } = 2000;
    public int EatenCalories { get; private set; }
    public int RemainingCalories { get; private set; }
    
    private INavigationService navigation;
    
    public DashboardViewModel()
    {
        // Try to get navigation service if available
        if (ServiceLocator.Has<INavigationService>())
        {
            navigation = ServiceLocator.Get<INavigationService>();
        }
    }
    
    // Load today's calorie data
    public void LoadTodayData()
    {
        EatenCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        RemainingCalories = GoalCalories - EatenCalories;
        
        Debug.Log("Loaded calories - Eaten: " + EatenCalories + ", Remaining: " + RemainingCalories);
    }
    
    // Add calories when user logs a meal
    public void AddCalories(int amount)
    {
        EatenCalories += amount;
        RemainingCalories = GoalCalories - EatenCalories;
        
        // Save to PlayerPrefs
        PlayerPrefs.SetInt("TodayCalories", EatenCalories);
        PlayerPrefs.Save();
        
        Debug.Log("Added " + amount + " calories. Total: " + EatenCalories);
    }
    
    // Reset daily calories (call at midnight or manually)
    public void ResetDaily()
    {
        EatenCalories = 0;
        RemainingCalories = GoalCalories;
        PlayerPrefs.SetInt("TodayCalories", 0);
        PlayerPrefs.Save();
        
        Debug.Log("Daily calories reset");
    }
    
    // Navigation methods
    public void GoToLogMeal()
    {
        if (navigation != null)
        {
            navigation.GoTo("LogMeals");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LogMeals");
        }
    }
    
    public void GoToScanFood()
    {
        if (navigation != null)
        {
            navigation.GoTo("ScanFood");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ScanFood");
        }
    }
}
