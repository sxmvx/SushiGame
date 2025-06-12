using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Slider를 사용하려면 추가해야 합니다.

public class UIController : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject ExpPanel;
    public Slider volumeSlider; // Slider를 연결할 변수
    private AudioSource audioSource; // AudioSource 변수 추가

    void Start()
    {/*
        // AudioSource 컴포넌트를 찾아서 설정
        audioSource = FindObjectOfType<AudioSource>();

        // 시작 시, 기존 볼륨 값 설정 (PlayerPrefs에서 불러오기)
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("Volume", 1f); // 기본값은 1
            volumeSlider.value = audioSource.volume; // 슬라이더의 초기 값 설정
        }

        // Slider의 ValueChanged 이벤트에 리스너 추가
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }/*/
    }

    // 볼륨 변경 시 호출되는 메서드
    /*public void OnVolumeChanged(float value)
    {
        if (audioSource != null)
        {
            audioSource.volume = value; // 볼륨 조절
            PlayerPrefs.SetFloat("Volume", value); // 변경된 볼륨 저장
        }
    }*/

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
