using UnityEngine;

public class ServerScript : MonoBehaviour
{
    // Example method to check if a table is free
    public bool IsTableFree(GameObject table)
    {
        return table.GetComponent<TableScript>().IsAvailable();
    }
}
