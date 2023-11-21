using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    public Agent agentPrefab;
    List<Agent> crowd = new List<Agent>();

    public int startingCount = 3;

    void Start()
    {
        // Randomly positioning an agent
        GameObject ground = GameObject.Find("Ground");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = groundpos.y + grounddim.y / 2;

        // Each agent randomly placed on the ground
        while (crowd.Count < startingCount)
        {
            var x = Random.Range(groundpos.x - grounddim.x / 2, groundpos.x + grounddim.x / 2);
            var z = Random.Range(groundpos.z - grounddim.z / 2, groundpos.z + grounddim.z / 2);
            Vector3 spawnPos = new Vector3(x, y, z);

            Agent agent = Instantiate(agentPrefab,
                                    spawnPos,
                                     Quaternion.identity);
            agent.name = "Agent-" + crowd.Count;
            crowd.Add(agent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // This is not used 
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < 3; i++)
            { // try 3 times
                Agent agent = crowd[Random.Range(0, crowd.Count)];
                if (agent.IsInQueue() == false && agent.MovingToQueue() == false)
                {
                    string q = agent.MoveToQueue();
                    Debug.Log("Moving agent " + agent.name + " to queue " + q);
                    break;
                }
            }
        }
    }

}