using UnityEngine;

// Handles BMI calculation logic
public class BMIViewModel
{
    public float Height { get; set; } // in cm
    public float Weight { get; set; } // in kg
    public float BMI { get; private set; }
    public string Category { get; private set; }
    public string FeedbackMessage { get; private set; }
    
    // Calculate BMI from height and weight
    public void Calculate()
    {
        if (Height <= 0 || Weight <= 0)
        {
            FeedbackMessage = "Please enter valid height and weight";
            return;
        }
        
        // BMI formula: weight(kg) / height(m)^2
        float heightInMeters = Height / 100f;
        BMI = Weight / (heightInMeters * heightInMeters);
        
        // Determine category
        if (BMI < 18.5f)
        {
            Category = "Underweight";
        }
        else if (BMI < 25f)
        {
            Category = "Normal";
        }
        else if (BMI < 30f)
        {
            Category = "Overweight";
        }
        else
        {
            Category = "Obese";
        }
        
        FeedbackMessage = "Your BMI is " + BMI.ToString("F1") + " (" + Category + ")";
        Debug.Log(FeedbackMessage);
    }
    
    // Save to PlayerPrefs
    public void SaveData()
    {
        PlayerPrefs.SetFloat("UserHeight", Height);
        PlayerPrefs.SetFloat("UserWeight", Weight);
        PlayerPrefs.SetFloat("UserBMI", BMI);
        PlayerPrefs.Save();
        
        Debug.Log("BMI data saved");
    }
    
    // Load from PlayerPrefs
    public void LoadData()
    {
        Height = PlayerPrefs.GetFloat("UserHeight", 0);
        Weight = PlayerPrefs.GetFloat("UserWeight", 0);
        
        if (Height > 0 && Weight > 0)
        {
            Calculate();
        }
    }
}
