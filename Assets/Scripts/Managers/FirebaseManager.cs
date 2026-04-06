using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }
    public DatabaseReference DatabaseRef { get; private set; }

    public bool IsInitialized { get; private set; } = false;

    public event Action OnFirebaseInitialized;
    public event Action<FirebaseUser> OnUserSignedIn;
    public event Action OnUserSignedOut;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Auth = FirebaseAuth.DefaultInstance;
                DatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                IsInitialized = true;

                Auth.StateChanged += OnAuthStateChanged;

                Debug.Log("Firebase initialized successfully!");
                OnFirebaseInitialized?.Invoke();
            }
            else
            {
                Debug.LogError("Could not initialize Firebase: " + task.Result);
            }
        });
    }

    private void OnAuthStateChanged(object sender, EventArgs e)
    {
        if (Auth.CurrentUser != CurrentUser)
        {
            bool signedIn = Auth.CurrentUser != null;
            CurrentUser = Auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("User signed in: " + CurrentUser.Email);
                OnUserSignedIn?.Invoke(CurrentUser);
            }
            else
            {
                Debug.Log("User signed out");
                OnUserSignedOut?.Invoke();
            }
        }
    }

    // Register new user
    public async Task<bool> RegisterUser(string email, string password)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log("User registered: " + result.User.Email);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Registration failed: " + e.Message);
            return false;
        }
    }

    // Sign in existing user
    public async Task<bool> SignInUser(string email, string password)
    {
        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log("User signed in: " + result.User.Email);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Sign in failed: " + e.Message);
            return false;
        }
    }

    // Sign out
    public void SignOut()
    {
        Auth.SignOut();
    }

    // Save data to database
    public async Task SaveUserData(string userId, string path, string jsonData)
    {
        try
        {
            await DatabaseRef.Child("users").Child(userId).Child(path).SetRawJsonValueAsync(jsonData);
            Debug.Log("Data saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    // Load data from database
    public async Task<string> LoadUserData(string userId, string path)
    {
        try
        {
            var snapshot = await DatabaseRef.Child("users").Child(userId).Child(path).GetValueAsync();
            if (snapshot.Exists)
            {
                return snapshot.GetRawJsonValue();
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError("Load failed: " + e.Message);
            return null;
        }
    }

    private void OnDestroy()
    {
        if (Auth != null)
        {
            Auth.StateChanged -= OnAuthStateChanged;
        }
    }
}