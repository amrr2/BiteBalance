using UnityEngine;
using System.Threading.Tasks;


public class AuthViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FeedbackMessage { get; private set; }
    public bool IsLoading { get; private set; }
    
    public async Task<bool> TryLogin()
    {
        
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            FeedbackMessage = "Please enter email and password";
            return false;
        }
        
        IsLoading = true;
        FeedbackMessage = "Logging in...";
        
        
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
    
    
    public void Reset()
    {
        Email = "";
        Password = "";
        FeedbackMessage = "";
        IsLoading = false;
    }
}
