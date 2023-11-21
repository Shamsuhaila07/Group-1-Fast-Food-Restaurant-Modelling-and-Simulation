using UnityEngine;
using UnityEngine.AI; // Include the NavMesh namespace.

public class SpawnCharacter : MonoBehaviour
{
    public GameObject CustomerNPCMale1; // The prefab for your customer character.
    public Transform SpawnArea; // The transform of the spawn area GameObject.

    public int numberOfCustomersToSpawn = 20; // Adjust as needed.
    public float wanderRadius = 5.0f; // The maximum distance the characters will wander.

    void Start()
    {
        // Loop to spawn customers randomly.
        for (int i = 0; i < numberOfCustomersToSpawn; i++)
        {
            // Generate a random position within the spawn area.
            Vector3 randomPosition = new Vector3(
                Random.Range(SpawnArea.position.x - SpawnArea.localScale.x / 2, SpawnArea.position.x + SpawnArea.localScale.x / 2),
                SpawnArea.position.y,
                Random.Range(SpawnArea.position.z - SpawnArea.localScale.z / 2, SpawnArea.position.z + SpawnArea.localScale.z / 2)
            );

            // Spawn a customer at the random position.
            GameObject customer = Instantiate(CustomerNPCMale1, randomPosition, Quaternion.identity);

            // Get the NavMeshAgent component on the customer.
            NavMeshAgent agent = customer.GetComponent<NavMeshAgent>();

            // Set the agent's destination to a random point within the wanderRadius.
            Vector3 randomWanderPoint = RandomWanderPoint(randomPosition, wanderRadius);
            agent.SetDestination(randomWanderPoint);
        }
    }

    // Function to generate a random point within a wander radius.
    Vector3 RandomWanderPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 10);
        return hit.position;
    }
}