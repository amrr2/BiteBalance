using UnityEngine;
using UnityEngine.SceneManagement;


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
        
        GoTo("Dashboard");
    }
}
