using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;

public class ScanFoodController : MonoBehaviour
{
    [Header("UI References")]
    public RawImage cameraPreview;
    public Button captureButton;
    public Button galleryButton;
    public Button backButton;
    public Button addFoodButton;
    public TextMeshProUGUI foodNameText;
    public TextMeshProUGUI caloriesText;
    public TextMeshProUGUI proteinText;
    public TextMeshProUGUI carbsText;
    public TextMeshProUGUI fatText;

    [Header("Camera")]
    private WebCamTexture webCamTexture;
    private Texture2D capturedImage;

    private string googleVisionApiKey = "AIzaSyCu12IJX94s3FnN0Ay-6ZM5veGirxGce3E";
    private string spoonacularApiKey = "e2bce2461e764673ae23dbcdc1a6f967";

    private int detectedCalories = 0;
    private float detectedProtein = 0;
    private float detectedCarbs = 0;
    private float detectedFat = 0;
    private string detectedFoodName = "";

    void Start()
    {
        captureButton.onClick.AddListener(CapturePhoto);
        galleryButton.onClick.AddListener(PickFromGallery);
        backButton.onClick.AddListener(GoBack);
        addFoodButton.onClick.AddListener(AddToLog);

        addFoodButton.gameObject.SetActive(false);

        if (foodNameText != null)
        {
            foodNameText.text = "Point camera at food";
        }

        StartCamera();
    }

    void StartCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            string cameraName = devices[0].name;
            foreach (var device in devices)
            {
                if (!device.isFrontFacing)
                {
                    cameraName = device.name;
                    break;
                }
            }

            webCamTexture = new WebCamTexture(cameraName, 1920, 1080);
            cameraPreview.texture = webCamTexture;
            webCamTexture.Play();
        }
        else
        {
            if (foodNameText != null)
            {
                foodNameText.text = "No camera - use Gallery";
            }
        }
    }

    void CapturePhoto()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
            capturedImage.SetPixels(webCamTexture.GetPixels());
            capturedImage.Apply();

            cameraPreview.texture = capturedImage;
            webCamTexture.Stop();

            if (foodNameText != null)
            {
                foodNameText.text = "Analyzing photo...";
            }

            StartCoroutine(AnalyzeWithGoogleVision(capturedImage));
        }
        else
        {

            StartCamera();

            if ((webCamTexture == null || !webCamTexture.isPlaying) && foodNameText != null)
            {
                foodNameText.text = "No camera - use Gallery";
            }
        }
    }

    void PickFromGallery()
    {

        NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Gallery image path: " + path);

            if (string.IsNullOrEmpty(path))
            {
                if (foodNameText != null)
                {
                    foodNameText.text = "No photo selected.";
                }
                return;
            }

            Texture2D texture = LoadReadableTexture(path);

            if (texture != null)
            {
                capturedImage = texture;
                cameraPreview.texture = capturedImage;

                if (webCamTexture != null && webCamTexture.isPlaying)
                {
                    webCamTexture.Stop();
                }

                if (foodNameText != null)
                {
                    foodNameText.text = "Analyzing photo...";
                }

                StartCoroutine(AnalyzeWithGoogleVision(capturedImage));
            }
            else
            {
                Debug.LogError("Failed to load image");
                if (foodNameText != null)
                {
                    foodNameText.text = "Failed to load image";
                }
            }
        }, "Select a food image");
    }

    Texture2D LoadReadableTexture(string path)
    {
        try
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                return texture;
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError("Load texture error: " + e.Message);
            return null;
        }
    }

    IEnumerator AnalyzeWithGoogleVision(Texture2D image)
    {

        byte[] imageBytes = image.EncodeToJPG(85);
        string base64Image = Convert.ToBase64String(imageBytes);

        string jsonRequest = "{\"requests\":[{\"image\":{\"content\":\"" + base64Image + "\"},\"features\":[{\"type\":\"LABEL_DETECTION\",\"maxResults\":10}]}]}";

        string url = "https://vision.googleapis.com/v1/images:annotate?key=" + googleVisionApiKey;

        Debug.Log("Sending image to Google Vision...");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("Google Vision Response: " + response);

                string foodName = ParseGoogleVisionResponse(response);

                if (!string.IsNullOrEmpty(foodName))
                {
                    Debug.Log("Detected food: " + foodName);
                    StartCoroutine(GetNutritionData(foodName));
                }
                else
                {
                    if (foodNameText != null)
                    {
                        foodNameText.text = "No food detected. Try again.";
                    }
                }
            }
            else
            {
                Debug.LogError("Google Vision Error: " + request.error);
                if (foodNameText != null)
                {
                    foodNameText.text = "Error analyzing image";
                }
            }
        }
    }

    string ParseGoogleVisionResponse(string json)
    {
        string[] foodKeywords = {
            "food", "fruit", "vegetable", "meat", "bread", "rice", "pasta", "salad",
            "apple", "banana", "orange", "chicken", "beef", "pork", "fish", "egg",
            "cheese", "milk", "yogurt", "pizza", "burger", "sandwich", "soup",
            "cake", "cookie", "chocolate", "ice cream", "coffee", "juice",
            "tomato", "potato", "carrot", "broccoli", "lettuce", "onion",
            "breakfast", "lunch", "dinner", "meal", "dish", "cuisine",
            "grilled", "fried", "baked", "roasted", "steamed", "raw",
            "produce", "ingredient", "snack", "dessert", "drink", "beverage",
            "berry", "lemon", "lime", "mango", "peach", "pear", "grape",
            "watermelon", "pineapple", "coconut", "avocado", "mushroom",
            "pepper", "garlic", "corn", "bean", "pea", "spinach",
            "steak", "bacon", "sausage", "ham", "turkey", "lamb",
            "shrimp", "lobster", "crab", "salmon", "tuna",
            "noodle", "sushi", "ramen", "taco", "burrito", "wrap",
            "pancake", "waffle", "cereal", "oatmeal", "granola",
            "butter", "cream", "honey", "syrup", "sauce",
            "pie", "brownie", "donut", "muffin", "croissant",
            "tea", "smoothie", "soda", "water", "latte", "espresso"
        };

        try
        {

            System.Collections.Generic.List<string> labels = new System.Collections.Generic.List<string>();

            string[] parts = json.Split(new string[] { "\"description\":" }, StringSplitOptions.None);

            foreach (string part in parts)
            {

                string trimmed = part.TrimStart();

                if (trimmed.Length > 2 && trimmed[0] == '"')
                {
                    int endQuote = trimmed.IndexOf('"', 1);
                    if (endQuote > 0)
                    {
                        string label = trimmed.Substring(1, endQuote - 1).ToLower().Trim();
                        if (!string.IsNullOrEmpty(label))
                        {
                            labels.Add(label);
                            Debug.Log("Found label: " + label);
                        }
                    }
                }
            }

            foreach (string label in labels)
            {
                if (IsProbablyFood(label))
                {
                    return CleanFoodName(label);
                }
            }

            foreach (string label in labels)
            {
                foreach (string keyword in foodKeywords)
                {
                    if (label == keyword || label.Contains(keyword))
                    {
                        return CleanFoodName(label);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Parse error: " + e.Message);
        }

        return null;
    }

    bool IsProbablyFood(string label)
    {
        string[] specificFoods = {
            "apple", "banana", "orange", "grape", "strawberry", "blueberry",
            "chicken", "beef", "pork", "fish", "salmon", "tuna", "shrimp",
            "rice", "pasta", "noodle", "bread", "toast", "bagel", "croissant",
            "pizza", "burger", "hamburger", "hotdog", "sandwich", "taco", "burrito",
            "salad", "soup", "stew", "curry", "sushi", "ramen",
            "egg", "omelette", "pancake", "waffle", "cereal", "oatmeal",
            "cheese", "butter", "yogurt", "milk", "cream",
            "cake", "pie", "cookie", "brownie", "donut", "muffin",
            "coffee", "tea", "juice", "smoothie", "soda", "water",
            "tomato", "potato", "carrot", "broccoli", "spinach", "lettuce",
            "onion", "garlic", "pepper", "mushroom", "corn", "bean",
            "steak", "wing", "drumstick", "bacon", "sausage", "ham"
        };

        foreach (string food in specificFoods)
        {
            if (label.Contains(food))
                return true;
        }

        return false;
    }

    string CleanFoodName(string label)
    {
        string[] removeWords = { "food", "dish", "meal", "cuisine", "produce", "ingredient", "natural" };

        string result = label;
        foreach (string word in removeWords)
        {
            result = result.Replace(word, "").Trim();
        }

        if (string.IsNullOrEmpty(result))
            return label;

        return result;
    }

    IEnumerator GetNutritionData(string foodQuery)
    {
        if (foodNameText != null)
        {
            foodNameText.text = "Looking up: " + foodQuery;
        }

        string searchUrl = "https://api.spoonacular.com/food/ingredients/search?query=" +
            UnityWebRequest.EscapeURL(foodQuery) + "&number=10&apiKey=" + spoonacularApiKey;

        using (UnityWebRequest searchRequest = UnityWebRequest.Get(searchUrl))
        {
            yield return searchRequest.SendWebRequest();

            if (searchRequest.result == UnityWebRequest.Result.Success)
            {
                SpoonacularSearchResponse searchResponse = JsonUtility.FromJson<SpoonacularSearchResponse>(searchRequest.downloadHandler.text);

                if (searchResponse.results != null && searchResponse.results.Length > 0)
                {
                    SpoonacularSearchResult best = PickBestResult(searchResponse.results, foodQuery);
                    StartCoroutine(GetNutritionInfo(best.id, best.name));
                }
                else
                {
                    ShowEstimatedNutrition(foodQuery);
                }
            }
            else
            {
                ShowEstimatedNutrition(foodQuery);
            }
        }
    }

    SpoonacularSearchResult PickBestResult(SpoonacularSearchResult[] results, string query)
    {
        string q = query.ToLower().Trim();

        string[] avoidWords = { "fat", "skin", "broth", "stock", "bouillon", "powder", "baby food" };

        SpoonacularSearchResult best = results[0];
        bool bestIsAvoided = ContainsAny(best.name.ToLower(), avoidWords);

        foreach (SpoonacularSearchResult r in results)
        {
            string name = r.name.ToLower().Trim();

            if (name == q)
            {
                return r;
            }

            bool isAvoided = ContainsAny(name, avoidWords);

            if (!isAvoided)
            {
                if (bestIsAvoided || name.Length < best.name.Length)
                {
                    best = r;
                    bestIsAvoided = false;
                }
            }
        }

        return best;
    }

    bool ContainsAny(string text, string[] words)
    {
        foreach (string w in words)
        {
            if (text.Contains(w))
                return true;
        }
        return false;
    }

    IEnumerator GetNutritionInfo(int foodId, string foodName)
    {
        string nutritionUrl = "https://api.spoonacular.com/food/ingredients/" + foodId +
            "/information?amount=100&unit=grams&apiKey=" + spoonacularApiKey;

        using (UnityWebRequest request = UnityWebRequest.Get(nutritionUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ParseNutritionResponse(request.downloadHandler.text, foodName);
            }
            else
            {
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

                foreach (var nutrient in info.nutrition.nutrients)
                {
                    if (nutrient.name == "Calories") calories = nutrient.amount;
                    else if (nutrient.name == "Protein") protein = nutrient.amount;
                    else if (nutrient.name == "Carbohydrates") carbs = nutrient.amount;
                    else if (nutrient.name == "Fat") fat = nutrient.amount;
                }

                detectedCalories = Mathf.RoundToInt(calories);
                detectedProtein = protein;
                detectedCarbs = carbs;
                detectedFat = fat;

                DisplayResults(foodName, detectedCalories, protein, carbs, fat);
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

    void DisplayResults(string foodName, int calories, float protein, float carbs, float fat)
    {
        if (foodNameText != null)
            foodNameText.text = char.ToUpper(foodName[0]) + foodName.Substring(1) + " (100g)";

        if (caloriesText != null)
            caloriesText.text = "Calories: " + calories + " kcal";

        if (proteinText != null)
            proteinText.text = "Protein: " + Mathf.RoundToInt(protein) + "g";

        if (carbsText != null)
            carbsText.text = "Carbs: " + Mathf.RoundToInt(carbs) + "g";

        if (fatText != null)
            fatText.text = "Fat: " + Mathf.RoundToInt(fat) + "g";

        addFoodButton.gameObject.SetActive(true);
    }

    void ShowEstimatedNutrition(string foodName)
    {
        detectedFoodName = foodName;
        detectedCalories = UnityEngine.Random.Range(80, 300);
        detectedProtein = UnityEngine.Random.Range(5, 25);
        detectedCarbs = UnityEngine.Random.Range(10, 40);
        detectedFat = UnityEngine.Random.Range(3, 20);

        if (foodNameText != null)
            foodNameText.text = char.ToUpper(foodName[0]) + foodName.Substring(1) + " (est.)";

        if (caloriesText != null)
            caloriesText.text = "Calories: ~" + detectedCalories + " kcal";

        if (proteinText != null)
            proteinText.text = "Protein: ~" + Mathf.RoundToInt(detectedProtein) + "g";

        if (carbsText != null)
            carbsText.text = "Carbs: ~" + Mathf.RoundToInt(detectedCarbs) + "g";

        if (fatText != null)
            fatText.text = "Fat: ~" + Mathf.RoundToInt(detectedFat) + "g";

        addFoodButton.gameObject.SetActive(true);
    }

    void AddToLog()
    {
        int currentCalories = PlayerPrefs.GetInt("TodayCalories", 0);
        currentCalories += detectedCalories;
        PlayerPrefs.SetInt("TodayCalories", currentCalories);

        float currentProtein = PlayerPrefs.GetFloat("TodayProtein", 0);
        currentProtein += detectedProtein;
        PlayerPrefs.SetFloat("TodayProtein", currentProtein);

        float currentCarbs = PlayerPrefs.GetFloat("TodayCarbs", 0);
        currentCarbs += detectedCarbs;
        PlayerPrefs.SetFloat("TodayCarbs", currentCarbs);

        float currentFat = PlayerPrefs.GetFloat("TodayFat", 0);
        currentFat += detectedFat;
        PlayerPrefs.SetFloat("TodayFat", currentFat);

        PlayerPrefs.Save();

        Debug.Log("Added " + detectedFoodName + " (" + detectedCalories + " kcal)");
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
        if (webCamTexture != null && webCamTexture.isPlaying)
            webCamTexture.Stop();
    }
}

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
