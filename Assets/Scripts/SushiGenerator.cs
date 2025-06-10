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

    // --- ���� �߰��� ���̵� ���� ������ ---
    public int baseScoreToIncreaseDifficulty = 15; // �⺻ ���̵� ��� ����
    private int currentScoreToIncreaseDifficulty;  // ���� ���̵� ��� ��ǥ ����
    public int difficultyLevel = 0; // ���� ���̵� ���� (0���� ����)
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

        // ������ ó�� ���۵� ���� �ʱ�ȭ
        ResetGameDifficulty(); // ���� ���� �� ���̵� ���� ������ �ʱ�ȭ

        Debug.Log($"SushiGenerator Awake: �ʱ� �ӵ� {currentSushiSpeed}, �ʱ� ���� �ֱ� {currentSpawnInterval}, �ʱ� ���̵� ��ǥ {currentScoreToIncreaseDifficulty}�� ������.");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SushiGenerator OnSceneLoaded: �� '{scene.name}' �ε��. ���� ��� �ʱ�ȭ ����.");

        CancelInvoke("SpawnSushi");
        Debug.Log("���� SpawnSushi InvokeRepeating ��� �Ϸ�.");

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
        Debug.Log($"���� �����̴� �ʹ�/���� ������Ʈ {existingMovers.Length}�� �ı� �Ϸ�.");

        CreateStaticSushi();
        Debug.Log("��ǥ �ʹ� �缳�� �Ϸ�.");

        InvokeRepeating("SpawnSushi", 1f, currentSpawnInterval);
        Debug.Log($"SpawnSushi InvokeRepeating �����. ���� ���� �ֱ�: {currentSpawnInterval}��.");
    }

    void Start()
    {
        // (���� Start ����)
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
            Debug.LogWarning($"������ �ʹ� '{newSushi.name}'�� SushiMover ������Ʈ�� �����ϴ�. �ʹ� �ӵ� ���� �Ұ�.", newSushi);
        }

        GameObject newPlate = Instantiate(plate, spawnPosition2, Quaternion.identity);
        SushiMover plateMover = newPlate.GetComponent<SushiMover>();
        if (plateMover != null)
        {
            plateMover.moveSpeed = currentSushiSpeed;
        }
        else
        {
            Debug.LogWarning($"������ ���� '{newPlate.name}'�� SushiMover ������Ʈ�� �����ϴ�. ���� �ӵ� ���� �Ұ�.", newPlate);
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
        Debug.Log($"���ο� ��ǥ �ʹ� {staticSushis.Count}�� ���� �Ϸ�.");
    }

    public void IncreaseSushiSpeed(float amount)
    {
        currentSushiSpeed += amount;
        Debug.Log($"SushiGenerator: �ʹ�/���� �̵� �ӵ� ����! ���� �ӵ�: {currentSushiSpeed}");
    }

    public void DecreaseSpawnInterval(float amount)
    {
        currentSpawnInterval -= amount;
        if (currentSpawnInterval < minSpawnInterval)
        {
            currentSpawnInterval = minSpawnInterval;
        }
        Debug.Log($"SushiGenerator: �ʹ� ���� �ֱ� ����! ���� �ֱ�: {currentSpawnInterval}��");
    }

    // --- ���� �߰��� ���̵� ���� �޼��� ---
    public void AdvanceDifficulty(float speedIncrease, float intervalDecrease, int scoreIncrease)
    {
        difficultyLevel++; // ���̵� ���� ����

        IncreaseSushiSpeed(speedIncrease); // �ʹ� �ӵ� ����
        DecreaseSpawnInterval(intervalDecrease); // ���� �ֱ� ����

        currentScoreToIncreaseDifficulty += scoreIncrease; // ���� ���̵� ��ǥ ���� ����

        Debug.Log($"SushiGenerator: ���̵� ���� {difficultyLevel}�� ���! ���� ��ǥ ����: {currentScoreToIncreaseDifficulty}");
    }

    // ���� ���̵� ��ǥ ������ �ܺο��� ������ �� �ִ� �Ӽ�
    public int CurrentScoreToIncreaseDifficulty
    {
        get { return currentScoreToIncreaseDifficulty; }
    }

    // ���� ���� �� ��� ���̵� ���� �������� �ʱ� ���·� �ǵ����� �޼���
    public void ResetGameDifficulty()
    {
        currentSushiSpeed = initialSushiSpeed;
        currentSpawnInterval = initialSpawnInterval;
        currentScoreToIncreaseDifficulty = baseScoreToIncreaseDifficulty;
        difficultyLevel = 0; // ���̵� ���� �ʱ�ȭ

        Debug.Log($"SushiGenerator: ���� ���̵� �ʱ�ȭ! �ӵ�: {currentSushiSpeed}, �ֱ�: {currentSpawnInterval}, ��ǥ ����: {currentScoreToIncreaseDifficulty}, ����: {difficultyLevel}");
    }
    // ---
}