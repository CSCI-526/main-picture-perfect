using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class ToLevelTwo : MonoBehaviour
{
    public GameObject uiPrompt; 
    public string nextLevelName = "MainLevel2";

    private bool playerInTrigger = false;

    void Start()
    {
        if (uiPrompt != null)
            uiPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            if (uiPrompt != null)
                uiPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (uiPrompt != null)
                uiPrompt.SetActive(false);
        }
    }
}
