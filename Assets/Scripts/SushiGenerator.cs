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

    void Start()
    {
        CreateStaticSushi();

        InvokeRepeating("SpawnSushi", 1f, 2f); // 2�ʸ��� ����
    }

    void SpawnSushi()
    {
        int index = Random.Range(0, sushiPrefabs.Length);
        Instantiate(sushiPrefabs[index], spawnPosition, Quaternion.identity);
        Instantiate(plate, spawnPosition2, Quaternion.identity);
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
}
