using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SushiGenerator : MonoBehaviour
{
    public GameObject[] sushiPrefabs;
    public GameObject plate;
    private Vector3 spawnPosition = new Vector3(12f, 0.5f, 0f);
    private Vector3 spawnPosition2 = new Vector3(12f, 0f, 0f);

    public static List<GameObject> staticSushis = new List<GameObject>(); // ���� �ʹ� ����Ʈ

    public float initialSushiSpeed = 2f; // �ʹ��� �ʱ� �̵� �ӵ�
    private float currentSushiSpeed;     // ���� �ʹ��� �̵� �ӵ�

    void Awake()
    {
        // �� �ε� �ø��� staticSushis ����Ʈ�� �ʱ�ȭ
        if (staticSushis == null)
        {
            staticSushis = new List<GameObject>();
        }
        else
        {
            staticSushis.Clear(); // ���� �ε�� ������ ���� ����Ʈ�� ���ϴ�.
        }

        // �ʱ� �ӵ� ����
        currentSushiSpeed = initialSushiSpeed;
    }
    void Start()
    {
        CreateStaticSushi();

        InvokeRepeating("SpawnSushi", 1f, 2f); // 2�ʸ��� ����
    }

    void SpawnSushi()
    {
        int index = Random.Range(0, sushiPrefabs.Length);
        GameObject newSushi = Instantiate(sushiPrefabs[index], spawnPosition, Quaternion.identity);
        Instantiate(plate, spawnPosition2, Quaternion.identity);

        SushiMover mover = newSushi.GetComponent<SushiMover>();
        if (mover != null)
        {
            mover.moveSpeed = currentSushiSpeed; // ���� ������ �ӵ��� �ʹ信 ����
        }
        else
        {
            Debug.LogWarning($"������ �ʹ� '{newSushi.name}'�� SushiMover ������Ʈ�� �����ϴ�.", newSushi);
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

        // �ߺ� ���� 3�� ����
        List<int> indices = Enumerable.Range(0, sushiPrefabs.Length).ToList();
        indices = indices.OrderBy(x => Random.value).ToList(); // ���� ����

        for (int i = 0; i < 3 && i < indices.Count; i++)
        {
            int prefabIndex = indices[i];
            Vector3 pos = fixedPositions[i];

            GameObject sushi = Instantiate(sushiPrefabs[prefabIndex], pos, Quaternion.identity);

            // �������� �ʵ��� ����
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
        Debug.Log($"�ʹ� �̵� �ӵ� ����! ���ο� �ӵ�: {currentSushiSpeed}");

        // ���� ����: �̹� ���� �ִ� �����̴� �ʹ���� �ӵ��� ������Ʈ
        // (���� InvokeRepeating���� �ʹ��� ��� �����ǹǷ�, �� �κ��� ���� �ʹ���� ����˴ϴ�.)
        // ���� �̹� ȭ�鿡 �ִ� �ʹ���� �ӵ��� �ٷ� �ٲ�� ���Ѵٸ�,
        // ��� 'Sushi' �±׸� ���� GameObject�� ã�Ƽ� SushiMover.moveSpeed�� ������Ʈ�ؾ� �մϴ�.
        // ������ ���� ����� �� ��� �ʹ��� ������Ƿ�, ���� �ʿ����� ���� �� �ֽ��ϴ�.
    }

    public void ResetSushiSpeed()
    {
        currentSushiSpeed = initialSushiSpeed;
        Debug.Log($"�ʹ� �̵� �ӵ� �ʱ�ȭ! ���ο� �ӵ�: {currentSushiSpeed}");
    }

}
