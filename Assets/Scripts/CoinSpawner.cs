using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class CoinSpawner : MonoBehaviour
{
    //public int      maxObstacles    = 100;
    public float spawnInterval = 5f;
    public float spawnIntervalMin = 4f;
    public float spawnIntervalMax = 10f;
    public float spawnRadiusMax = 10f;
    public float graceRadius = 5f;

    public float minX = -12f;
    public float maxX = 12f;
    public float minY = -6f;
    public float maxY = 6f;

    public GameObject coinPrefab;
    public GameObject player;

    private float elapsedTime = 0f;
    //private int     obstacleCount   = 1;


    // functions
    void StopSpawning()
    {
        this.enabled = false;
    }
    bool SpawnCoin()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= spawnInterval)
        {
            elapsedTime = 0f;
            spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);

            //Debug.Log($"Next spawn in: {spawnInterval} seconds");

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
        if (SpawnCoin())
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
            while (
                    spawnPos.x < minX || spawnPos.x > maxX ||
                    spawnPos.y < minY || spawnPos.y > maxY
                    );

            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            //Debug.Log($"Spawning coin at: {spawnPos}");
        }
    }
}
