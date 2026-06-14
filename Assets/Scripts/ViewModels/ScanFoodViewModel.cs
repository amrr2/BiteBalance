using UnityEngine;
using System.Threading.Tasks;


public class ScanFoodViewModel
{
    public bool IsScanning { get; private set; }
    public bool HasResult { get; private set; }
    public FoodItem ScannedFood { get; private set; }
    public string FeedbackMessage { get; private set; }
    
   
    public void StartScanning()
    {
        IsScanning = true;
        HasResult = false;
        FeedbackMessage = "Point camera at food...";
        Debug.Log("Scanning started");
    }
    
    
    public void StopScanning()
    {
        IsScanning = false;
        Debug.Log("Scanning stopped");
    }
    
    
    public async Task ProcessImage(Texture2D image)
    {
        if (image == null)
        {
            FeedbackMessage = "No image captured";
            return;
        }
        
        FeedbackMessage = "Analyzing food...";
        
     
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
        
        
        ScannedFood = null;
        HasResult = false;
    }
}
