using UnityEngine;
using System.Collections;
using TMPro;

public class ChopstickController : MonoBehaviour
{
    public Animator animator; // 젓가락 애니메이터
    private GameObject caughtSushi; // 충돌한 초밥 저장용
    public Vector3 targetPosition = new Vector3(-6.75f, -3f, 0f);

    public int score = 0;
    public TextMeshProUGUI ScoreText;

    private void Start()
    {
        animator = GetComponent<Animator>();
        UpdateScoreUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 화면 클릭 감지
        {
            animator.SetTrigger("clickTrigger"); // 애니메이션 실행

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
        }
        else
        {
            score -= 5;
            Debug.Log("오답! -5점");
        }
        UpdateScoreUI(); // 점수 UI 갱신
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
}
