using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SushiGenerator : MonoBehaviour
{
    public GameObject[] sushiPrefabs;
    public GameObject plate;
    private Vector3 spawnPosition = new Vector3(12f, 0.5f, 0f);
    private Vector3 spawnPosition2 = new Vector3(12f, 0f, 0f);

    public static List<GameObject> staticSushis = new List<GameObject>();

    public float initialSushiSpeed = 2f;
    private float currentSushiSpeed;

    public float initialSpawnInterval = 2f;
    private float currentSpawnInterval;
    public float minSpawnInterval = 0.5f;

    // --- 새로 추가된 난이도 관련 변수들 ---
    public int baseScoreToIncreaseDifficulty = 15; // 기본 난이도 상승 점수
    private int currentScoreToIncreaseDifficulty;  // 현재 난이도 상승 목표 점수
    public int difficultyLevel = 0; // 현재 난이도 레벨 (0부터 시작)
    // ---

    public float CurrentSushiSpeed
    {
        get { return currentSushiSpeed; }
    }

    public float CurrentSpawnInterval
    {
        get { return currentSpawnInterval; }
    }

    public static SushiGenerator Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 게임이 처음 시작될 때만 초기화
        ResetGameDifficulty(); // 게임 시작 시 난이도 관련 변수들 초기화

        Debug.Log($"SushiGenerator Awake: 초기 속도 {currentSushiSpeed}, 초기 생성 주기 {currentSpawnInterval}, 초기 난이도 목표 {currentScoreToIncreaseDifficulty}로 설정됨.");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SushiGenerator OnSceneLoaded: 씬 '{scene.name}' 로드됨. 게임 요소 초기화 시작.");

        CancelInvoke("SpawnSushi");
        Debug.Log("이전 SpawnSushi InvokeRepeating 취소 완료.");

        GameObject[] existingMovers = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(obj => obj.name.Contains("Sushi(Clone)") || obj.name.Contains("Plate(Clone)"))
            .ToArray();

        foreach (GameObject obj in existingMovers)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        Debug.Log($"이전 움직이는 초밥/접시 오브젝트 {existingMovers.Length}개 파괴 완료.");

        CreateStaticSushi();
        Debug.Log("목표 초밥 재설정 완료.");

        InvokeRepeating("SpawnSushi", 1f, currentSpawnInterval);
        Debug.Log($"SpawnSushi InvokeRepeating 재시작. 현재 스폰 주기: {currentSpawnInterval}초.");
    }

    void Start()
    {
        // (기존 Start 내용)
    }

    public void SpawnSushi()
    {
        GameObject newSushi = Instantiate(sushiPrefabs[Random.Range(0, sushiPrefabs.Length)], spawnPosition, Quaternion.identity);
        SushiMover sushiMover = newSushi.GetComponent<SushiMover>();
        if (sushiMover != null)
        {
            sushiMover.moveSpeed = currentSushiSpeed;
        }
        else
        {
            Debug.LogWarning($"생성된 초밥 '{newSushi.name}'에 SushiMover 컴포넌트가 없습니다. 초밥 속도 설정 불가.", newSushi);
        }

        GameObject newPlate = Instantiate(plate, spawnPosition2, Quaternion.identity);
        SushiMover plateMover = newPlate.GetComponent<SushiMover>();
        if (plateMover != null)
        {
            plateMover.moveSpeed = currentSushiSpeed;
        }
        else
        {
            Debug.LogWarning($"생성된 접시 '{newPlate.name}'에 SushiMover 컴포넌트가 없습니다. 접시 속도 설정 불가.", newPlate);
        }
    }

    public void CreateStaticSushi()
    {
        Vector3[] fixedPositions = new Vector3[]
        {
            new Vector3(-3f, 3f, 0f),
            new Vector3(0f, 3f, 0f),
            new Vector3(3f, 3f, 0f)
        };

        foreach (GameObject sushi in staticSushis)
        {
            if (sushi != null) Destroy(sushi);
        }
        staticSushis.Clear();

        List<int> indices = Enumerable.Range(0, sushiPrefabs.Length).ToList();
        indices = indices.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < 3 && i < indices.Count; i++)
        {
            int prefabIndex = indices[i];
            Vector3 pos = fixedPositions[i];
            GameObject sushi = Instantiate(sushiPrefabs[prefabIndex], pos, Quaternion.identity);

            SushiMover mover = sushi.GetComponent<SushiMover>();
            if (mover != null)
            {
                mover.canMove = false;
            }
            sushi.name = sushiPrefabs[prefabIndex].name;
            staticSushis.Add(sushi);
        }
        Debug.Log($"새로운 목표 초밥 {staticSushis.Count}개 생성 완료.");
    }

    public void IncreaseSushiSpeed(float amount)
    {
        currentSushiSpeed += amount;
        Debug.Log($"SushiGenerator: 초밥/접시 이동 속도 증가! 현재 속도: {currentSushiSpeed}");
    }

    public void DecreaseSpawnInterval(float amount)
    {
        currentSpawnInterval -= amount;
        if (currentSpawnInterval < minSpawnInterval)
        {
            currentSpawnInterval = minSpawnInterval;
        }
        Debug.Log($"SushiGenerator: 초밥 생성 주기 감소! 현재 주기: {currentSpawnInterval}초");
    }

    // --- 새로 추가된 난이도 관리 메서드 ---
    public void AdvanceDifficulty(float speedIncrease, float intervalDecrease, int scoreIncrease)
    {
        difficultyLevel++; // 난이도 레벨 증가

        IncreaseSushiSpeed(speedIncrease); // 초밥 속도 증가
        DecreaseSpawnInterval(intervalDecrease); // 생성 주기 감소

        currentScoreToIncreaseDifficulty += scoreIncrease; // 다음 난이도 목표 점수 증가

        Debug.Log($"SushiGenerator: 난이도 레벨 {difficultyLevel}로 상승! 다음 목표 점수: {currentScoreToIncreaseDifficulty}");
    }

    // 현재 난이도 목표 점수를 외부에서 가져올 수 있는 속성
    public int CurrentScoreToIncreaseDifficulty
    {
        get { return currentScoreToIncreaseDifficulty; }
    }

    // 게임 시작 시 모든 난이도 관련 변수들을 초기 상태로 되돌리는 메서드
    public void ResetGameDifficulty()
    {
        currentSushiSpeed = initialSushiSpeed;
        currentSpawnInterval = initialSpawnInterval;
        currentScoreToIncreaseDifficulty = baseScoreToIncreaseDifficulty;
        difficultyLevel = 0; // 난이도 레벨 초기화

        Debug.Log($"SushiGenerator: 게임 난이도 초기화! 속도: {currentSushiSpeed}, 주기: {currentSpawnInterval}, 목표 점수: {currentScoreToIncreaseDifficulty}, 레벨: {difficultyLevel}");
    }
    // ---
}