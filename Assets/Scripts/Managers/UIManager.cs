using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject dashboardPanel;
    public GameObject loadingPanel;

    [Header("Loading")]
    public TextMeshProUGUI loadingText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Show/Hide Panels
    public void ShowLogin()
    {
        HideAllPanels();
        if (loginPanel != null) loginPanel.SetActive(true);
    }

    public void ShowRegister()
    {
        HideAllPanels();
        if (registerPanel != null) registerPanel.SetActive(true);
    }

    public void ShowDashboard()
    {
        HideAllPanels();
        if (dashboardPanel != null) dashboardPanel.SetActive(true);
    }

    public void HideAllPanels()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(false);
        if (dashboardPanel != null) dashboardPanel.SetActive(false);
        HideLoading();
    }

    // Loading indicator
    public void ShowLoading(string message = "Loading...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            if (loadingText != null) loadingText.text = message;
        }
    }

    public void HideLoading()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
    }

    // Show temporary message (like a toast)
    public void ShowMessage(string message, float duration = 2f)
    {
        StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        ShowLoading(message);
        yield return new WaitForSeconds(duration);
        HideLoading();
    }
}