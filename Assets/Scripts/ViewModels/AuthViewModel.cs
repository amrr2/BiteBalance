using UnityEngine;
using System.Threading.Tasks;

// Handles login and register logic
// UI (AuthController) uses this for all auth operations
public class AuthViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FeedbackMessage { get; private set; }
    public bool IsLoading { get; private set; }
    
    public async Task<bool> TryLogin()
    {
        // Validate input
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            FeedbackMessage = "Please enter email and password";
            return false;
        }
        
        IsLoading = true;
        FeedbackMessage = "Logging in...";
        
        // Call Firebase through the manager
        bool success = await FirebaseManager.Instance.SignInUser(Email, Password);
        
        IsLoading = false;
        
        if (success)
        {
            FeedbackMessage = "Login successful!";
            Debug.Log("Login successful for: " + Email);
        }
        else
        {
            FeedbackMessage = "Login failed. Check your credentials.";
            Debug.Log("Login failed for: " + Email);
        }
        
        return success;
    }
    
    public async Task<bool> TryRegister()
    {
        // Validate input
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            FeedbackMessage = "Please enter email and password";
            return false;
        }
        
        if (Password.Length < 6)
        {
            FeedbackMessage = "Password must be at least 6 characters";
            return false;
        }
        
        IsLoading = true;
        FeedbackMessage = "Creating account...";
        
        bool success = await FirebaseManager.Instance.RegisterUser(Email, Password);
        
        IsLoading = false;
        
        if (success)
        {
            FeedbackMessage = "Account created! You can now login.";
            Debug.Log("Account created for: " + Email);
        }
        else
        {
            FeedbackMessage = "Registration failed. Try a different email.";
            Debug.Log("Registration failed for: " + Email);
        }
        
        return success;
    }
    
    // Clear the form
    public void Reset()
    {
        Email = "";
        Password = "";
        FeedbackMessage = "";
        IsLoading = false;
    }
}
