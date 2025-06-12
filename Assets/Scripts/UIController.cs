using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Slider�� ����Ϸ��� �߰��ؾ� �մϴ�.

public class UIController : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject ExpPanel;
    public Slider volumeSlider; // Slider�� ������ ����
    private AudioSource audioSource; // AudioSource ���� �߰�

    void Start()
    {/*
        // AudioSource ������Ʈ�� ã�Ƽ� ����
        audioSource = FindObjectOfType<AudioSource>();

        // ���� ��, ���� ���� �� ���� (PlayerPrefs���� �ҷ�����)
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("Volume", 1f); // �⺻���� 1
            volumeSlider.value = audioSource.volume; // �����̴��� �ʱ� �� ����
        }

        // Slider�� ValueChanged �̺�Ʈ�� ������ �߰�
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }/*/
    }

    // ���� ���� �� ȣ��Ǵ� �޼���
    /*public void OnVolumeChanged(float value)
    {
        if (audioSource != null)
        {
            audioSource.volume = value; // ���� ����
            PlayerPrefs.SetFloat("Volume", value); // ����� ���� ����
        }
    }*/

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
