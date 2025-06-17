using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ChopstickController : MonoBehaviour
{
    public Animator chopAnimator;
    public Animator catAnimator;
    private GameObject caughtSushi;
    public Vector3 targetPosition = new Vector3(-6.75f, -3f, 0f);       // 먹은 초밥 접시로 이동

    public int score = 0; // 현재 점수
    public TextMeshProUGUI ScoreText;   
    public TextMeshProUGUI LifeText;   // 목숨 UI
    private int life = 3;              // 남은 목숨 수

    public float sushiSpeedIncreaseAmount = 2.0f;   // 이동 속도 증가값
    public float spawnIntervalDecreaseAmount = 0.5f;    // 생성 주기 감소값
    public int scoreIncreaseForNextLevel = 10; // 다음 난이도 목표 점수 증가량

    // 효과음 관련 변수
    public AudioClip correctSound;  // 정답일 때의 효과음
    public AudioClip wrongSound;    // 오답일 때의 효과음
    private AudioSource audioSource; // AudioSource 컴포넌트

    public UnityEngine.UI.Toggle soundToggle; // 인스펙터에서 연결
    private bool isSoundOn = true;            // 기본값 ON

    public UIController uiController;  // 인스펙터에서 연결

    private void Start()
    {
        // 애니메이션, 점수, 생명주기 초기화
        chopAnimator = GetComponent<Animator>();
        score = 0;
        UpdateScoreUI();
        life = 3;
        UpdateLifeUI();

        // 확인용 로그 출력
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
        // 설정 패널 활성화 되었을 때는 젓가락 클릭 이벤트 실행 X
        if (uiController != null && uiController.IsSettingsPanelOpen)
            return;
        else    // 설정 패널 비활성화일 때만 젓가락 클릭 이벤트 실행
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (chopAnimator != null)
                {
                    chopAnimator.SetTrigger("clickTrigger");
                    // Chop sound 효과음 재생
                    if (audioSource != null && isSoundOn)
                    {
                        audioSource.Play();
                    }
                }
                // 클릭해서 초밥 먹었을 시 
                if (caughtSushi != null)
                {
                    Score();    // 점수 세팅

                    caughtSushi.transform.position = targetPosition;    // 초밥 접시로 이동

                    SushiMover mover = caughtSushi.GetComponent<SushiMover>();
                    if (mover != null) mover.canMove = false;   // 이동한 초밥은 회전 멈춤

                    StartCoroutine(DestroySushiAfterDelay(caughtSushi, 3f));    // 3초 딜레이 후 삭제

                    caughtSushi = null;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {  
        // 충돌 시 태그가 Sushi인 프리팹만 저장
        if (other.CompareTag("Sushi"))
        {
            caughtSushi = other.gameObject;
        }
    }

    // 점수 세팅 함수
    private void Score()
    {
        // 목표 초밥과 동일한지 판별 후 저장하는 변수
        bool matchFound = false;

        if (SushiGenerator.staticSushis == null || SushiGenerator.staticSushis.Count == 0)
        {
            Debug.LogWarning("SushiGenerator.staticSushis가 비어있거나 초기화되지 않았습니다. 초밥 비교 불가.");
        }
        else
        {
            // 목표 초밥을 먹은 경우
            foreach (var staticSushi in SushiGenerator.staticSushis)
            {
                if (staticSushi != null && caughtSushi.name.Contains(staticSushi.name.Trim()))
                {
                    matchFound = true;
                    break;
                }
            }
        }

        // 목표 초밥을 먹어 matchFound == true -> 점수 획득
        if (matchFound)
        {
            score += 3;
            Debug.Log("정답! +3점");
            if (catAnimator != null) catAnimator.SetTrigger("GetTrigger");

            // 정답일 때의 효과음 재생
            if (audioSource != null && correctSound != null && isSoundOn)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }
        // 목표 초밥이 아닌 초밥을 먹었을 경우 
        else
        {
            score -= 5;
            Debug.Log("오답! -5점");
            if (catAnimator != null) catAnimator.SetTrigger("LoseTrigger");

            if (audioSource != null && wrongSound != null && isSoundOn)
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

        // 점수 UI 업데이트
        UpdateScoreUI();

        // 목표 점수 넘었을 경우 레벨 세팅 & 재시작
        CheckDifficultyAndRestartGame();
    }

    // SFX 세팅
    public void OnSoundToggleChanged(bool isOn)
    {
        isSoundOn = isOn;

        if (audioSource != null)
        {
            audioSource.mute = !isOn; // 직접 뮤트 처리도 가능
        }

        Debug.Log("효과음 " + (isOn ? "켜짐" : "꺼짐"));
    }

    // 점수 UI 업데이트
    private void UpdateScoreUI()
    {
        if (ScoreText != null)
        {
            ScoreText.text = "점수 " + score;
        }
    }

    // 딜레이 후 프리팹 제거
    private IEnumerator DestroySushiAfterDelay(GameObject sushi, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sushi != null)
        {
            Destroy(sushi);
        }
    }

    // 레벨 설정 & 게임 재시작
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

    // 목숨 감소 세팅
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
