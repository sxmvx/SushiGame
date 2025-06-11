using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject settingPanel;

    public void TogglePanel()
    {
        bool isActive = !settingPanel.activeSelf;
        settingPanel.SetActive(isActive);

        // �Ͻ����� / �簳
        Time.timeScale = isActive ? 0f : 1f;
    }

    public void RestartGame()
    {
        // �ð� �ٽ� �帣�� �ϰ� ���� �� �ٽ� �ε�
        // GameScene�϶��� Ȱ��ȭ, StartScene�� ���� ��Ȱ��ȭ�ϱ�
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene"); 
    }

    public void StartBtn()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void ExpBtn()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("StartScene"); // ���� �� �̸��� �°� ����!
    }
}
