using UnityEngine;


public class DashboardViewModel
{
    public int GoalCalories { get; set; } = 2000;
    public int EatenCalories { get; private set; }
    public int RemainingCalories { get; private set; }
    
    private INavigationService navigation;
    
    public DashboardViewModel()
    {
        
        if (ServiceLocator.Has<INavigationService>())
        {
            navigation = ServiceLocator.Get<INavigationService>();
        }
    }
    
    
    public void LoadTodayData()
    {
        EatenCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        RemainingCalories = GoalCalories - EatenCalories;
        
        Debug.Log("Loaded calories - Eaten: " + EatenCalories + ", Remaining: " + RemainingCalories);
    }
    
    
    public void AddCalories(int amount)
    {
        EatenCalories += amount;
        RemainingCalories = GoalCalories - EatenCalories;
        
        
        PlayerPrefs.SetInt("TodayCalories", EatenCalories);
        PlayerPrefs.Save();
        
        Debug.Log("Added " + amount + " calories. Total: " + EatenCalories);
    }
    
    
    public void ResetDaily()
    {
        EatenCalories = 0;
        RemainingCalories = GoalCalories;
        PlayerPrefs.SetInt("TodayCalories", 0);
        PlayerPrefs.Save();
        
        Debug.Log("Daily calories reset");
    }
    
    
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
