using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 게임 실패/엔딩시 메인 화면으로 돌아가기
    private void OnMouseDown()
    {
        SceneManager.LoadScene("StartScene");
    }
}
