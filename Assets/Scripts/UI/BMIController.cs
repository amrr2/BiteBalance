using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BMIController : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField ageInput;
    public TMP_InputField heightInput;
    public TMP_InputField weightInput;

    [Header("Gender Buttons")]
    public Button maleButton;
    public Button femaleButton;

    [Header("Activity Dropdown")]
    public TMP_Dropdown activityDropdown;

    [Header("Action Buttons")]
    public Button calculateButton;
    public Button backButton;

    [Header("Results Display")]
    public TextMeshProUGUI bmiValue;
    public TextMeshProUGUI bmiCategory;
    public TextMeshProUGUI maintenanceCalories;

    private bool isMale = true;

    private Color selectedColor = new Color(0.26f, 0.6f, 0.88f);
    private Color unselectedColor = new Color(0.18f, 0.22f, 0.28f);

    void Start()
    {
        calculateButton.onClick.AddListener(CalculateBMI);
        backButton.onClick.AddListener(GoBack);

        maleButton.onClick.AddListener(SelectMale);
        femaleButton.onClick.AddListener(SelectFemale);

        SelectMale();
    }

    void SelectMale()
    {
        isMale = true;

        maleButton.GetComponent<Image>().color = selectedColor;
        femaleButton.GetComponent<Image>().color = unselectedColor;

        Debug.Log("Gender selected: Male");
    }

    void SelectFemale()
    {
        isMale = false;

        femaleButton.GetComponent<Image>().color = selectedColor;
        maleButton.GetComponent<Image>().color = unselectedColor;

        Debug.Log("Gender selected: Female");
    }

    void CalculateBMI()
    {

        int age = 0;
        float height = 0;
        float weight = 0;

        int.TryParse(ageInput.text, out age);
        float.TryParse(heightInput.text, out height);
        float.TryParse(weightInput.text, out weight);

        if (age <= 0 || height <= 0 || weight <= 0)
        {
            bmiValue.text = "--";
            bmiCategory.text = "Enter your details";
            maintenanceCalories.text = "Maintenance: -- kcal/day";
            return;
        }

        float heightInMeters = height / 100f;

        float bmi = weight / (heightInMeters * heightInMeters);

        bmiValue.text = bmi.ToString("F1");

        string category = "";
        Color categoryColor = Color.white;

        if (bmi < 18.5f)
        {
            category = "Underweight";
            categoryColor = new Color(0.26f, 0.6f, 0.88f);
        }
        else if (bmi < 25f)
        {
            category = "Normal weight";
            categoryColor = new Color(0.3f, 0.69f, 0.31f);
        }
        else if (bmi < 30f)
        {
            category = "Overweight";
            categoryColor = new Color(1f, 0.6f, 0f);
        }
        else
        {
            category = "Obese";
            categoryColor = new Color(0.9f, 0.3f, 0.3f);
        }

        bmiCategory.text = category;
        bmiCategory.color = categoryColor;

        float bmr;
        if (isMale)
        {

            bmr = 10f * weight + 6.25f * height - 5f * age + 5f;
        }
        else
        {

            bmr = 10f * weight + 6.25f * height - 5f * age - 161f;
        }

        float activityMultiplier = GetActivityMultiplier();

        float maintenance = bmr * activityMultiplier;

        maintenanceCalories.text = "Maintenance: " + maintenance.ToString("F0") + " kcal/day";

        PlayerPrefs.SetInt("MaintenanceCalories", (int)maintenance);
        PlayerPrefs.SetFloat("UserBMI", bmi);
        PlayerPrefs.SetInt("UserAge", age);
        PlayerPrefs.SetFloat("UserHeight", height);
        PlayerPrefs.SetFloat("UserWeight", weight);
        PlayerPrefs.Save();

        Debug.Log("BMI: " + bmi.ToString("F1") + ", Maintenance: " + maintenance.ToString("F0") + " kcal/day");
    }

    float GetActivityMultiplier()
    {

        int selected = activityDropdown.value;

        switch (selected)
        {
            case 0: return 1.2f;
            case 1: return 1.375f;
            case 2: return 1.55f;
            case 3: return 1.725f;
            case 4: return 1.9f;
            case 5: return 2.0f;
            default: return 1.55f;
        }
    }

    void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
}
