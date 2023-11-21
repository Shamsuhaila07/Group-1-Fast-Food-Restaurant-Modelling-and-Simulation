using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/KioskQueueList")]
public class KioskQueueList : ScriptableObject
{

    [System.NonSerialized]
    List<AgentKioskQueue> queues = new List<AgentKioskQueue>();

    public AgentKioskQueue Get(int i)
    {
        return queues[i];
    }

    public int Count()
    {
        return queues.Count;
    }

    public void Add(AgentKioskQueue queue)
    {
        queues.Add(queue);
    }
}

