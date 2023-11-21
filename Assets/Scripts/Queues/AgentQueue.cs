using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentQueue : MonoBehaviour
{
    public QueueList queueList;

    //the agent queing up
    List<Agent> agentqueue = new List<Agent>();
    private Queue<DateTime> entryTimes = new Queue<DateTime>(); // New queue to store entry times

    //string counterId;
    Vector3 qdir;  // queue direction
    Vector3 qPos;  // next position in the queue
    Agent incomingAgent; // an agent in a Toqueue state
    float queueSpacing = 1; // how much agent in the queue to be space

    // Start is called before the first frame update
    void Start()
    {
        qdir = -transform.right; // the red vector
        qPos = transform.position + qdir * (agentqueue.Count + 1) * queueSpacing;
        queueList.Add(this); // adding the agent to the queue, if there's an incoming agent then we will reject and accept the agent

        Debug.Log("Queue created for " + gameObject.name + ". With pos: " + qPos);

    }

    public bool Add(Agent agent)
    {
        // TODO: remove this condition
        if (incomingAgent != null)
        {
            return false;
        }
        incomingAgent = agent;
        qPos.y = agent.GetPos().y;
        agent.SetDestination(qPos);
        queueSpacing = 2 * 1f;
        // Calculate the qpos for the incoming agent
        qPos += qdir * queueSpacing;
        entryTimes.Enqueue(DateTime.Now);
        return true;
    }

    // we called whenever someone leaves the queue
    public void Shift()
    {
        foreach (Agent agent in agentqueue)
        {
            agent.transform.position -= qdir * queueSpacing;
        }
        qPos -= qdir * queueSpacing;
    }

    // remove agent from the front
    public Agent Pop()
    {
        Agent agent = agentqueue[0];

        agentqueue.RemoveAt(0);

        //method is called from agent behaviour object
        agent.MoveFromQueue();

        // an agent leaving the queue go to a random spot at the door
        agent.MoveToDoor();

        Shift(); // shift the queue to the front

        return agent;
    }

    public DateTime GetEntryTime(Agent agent)
    {
        int index = agentqueue.IndexOf(agent);
        if (index >= 0 && index < entryTimes.Count)
        {
            DateTime entryTime = entryTimes.ElementAt(index);
            Debug.Log("Entry time for Agent " + agent.name + ": " + entryTime);
            return entryTime;
        }
        return DateTime.MinValue;
    }

    public int Size()
    {
        return agentqueue.Count;
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
                agentqueue.Add(incomingAgent);
                Debug.Log("Queue for " + gameObject.name + " now has size " + agentqueue.Count + ". Next pos: " + qPos);
                incomingAgent = null;
            }
        }
    }
}