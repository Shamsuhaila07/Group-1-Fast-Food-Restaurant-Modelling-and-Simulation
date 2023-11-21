using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Server: worker, service provider, etc.
class Server : MonoBehaviour
{

    // serving time, assume normal/ gaussian, how long it takes to serve someone
    public float mean;
    public float std;

    // every server has a queue or customer
    // assume one server address one queue
    public AgentQueue customers;
    private List<float> agentWaitTimes = new List<float>();

    public List<float> GetAgentWaitTimes()
    {
        Debug.Log("Server " + name + " has " + agentWaitTimes.Count + " wait times.");
        return agentWaitTimes;
    }

    // who's being served
    Agent currCustomer = null!;
    bool serverFree = true;
    int serviceTime = 0;
    int cumulativeQSize = 0;
    int maxQSize = 0;
    Normal serviceGaussian;

    DateTime prev;
    const int updateRate = 1;
    long time;
    float lastUpdate;

    void Start()
    {
        serviceGaussian = new Normal(mean, std);
        prev = DateTime.Now;
    }

    void Update()
    { //simulation loop
        time++; // just to indicate the time on the visual clock

        lastUpdate += Time.deltaTime;
        if (lastUpdate > updateRate)
        { // perform server update
            update(customers, time);
            lastUpdate = 0;
        }
    }

    public Agent getCurrCustomer()
    {
        return currCustomer;
    }

    public int getCumulativeQSize()
    {
        return cumulativeQSize;
    }

    public int getMaxQSize()
    {
        return maxQSize;
    }

    public bool update(AgentQueue agentQueue, long Time)
    {

        // statistics update
        int currQSize = agentQueue.Size();
        cumulativeQSize += currQSize;
        if (currQSize > maxQSize)
            maxQSize = currQSize;

        // check whether server is busy
        if (serverFree == false)
        {
            serviceTime--;
            Debug.Log(serviceTime + " second remaining for server " + name);


            if (serviceTime <= 0)
            { //Server is done with current customer
                Debug.Log(name + " server done at time=" + Time);

                currCustomer = agentQueue.Pop(); // TODO: Clarify the logic here

                //implement TOGO TO OTHER QUEUE HERE

                /*FinishedQueueCust = agentQueue.PoppedAgentAction();*/
                DateTime entryTime = agentQueue.GetEntryTime(currCustomer);
                if (entryTime != DateTime.MinValue)
                {
                    float waitTime = (float)(DateTime.Now - entryTime).TotalSeconds;
                    agentWaitTimes.Add(waitTime);
                }

                serverFree = true;
                return true;    // done
            }
        }

        //if server is not busy then we sample the next service time
        if (serverFree == true && agentQueue.Size() > 0)
        {
            serviceTime = (int)serviceGaussian.Sample();
            Debug.Log(name + " server starts. To finish in " + serviceTime);
            serverFree = false;
        }

        return false;
    }


}