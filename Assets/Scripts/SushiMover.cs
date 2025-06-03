using UnityEngine;

public class SushiMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    public bool canMove = true; // 움직임 여부 설정
    void Update()
    {
        if (!canMove) return;

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (transform.position.x < -13f)
        {
            Destroy(gameObject);
        }
    }

}
