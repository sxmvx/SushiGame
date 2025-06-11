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

    public float sushiSpeedIncreaseAmount = 2.0f;
    public float spawnIntervalDecreaseAmount = 0.5f;
    public int scoreIncreaseForNextLevel = 10; // ���� ���̵� ��ǥ ���� ������

    private void Start()
    {
        chopAnimator = GetComponent<Animator>();
        score = 0;
        UpdateScoreUI();

        if (catAnimator == null) Debug.LogError("Cat Animator�� �ν����Ϳ��� ������� �ʾҽ��ϴ�! Ȯ�����ּ���.", this);
        if (ScoreText == null) Debug.LogError("ScoreText (TextMeshProUGUI)�� �ν����Ϳ��� ������� �ʾҽ��ϴ�!", this);
        if (chopAnimator == null) Debug.LogError("Chop Animator�� �� GameObject�� �����ϴ�! Ȯ�����ּ���.", this);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (chopAnimator != null)
            {
                chopAnimator.SetTrigger("clickTrigger");
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
        }
        else
        {
            score -= 5;
            Debug.Log("����! -5��");
            if (catAnimator != null) catAnimator.SetTrigger("LoseTrigger");
        }

        UpdateScoreUI();

        CheckDifficultyAndRestartGame();
    }

    private void UpdateScoreUI()
    {
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + score;
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

}