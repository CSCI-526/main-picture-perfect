using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialChooseUI : MonoBehaviour
{
    public void GoToLevelMove()
    {
        SceneManager.LoadScene("Level_Tutorial_Move");
    }

    public void GoToLevelFreeze()
    {
        SceneManager.LoadScene("Level_Tutorial_Freeze");
    }

    public void GoToLevelNPC()
    {
        SceneManager.LoadScene("Level_Tutorial_NPC");
    }

    public void BackToLevelChoose()
    {
        SceneManager.LoadScene("Level_Choose");
    }
}
