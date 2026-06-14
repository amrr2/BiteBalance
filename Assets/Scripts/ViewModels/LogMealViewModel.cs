using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;


public class LogMealViewModel
{
    public string SearchQuery { get; set; }
    public List<FoodItem> SearchResults { get; private set; }
    public FoodItem SelectedFood { get; private set; }
    public float Servings { get; set; } = 1f;
    public string FeedbackMessage { get; private set; }
    public bool IsLoading { get; private set; }
    
    
    public int TotalCalories 
    { 
        get { return SelectedFood != null ? Mathf.RoundToInt(SelectedFood.calories * Servings) : 0; }
    }
    
    public float TotalProtein
    {
        get { return SelectedFood != null ? SelectedFood.protein * Servings : 0; }
    }
    
    public float TotalCarbs
    {
        get { return SelectedFood != null ? SelectedFood.carbs * Servings : 0; }
    }
    
    public float TotalFat
    {
        get { return SelectedFood != null ? SelectedFood.fat * Servings : 0; }
    }
    
    public LogMealViewModel()
    {
        SearchResults = new List<FoodItem>();
    }
    
    
    public async Task SearchFood()
    {
        if (string.IsNullOrEmpty(SearchQuery))
        {
            FeedbackMessage = "Please enter a food to search";
            return;
        }
        
        IsLoading = true;
        FeedbackMessage = "Searching...";
        
  
        
        IsLoading = false;
        
        if (SearchResults.Count > 0)
        {
            FeedbackMessage = "Found " + SearchResults.Count + " results";
        }
        else
        {
            FeedbackMessage = "No results found";
        }
    }
    
    
    public void SelectFood(FoodItem food)
    {
        SelectedFood = food;
        Servings = 1f;
        Debug.Log("Selected: " + food.name);
    }
    
    
    public void ClearSelection()
    {
        SelectedFood = null;
        Servings = 1f;
    }
    
    
    public void AddToLog()
    {
        if (SelectedFood == null)
        {
            FeedbackMessage = "Please select a food first";
            return;
        }
        
        
        int currentCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        currentCalories += TotalCalories;
        PlayerPrefs.SetInt("TodayCalories", currentCalories);
        PlayerPrefs.Save();
        
        FeedbackMessage = "Added " + SelectedFood.name + " (" + TotalCalories + " cal)";
        Debug.Log(FeedbackMessage);
        
        
        ClearSelection();
    }
}


[System.Serializable]
public class FoodItem
{
    public int id;
    public string name;
    public int calories;
    public float protein;
    public float carbs;
    public float fat;
    public string imageUrl;
}
