using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

// Saves meals to Firebase Realtime Database
public class FirebaseDiaryService : IFoodDiaryService
{
    private DatabaseReference dbReference;
    
    public FirebaseDiaryService()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    
    public async Task SaveMealAsync(string odId, MealRecord meal)
    {
        try
        {
            string json = JsonUtility.ToJson(meal);
            string path = "users/" + odId + "/meals/" + meal.date + "/" + meal.id;
            
            await dbReference.Child(path).SetRawJsonValueAsync(json);
            Debug.Log("Meal saved to Firebase: " + meal.name);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save meal: " + e.Message);
        }
    }
    
    public async Task<List<MealRecord>> GetMealsForDateAsync(string odId, string date)
    {
        List<MealRecord> meals = new List<MealRecord>();
        
        try
        {
            string path = "users/" + odId + "/meals/" + date;
            DataSnapshot snapshot = await dbReference.Child(path).GetValueAsync();
            
            if (snapshot.Exists)
            {
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    MealRecord meal = JsonUtility.FromJson<MealRecord>(json);
                    meals.Add(meal);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to get meals: " + e.Message);
        }
        
        return meals;
    }
}
