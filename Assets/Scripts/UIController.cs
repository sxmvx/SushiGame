using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject ExpPanel;
    public AudioSource bgmSource; // BGM 오브젝트의 AudioSource를 인스펙터에서 연결
    private bool isBGMOn = true;
    public Toggle bgmToggle;      // 토글 UI

    void Start()
    {
        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();

        // 토글 상태에 맞춰 배경음악 켜기/끄기
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
