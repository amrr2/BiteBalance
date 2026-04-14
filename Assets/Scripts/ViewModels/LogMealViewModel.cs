using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

// Handles meal logging and food search logic
public class LogMealViewModel
{
    public string SearchQuery { get; set; }
    public List<FoodItem> SearchResults { get; private set; }
    public FoodItem SelectedFood { get; private set; }
    public float Servings { get; set; } = 1f;
    public string FeedbackMessage { get; private set; }
    public bool IsLoading { get; private set; }
    
    // Calculated values based on servings
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
    
    // Search for food using Spoonacular API
    public async Task SearchFood()
    {
        if (string.IsNullOrEmpty(SearchQuery))
        {
            FeedbackMessage = "Please enter a food to search";
            return;
        }
        
        IsLoading = true;
        FeedbackMessage = "Searching...";
        
        // Call your existing API manager here
        // SearchResults = await SpoonacularManager.Instance.SearchFood(SearchQuery);
        
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
    
    // Select a food from search results
    public void SelectFood(FoodItem food)
    {
        SelectedFood = food;
        Servings = 1f;
        Debug.Log("Selected: " + food.name);
    }
    
    // Clear selection
    public void ClearSelection()
    {
        SelectedFood = null;
        Servings = 1f;
    }
    
    // Add the selected food to today's log
    public void AddToLog()
    {
        if (SelectedFood == null)
        {
            FeedbackMessage = "Please select a food first";
            return;
        }
        
        // Add calories to PlayerPrefs
        int currentCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        currentCalories += TotalCalories;
        PlayerPrefs.SetInt("TodayCalories", currentCalories);
        PlayerPrefs.Save();
        
        FeedbackMessage = "Added " + SelectedFood.name + " (" + TotalCalories + " cal)";
        Debug.Log(FeedbackMessage);
        
        // Clear for next entry
        ClearSelection();
    }
}

// Simple food item class
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
