using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BMIController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField heightInput;
    public TMP_InputField weightInput;
    public Button calculateButton;
    public Button backButton;
    public TextMeshProUGUI bmiValue;
    public TextMeshProUGUI bmiCategory;
    public TextMeshProUGUI maintenanceCalories;

    void Start()
    {
        calculateButton.onClick.AddListener(CalculateBMI);
        backButton.onClick.AddListener(GoBack);
    }

    void CalculateBMI()
    {
        float height = 0;
        float weight = 0;

        float.TryParse(heightInput.text, out height);
        float.TryParse(weightInput.text, out weight);

        if (height <= 0 || weight <= 0)
        {
            bmiValue.text = "--";
            bmiCategory.text = "Please enter valid values";
            maintenanceCalories.text = "Maintenance: -- kcal/day";
            return;
        }

        // Convert height from cm to meters
        float heightInMeters = height / 100f;

        // Calculate BMI: weight (kg) / height (m)^2
        float bmi = weight / (heightInMeters * heightInMeters);

        // Display BMI
        bmiValue.text = bmi.ToString("F1");

        // Determine category and color
        string category = "";
        Color categoryColor = Color.white;

        if (bmi < 18.5f)
        {
            category = "Underweight";
            categoryColor = new Color(0.26f, 0.6f, 0.88f); // Blue
        }
        else if (bmi < 25f)
        {
            category = "Normal weight";
            categoryColor = new Color(0.3f, 0.69f, 0.31f); // Green
        }
        else if (bmi < 30f)
        {
            category = "Overweight";
            categoryColor = new Color(1f, 0.6f, 0f); // Orange
        }
        else
        {
            category = "Obese";
            categoryColor = new Color(0.9f, 0.3f, 0.3f); // Red
        }

        bmiCategory.text = category;
        bmiCategory.color = categoryColor;

        // Calculate maintenance calories (Mifflin-St Jeor formula, assuming average activity)
        // Using a simplified version assuming age 25 and male
        // BMR = 10 * weight + 6.25 * height - 5 * age + 5 (male)
        float bmr = 10f * weight + 6.25f * height - 5f * 25f + 5f;
        float maintenance = bmr * 1.55f; // Moderate activity multiplier

        maintenanceCalories.text = "Maintenance: " + maintenance.ToString("F0") + " kcal/day";

        // Save maintenance calories for use in dashboard
        PlayerPrefs.SetInt("MaintenanceCalories", (int)maintenance);
        PlayerPrefs.Save();
    }

    void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
}