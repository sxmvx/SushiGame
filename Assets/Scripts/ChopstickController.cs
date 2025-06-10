using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ChopstickController : MonoBehaviour
{
    public Animator chopAnimator; // 젓가락 애니메이터
    public Animator catAnimator; // 고양이 애니메이터
    private GameObject caughtSushi; // 충돌한 초밥 저장용
    public Vector3 targetPosition = new Vector3(-6.75f, -3f, 0f);

    public int score = 0;
    public TextMeshProUGUI ScoreText;

    public int scoreToIncreaseDifficulty = 15; // 난이도 올릴 점수 임계값
    public float sushiSpeedIncreaseAmount = 1.0f; // 초밥 속도 증가량

    public SushiGenerator sushiGenerator; // 인스펙터에서 SushiGenerator 스크립트가 붙은 GameObject를 연결해주세요.


    private void Start()
    {
        chopAnimator = GetComponent<Animator>();
        UpdateScoreUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 화면 클릭 감지
        {
            chopAnimator.SetTrigger("clickTrigger"); // 애니메이션 실행

            // 애니메이션 중에 충돌된 초밥 이동 (바로 실행 또는 Coroutine으로 약간 딜레이 가능)
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
            // 이름 비교 (프리팹 이름 기준)
            if (caughtSushi.name.Contains(staticSushi.name.Replace("(Clone)", "").Trim()))
            {
                matchFound = true;
                break;
            }
        }

        if (matchFound)
        {
            score += 3;
            Debug.Log("정답! +3점");
            catAnimator.SetTrigger("GetTrigger"); // 애니메이션 실행
        }
        else
        {
            score -= 5;
            Debug.Log("오답! -5점");
            catAnimator.SetTrigger("LoseTrigger"); // 애니메이션 실행
        }

        UpdateScoreUI(); // 점수 UI 갱신

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
            Debug.Log($"점수 {score} 달성! 난이도 증가 및 게임 재시작 준비.");

            // 현재 씬의 이름을 가져와 다시 로드합니다.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // SushiGenerator의 초밥 속도를 증가시킵니다.
            if (sushiGenerator != null)
            {
                sushiGenerator.IncreaseSushiSpeed(sushiSpeedIncreaseAmount);
            }
            else
            {
                Debug.LogError("SushiGenerator 참조가 없어 초밥 속도를 증가시킬 수 없습니다!");
            }

            // 씬 재시작 (로딩)
            SceneManager.LoadScene(currentSceneName);

            // 점수 임계값을 다음 난이도 레벨에 맞게 조정 (선택 사항)
            scoreToIncreaseDifficulty += 5; 
        }
    }
}
