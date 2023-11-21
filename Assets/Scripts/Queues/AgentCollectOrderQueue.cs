using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCollectOrderQueue : MonoBehaviour
{
    // static scriptable, there's only one copy in the unity system
    public CollectOrderQueueList queueCollectOrderList;

    //the agent queing up
    List<Agent> agentcollectorderqueue = new List<Agent>();

    //string counterId;
    Vector3 qdir;  // queue direction
    Vector3 qPos;  // next position in the queue
    Agent incomingAgent; // an agent in a Toqueue state
    float queueSpacing = 1; // how much agent in the queue to be space

    // Start is called before the first frame update
    void Start()
    {
        qdir = -transform.forward; // the red vector
        qPos = transform.position + qdir * (agentcollectorderqueue.Count + 1) * queueSpacing;
        queueCollectOrderList.Add(this); // adding the agent to the queue, if there's an incoming agent then we will reject and accept the agent

        Debug.Log("Queue created for " + gameObject.name + ". With pos: " + qPos);

    }

    public bool Add(Agent agent)
    {
        // TODO: remove this condition
        if (incomingAgent != null)
        {
            return false;
        }

        Debug.Log("Adding agent to queue.");

        incomingAgent = agent;
        qPos.y = agent.GetPos().y;
        agent.SetDestination(qPos);
        queueSpacing = 2 * 1f;
        // Calculate the qpos for the incoming agent
        qPos += qdir * queueSpacing;
        return true;
    }

    // we called whenever someone leaves the queue
    public void Shift()
    {
        foreach (Agent agent in agentcollectorderqueue)
        {
            agent.transform.position -= qdir * queueSpacing;
        }
        qPos -= qdir * queueSpacing;
    }

    // remove agent from the front
    public Agent Pop()
    {
        //GameObject foodPrefab = GetComponent<Agent>().foodPrefab;
        Agent agent = agentcollectorderqueue[0];
        agentcollectorderqueue.RemoveAt(0);

        //turn on back navmesh
        agent.MoveFromQueue();

        // get their food
        agent.SpawnFood();

        //decide to takeaway and dine in then different action for both 
        // exit with food or dine in
        agent.AgentDecideTakeawayorDineIn();

        Shift(); // shift the queue to the front

        return agent;
    }

    public int Size()
    {
        return agentcollectorderqueue.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (incomingAgent == null)
            return;

        if (incomingAgent.IsInQueue())
        {
            // Add a null check for incomingAgent
            if (incomingAgent != null)
            {
                // Turn off the nav mesh agent and then arrange it manually
                incomingAgent.TurnOffNavMeshAgent(qPos);
                agentcollectorderqueue.Add(incomingAgent);
                Debug.Log("Queue for " + gameObject.name + " now has size " + agentcollectorderqueue.Count + ". Next pos: " + qPos);
                incomingAgent = null;
            }
        }
    }
}