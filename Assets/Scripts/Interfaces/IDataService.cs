using System.Threading.Tasks;
using System.Collections.Generic;


public interface IDataService
{
    Task SaveMeal(string odId, MealData meal);
    Task<List<MealData>> GetMealsForToday(string userId);
    Task<int> GetTodayCalories(string odId);
    Task SaveUserProfile(string odId, UserProfile profile);
    Task<UserProfile> GetUserProfile(string odId);
}


[System.Serializable]
public class MealData
{
    public string id;
    public string name;
    public int calories;
    public float protein;
    public float carbs;
    public float fat;
    public string date;
    public string mealType; 
}


[System.Serializable]
public class UserProfile
{
    public string odId;
    public string email;
    public int goalCalories = 2000;
    public float height;
    public float weight;
    public int age;
}
