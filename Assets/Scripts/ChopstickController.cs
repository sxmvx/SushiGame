using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ChopstickController : MonoBehaviour
{
    public Animator chopAnimator;
    public Animator catAnimator;
    private GameObject caughtSushi;
    public Vector3 targetPosition = new Vector3(-6.75f, -3f, 0f);

    public int score = 0; // ���� ����
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI LifeText;   // ��� UI
    private int life = 3;              // ���� ��� ��

    public float sushiSpeedIncreaseAmount = 2.0f;
    public float spawnIntervalDecreaseAmount = 0.5f;
    public int scoreIncreaseForNextLevel = 10; // ���� ���̵� ��ǥ ���� ������

    // ȿ���� ���� ����
    public AudioClip correctSound;  // ������ ���� ȿ����
    public AudioClip wrongSound;    // ������ ���� ȿ����
    private AudioSource audioSource; // AudioSource ������Ʈ

    private void Start()
    {
        chopAnimator = GetComponent<Animator>();
        score = 0;
        UpdateScoreUI();
        life = 3;
        UpdateLifeUI();

        if (LifeText == null) Debug.LogError("LifeText�� �ν����Ϳ��� ������� �ʾҽ��ϴ�!", this);
        if (catAnimator == null) Debug.LogError("Cat Animator�� �ν����Ϳ��� ������� �ʾҽ��ϴ�! Ȯ�����ּ���.", this);
        if (ScoreText == null) Debug.LogError("ScoreText (TextMeshProUGUI)�� �ν����Ϳ��� ������� �ʾҽ��ϴ�!", this);
        if (chopAnimator == null) Debug.LogError("Chop Animator�� �� GameObject�� �����ϴ�! Ȯ�����ּ���.", this);

        // AudioSource ������Ʈ ����
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource�� �� GameObject�� �����ϴ�! Ȯ�����ּ���.", this);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (chopAnimator != null)
            {
                chopAnimator.SetTrigger("clickTrigger");
                // Chop sound ȿ���� ���
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }

            if (caughtSushi != null)
            {
                Score();

                caughtSushi.transform.position = targetPosition;

                SushiMover mover = caughtSushi.GetComponent<SushiMover>();
                if (mover != null) mover.canMove = false;

                StartCoroutine(DestroySushiAfterDelay(caughtSushi, 3f));

                caughtSushi = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sushi"))
        {
            caughtSushi = other.gameObject;
        }
    }

    private void Score()
    {
        bool matchFound = false;

        if (SushiGenerator.staticSushis == null || SushiGenerator.staticSushis.Count == 0)
        {
            Debug.LogWarning("SushiGenerator.staticSushis�� ����ְų� �ʱ�ȭ���� �ʾҽ��ϴ�. �ʹ� �� �Ұ�.");
        }
        else
        {
            foreach (var staticSushi in SushiGenerator.staticSushis)
            {
                if (staticSushi != null && caughtSushi.name.Contains(staticSushi.name.Trim()))
                {
                    matchFound = true;
                    break;
                }
            }
        }

        if (matchFound)
        {
            score += 3;
            Debug.Log("����! +3��");
            if (catAnimator != null) catAnimator.SetTrigger("GetTrigger");

            // ������ ���� ȿ���� ���
            if (audioSource != null && correctSound != null)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }
        else
        {
            score -= 5;
            Debug.Log("����! -5��");
            if (catAnimator != null) catAnimator.SetTrigger("LoseTrigger");

            if (audioSource != null && wrongSound != null)
            {
                audioSource.PlayOneShot(wrongSound);
            }

            // ��� ����
            life--;
            UpdateLifeUI();

            // ��� �� ���������� FailScene���� �̵�
            if (life <= 0)
            {
                Debug.Log("����� ��� ��������ϴ�! FailScene���� �̵��մϴ�.");

                if (SushiGenerator.Instance != null)
                {
                    Destroy(SushiGenerator.Instance.gameObject);
                }

                Time.timeScale = 1f;
                SceneManager.LoadScene("FailScene");
                return; // �� �̻� ó������ �ʵ��� return
            }
        }

        UpdateScoreUI();

        CheckDifficultyAndRestartGame();
    }

    private void UpdateScoreUI()
    {
        if (ScoreText != null)
        {
            ScoreText.text = "���� " + score;
        }
    }

    private IEnumerator DestroySushiAfterDelay(GameObject sushi, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sushi != null)
        {
            Destroy(sushi);
        }
    }

    private void CheckDifficultyAndRestartGame()
    {
        if (SushiGenerator.Instance != null && score >= SushiGenerator.Instance.CurrentScoreToIncreaseDifficulty)
        {
            Debug.Log($"���� {score} �޼�! ���̵� ���� �� ���� ����� �غ�.");

            if (SushiGenerator.Instance != null)
            {
                SushiGenerator.Instance.AdvanceDifficulty(sushiSpeedIncreaseAmount, spawnIntervalDecreaseAmount, scoreIncreaseForNextLevel);

                Debug.Log($"ChopstickController: ���̵� ��� �Ϸ�. ���� ����: {SushiGenerator.Instance.difficultyLevel}");
            }

            // ������ 5 �̻��̸� ���� ������ �̵�
            if (SushiGenerator.Instance.difficultyLevel >= 5)
            {
                Debug.Log("���� 5 ����! EndingScene���� �̵��մϴ�.");

                // SushiGenerator ����
                if (SushiGenerator.Instance != null)
                {
                    Destroy(SushiGenerator.Instance.gameObject); // ���� ������Ʈ ����
                }

                Time.timeScale = 1f;
                SceneManager.LoadScene("EndingScene");
            }
            else
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }
    private void UpdateLifeUI()
    {
        string[] hearts = new string[] { "��", "��", "��" };
        for (int i = 0; i < life; i++)
        {
            hearts[i] = "��";
        }
        LifeText.text = string.Join(" ", hearts); // ��: �� �� ��
    }
}
