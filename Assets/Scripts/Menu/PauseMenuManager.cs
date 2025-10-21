using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;//data


public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;           // 整个暂停菜单 Canvas
    public GameObject mainButtonPanel;       // 主按钮区域（Resume, Settings, Back to Menu）
    public GameObject settingsPanel;         // 设置面板（包含 Slider 和 Close）

    public Slider sensitivitySlider;         // 灵敏度滑动条
    public PlayerController playerController;

    private bool isPaused = false;

    private SendToGoogle sender;//data


    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        pauseMenuUI.SetActive(false); // 游戏开始时隐藏菜单
        mainButtonPanel.SetActive(true);
        settingsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (sensitivitySlider != null && playerController != null)
        {
            sensitivitySlider.value = playerController.mouseSensitivity;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        mainButtonPanel.SetActive(true);
        settingsPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;   // 必须在 timeScale 之后
        Cursor.visible = false;

        playerController.enabled = true;
        isPaused = false;
}


    public void Pause()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        mainButtonPanel.SetActive(true);      // 显示主按钮
        settingsPanel.SetActive(false);       // 隐藏设置面板

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Settings()
    {
        mainButtonPanel.SetActive(false);     // 隐藏主按钮
        settingsPanel.SetActive(true);        // 显示设置界面
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);       // 隐藏设置界面
        mainButtonPanel.SetActive(true);      // 显示主按钮
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            sender = FindObjectOfType<SendToGoogle>();
            if (sender != null && AnalyticsManager.Instance != null)
            {
                sender.Send();
                Debug.Log("data upload before returning to menu.");
            }
        }

        SceneManager.LoadScene("Level_Tutorial_Choose");
    }

    public void OnSensitivityChanged(float value)
    {
        Debug.Log("Sensitivity changed to: " + value);
        playerController.mouseSensitivity = value;

        // 保存设置
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save(); // 可选，立即写入
    }

}
