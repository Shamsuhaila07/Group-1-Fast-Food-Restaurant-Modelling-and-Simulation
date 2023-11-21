using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/CollectOrderQueueList")]
public class CollectOrderQueueList : ScriptableObject
{

    [System.NonSerialized]
    List<AgentCollectOrderQueue> queues = new List<AgentCollectOrderQueue>();

    public AgentCollectOrderQueue Get(int i)
    {
        return queues[i];
    }

    public int Count()
    {
        return queues.Count;
    }

    public void Add(AgentCollectOrderQueue queue)
    {
        queues.Add(queue);
    }
}

