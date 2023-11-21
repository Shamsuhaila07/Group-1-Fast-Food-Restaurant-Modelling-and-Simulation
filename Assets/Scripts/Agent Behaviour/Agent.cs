using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


public class Agent : MonoBehaviour
{
    public GameObject foodPrefab; // Reference to the food prefab
    private GameObject instantiatedFood; // Reference to the instantiated food

    // possibleTags of agent
    static string[] possibleTags = { "Cash", "Cashless" };

    NavMeshAgent navagent;

    //Scriptable object for storing queue lists for each type of queue
    public QueueList queueList;
    public KioskQueueList queueKioskList;
    public CollectOrderQueueList queueCollectOrderList;

    System.Random rnd = new System.Random();
    DateTime motionT;
    AgentState currState;

    int destinationStall;
    private int kioskIndex;
    private int CollectOrderCounterIndex;
    public float thinkingDuration = 5.0f;

    GameObject[] stalls;
    GameObject[] kiosks;
    GameObject[] collectordercounters;
    GameObject[] dineintables;
    GameObject Door;
    GameObject ExitDoor;
    
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    // Start is called before the first frame update
    void Start()
    {
        SimulationManager.Instance.RegisterAgent(this);
        agentCollider = GetComponent<CapsuleCollider>();
        navagent = GetComponent<NavMeshAgent>();

        // assign a random identity to an agent
        int tagIndex = UnityEngine.Random.Range(0, possibleTags.Length);
        this.tag = possibleTags[tagIndex];
        Debug.Log("Created a " + this.tag + " agent");

        // finding the gameobject in unity based on tag
        stalls = GameObject.FindGameObjectsWithTag("Stall");
        kiosks = GameObject.FindGameObjectsWithTag("Kiosk");
        Debug.Log("Number of kiosks: " + kiosks.Length);
        Door = GameObject.FindGameObjectWithTag("Door");
        ExitDoor = GameObject.FindGameObjectWithTag("ExitDoor");
        dineintables = GameObject.FindGameObjectsWithTag("DineInTable");
        collectordercounters = GameObject.FindGameObjectsWithTag("CollectOrderCounter");
        int stall = GetRandomStall();
        GotoStall(stall);
        currState = AgentState.Wandering;
    }

    // Update is called once per frame
    void Update()
    {
        if (navagent == null)
            return;

        // Check the current state and perform actions accordingly
        switch (currState)
        {
            case AgentState.InQueue:
                // Do nothing when the agent is already in the queue
                break;

            case AgentState.ToQueue:
                // Check if the agent has reached the destination
                if (ReachedDestination())
                {
                    currState = AgentState.InQueue;
                }
                break;

            case AgentState.Ordering:
                HandleOrderingState();
                break;

            case AgentState.EnteringKioskQueue:
                // Logic for entering the kiosk queue
                MoveToKioskQueue(kioskIndex);
                break;

            case AgentState.GettingOrder:
                HandleCollectOrderState();
                break;

            case AgentState.EnteringCollectOrderQueue:
                // Logic for entering the collect order queue
                MoveToCollectOrderQueue(CollectOrderCounterIndex);
                break;

            case AgentState.Wandering:
                // Wandering logic
                if (ReachedDestination(10))
                {
                    bool joinQ = UnityEngine.Random.value < 0.5;
                    if (joinQ)
                    {
                        joinQ = MoveToQueue(destinationStall);
                    }

                    if (!joinQ)
                    {
                        destinationStall = GetRandomStall();
                        GotoStall(destinationStall);
                    }
                }
                break;

            default:

                break;
        }
    }

    void HandleOrderingState()
    {
        if (kioskIndex < 0 || kioskIndex >= queueKioskList.Count())
        {
            Debug.LogError("Invalid kioskIndex: " + kioskIndex + ". Ensure it's within the valid range.");
            return;
        }

        Debug.Log("QueueKioskList count: " + queueKioskList.Count());
        Debug.Log("Attempting to move to KioskQueue with index: " + kioskIndex);
        GotoKiosk();
        currState = AgentState.EnteringKioskQueue;
    }

    void HandleCollectOrderState()
    {
        CollectOrderCounterIndex = GetRandomCollectOrderCounter();
        if (CollectOrderCounterIndex < 0 || CollectOrderCounterIndex >= queueCollectOrderList.Count())
        {
            Debug.LogError("Invalid CollectOrderCounterIndex: " + CollectOrderCounterIndex + ". Ensure it's within the valid range.");
            return;
        }

        Debug.Log("QueueKioskList count: " + queueCollectOrderList.Count());
        Debug.Log("Attempting to move to KioskQueue with index: " + CollectOrderCounterIndex);
        GotoCollectOrderCounter();
        currState = AgentState.EnteringCollectOrderQueue;
    }

    int GetRandomCollectOrderCounter()
    {
        int index = UnityEngine.Random.Range(0, queueCollectOrderList.Count());
        return index;
    }

    void GotoCollectOrderCounter()
    {
        if (CollectOrderCounterIndex >= 0 && CollectOrderCounterIndex < collectordercounters.Length)
        {
            // Get the position of the chosen kiosk
            Vector3 CollectOrderCounterPosition = collectordercounters[CollectOrderCounterIndex].transform.position;
            Debug.Log("Agent is moving to Kiosk with index" + CollectOrderCounterIndex + "at position: " + CollectOrderCounterPosition);

            // Set the agent's destination to the kiosk position
            SetDestination(CollectOrderCounterPosition);
        }
        else
        {
            Debug.LogError("Invalid CollectOrderCounter index: " + CollectOrderCounterIndex);
        }
    }

    int GetRandomKiosk()
    {
        int index = UnityEngine.Random.Range(0, queueKioskList.Count());
        return index;
    }

    void GotoKiosk()
    {

        // Check if the kiosk index is valid
        if (kioskIndex >= 0 && kioskIndex < kiosks.Length)
        {
            // Get the position of the chosen kiosk
            Vector3 kioskPosition = kiosks[kioskIndex].transform.position;
            Debug.Log("Agent is moving to Kiosk with index" + kioskIndex + "at position: " + kioskPosition);

            // Set the agent's destination to the kiosk position
            SetDestination(kioskPosition);
        }
        else
        {
            Debug.LogError("Invalid kiosk index: " + kioskIndex);
        }
    }

    int GetStall()
    {
        return destinationStall;
    }

    int GetRandomStall()
    {

        int index = UnityEngine.Random.Range(0, stalls.Length);
        return index;
    }

    void GotoStall(int index)
    {
        SetDestination(RandomStallPos(index));
    }

    public bool MovingToQueue()
    {
        return currState == AgentState.ToQueue;
    }

    public bool IsInQueue()
    {
        return currState == AgentState.InQueue;
    }

    public bool MoveToQueue(int qindex)
    {
        AgentQueue chosenQ = queueList.Get(qindex);
        bool success = chosenQ.Add(this);
        if (!success)
            return false;
        currState = AgentState.ToQueue;
        return true;
    }

    public string MoveToQueue()
    {
        AgentQueue chosenQ = queueList.Get(rnd.Next(queueList.Count()));
        bool success = chosenQ.Add(this);
        if (!success)
            return "None";
        currState = AgentState.ToQueue;
        return chosenQ.name;
    }

    public bool MoveToKioskQueue(int queueindex)
    {
        Debug.Log("QueueKioskList count: " + queueKioskList.Count());

        if (queueindex < 0 || queueindex >= queueKioskList.Count())
        {
            Debug.LogError("Invalid queueindex: " + queueindex + ". Ensure it's within the valid range.");
            return false;
        }

        AgentKioskQueue chosenKQ = queueKioskList.Get(queueindex);
        bool success = chosenKQ.Add(this);

        if (!success)
            return false;

        currState = AgentState.ToQueue;
        return true;
    }

    public bool MoveToCollectOrderQueue(int queueindex2)
    {
        Debug.Log("QueueCollectOrderList count: " + queueCollectOrderList.Count());

        if (queueindex2 < 0 || queueindex2 >= queueCollectOrderList.Count())
        {
            Debug.LogError("Invalid queueindex2: " + queueindex2 + ". Ensure it's within the valid range.");
            return false;
        }

        AgentCollectOrderQueue chosenCOCQ = queueCollectOrderList.Get(queueindex2);
        bool success = chosenCOCQ.Add(this);

        if (!success)
            return false;

        currState = AgentState.ToQueue;
        return true;
    }

    public void MoveFromQueue()
    {
        TurnOnNavMeshAgent();
    }

    public void SetDirection(Vector3 dir)
    {
        navagent.Move(dir);
    }

    public void SetDestination(Vector3 pos)
    {
        navagent.SetDestination(pos);
        motionT = DateTime.Now;
    }

    public float GetTimeInMotion()
    {
        return (DateTime.Now - motionT).Seconds;
    }

    public bool ReachedDestination()
    {
        return (navagent.remainingDistance <= navagent.stoppingDistance);
    }

    public bool ReachedDestination(float maxsec)
    {
        if (navagent.remainingDistance <= navagent.stoppingDistance)
        {
            return true;
        }

        if (GetTimeInMotion() > maxsec)
        {
            return true;
        }

        return false;
    }


    public Vector3 RandomStallPos(int index)
    {
        GameObject stall = stalls[index];
        Vector3 stalldim = stall.transform.localScale;
        Vector3 stallpos = stall.transform.position;
        float y = transform.position.y;

        float sign = UnityEngine.Random.value < 0.5 ? -1 : 1;

        var x = stallpos.x + sign * UnityEngine.Random.Range(stalldim.x, 3 * stalldim.x);
        var z = stallpos.z + sign * UnityEngine.Random.Range(stalldim.z, 3 * stalldim.z);
        Vector3 randPos = new Vector3(x, y, z);


        return randPos;
    }

    public Vector3 RandomNearPosition()
    {

        GameObject ground = GameObject.Find("Ground");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = transform.position.y;

        var x = UnityEngine.Random.Range(groundpos.x - grounddim.x / 2, groundpos.x + grounddim.x / 2);
        var z = UnityEngine.Random.Range(groundpos.z - grounddim.z / 2, groundpos.z + grounddim.z / 2);
        Vector3 randPos = new Vector3(x, y, z);
        Vector3 rdir = randPos - transform.position;

        return rdir;
    }


    public void SetRandomDestination()
    {
        SetDestination(transform.position + RandomNearPosition());
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public float GetRadius()
    {
        return ((CapsuleCollider)agentCollider).radius;
    }

    public void TurnOffNavMeshAgent(Vector3 pos)
    {
        navagent.updatePosition = false;
        transform.position = pos;
    }

    public void TurnOnNavMeshAgent()
    {
        navagent.updatePosition = true;
    }

    public void GettingOrder()
    {
        Vector3 doorPos = RandomDoorPos();
        SetDestination(doorPos);
        Debug.Log("Agent is moving to the door Destination" + doorPos);
        currState = AgentState.GettingOrder;

    }

    public void MoveToDoor()
    {

        Vector3 doorPos = RandomDoorPos();
        SetDestination(doorPos);
        Debug.Log("Agent is moving to the door Destination" + doorPos);

        StartCoroutine(DelayedStartThinking());
    }

    private IEnumerator DelayedStartThinking()
    {
        yield return new WaitForSeconds(0.1f); // Adjust the delay as needed
        StartCoroutine(ThinkingTime(UnityEngine.Random.Range(3.0f, 7.0f)));
    }

    public Vector3 RandomDoorPos()
    {
        GameObject doorGameObject = Door;

        if (doorGameObject == null)
        {
            Debug.LogError("Door GameObject with tag 'Door' not found!");
        }

        // Always return the position, even if the door is null
        Vector3 doorPosition = doorGameObject.transform.position;

        // Optionally, add some random offset if needed
        float offsetX = UnityEngine.Random.Range(-1f, 1f);
        float offsetZ = UnityEngine.Random.Range(-1f, 1f);

        Vector3 doorRandomPosition = new Vector3(doorPosition.x + offsetX, doorPosition.y, doorPosition.z + offsetZ);

        return doorRandomPosition;
    }

    public IEnumerator ThinkingTime(float duration)
    {

        Debug.Log("Agent is thinking for " + duration + " seconds...");

        yield return new WaitForSeconds(duration);
        Debug.Log("Agent is done thinking!");

        // choose if tag = cash call different function to go to order counter else if tag == cashless then agent will move to kiosk

        if (tag == "Cash")
        {
            MoveToOrderCounter();
        }

        else if (tag == "Cashless")
        {
            currState = AgentState.Ordering;
        }
    }


    public void MoveToExitDoor()
    {

        Vector3 ExitPos = RandomExitPos();
        SetDestination(ExitPos);
        Debug.Log("Agent is moving to the Exit Door" + ExitPos);

        // Wait for seconds before destroying the instantiated food prefab
        StartCoroutine(DestroyFoodAfterDelay(5.00f));
    }

    public Vector3 RandomExitPos()
    {
        GameObject ExitDoorGameObject = ExitDoor;

        if (ExitDoorGameObject == null)
        {
            Debug.LogError("ExitDoor GameObject with tag 'Door' not found!");
        }

        // Always return the position, even if the door is null
        Vector3 ExitdoorPosition = ExitDoorGameObject.transform.position;

        // Optionally, add some random offset if needed
        float offsetX = UnityEngine.Random.Range(-1f, 1f);
        float offsetZ = UnityEngine.Random.Range(-1f, 1f);

        Vector3 ExitdoorRandomPos = new Vector3(ExitdoorPosition.x + offsetX, ExitdoorPosition.y, ExitdoorPosition.z + offsetZ);

        return ExitdoorRandomPos;

    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Code to execute after the delay
        Debug.Log("Destroying agent after delay...");

        // Destroy the GameObject
        Destroy(gameObject);
    }

    public void ExitingRestaurant()
    {
        //currState = AgentState.Wandering;
        //Destroy(AgentToDelete);
    }

    private IEnumerator MoveToRandomDestinationAndWait(Vector3 destinationPos, float waitDuration)
    {
        // Move to the destination
        SetDestination(destinationPos);
        Debug.Log("Agent is moving to: " + destinationPos);

        // Wait for a few seconds at the destination
        yield return new WaitForSeconds(waitDuration);

        // Move to the next destination after waiting
        MoveToNextRandomDestination(new string[] { "DineInArea1", "DineInArea2", "DineInArea3", "DineInArea4", "ExitDoor" });
    }

    // Add a boolean flag to track whether the agent has already selected a random destination
    private bool hasSelectedRandomDestination = false;

    private void MoveToNextRandomDestination(string[] tags)
    {

        // If the agent has not selected a random destination yet
        if (!hasSelectedRandomDestination)
        {
            // Choose a random destination based on the provided tags
            string nextDestinationTag = tags[UnityEngine.Random.Range(0, tags.Length)];

            // Get all GameObjects with the selected tag
            GameObject[] destinations = GameObject.FindGameObjectsWithTag(nextDestinationTag);

            if (destinations.Length > 0)
            {
                // Choose a random destination from the available options
                Vector3 destinationPos = destinations[UnityEngine.Random.Range(0, destinations.Length)].transform.position;

                // If the next destination is ExitDoor and it's not one of the dining areas, move directly without waiting
                if (nextDestinationTag == "ExitDoor" && !Array.Exists(tags, tag => tag.Contains("DineInArea")))
                {
                    SetDestination(destinationPos);
                    Debug.Log("Agent is moving to: " + destinationPos);
                    MoveToExitDoor();
                    hasSelectedRandomDestination = true; // Mark that the agent has selected a random destination
                }
                else
                {
                    // Otherwise, generate a new random waiting duration between 3 and 7 seconds
                    float waitDuration = UnityEngine.Random.Range(3.0f, 7.0f);

                    // Start a coroutine to move to the next destination and wait
                    StartCoroutine(MoveToRandomDestinationAndWait(destinationPos, waitDuration));
                    hasSelectedRandomDestination = true; // Mark that the agent has selected a random destination
                }
            }
            else
            {
                Debug.LogError("No GameObject with the selected tag found!");
            }
        }
        else
        {
            // Unparent the food from the agent before destroying
            instantiatedFood.transform.parent = null;
            // Destroy the instantiated food prefab
            Destroy(instantiatedFood);
            // The agent has already selected a random destination, so move directly to the ExitDoor
            MoveToExitDoor();
        }
    }

    private IEnumerator MoveToOrderCounterAndWait(Vector3 orderCounterPos, float waitDuration)
    {
        // Move to the OrderCounter destination
        SetDestination(orderCounterPos);
        Debug.Log("Agent is moving to OrderCounter: " + orderCounterPos);

        // Wait for a few seconds at the OrderCounter
        yield return new WaitForSeconds(waitDuration);

        // Move to the CashCollectionPoint after waiting
        MoveToCashCollectionPoint();
    }

    private IEnumerator MoveToCashCollectionPointAndWait(Vector3 cashColPointPos, float waitDuration)
    {
        // Move to the CashCollectionPoint destination
        SetDestination(cashColPointPos);
        Debug.Log("Agent is moving to CashCollectionPoint: " + cashColPointPos);

        // Wait for a few seconds at the CashCollectionPoint
        yield return new WaitForSeconds(waitDuration);

        // Modify the position of the food prefab as needed
        Vector3 foodInitialPosition = cashColPointPos + new Vector3(0f, 0f, 0.1f); // Replace with your desired position

        // Instantiate the food prefab at the modified position
        instantiatedFood = Instantiate(foodPrefab, foodInitialPosition, Quaternion.identity);

        // Parent the food to the agent so it follows the agent
        instantiatedFood.transform.parent = transform;

        // Move to a random destination (DineInArea1, DineInArea2, DineInArea3, DineInArea4, ExitDoor)
        MoveToNextRandomDestination(new string[] { "DineInArea1", "DineInArea2", "DineInArea3", "DineInArea4", "ExitDoor" });
    }

    public void MoveToOrderCounter()
    {
        // Check if the agent has the tag "Group-2"
        if (this.tag == "Cash")
        {
            GameObject[] orderCounters = GameObject.FindGameObjectsWithTag("OrderCounter");

            if (orderCounters.Length > 0)
            {
                int orderCounterIndex = UnityEngine.Random.Range(0, orderCounters.Length);
                Vector3 orderCounterPos = orderCounters[orderCounterIndex].transform.position;

                // Generate a random waiting duration between 3 and 7 seconds
                float waitDuration = UnityEngine.Random.Range(3.0f, 7.0f);

                // Start a coroutine to move to OrderCounter and wait
                StartCoroutine(MoveToOrderCounterAndWait(orderCounterPos, waitDuration));
            }
            else
            {
                Debug.LogError("No GameObject with tag 'OrderCounter' found!");
            }
        }
        else
        {
            Debug.Log("Agent with tag " + this.tag + " is not allowed to move to the order counter.");
        }
    }

    public void MoveToCashCollectionPoint()
    {
        GameObject[] cashCollectionPoints = GameObject.FindGameObjectsWithTag("CashColPoint");

        if (cashCollectionPoints.Length > 0)
        {
            int cashColPointIndex = UnityEngine.Random.Range(0, cashCollectionPoints.Length);
            Vector3 cashColPointPos = cashCollectionPoints[cashColPointIndex].transform.position;

            // Generate a random waiting duration between 3 and 7 seconds
            float waitDuration = UnityEngine.Random.Range(3.0f, 7.0f);

            // Start a coroutine to move to CashCollectionPoint and wait
            StartCoroutine(MoveToCashCollectionPointAndWait(cashColPointPos, waitDuration));
        }
        else
        {
            Debug.LogError("No GameObject with tag 'CashColPoint' found!");
        }
    }

    IEnumerator DestroyFoodAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Code to execute after the delay
        Debug.Log("Destroying food after delay...");

        Destroy(gameObject);
    }

    public void SpawnFood()
    {
        // Calculate the position slightly in front and above the agent
        Vector3 offset = transform.forward * 0.5f + Vector3.up * 1.5f;
        Vector3 foodInitialPosition = transform.position + offset;

        // Instantiate the food prefab at the modified position
        GameObject instantiatedFood = Instantiate(foodPrefab, foodInitialPosition, Quaternion.identity);

        // Parent the food to the agent so it follows the agent
        instantiatedFood.transform.parent = transform;

        Debug.Log("Food Spawned: " + instantiatedFood.name);
    }

    public void AgentDecideTakeawayorDineIn()
    {
        bool isDineIn = IsDineInAgent();

        if (isDineIn)
        {
            Debug.Log("Agent choose to Dine In in the restaurant");
            MoveToDineInTable();
        }
        else
        {
            Debug.Log("Agent choose to Takeaway");
            MoveToExitDoor();
        }
    }

    private bool IsDineInAgent()
    {
        // Adjust the threshold as needed for your probability distribution
        return UnityEngine.Random.Range(0f, 1f) < 0.5f;
    }

    public void MoveToDineInTable()
    {
        // Find all game objects with the "DineInTable" tag

        if (dineintables.Length > 0)
        {
            // Randomly select a dining table
            int randomIndex = UnityEngine.Random.Range(0, dineintables.Length);
            Vector3 tablePosition = dineintables[randomIndex].transform.position;

            // Set the agent's destination to the selected dining table
            SetDestination(tablePosition);

            Debug.Log("Agent is moving to dine-in table at position: " + tablePosition);

            // Start the coroutine to wait at the table
            StartCoroutine(EatAtTable(UnityEngine.Random.Range(10f, 15f)));
        }
        else
        {
            Debug.LogError("No GameObject with tag 'DineInTable' found!");
        }
    }

    private IEnumerator EatAtTable(float waitDuration)
    {
        Debug.Log("Agent is eating at the table...");

        // Wait for the specified duration
        yield return new WaitForSeconds(waitDuration);

        Debug.Log("Agent is done eating at the table!");

        DestroyFood();

        // After waiting, move to the exit door or perform other actions
        MoveToExitDoor();
    }

    private void DestroyFood()
    {

        Transform childFood = transform.Find("Tray_01_Setup_03(Clone)");
        if (childFood != null)
        {
            Destroy(childFood.gameObject);
            Debug.Log("Existing food is destroyed");
        }
    }

    private void OnDestroy()
    {
        SimulationManager.Instance.UnregisterAgent(this);
    }

}
