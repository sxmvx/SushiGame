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

    // ChopstickController에서는 더 이상 난이도 임계값을 직접 관리하지 않습니다.
    // public int scoreToIncreaseDifficulty = 15;
    public float sushiSpeedIncreaseAmount = 2.0f;
    public float spawnIntervalDecreaseAmount = 0.2f;
    public int scoreIncreaseForNextLevel = 5; // 다음 난이도 목표 점수 증가량

    private void Start()
    {
        chopAnimator = GetComponent<Animator>();
        // 씬이 재로드될 때 ChopstickController는 새로 생성되므로,
        // 현재 점수를 0으로 초기화해야 합니다.
        score = 0;
        UpdateScoreUI();

        // 게임이 처음 시작될 때 (예: 메인 메뉴에서 게임 씬으로 넘어올 때)
        // Time.timeScale = 1f; 을 한 번만 설정하는 것이 좋습니다.
        // 여기서는 매 씬 로드 시 ChopstickController가 생성되므로 삭제합니다.

        if (catAnimator == null) Debug.LogError("Cat Animator가 인스펙터에서 연결되지 않았습니다! 확인해주세요.", this);
        if (ScoreText == null) Debug.LogError("ScoreText (TextMeshProUGUI)가 인스펙터에서 연결되지 않았습니다!", this);
        if (chopAnimator == null) Debug.LogError("Chop Animator가 이 GameObject에 없습니다! 확인해주세요.", this);

        // 게임 세션이 새로 시작될 때 (예: 메인 메뉴에서 '게임 시작' 버튼 클릭 시)
        // SushiGenerator.Instance.ResetGameDifficulty(); 를 호출해줘야 합니다.
        // 지금은 씬 재시작이 '다음 라운드'의 개념으로 사용되므로,
        // 씬 재시작 시 난이도 설정을 초기화할 필요는 없습니다.
        // 만약 게임 오버 후 다시 시작할 때 전체 난이도를 리셋하고 싶다면,
        // 별도의 게임오버 씬이나 로직에서 이 메서드를 호출해야 합니다.
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
        // SushiGenerator의 CurrentScoreToIncreaseDifficulty 속성을 통해 현재 난이도 목표 점수를 가져옵니다.
        if (SushiGenerator.Instance != null && score >= SushiGenerator.Instance.CurrentScoreToIncreaseDifficulty)
        {
            Debug.Log($"점수 {score} 달성! 난이도 증가 및 게임 재시작 준비.");

            // SushiGenerator의 AdvanceDifficulty 메서드를 호출하여 난이도 관련 변수들을 업데이트합니다.
            if (SushiGenerator.Instance != null)
            {
                SushiGenerator.Instance.AdvanceDifficulty(sushiSpeedIncreaseAmount, spawnIntervalDecreaseAmount, scoreIncreaseForNextLevel);
                Debug.Log($"ChopstickController: 난이도 상승 요청 완료. 현재 속도: {SushiGenerator.Instance.CurrentSushiSpeed}, 현재 주기: {SushiGenerator.Instance.CurrentSpawnInterval}, 다음 목표 점수: {SushiGenerator.Instance.CurrentScoreToIncreaseDifficulty}");
            }
            else
            {
                Debug.LogError("ChopstickController: SushiGenerator.Instance를 찾을 수 없습니다! 난이도 상승을 처리할 수 없습니다.");
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);

            // ChopstickController의 score는 씬 재시작 시 Start()에서 0으로 초기화됩니다.
            // scoreToIncreaseDifficulty는 더 이상 여기서 관리하지 않습니다.
        }
    }
}