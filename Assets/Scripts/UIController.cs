using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject settingPanel;

    public void TogglePanel()
    {
        bool isActive = !settingPanel.activeSelf;
        settingPanel.SetActive(isActive);

        // 일시정지 / 재개
        Time.timeScale = isActive ? 0f : 1f;
    }

    public void RestartGame()
    {
        // 시간 다시 흐르게 하고 현재 씬 다시 로드
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("MainMenu"); // 메인 씬 이름에 맞게 수정!
    }
}
