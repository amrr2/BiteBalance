using System.Threading.Tasks;
using System.Collections.Generic;

// Interface for saving meals to Firebase
public interface IFoodDiaryService
{
    Task SaveMealAsync(string odId, MealRecord meal);
    Task<List<MealRecord>> GetMealsForDateAsync(string odId, string date);
}

// Meal record class
[System.Serializable]
public class MealRecord
{
    public string id;
    public string name;
    public int calories;
    public float protein;
    public float carbs;
    public float fat;
    public string date;
    public string mealType;
    
    public MealRecord() { }
    
    public MealRecord(string name, int calories, float protein, float carbs, float fat)
    {
        this.id = System.Guid.NewGuid().ToString();
        this.name = name;
        this.calories = calories;
        this.protein = protein;
        this.carbs = carbs;
        this.fat = fat;
        this.date = System.DateTime.Now.ToString("yyyy-MM-dd");
        this.mealType = "snack";
    }
}
