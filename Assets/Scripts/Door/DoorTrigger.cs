using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DoorTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject uiPrompt;            // “按E输入密码” 提示
    public GameObject inputUI;             // 输入界面 Panel
    public TMP_InputField passwordInput;   // TMP 输入框
    public TMP_InputField errorText;             // 密码错误提示

    [Header("Door Object")]
    public GameObject door;                // 要移除的大门

    private bool isPlayerInRange = false;

    void Start()
    {
        uiPrompt.SetActive(false);
        inputUI.SetActive(false);
        errorText.gameObject.SetActive(false);

        // ⌨️ 绑定按Enter触发输入验证
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

        passwordInput.text = "";     // 清空旧输入
        passwordInput.Select();      // 聚焦输入框

        errorText.gameObject.SetActive(false);
        Time.timeScale = 0f;         // 可选：暂停游戏
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
