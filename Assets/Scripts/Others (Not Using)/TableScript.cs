using UnityEngine;

public class TableScript : MonoBehaviour
{
    private bool isOccupied = false;

    public bool IsAvailable()
    {
        return !isOccupied;
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}
