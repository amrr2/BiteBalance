using UnityEngine;
using System.Threading.Tasks;

// Handles food scanning logic
public class ScanFoodViewModel
{
    public bool IsScanning { get; private set; }
    public bool HasResult { get; private set; }
    public FoodItem ScannedFood { get; private set; }
    public string FeedbackMessage { get; private set; }
    
    // Start camera scanning
    public void StartScanning()
    {
        IsScanning = true;
        HasResult = false;
        FeedbackMessage = "Point camera at food...";
        Debug.Log("Scanning started");
    }
    
    // Stop scanning
    public void StopScanning()
    {
        IsScanning = false;
        Debug.Log("Scanning stopped");
    }
    
    // Process scanned image (call your API here)
    public async Task ProcessImage(Texture2D image)
    {
        if (image == null)
        {
            FeedbackMessage = "No image captured";
            return;
        }
        
        FeedbackMessage = "Analyzing food...";
        
        // Call your food recognition API here
        // ScannedFood = await FoodRecognitionAPI.Analyze(image);
        
        // For now, simulate a result
        await Task.Delay(1000);
        
        HasResult = ScannedFood != null;
        
        if (HasResult)
        {
            FeedbackMessage = "Found: " + ScannedFood.name;
        }
        else
        {
            FeedbackMessage = "Could not identify food";
        }
    }
    
    // Add scanned food to log
    public void AddToLog()
    {
        if (ScannedFood == null)
        {
            FeedbackMessage = "No food scanned yet";
            return;
        }
        
        int currentCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        currentCalories += ScannedFood.calories;
        PlayerPrefs.SetInt("TodayCalories", currentCalories);
        PlayerPrefs.Save();
        
        FeedbackMessage = "Added " + ScannedFood.name;
        Debug.Log("Added scanned food: " + ScannedFood.name);
        
        // Reset for next scan
        ScannedFood = null;
        HasResult = false;
    }
}
