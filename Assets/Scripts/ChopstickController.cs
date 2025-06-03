using UnityEngine;
using System.Collections;
using TMPro;

public class ChopstickController : MonoBehaviour
{
    public Animator animator; // ������ �ִϸ�����
    private GameObject caughtSushi; // �浹�� �ʹ� �����
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
        if (Input.GetMouseButtonDown(0)) // ȭ�� Ŭ�� ����
        {
            animator.SetTrigger("clickTrigger"); // �ִϸ��̼� ����

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
        }
        else
        {
            score -= 5;
            Debug.Log("����! -5��");
        }
        UpdateScoreUI(); // ���� UI ����
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
