using UnityEngine;

// Initializes all services when the app starts
// Add this to an empty GameObject in your first scene
public class AppSetup : MonoBehaviour
{
    private static bool isInitialized = false;
    
    void Awake()
    {
        if (isInitialized) 
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        SetupServices();
        isInitialized = true;
    }
    
    void SetupServices()
    {
        Debug.Log("Setting up services...");
        
        // Register navigation service
        ServiceLocator.Register<INavigationService>(new NavigationService());
        
        Debug.Log("Services ready!");
    }
}
