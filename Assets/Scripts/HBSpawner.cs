using UnityEngine;
using UnityEngine.Rendering;

public class HBSpawner : MonoBehaviour
{
    //public int      maxObstacles    = 100;
    public float    spawnInterval   = 5f;
    public float    spawnRadiusMax  = 6f;
    public float    graceRadius     = 3f;

    public float    minX            = -11f;
    public float    maxX            = 11f;
    public float    minY            = -5f;
    public float    maxY            = 5f;

    public GameObject obstaclePrefab;
    public GameObject player;

    private float   elapsedTime     = 0f;
    private int     fiveSecondTimer = 0;
    //private int     obstacleCount   = 1;


    // functions
    void StopSpawning()
    {
        this.enabled = false;
    }
    bool SpawnObstacle()
    {
        elapsedTime += Time.deltaTime;
        fiveSecondTimer = Mathf.FloorToInt(elapsedTime);

        //Debug.Log($"Elapsed Time: {elapsedTime}, fiveSecondTimer: {fiveSecondTimer}");

        if (fiveSecondTimer >= spawnInterval)
        {
            elapsedTime = 0f;
            fiveSecondTimer = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Stop spawning obstacles when the player dies
        PlayerController playerScript = player.GetComponent<PlayerController>();
        playerScript.OnDeath += StopSpawning;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnObstacle())
        {
            Vector3 spawnPos;

            do
            {
                Vector3 playerPos = player.transform.position;

                // random angle in radians
                float angle = Random.Range(0f, Mathf.PI * 2f);

                // IMPORTANT: sqrt for uniform distribution (Area = πr^2 so the area grows by r^2 we need to compensate)
                float r = Mathf.Sqrt(Random.Range(graceRadius * graceRadius, spawnRadiusMax * spawnRadiusMax));

                // conversion from polar to cartesian coordinates
                float x = Mathf.Cos(angle) * r;
                float y = Mathf.Sin(angle) * r;

                spawnPos = playerPos + new Vector3(x, y, 0f);
            }
            while   (
                    spawnPos.x < minX || spawnPos.x > maxX ||
                    spawnPos.y < minY || spawnPos.y > maxY
                    );

            Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
            //Debug.Log($"Spawning obstacle at: {spawnPos}");
            //Debug.Log($"BOUNDS: X[{minX},{maxX}] Y[{minY},{maxY}]");
        }
    }
}
