/*using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public Transform counterTransform;
    public Transform tableTransform;
    public float spawnDelay = 0f;
    public float pickupRadius = 1.5f; // Adjust the radius as needed

    private float timeElapsed = 0f;
    private bool canSpawn = false;
    private GameObject spawnedFood;

    void Start()
    {
        Debug.Log("FoodSpawner script started.");
        // Start the countdown when the script is enabled
        canSpawn = true;
    }

    void Update()
    {
        if (canSpawn)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= spawnDelay)
            {
                // Spawn food on the counter
                Debug.Log("Spawning food.");
                SpawnFood();

                // Reset the timer
                timeElapsed = 0f;
                canSpawn = false;
            }
        }
    }

    void SpawnFood()
    {
        // Instantiate a new food object at the counter's position
        spawnedFood = Instantiate(foodPrefab, counterTransform.position, Quaternion.identity);

        // Attach a script to the spawned food to handle pickup
        FoodDestroy foodDestroy = spawnedFood.AddComponent<FoodDestroy>();
        foodDestroy.SetPickupRadius(pickupRadius);

        // Parent the foodPrefab to the counter
        spawnedFood.transform.parent = counterTransform;
    }

    public void AgentArrivedAtTable()
    {
        // Unparent the foodPrefab when the agent arrives at the table
        spawnedFood.transform.parent = null;

        // Optionally, you can add additional logic here (e.g., start eating animation, etc.)
    }

    public void AgentFinishedEating()
    {
        // Destroy the foodPrefab when the agent finishes eating
        Destroy(spawnedFood);
    }
}*/
