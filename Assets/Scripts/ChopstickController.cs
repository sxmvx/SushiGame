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
    public TextMeshProUGUI LifeText;   // 목숨 UI
    private int life = 3;              // 남은 목숨 수

    public float sushiSpeedIncreaseAmount = 2.0f;
    public float spawnIntervalDecreaseAmount = 0.5f;
    public int scoreIncreaseForNextLevel = 10; // 다음 난이도 목표 점수 증가량

    // 효과음 관련 변수
    public AudioClip correctSound;  // 정답일 때의 효과음
    public AudioClip wrongSound;    // 오답일 때의 효과음
    private AudioSource audioSource; // AudioSource 컴포넌트

    private void Start()
    {
        chopAnimator = GetComponent<Animator>();
        score = 0;
        UpdateScoreUI();
        life = 3;
        UpdateLifeUI();

        if (LifeText == null) Debug.LogError("LifeText가 인스펙터에서 연결되지 않았습니다!", this);
        if (catAnimator == null) Debug.LogError("Cat Animator가 인스펙터에서 연결되지 않았습니다! 확인해주세요.", this);
        if (ScoreText == null) Debug.LogError("ScoreText (TextMeshProUGUI)가 인스펙터에서 연결되지 않았습니다!", this);
        if (chopAnimator == null) Debug.LogError("Chop Animator가 이 GameObject에 없습니다! 확인해주세요.", this);

        // AudioSource 컴포넌트 연결
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource가 이 GameObject에 없습니다! 확인해주세요.", this);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (chopAnimator != null)
            {
                chopAnimator.SetTrigger("clickTrigger");
                // Chop sound 효과음 재생
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

            // 정답일 때의 효과음 재생
            if (audioSource != null && correctSound != null)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }
        else
        {
            score -= 5;
            Debug.Log("오답! -5점");
            if (catAnimator != null) catAnimator.SetTrigger("LoseTrigger");

            if (audioSource != null && wrongSound != null)
            {
                audioSource.PlayOneShot(wrongSound);
            }

            // 목숨 감소
            life--;
            UpdateLifeUI();

            // 목숨 다 떨어졌으면 FailScene으로 이동
            if (life <= 0)
            {
                Debug.Log("목숨이 모두 사라졌습니다! FailScene으로 이동합니다.");

                if (SushiGenerator.Instance != null)
                {
                    Destroy(SushiGenerator.Instance.gameObject);
                }

                Time.timeScale = 1f;
                SceneManager.LoadScene("FailScene");
                return; // 더 이상 처리하지 않도록 return
            }
        }

        UpdateScoreUI();

        CheckDifficultyAndRestartGame();
    }

    private void UpdateScoreUI()
    {
        if (ScoreText != null)
        {
            ScoreText.text = "점수 " + score;
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
    private void UpdateLifeUI()
    {
        string[] hearts = new string[] { "♡", "♡", "♡" };
        for (int i = 0; i < life; i++)
        {
            hearts[i] = "♥";
        }
        LifeText.text = string.Join(" ", hearts); // 예: ♥ ♥ ♡
    }
}
