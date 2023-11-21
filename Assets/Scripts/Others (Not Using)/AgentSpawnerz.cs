using UnityEngine;

public class AgentSpawnerz : MonoBehaviour
{
    public GameObject agentPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;

    void Start()
    {
        //InvokeRepeating("SpawnAgent", 0f, spawnInterval);
    }

    void SpawnAgent()
    {
        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(agentPrefab, spawnPoints[randomSpawnPointIndex].position, Quaternion.identity);
    }
}