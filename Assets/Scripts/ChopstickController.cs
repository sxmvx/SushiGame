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

    public int score = 0; // 현재 점수
    public TextMeshProUGUI ScoreText;

    public float sushiSpeedIncreaseAmount = 2.0f;
    public float spawnIntervalDecreaseAmount = 0.5f;
    public int scoreIncreaseForNextLevel = 10; // 다음 난이도 목표 점수 증가량

    private void Start()
    {
        chopAnimator = GetComponent<Animator>();
        score = 0;
        UpdateScoreUI();

        if (catAnimator == null) Debug.LogError("Cat Animator가 인스펙터에서 연결되지 않았습니다! 확인해주세요.", this);
        if (ScoreText == null) Debug.LogError("ScoreText (TextMeshProUGUI)가 인스펙터에서 연결되지 않았습니다!", this);
        if (chopAnimator == null) Debug.LogError("Chop Animator가 이 GameObject에 없습니다! 확인해주세요.", this);
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
            Debug.LogWarning("SushiGenerator.staticSushis가 비어있거나 초기화되지 않았습니다. 초밥 비교 불가.");
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
            Debug.Log("정답! +3점");
            if (catAnimator != null) catAnimator.SetTrigger("GetTrigger");
        }
        else
        {
            score -= 5;
            Debug.Log("오답! -5점");
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
            Debug.Log($"점수 {score} 달성! 난이도 증가 및 게임 재시작 준비.");

            if (SushiGenerator.Instance != null)
            {
                SushiGenerator.Instance.AdvanceDifficulty(sushiSpeedIncreaseAmount, spawnIntervalDecreaseAmount, scoreIncreaseForNextLevel);

                Debug.Log($"ChopstickController: 난이도 상승 완료. 현재 레벨: {SushiGenerator.Instance.difficultyLevel}");
            }

            // 레벨이 5 이상이면 엔딩 씬으로 이동
            if (SushiGenerator.Instance.difficultyLevel >= 5)
            {
                Debug.Log("레벨 5 도달! EndingScene으로 이동합니다.");

                // SushiGenerator 제거
                if (SushiGenerator.Instance != null)
                {
                    Destroy(SushiGenerator.Instance.gameObject); // 게임 오브젝트 제거
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