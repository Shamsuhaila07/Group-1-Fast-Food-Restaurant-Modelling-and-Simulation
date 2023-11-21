
// this doesnt work in Unity
//using MathNet.Numerics.Distributions;  

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Server: worker, service provider, etc.
class CollectOrderServer : MonoBehaviour
{

    // serving time, assume normal/ gaussian, how long it takes to serve someone
    public float mean;
    public float std;

    // every server has a queue or customer
    // assume one server address one queue
    public AgentCollectOrderQueue customers;

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

    public bool update(AgentCollectOrderQueue agentCollectOrderQueue, long Time)
    {

        // statistics update
        int currQSize = agentCollectOrderQueue.Size();
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

                currCustomer = agentCollectOrderQueue.Pop();
                // customer will perform the agent behaviour contained in Pop method inside Agent Collect Order Queue Script
                // The food will be spawned
                // The agent will randomly decide takeaway or dine in
                // action until leaving the restaurant

                serverFree = true;
                return true;    // done
            }
        }

        //if server is not busy then we sample the next service time
        if (serverFree == true && agentCollectOrderQueue.Size() > 0)
        {
            serviceTime = (int)serviceGaussian.Sample();
            Debug.Log(name + " server starts. To finish in " + serviceTime);
            serverFree = false;
        }

        return false;
    }


}

