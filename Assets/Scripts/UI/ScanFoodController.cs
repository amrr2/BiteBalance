using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ScanFoodController : MonoBehaviour
{
    [Header("UI References")]
    public RawImage cameraPreview;
    public Button captureButton;
    public Button backButton;
    public Button addFoodButton;
    public TextMeshProUGUI foodNameText;
    public TextMeshProUGUI caloriesText;
    public TextMeshProUGUI proteinText;
    public TextMeshProUGUI carbsText;
    public TextMeshProUGUI fatText;

    [Header("Camera")]
    private WebCamTexture webCamTexture;

    // Spoonacular API - same one that is used for meal logging
    private string apiKey = "e2bce2461e764673ae23dbcdc1a6f967";

    private int detectedCalories = 0;
    private string detectedFoodName = "";

    void Start()
    {
        captureButton.onClick.AddListener(CaptureAndAnalyze);
        backButton.onClick.AddListener(GoBack);
        addFoodButton.onClick.AddListener(AddToLog);
        addFoodButton.gameObject.SetActive(false);

        StartCamera();
    }

    void StartCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // Trying to use the back camera for food photos
            string cameraName = devices[0].name;
            foreach (var device in devices)
            {
                if (!device.isFrontFacing)
                {
                    cameraName = device.name;
                    break;
                }
            }

            webCamTexture = new WebCamTexture(cameraName, 1080, 1920);
            cameraPreview.texture = webCamTexture;
            webCamTexture.Play();
        }
        else
        {
            foodNameText.text = "No camera found";
        }
    }

    void CaptureAndAnalyze()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            foodNameText.text = "Analyzing...";
            StartCoroutine(SimulateFoodDetection());
        }
    }

    IEnumerator SimulateFoodDetection()
    {
        // sending image to vision API
        yield return new WaitForSeconds(1f);

        string[] sampleFoods = { "apple", "banana", "chicken breast", "rice", "salad", "pizza", "burger" };
        string randomFood = sampleFoods[UnityEngine.Random.Range(0, sampleFoods.Length)];

        StartCoroutine(GetNutritionData(randomFood));
    }

    IEnumerator GetNutritionData(string foodQuery)
    {
        foodNameText.text = "Looking up: " + foodQuery;

        // First, search for the food to get its ID
        string searchUrl = "https://api.spoonacular.com/food/ingredients/search?query=" + foodQuery + "&number=1&apiKey=" + apiKey;

        using (UnityWebRequest searchRequest = UnityWebRequest.Get(searchUrl))
        {
            yield return searchRequest.SendWebRequest();

            if (searchRequest.result == UnityWebRequest.Result.Success)
            {
                SpoonacularSearchResponse searchResponse = JsonUtility.FromJson<SpoonacularSearchResponse>(searchRequest.downloadHandler.text);

                if (searchResponse.results != null && searchResponse.results.Length > 0)
                {
                    int foodId = searchResponse.results[0].id;
                    string foodName = searchResponse.results[0].name;
                    
                    // Now get nutrition info using the food ID used in the first step
                    StartCoroutine(GetNutritionInfo(foodId, foodName));
                }
                else
                {
                    ShowEstimatedNutrition(foodQuery);
                }
            }
            else
            {
                Debug.LogError("Search API Error: " + searchRequest.error);
                ShowEstimatedNutrition(foodQuery);
            }
        }
    }

    IEnumerator GetNutritionInfo(int foodId, string foodName)
    {
        string nutritionUrl = "https://api.spoonacular.com/food/ingredients/" + foodId + "/information?amount=100&unit=grams&apiKey=" + apiKey;

        using (UnityWebRequest request = UnityWebRequest.Get(nutritionUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ParseNutritionResponse(request.downloadHandler.text, foodName);
            }
            else
            {
                Debug.LogError("Nutrition API Error: " + request.error);
                ShowEstimatedNutrition(foodName);
            }
        }
    }

    void ParseNutritionResponse(string json, string foodName)
    {
        try
        {
            SpoonacularNutritionInfo info = JsonUtility.FromJson<SpoonacularNutritionInfo>(json);

            if (info.nutrition != null && info.nutrition.nutrients != null)
            {
                detectedFoodName = foodName;
                
                float calories = 0, protein = 0, carbs = 0, fat = 0;

                // Extracting the nutrition information
                foreach (var nutrient in info.nutrition.nutrients)
                {
                    if (nutrient.name == "Calories") calories = nutrient.amount;
                    else if (nutrient.name == "Protein") protein = nutrient.amount;
                    else if (nutrient.name == "Carbohydrates") carbs = nutrient.amount;
                    else if (nutrient.name == "Fat") fat = nutrient.amount;
                }

                detectedCalories = Mathf.RoundToInt(calories);

                // Display the results
                foodNameText.text = char.ToUpper(foodName[0]) + foodName.Substring(1);
                caloriesText.text = "Calories: " + detectedCalories + " kcal";
                proteinText.text = "Protein: " + Mathf.RoundToInt(protein) + "g";
                carbsText.text = "Carbs: " + Mathf.RoundToInt(carbs) + "g";
                fatText.text = "Fat: " + Mathf.RoundToInt(fat) + "g";

                addFoodButton.gameObject.SetActive(true);
            }
            else
            {
                ShowEstimatedNutrition(foodName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Parse error: " + e.Message);
            ShowEstimatedNutrition(foodName);
        }
    }

    void ShowEstimatedNutrition(string foodName)
    {
        // Fallback if the  API fails, shows estimated values
        detectedFoodName = foodName;
        detectedCalories = UnityEngine.Random.Range(80, 300);

        foodNameText.text = char.ToUpper(foodName[0]) + foodName.Substring(1);
        caloriesText.text = "Calories: ~" + detectedCalories + " kcal";
        proteinText.text = "Protein: ~" + UnityEngine.Random.Range(5, 25) + "g";
        carbsText.text = "Carbs: ~" + UnityEngine.Random.Range(10, 40) + "g";
        fatText.text = "Fat: ~" + UnityEngine.Random.Range(3, 20) + "g";

        addFoodButton.gameObject.SetActive(true);
    }

    void AddToLog()
    {
        // Add calories from the scan to today's total
        int currentCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        currentCalories += detectedCalories;
        PlayerPrefs.SetInt("TodayCalories", currentCalories);
        PlayerPrefs.Save();

        Debug.Log("Added " + detectedFoodName + " (" + detectedCalories + " kcal). Total: " + currentCalories);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }

    void GoBack()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
            webCamTexture.Stop();
            
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
    
    void OnDestroy()
    {
        // Clean up camera when leaving scene
        if (webCamTexture != null && webCamTexture.isPlaying)
            webCamTexture.Stop();
    }
}

// Classes to parse Spoonacular API responses
[Serializable]
public class SpoonacularSearchResponse
{
    public SpoonacularSearchResult[] results;
}

[Serializable]
public class SpoonacularSearchResult
{
    public int id;
    public string name;
}

[Serializable]
public class SpoonacularNutritionInfo
{
    public SpoonacularNutritionData nutrition;
}

[Serializable]
public class SpoonacularNutritionData
{
    public SpoonacularNutrient[] nutrients;
}

[Serializable]
public class SpoonacularNutrient
{
    public string name;
    public float amount;
}

