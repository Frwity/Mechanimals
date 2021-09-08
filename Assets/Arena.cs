using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [HideInInspector] public WorldManager worldMananger;
    [SerializeField] GameObject[] enemySpawnPoint;

    [SerializeField] GameObject firstDoor;
    [SerializeField] GameObject secondDoor;

    [SerializeField] GameObject firstTriggerZone;
    [SerializeField] GameObject secondTriggerZone;

    bool isFighting = false;

    [System.Serializable]
    public class Wave
    {
        [SerializeField] public int normalEnemyCount;
        [SerializeField] public int normalSpawnAtOnce;

        [SerializeField] public int flyingEnemyCount;
        [SerializeField] public int flyingSpawnAtOnce;

        [SerializeField] public float spawnDelay;
    };
    [SerializeField] Wave[] waves;

    [SerializeField] float timeBetweenWaves = 5f;
    public float timerBetweenWaves = 0f;
    public bool isWaveFinished = false;
    public bool isWaveFinishedSpawning = false;

    public int currentWave = 0;
    public int totalWaveEnemy;
    public int currentEnemykilled = 0;

    public float spawnTimer = 0f;

    public int currentSpawnedNormal = 0;
    int currentSpawnedFlying = 0;


    public void Start()
    {
        firstDoor.SetActive(false);
    }

    public void Update()
    {
        if (currentWave >= waves.Length)
            return;

        // if trigger zone 1
        if (firstTriggerZone.GetComponent<TriggerZone>().isTrigger)
        {
            worldMananger.currentArena = this;
            firstTriggerZone.SetActive(false);
            firstDoor.SetActive(true);
        }
        // if trigger zone 2
        if (secondTriggerZone.GetComponent<TriggerZone>().isTrigger)
        {
            secondTriggerZone.SetActive(false);
            isFighting = true;
            totalWaveEnemy = waves[currentWave].flyingEnemyCount + waves[currentWave].normalEnemyCount;
        }

        if (isFighting) // fight
        {
            if (!isWaveFinishedSpawning)
            {
                if (spawnTimer > waves[currentWave].spawnDelay)
                {
                    for (int i = 0; i < waves[currentWave].normalSpawnAtOnce; ++i) // spawn normal
                    {
                        worldMananger.SummonNormalEnemyAt(GetRandomSpawnpointPosition());
                        if (++currentSpawnedNormal >= waves[currentWave].normalEnemyCount)
                            break;
                    }

                    for (int i = 0; i < waves[currentWave].flyingSpawnAtOnce; ++i) // spawn flying
                    {
                        worldMananger.SummonFlyingEnemyAt(GetRandomSpawnpointPosition());
                        if (++currentSpawnedFlying >= waves[currentWave].flyingEnemyCount)
                            break;
                    }
                    if (currentSpawnedNormal >= waves[currentWave].normalEnemyCount && currentSpawnedFlying >= waves[currentWave].flyingEnemyCount)
                        isWaveFinishedSpawning = true;
                    spawnTimer = 0f;
                }

                spawnTimer += Time.deltaTime;
            }

            // check wave ending 
            if (!isWaveFinished && totalWaveEnemy == currentEnemykilled)
            {
                timerBetweenWaves = 0f;
                isWaveFinished = true;
            }

            if (isWaveFinished) // check and reset for next wave or next zone
            {
                timerBetweenWaves += Time.deltaTime;
                if (timerBetweenWaves > timeBetweenWaves)
                {
                    currentSpawnedNormal = 0;
                    currentSpawnedFlying = 0;
                    timerBetweenWaves = 0f;
                    isWaveFinished = false;
                    spawnTimer = 0f;
                    isWaveFinishedSpawning = false;
                    ++currentWave;
                    if (currentWave >= waves.Length)
                    {
                        worldMananger.currentArena = null;
                        isFighting = false;
                        secondDoor.SetActive(false);
                        return;
                    }
                    totalWaveEnemy = waves[currentWave].flyingEnemyCount + waves[currentWave].normalEnemyCount;
                    currentEnemykilled = 0;
                }
            }
        }
    }

    Vector3 GetRandomSpawnpointPosition()
    {
        return enemySpawnPoint[Random.Range(0, enemySpawnPoint.Length)].transform.position;
    }

    public void AddEnemyKill()
    {
        currentEnemykilled++;
    }
}
