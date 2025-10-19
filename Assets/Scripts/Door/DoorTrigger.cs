using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DoorTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject uiPrompt;            // press E shows
    public GameObject inputUI;             // input panel
    public TMP_InputField passwordInput;   // TMP input field
    public TMP_InputField errorText;             // error message text

    [Header("Door Object")]
    public GameObject door;                // the door to open

    private bool isPlayerInRange = false;

    void Start()
    {
        uiPrompt.SetActive(false);
        inputUI.SetActive(false);
        errorText.gameObject.SetActive(false);

        // Bind confirm button
        passwordInput.onSubmit.AddListener(delegate { OnConfirmClicked(); });
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowInputUI();
        }
    }

    void ShowInputUI()
    {
        uiPrompt.SetActive(false);
        inputUI.SetActive(true);
        errorText.gameObject.SetActive(false);

        passwordInput.text = "";     // Clear previous input
        passwordInput.Select();      // Focus on input field

        errorText.gameObject.SetActive(false);
        Time.timeScale = 0f;         // Pause game
    }

    public void OnConfirmClicked()
    {
        string input = passwordInput.text;

        if (input == "1234")
        {
            door.SetActive(false);
            inputUI.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            errorText.gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            uiPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            uiPrompt.SetActive(false);
            inputUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
