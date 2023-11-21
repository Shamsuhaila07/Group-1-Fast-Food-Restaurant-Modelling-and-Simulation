using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentMoveTowardsFood : MonoBehaviour
{
    public Transform foodCounter;
    public Transform ExitDoor;
    public float interactionDelay = 2f; // Adjust the delay time as needed
    //public float interactionRadius = 1.5f;

    private NavMeshAgent agent;
    private bool atCounter = false;
    private float elapsedTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToFoodCounter();
    }

    void Update()
    {
        if (atCounter)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= interactionDelay)
            {
                MoveToExitDoor();
            }
        }
    }

    void MoveToFoodCounter()
    {
        if (foodCounter != null)
        {
            agent.SetDestination(foodCounter.position);
        }
        else
        {
            Debug.LogWarning("Food counter not assigned to the agent.");
        }
    }

    void MoveToExitDoor()
    {
        if (ExitDoor != null)
        {
            agent.SetDestination(ExitDoor.position);
        }
        else
        {
            Debug.LogWarning("Exit door not assigned!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodCounter"))
        {
            atCounter = true;
            // You may want to add some additional logic here if needed
        }
    }

    // You can use OnTriggerExit to reset the delay if the agent leaves the food counter
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FoodCounter"))
        {
            atCounter = false;
            elapsedTime = 0f;
            MoveToExitDoor(); // Uncomment this line if you want the agent to start moving immediately when leaving the food counter
        }
    }
}


    /*void Update()
    {
        CheckForFoodInteraction();
    }

    void CheckForFoodInteraction()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius);

        foreach (var collider in colliders)
        {
            //Debug.Log("Collider detected: " + collider.gameObject.name);
            
            if (collider.CompareTag("Food"))
            {
                // Interact with food (e.g., play an animation, update a score, etc.)
                Debug.Log("Agent interacting with food!");
            }
        }
    }*/