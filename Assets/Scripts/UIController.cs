using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject settingPanel;      // 설정 패널 오브젝트
    public GameObject ExpPanel;          // 설명 패널 오브젝트
    public AudioSource bgmSource;        // BGM 오브젝트의 AudioSource를 인스펙터에서 연결
    public Toggle bgmToggle;             // 배경음 토글 UI
    public GameObject settingsPanel;     // 설정 패널 (젓가락 제어 중단 여부 확인용)
    public bool IsSettingsPanelOpen => settingsPanel != null && settingsPanel.activeSelf;

    void Start()
    {
        // bgm Audio source 자동 연결
        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();

        // 토글 이벤트 리스너 연결
        bgmToggle.onValueChanged.AddListener(OnBgmToggleChanged);

        // 현재 토글 상태에 맞춰 BGM 상태 초기화
        OnBgmToggleChanged(bgmToggle.isOn);
    }

    // 토글 상태 변경 시 BGM 제어
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

    // 설정 패널 열기/닫기
    public void TogglePanel()
    {
        bool isActive = !settingPanel.activeSelf;
        settingPanel.SetActive(isActive);

        // 패널 열면 일시정지, 닫으면 재개
        Time.timeScale = isActive ? 0f : 1f;
    }

    // 재시작 버튼 클릭시
    public void RestartGame()
    {
        // 시간 다시 흐르게 하고 현재 씬 다시 로드
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // 메인으로 돌아가기 버튼 클릭시
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        // SushiGenerator 제거
        if (SushiGenerator.Instance != null)
        {
            Destroy(SushiGenerator.Instance.gameObject);
        }

        SceneManager.LoadScene("StartScene");
    }

    // 게임 시작 버튼 클릭시
    public void StartBtn()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 설명 버튼 클릭시
    public void ExpBtn()
    {
        ExpPanel.SetActive(true);
    }

    // 설명 패널의 닫기 버튼 클릭시
    public void CloseBtn()
    {
        ExpPanel.SetActive(false);
    }
}
