using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnToMenu : MonoBehaviour
{
    private SendToGoogle sender;
    private bool isReturning = false;

    void Start()
    {
        sender = FindObjectOfType<SendToGoogle>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isReturning)
        {
            isReturning = true;
            StartCoroutine(ReturnAfterUpload());
        }
    }

    IEnumerator ReturnAfterUpload()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
        if (sender != null)
        {
            if (AnalyticsManager.Instance != null){

            
                sender.Send();
                yield return new WaitForSeconds(1f); 
            }
        }
        SceneManager.LoadScene("StartMenuScene");
    }
}
