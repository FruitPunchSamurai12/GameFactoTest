using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//THIS WAS INCREDIBLY DIFFICULT TO MAKE IT WORK
public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    PooledMonoBehaviour coverPrefab;

    [SerializeField]
    PooledMonoBehaviour[] obstacles;

    [SerializeField]
    PooledMonoBehaviour speedBoostPrefab;

    [SerializeField]
    float minX =-4;
    [SerializeField]
    float maxX = 4;


    [SerializeField]
    float zPositionToSpawnObstacles;
    [SerializeField]
    float yMinPositionToSpawnObstacles = 10f;
    [SerializeField]
    float yMaxPositionToSpawnObstacles = 20f;
    [SerializeField]
    float minTimeForNextObstacle = .5f;
    [SerializeField]
    float maxTimeForNextObstacle = 2f;
    float obstacleTimer = 0;
    float timeForNextObstacle;

    [SerializeField]
    float minDistanceFromFurthestCoverToSpawnCovers = 8;
    [SerializeField]
    float maxDistanceFromFurthestCoverToSpawnCovers = 32;
    [SerializeField]
    float maxZPositionCoverOrSpeedBoostCanSpawn;
    [SerializeField]
    float minDistanceBetweenCovers = 4;

    [SerializeField]
    int minSpeedBoostsToSpawn = 2;
    [SerializeField]
    int maxSpeedBoostsToSpawn = 4;
    [SerializeField]
    float minDistanceBetweenSpeedBoosts = 8;
    [SerializeField]
    float minZDistanceOfSpeedBoostsFromLastPlayer = 8;
    [SerializeField]
    float maxZDistanceOfSpeedBoostsFromLastPlayer = 48;

    Dictionary<PooledMonoBehaviour, Vector3> speedBoostsPositions = new Dictionary<PooledMonoBehaviour, Vector3>();


    float minDistanceBetweenCoversSquared;
    float minDistanceBetweenSpeedBoostsSquared;
    float furthestCoverZ =0;
    bool stopSpawningCovers = false;

    bool gameStarted = false;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        coverPrefab.InitializePool();
        foreach (var obs in obstacles)
        {
            obs.InitializePool();
        }
        speedBoostPrefab.InitializePool();
    }


    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        minDistanceBetweenCoversSquared = minDistanceBetweenCovers * minDistanceBetweenCovers;
        minDistanceBetweenSpeedBoostsSquared = minDistanceBetweenSpeedBoosts * minDistanceBetweenSpeedBoosts;
        var jetEngine = FindObjectOfType<JetEngine>();
        jetEngine.onStrongWind += SpawnCovers;
        jetEngine.onStrongWind += SpawnSpeedBoosts;
        GameManager.Instance.onGameStart += HandleGameStart;
        SpawnCovers();
        timeForNextObstacle = Random.Range(minTimeForNextObstacle, maxTimeForNextObstacle);
    }

    private void OnDestroy()
    {
        GameManager.Instance.onGameStart -= HandleGameStart;
    }

    void HandleGameStart()
    {
        gameStarted = true;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient || !gameStarted)
            return;
        obstacleTimer += Time.deltaTime;
        if(obstacleTimer>timeForNextObstacle)
        {
            SpawnObstacles();
            obstacleTimer = 0;
            timeForNextObstacle = Random.Range(minTimeForNextObstacle, maxTimeForNextObstacle);
        }
    }

    void SpawnCovers()
    {
        if (stopSpawningCovers)
            return;
        int numberOfCoversToSpawn = GameManager.Instance.PlayersRemaining -1;      
        Vector3[] newCoversPositions = new Vector3[numberOfCoversToSpawn];
        float furthestNewCoverZ = furthestCoverZ;
        for (int i = 0; i < numberOfCoversToSpawn; i++)
        {
            int numberOfTries = 0;
            while (numberOfTries < 100)
            {
                float randomX = Random.Range(minX, maxX);
                float randomZ = furthestCoverZ + Random.Range(minDistanceFromFurthestCoverToSpawnCovers, maxDistanceFromFurthestCoverToSpawnCovers);
                randomZ = Mathf.Clamp(randomZ, 0, maxZPositionCoverOrSpeedBoostCanSpawn);
                Vector3 newCoverPos = new Vector3(randomX, 20f, randomZ);
                bool success = true;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (newCoverPos.FlatVectorDistanceSquared(newCoversPositions[j]) < minDistanceBetweenCoversSquared)
                    {
                        success = false;
                        numberOfTries++;
                        break;
                    }
                }

                if (success)
                {
                    newCoversPositions[i] = newCoverPos;
                    if (newCoverPos.z > furthestNewCoverZ)
                        furthestNewCoverZ = newCoverPos.z;
                    break;
                }
            }
        }
        furthestCoverZ = furthestNewCoverZ;
        if (Mathf.Approximately(furthestCoverZ, maxZPositionCoverOrSpeedBoostCanSpawn))
            stopSpawningCovers = true;

        for (int i = 0; i < newCoversPositions.Length; i++)
        {
            if (newCoversPositions[i] != Vector3.zero)
                coverPrefab.Get<PooledMonoBehaviour>(newCoversPositions[i], Quaternion.identity);
        }
    }

    void SpawnObstacles()
    {
        float x = Random.Range(minX, maxX);
        float y = (Random.Range(yMinPositionToSpawnObstacles, yMaxPositionToSpawnObstacles));
        float z = zPositionToSpawnObstacles;
        var obstacleToSpawn = obstacles[Random.Range(0, obstacles.Length)];
        obstacleToSpawn.Get<PooledMonoBehaviour>(new Vector3(x, y, z), Quaternion.identity);       
    }

    void SpawnSpeedBoosts()
    {
        if (stopSpawningCovers)//if we are already not spawning any more covers the game is almost over so there is no reason to keep spawning speed boosts
            return;
        int numberOfSpeedBoostsToSpawn = Random.Range(minSpeedBoostsToSpawn, maxSpeedBoostsToSpawn+1);
        float lastPlayerZ = GameManager.Instance.LastPlayerZ;
        float minZ = lastPlayerZ + minZDistanceOfSpeedBoostsFromLastPlayer;
        float maxZ = lastPlayerZ + maxZDistanceOfSpeedBoostsFromLastPlayer;
        for (int i = 0; i < numberOfSpeedBoostsToSpawn; i++)
        {
            int numberOfTries = 0;
            while (numberOfTries < 100)
            {
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                randomZ = Mathf.Clamp(randomZ, 0, maxZPositionCoverOrSpeedBoostCanSpawn);
                Vector3 newSpeedBoostPos = new Vector3(randomX, 1f, randomZ);
                bool success = true;
                foreach (var keyValuePair in speedBoostsPositions)
                {
                    if (newSpeedBoostPos.FlatVectorDistanceSquared(keyValuePair.Value) < minDistanceBetweenCoversSquared)
                    {
                        success = false;
                        numberOfTries++;
                        break;
                    }
                }

                if (success)
                {
                    var newSpeedBoost = speedBoostPrefab.Get<PooledMonoBehaviour>(newSpeedBoostPos, Quaternion.identity);
                    speedBoostsPositions.Add(newSpeedBoost, newSpeedBoostPos);
                    newSpeedBoost.OnReturnToPool += HandleSpeedBoostPicked;
                    break;
                }
            }
        }
    }

    void HandleSpeedBoostPicked(PooledMonoBehaviour speedBoost)
    {
        if(speedBoostsPositions.ContainsKey(speedBoost))
            speedBoostsPositions.Remove(speedBoost);
        speedBoost.OnReturnToPool -= HandleSpeedBoostPicked;
    }
}
