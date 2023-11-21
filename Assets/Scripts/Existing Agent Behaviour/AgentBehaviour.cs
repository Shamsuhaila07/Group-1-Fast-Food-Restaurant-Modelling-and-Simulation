using UnityEngine;
using UnityEngine.AI;

public class AgentBehavior : MonoBehaviour
{
    public Transform foodCounter;
    private NavMeshAgent agent;
    private ServerScript server;
    private bool atCounter = false;
    private float elapsedTime = 0f;
    public float interactionDelay = 2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        server = GetComponent<ServerScript>(); // Initialize the server reference
        MoveToFoodCounter();
    }

    void Update()
    {
        if (atCounter)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= interactionDelay)
            {
                FindRandomTable();
            }
        }
    }

        void MoveToFoodCounter()
    {
        agent.SetDestination(foodCounter.position);
        atCounter = true; // Update the flag when moving to the food counter
    }

    void FindRandomTable()
    {
        // Reset interaction variables
        elapsedTime = 0f;
        atCounter = false;

        // get random table
        GameObject randomTable = GetRandomAvailableTable();

        // Move towards the chosen table
        if (randomTable != null)
        {
            if (server.IsTableFree(randomTable))
            {
                agent.SetDestination(randomTable.transform.position);
            }
            else
            {
                Debug.LogWarning("Selected table is not available. Finding another table...");
                FindRandomTable(); // Try finding another table
            }
        }
        else
        {
            Debug.LogWarning("No available dining tables!");
        }
    }

    GameObject GetRandomAvailableTable()
    {

        GameObject[] tables = GameObject.FindGameObjectsWithTag("DiningTable");

        foreach (GameObject table in tables)
        {
            // Check if the table is available
            TableScript tableScript = table.GetComponent<TableScript>();

            if (tableScript != null && tableScript.IsAvailable())
            {
                // Mark the table as occupied
                tableScript.SetOccupied(true);

                return table;
            }
        }
        return null;
    }
}
