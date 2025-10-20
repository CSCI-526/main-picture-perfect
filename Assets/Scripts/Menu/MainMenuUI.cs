using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        AnalyticsManager.Instance.ResetData();
        SceneManager.LoadScene("MainScene"); 
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked (not implemented)");
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked");
    }
}
