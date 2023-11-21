using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    // List to store active agents
    private List<Agent> activeAgents = new List<Agent>();
    public GameObject canvasEndOfSimulation;
    public GameObject canvasCamera1;
    public GameObject GamePlayer;
    public GameObject SimulationCamera;
    public GameObject cursorPlayer;

    private float simulationStartTime;

    // Singleton pattern to ensure only one instance of the manager
    private static SimulationManager instance;
    public static SimulationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SimulationManager>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("SimulationManager");
                    instance = managerObject.AddComponent<SimulationManager>();
                }
            }
            return instance;
        }
    }

    // Method to register an agent when it starts
    public void RegisterAgent(Agent agent)
    {
        if (!activeAgents.Contains(agent))
        {
            activeAgents.Add(agent);
        }
    }

    // Method to unregister an agent when it ends
    public void UnregisterAgent(Agent agent)
    {
        if (activeAgents.Contains(agent))
        {
            activeAgents.Remove(agent);
        }

        // Check if there are no more active agents, then end the simulation
        if (activeAgents.Count == 0)
        {
            EndSimulation();
        }
    }

    private void Start()
    {
        // Record the start time when the simulation starts
        simulationStartTime = Time.time;
    }

    // Method to end the simulation
    private void EndSimulation()
        {
            Debug.Log("Simulation Ended");

        // Collect performance measures from the Server
        List<float> agentWaitTimes = new List<float>();
        foreach (var server in FindObjectsOfType<Server>())
        {
            var serverWaitTimes = server.GetAgentWaitTimes();
            agentWaitTimes.AddRange(serverWaitTimes);

            Debug.Log($"Server {server.name} has {serverWaitTimes.Count} wait times.");
        }

        // Calculate and log average wait time
        if (agentWaitTimes.Count > 0)
            {
                float averageWaitTime = agentWaitTimes.Sum() / agentWaitTimes.Count;
                Debug.Log("Average Wait Time: " + averageWaitTime + " seconds");
            }

            // Calculate the total simulation time
            float totalSimulationTime = Time.time - simulationStartTime;
            Debug.Log("Total Simulation Time: " + totalSimulationTime + " seconds");

            if (canvasEndOfSimulation != null)
                canvasEndOfSimulation.SetActive(true);
            else
                Debug.LogWarning("canvasEndOfSimulation is null");

            if (canvasCamera1 != null)
                canvasCamera1.SetActive(true);
            else
                Debug.LogWarning("canvasCamera1 is null");

            if (GamePlayer != null)
                GamePlayer.SetActive(false);
            else
                Debug.LogWarning("GamePlayer is null");

            if (cursorPlayer != null)
                cursorPlayer.SetActive(false);
            else
                Debug.LogWarning("cursorPlayer is null");

            if (SimulationCamera != null)
                SimulationCamera.SetActive(false);
            else
                Debug.LogWarning("SimulationCamera is null");
        }

}