using UnityEngine;
using UnityEngine.SceneManagement;

// Handles all scene navigation in one place
public class NavigationService : INavigationService
{
    public string CurrentScene 
    { 
        get { return SceneManager.GetActiveScene().name; }
    }
    
    public void GoTo(string sceneName)
    {
        Debug.Log("Navigating to: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
    
    public void GoBack()
    {
        // For now just go to dashboard as default
        GoTo("Dashboard");
    }
}
