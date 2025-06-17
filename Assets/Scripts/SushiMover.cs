using UnityEngine;

public class SushiMover : MonoBehaviour
{
    // 초밥 이동 속도
    public float moveSpeed = 2f;
    public bool canMove = true; // 움직임 여부 설정
    
    void Update()
    {
        // 움직이지 않는 초밥일 경우
        if (!canMove) return;

        // 움직이는 초밥일 경우 이동
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // 초밥이 화면에서 벗어났을 시 오브젝트 제거
        if (transform.position.x < -13f)
        {
            Destroy(gameObject);
        }
    }

}
