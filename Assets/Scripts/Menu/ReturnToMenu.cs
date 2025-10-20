using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnToMenu : MonoBehaviour
{
    private bool isReturning = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isReturning)
        {
            isReturning = true;
            StartCoroutine(ReturnToMenuScene());
        }
    }

    IEnumerator ReturnToMenuScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("StartMenuScene");
    }
}

