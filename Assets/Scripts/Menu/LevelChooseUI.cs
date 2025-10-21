using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChooseUI : MonoBehaviour
{
    public void GoToTutorial()
    {
        SceneManager.LoadScene("Level_Tutorial_Choose");
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartMenuScene");
    }
}
