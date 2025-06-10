using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SushiGenerator : MonoBehaviour
{
    public GameObject[] sushiPrefabs;
    public GameObject plate;
    private Vector3 spawnPosition = new Vector3(12f, 0.5f, 0f);
    private Vector3 spawnPosition2 = new Vector3(12f, 0f, 0f);

    public static List<GameObject> staticSushis = new List<GameObject>(); // 고정 초밥 리스트

    public float initialSushiSpeed = 2f; // 초밥의 초기 이동 속도
    private float currentSushiSpeed;     // 현재 초밥의 이동 속도

    void Awake()
    {
        // 씬 로드 시마다 staticSushis 리스트를 초기화
        if (staticSushis == null)
        {
            staticSushis = new List<GameObject>();
        }
        else
        {
            staticSushis.Clear(); // 씬이 로드될 때마다 이전 리스트를 비웁니다.
        }

        // 초기 속도 설정
        currentSushiSpeed = initialSushiSpeed;
    }
    void Start()
    {
        CreateStaticSushi();

        InvokeRepeating("SpawnSushi", 1f, 2f); // 2초마다 생성
    }

    void SpawnSushi()
    {
        int index = Random.Range(0, sushiPrefabs.Length);
        GameObject newSushi = Instantiate(sushiPrefabs[index], spawnPosition, Quaternion.identity);
        Instantiate(plate, spawnPosition2, Quaternion.identity);

        SushiMover mover = newSushi.GetComponent<SushiMover>();
        if (mover != null)
        {
            mover.moveSpeed = currentSushiSpeed; // 현재 설정된 속도를 초밥에 적용
        }
        else
        {
            Debug.LogWarning($"생성된 초밥 '{newSushi.name}'에 SushiMover 컴포넌트가 없습니다.", newSushi);
        }
    }

    void CreateStaticSushi()
    {
        Vector3[] fixedPositions = new Vector3[]
        {
            new Vector3(-3f, 3f, 0f),
            new Vector3(0f, 3f, 0f),
            new Vector3(3f, 3f, 0f)
        };

        staticSushis.Clear();

        // 중복 없이 3개 선택
        List<int> indices = Enumerable.Range(0, sushiPrefabs.Length).ToList();
        indices = indices.OrderBy(x => Random.value).ToList(); // 랜덤 셔플

        for (int i = 0; i < 3 && i < indices.Count; i++)
        {
            int prefabIndex = indices[i];
            Vector3 pos = fixedPositions[i];

            GameObject sushi = Instantiate(sushiPrefabs[prefabIndex], pos, Quaternion.identity);

            // 움직이지 않도록 설정
            SushiMover mover = sushi.GetComponent<SushiMover>();
            if (mover != null)
            {
                mover.canMove = false;
            }

            staticSushis.Add(sushi);
        }
    }
    public void IncreaseSushiSpeed(float amount)
    {
        currentSushiSpeed += amount;
        Debug.Log($"초밥 이동 속도 증가! 새로운 속도: {currentSushiSpeed}");

        // 선택 사항: 이미 씬에 있는 움직이는 초밥들의 속도도 업데이트
        // (현재 InvokeRepeating으로 초밥이 계속 생성되므로, 이 부분은 다음 초밥부터 적용됩니다.)
        // 만약 이미 화면에 있는 초밥들의 속도도 바로 바뀌길 원한다면,
        // 모든 'Sushi' 태그를 가진 GameObject를 찾아서 SushiMover.moveSpeed를 업데이트해야 합니다.
        // 하지만 게임 재시작 시 모든 초밥이 사라지므로, 굳이 필요하지 않을 수 있습니다.
    }

    public void ResetSushiSpeed()
    {
        currentSushiSpeed = initialSushiSpeed;
        Debug.Log($"초밥 이동 속도 초기화! 새로운 속도: {currentSushiSpeed}");
    }

}
