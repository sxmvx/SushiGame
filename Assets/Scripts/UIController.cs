using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject ExpPanel;
    public AudioSource bgmSource; // BGM ������Ʈ�� AudioSource�� �ν����Ϳ��� ����
    private bool isBGMOn = true;
    public Toggle bgmToggle;      // ��� UI

    void Start()
    {
        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();

        // ��� ���¿� ���� ������� �ѱ�/����
        bgmToggle.onValueChanged.AddListener(OnBgmToggleChanged);

        if (bgmToggle.isOn)
        {
            if (!bgmSource.isPlaying)
                bgmSource.Play();
            bgmSource.mute = false;
        }
        else
        {
            bgmSource.Pause();
            bgmSource.mute = true;
        }
    }

    void OnBgmToggleChanged(bool isOn)
    {
        if (isOn)
        {
            bgmSource.mute = false;
            if (!bgmSource.isPlaying)
                bgmSource.Play();
        }
        else
        {
            bgmSource.Pause();
            bgmSource.mute = true;
        }
    }

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
        ExpPanel.SetActive(true);
    }

    public void CloseBtn()
    {
        ExpPanel.SetActive(false);
    }
}
