using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public GameObject foodPrefab; // Reference to the food prefab
    public Transform[] spawnPoints;
    public GameObject restaurantBounds;
    public Transform[] tablePositions;
    public GameObject exitArea; // Reference to the ExitArea GameObject
    public int numberOfCustomers = 5;
    public float spawnInterval = 2f;
    public float initialWaitTime = 3f;
    public float timeAtTable = 10f; // Time customer spends at the table

    private bool[] tableOccupied;

    void Start()
    {
        tableOccupied = new bool[tablePositions.Length];
        StartCoroutine(SpawnCustomers());
    }

    IEnumerator SpawnCustomers()
    {
        if (restaurantBounds == null || tablePositions.Length == 0 || exitArea == null)
        {
            Debug.LogError("Invalid setup!");
            yield break;
        }

        for (int i = 0; i < numberOfCustomers; i++)
        {

            //Vector3 randomSpawnPoint = GetRandomPointInBounds(restaurantBounds.GetComponent<Collider>().bounds);
            // Randomly choose a spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate a customer at the chosen spawn point
            GameObject customer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            //GameObject customer = Instantiate(customerPrefab, randomSpawnPoint, Quaternion.identity);

            // Wait for the initial period
            yield return new WaitForSeconds(initialWaitTime);

            // Move the customer to an available table
            MoveCustomerToTable(customer);

            // Optionally, you can set additional properties of the customer here
            // customer.GetComponent<CustomerScript>().Initialize();

            yield return new WaitForSeconds(spawnInterval - initialWaitTime);
        }
    }

    void MoveCustomerToTable(GameObject customer)
    {
        StartCoroutine(MoveCustomerToTableCoroutine(customer));
    }

    IEnumerator MoveCustomerToTableCoroutine(GameObject customer)
    {
        NavMeshAgent customerAgent = customer.GetComponent<NavMeshAgent>();

        if (customerAgent != null)
        {
            customerAgent.enabled = true;

            // Find an available table
            Transform randomTable = GetAvailableTable();

            if (randomTable != null)
            {
                tableOccupied[System.Array.IndexOf(tablePositions, randomTable)] = true;
                StartCoroutine(MoveAgent(customerAgent, randomTable.position));

                // Adjust Y-position after moving to the table
                float yOffset = customerAgent.height / 2f;
                customer.transform.position = new Vector3(customer.transform.position.x, yOffset, customer.transform.position.z);

                // Move the customer to the exit area after spending time at the table
                StartCoroutine(MoveCustomerToExitArea(customer));

                // Introduce a 3-second delay before instantiating the food
                yield return new WaitForSeconds(2.5f);

                // Instantiate the foodPrefab as a child of the customer with an offset
                Vector3 foodOffset = new Vector3(1f, 1.5f, 0.5f); // Adjust the offset as needed
                GameObject food = Instantiate(foodPrefab, customer.transform.position + foodOffset, Quaternion.identity);
                food.transform.parent = customer.transform;

                // Start a coroutine to destroy the food after the specified time at the table
                StartCoroutine(DestroyFoodAfterTime(food, timeAtTable));
            }
            else
            {
                Debug.LogError("No available tables!");
            }
        }
        else
        {
            Debug.LogError("Customer is missing NavMeshAgent component!");
        }
    }

    IEnumerator DestroyFoodAfterTime(GameObject food, float time)
    {
        float destroyTime = time * 0.7f;  // Adjust the multiplier as needed (e.g., 0.8f for 80% of timeAtTable)
        yield return new WaitForSeconds(destroyTime);
        Destroy(food);
    }


    IEnumerator MoveCustomerToExitArea(GameObject customer)
    {
        // Wait for the specified time at the table
        yield return new WaitForSeconds(timeAtTable);

        NavMeshAgent customerAgent = customer.GetComponent<NavMeshAgent>();

        if (customerAgent != null)
        {
            // Set the destination of the customer to the exit area
            StartCoroutine(MoveAgent(customerAgent, exitArea.transform.position));

            // Adjust Y-position after moving to the exit area
            float yOffset = customerAgent.height / 2f;
            customer.transform.position = new Vector3(customer.transform.position.x, yOffset, customer.transform.position.z);

            // Destroy the customer after reaching the exit area
            Destroy(customer, 10f); // You can adjust the delay before destroying the customer
        }
        else
        {
            Debug.LogError("Customer is missing NavMeshAgent component!");
        }
    }

    Transform GetAvailableTable()
    {
        for (int i = 0; i < tableOccupied.Length; i++)
        {
            if (!tableOccupied[i])
            {
                return tablePositions[i];
            }
        }
        return null;
    }

    IEnumerator MoveAgent(NavMeshAgent agent, Vector3 destination)
    {
        yield return null;
        agent.SetDestination(destination);
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(randomX, bounds.min.y, randomZ);
    }
}
