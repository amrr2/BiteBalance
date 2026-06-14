using UnityEngine;


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
        
        
        ServiceLocator.Register<INavigationService>(new NavigationService());
        
        Debug.Log("Services ready!");
    }
}
