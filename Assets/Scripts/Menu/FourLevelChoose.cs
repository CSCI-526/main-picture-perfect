using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FourLevelChoose : MonoBehaviour
{
    // Start is called before the first frame update
    public void GoToLevelOne()
    {
        SceneManager.LoadScene("MainLevel1");
    }

    public void GoToLevelTwo()
    {
        SceneManager.LoadScene("MainLevel2");
    }

    public void GoToLevelThree()
    {
        SceneManager.LoadScene("MainLevel3");
    }

    public void GoToLevelFour()
    {
        SceneManager.LoadScene("MainLevel4");
    }

    public void BackToLevelChoose()
    {
        SceneManager.LoadScene("Mode_Choose");
    }
}
