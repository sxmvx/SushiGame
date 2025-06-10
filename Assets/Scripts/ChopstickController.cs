using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ChopstickController : MonoBehaviour
{
    public Animator chopAnimator; // ������ �ִϸ�����
    public Animator catAnimator; // ����� �ִϸ�����
    private GameObject caughtSushi; // �浹�� �ʹ� �����
    public Vector3 targetPosition = new Vector3(-6.75f, -3f, 0f);

    public int score = 0;
    public TextMeshProUGUI ScoreText;

    public int scoreToIncreaseDifficulty = 15; // ���̵� �ø� ���� �Ӱ谪
    public float sushiSpeedIncreaseAmount = 1.0f; // �ʹ� �ӵ� ������

    public SushiGenerator sushiGenerator; // �ν����Ϳ��� SushiGenerator ��ũ��Ʈ�� ���� GameObject�� �������ּ���.


    private void Start()
    {
        chopAnimator = GetComponent<Animator>();
        UpdateScoreUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ȭ�� Ŭ�� ����
        {
            chopAnimator.SetTrigger("clickTrigger"); // �ִϸ��̼� ����

            // �ִϸ��̼� �߿� �浹�� �ʹ� �̵� (�ٷ� ���� �Ǵ� Coroutine���� �ణ ������ ����)
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

        foreach (var staticSushi in SushiGenerator.staticSushis)
        {
            // �̸� �� (������ �̸� ����)
            if (caughtSushi.name.Contains(staticSushi.name.Replace("(Clone)", "").Trim()))
            {
                matchFound = true;
                break;
            }
        }

        if (matchFound)
        {
            score += 3;
            Debug.Log("����! +3��");
            catAnimator.SetTrigger("GetTrigger"); // �ִϸ��̼� ����
        }
        else
        {
            score -= 5;
            Debug.Log("����! -5��");
            catAnimator.SetTrigger("LoseTrigger"); // �ִϸ��̼� ����
        }

        UpdateScoreUI(); // ���� UI ����

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
        Destroy(sushi);
    }
    private void CheckDifficultyAndRestartGame()
    {
        if (score >= scoreToIncreaseDifficulty)
        {
            Debug.Log($"���� {score} �޼�! ���̵� ���� �� ���� ����� �غ�.");

            // ���� ���� �̸��� ������ �ٽ� �ε��մϴ�.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // SushiGenerator�� �ʹ� �ӵ��� ������ŵ�ϴ�.
            if (sushiGenerator != null)
            {
                sushiGenerator.IncreaseSushiSpeed(sushiSpeedIncreaseAmount);
            }
            else
            {
                Debug.LogError("SushiGenerator ������ ���� �ʹ� �ӵ��� ������ų �� �����ϴ�!");
            }

            // �� ����� (�ε�)
            SceneManager.LoadScene(currentSceneName);

            // ���� �Ӱ谪�� ���� ���̵� ������ �°� ���� (���� ����)
            scoreToIncreaseDifficulty += 5; 
        }
    }
}
