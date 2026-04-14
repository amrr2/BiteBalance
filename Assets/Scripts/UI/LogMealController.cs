using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using Firebase.Auth;
using System.Threading.Tasks;

public class LogMealController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField searchInput;
    public Button searchButton;
    public Button backButton;
    public Transform resultsPanel;

    [Header("API Settings")]
    private string apiKey = "e2bce2461e764673ae23dbcdc1a6f967";
    private string apiUrl = "https://api.spoonacular.com/food/ingredients/search?query=";
    
    // Service for saving to Firebase
    private IFoodDiaryService diaryService;
    
    void Start()
    {
        // Create the service
        diaryService = new FirebaseDiaryService();
        
        searchButton.onClick.AddListener(OnSearchClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    void OnSearchClicked()
    {
        string query = searchInput.text;
        if (!string.IsNullOrEmpty(query))
        {
            StartCoroutine(SearchFood(query));
        }
    }

    void OnBackClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }

    IEnumerator SearchFood(string query)
    {
        // Clear previous results
        foreach (Transform child in resultsPanel)
        {
            Destroy(child.gameObject);
        }

        CreateResultItem("Searching...", "", "", 0, 0, 0);

        string encodedQuery = UnityWebRequest.EscapeURL(query);
        string fullUrl = apiUrl + encodedQuery + "&number=5&apiKey=" + apiKey;

        Debug.Log("Requesting: " + fullUrl);

        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            // Clear loading text
            foreach (Transform child in resultsPanel)
            {
                Destroy(child.gameObject);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("API Response: " + json);

                SpoonacularResponse response = JsonUtility.FromJson<SpoonacularResponse>(json);

                if (response.results != null && response.results.Length > 0)
                {
                    foreach (SpoonacularItem item in response.results)
                    {
                        StartCoroutine(GetNutritionInfo(item.id, item.name));
                    }
                }
                else
                {
                    CreateResultItem("No results found", "", "", 0, 0, 0);
                }
            }
            else
            {
                Debug.LogError("API Error: " + request.error);
                CreateResultItem("Error searching", "", request.error, 0, 0, 0);
            }
        }
    }

    IEnumerator GetNutritionInfo(int id, string name)
    {
        string nutritionUrl = "https://api.spoonacular.com/food/ingredients/" + id + "/information?amount=100&unit=grams&apiKey=" + apiKey;

        using (UnityWebRequest request = UnityWebRequest.Get(nutritionUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("Nutrition Response: " + json);
                
                NutritionInfo info = JsonUtility.FromJson<NutritionInfo>(json);

                float calories = 0, protein = 0, carbs = 0, fat = 0;

                if (info.nutrition != null && info.nutrition.nutrients != null)
                {
                    foreach (Nutrient n in info.nutrition.nutrients)
                    {
                        if (n.name == "Calories") calories = n.amount;
                        if (n.name == "Protein") protein = n.amount;
                        if (n.name == "Carbohydrates") carbs = n.amount;
                        if (n.name == "Fat") fat = n.amount;
                    }
                }

                CreateResultItem(
                    CapitalizeFirst(name) + " (100g)",
                    calories.ToString("F0"),
                    "P: " + protein.ToString("F1") + "g | C: " + carbs.ToString("F1") + "g | F: " + fat.ToString("F1") + "g",
                    protein,
                    carbs,
                    fat
                );
            }
        }
    }

    string CapitalizeFirst(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    void CreateResultItem(string foodName, string calories, string macros, float protein, float carbs, float fat)
    {
        GameObject item = new GameObject("FoodItem");
        item.transform.SetParent(resultsPanel, false);

        RectTransform rect = item.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(950, 120);

        int index = resultsPanel.childCount - 1;
        rect.anchoredPosition = new Vector2(0, 350 - (index * 130));

        Image bg = item.AddComponent<Image>();
        bg.color = new Color(0.17f, 0.22f, 0.28f);

        // Food name
        GameObject nameObj = new GameObject("FoodName");
        nameObj.transform.SetParent(item.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchoredPosition = new Vector2(-200, 25);
        nameRect.sizeDelta = new Vector2(500, 50);
        TextMeshProUGUI nameTmp = nameObj.AddComponent<TextMeshProUGUI>();
        nameTmp.text = foodName;
        nameTmp.fontSize = 28;
        nameTmp.color = Color.white;

        // Calories
        if (!string.IsNullOrEmpty(calories))
        {
            GameObject calObj = new GameObject("Calories");
            calObj.transform.SetParent(item.transform, false);
            RectTransform calRect = calObj.AddComponent<RectTransform>();
            calRect.anchoredPosition = new Vector2(350, 25);
            calRect.sizeDelta = new Vector2(200, 50);
            TextMeshProUGUI calTmp = calObj.AddComponent<TextMeshProUGUI>();
            calTmp.text = calories + " kcal";
            calTmp.fontSize = 28;
            calTmp.color = new Color(0.26f, 0.6f, 0.88f);
            calTmp.alignment = TextAlignmentOptions.Right;
        }

        // Macros
        if (!string.IsNullOrEmpty(macros))
        {
            GameObject macroObj = new GameObject("Macros");
            macroObj.transform.SetParent(item.transform, false);
            RectTransform macroRect = macroObj.AddComponent<RectTransform>();
            macroRect.anchoredPosition = new Vector2(0, -25);
            macroRect.sizeDelta = new Vector2(900, 40);
            TextMeshProUGUI macroTmp = macroObj.AddComponent<TextMeshProUGUI>();
            macroTmp.text = macros;
            macroTmp.fontSize = 22;
            macroTmp.color = new Color(0.53f, 0.53f, 0.53f);
            macroTmp.alignment = TextAlignmentOptions.Center;
        }

        // Make clickable - pass all nutrition data
        Button btn = item.AddComponent<Button>();
        string calStr = calories;
        string food = foodName;
        float p = protein;
        float c = carbs;
        float f = fat;
        btn.onClick.AddListener(() => OnFoodSelected(food, calStr, p, c, f));
    }

    void OnFoodSelected(string foodName, string calories, float protein, float carbs, float fat)
    {
        Debug.Log("Selected: " + foodName + " - " + calories + " kcal");
        
        // Parse calories to int
        int calAmount = 0;
        int.TryParse(calories, out calAmount);
        
        // Save to PlayerPrefs
        int currentCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        currentCalories += calAmount;
        PlayerPrefs.SetInt("TodayCalories", currentCalories);
        PlayerPrefs.Save();
        
        // Save to Firebase if user is logged in
        FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser != null)
        {
            // Create meal record with actual nutrition data
            MealRecord newMeal = new MealRecord(foodName, calAmount, protein, carbs, fat);
            
            // Save to cloud
            _ = diaryService.SaveMealAsync(currentUser.UserId, newMeal);
            Debug.Log("Meal saved to cloud: " + foodName);
        }
        else
        {
            Debug.LogWarning("User not logged in. Saved locally only.");
        }
        
        Debug.Log("Total calories today: " + currentCalories);
        
        // Go back to Dashboard
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
}

[System.Serializable]
public class SpoonacularResponse
{
    public SpoonacularItem[] results;
}

[System.Serializable]
public class SpoonacularItem
{
    public int id;
    public string name;
}

[System.Serializable]
public class NutritionInfo
{
    public NutritionData nutrition;
}

[System.Serializable]
public class NutritionData
{
    public Nutrient[] nutrients;
}

[System.Serializable]
public class Nutrient
{
    public string name;
    public float amount;
}
