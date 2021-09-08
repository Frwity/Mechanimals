using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [HideInInspector] public WorldManager worldMananger;

    [SerializeField] GameObject firstDoor;
    [SerializeField] GameObject secondDoor;

    [SerializeField] GameObject firstTriggerZone;
    [SerializeField] GameObject secondTriggerZone;

    [SerializeField] GameObject cameraAnchor;

    bool isFighting = false;

    [System.Serializable]
    public class Wave
    {
        [SerializeField] public GameObject[] normalSpawn;
        [SerializeField] public GameObject[] flyingSpawn;

    };
    [SerializeField] Wave[] waves;

    [SerializeField] float timeBetweenWaves = 5f;
    float timerBetweenWaves = 0f;
    bool isWaveFinished = false;
    bool isWaveFinishedSpawning = false;

    int currentWave = 0;
    int totalWaveEnemy;
    int currentEnemykilled = 0;

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
            worldMananger.currentScrollingZone = null;
            firstTriggerZone.SetActive(false);
        }
        // if trigger zone 2
        if (secondTriggerZone.GetComponent<TriggerZone>().isTrigger)
        {
            firstDoor.SetActive(true);
            secondTriggerZone.SetActive(false);
            isFighting = true;
            totalWaveEnemy = waves[currentWave].flyingSpawn.Length + waves[currentWave].normalSpawn.Length;
        }

        if (isFighting) // fight
        {
            if (!isWaveFinishedSpawning)
            {
                for (int i = 0; i < waves[currentWave].normalSpawn.Length; ++i) // spawn normal
                    worldMananger.SummonNormalEnemyAt(waves[currentWave].normalSpawn[i].transform.position);

                for (int i = 0; i < waves[currentWave].flyingSpawn.Length; ++i) // spawn normal
                    worldMananger.SummonFlyingEnemyAt(waves[currentWave].flyingSpawn[i].transform.position);
                isWaveFinishedSpawning = true;
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
                    timerBetweenWaves = 0f;
                    isWaveFinished = false;
                    isWaveFinishedSpawning = false;
                    ++currentWave;
                    if (currentWave >= waves.Length)
                    {
                        worldMananger.currentArena = null;
                        isFighting = false;
                        secondDoor.SetActive(false);
                        return;
                    }
                    totalWaveEnemy = waves[currentWave].flyingSpawn.Length + waves[currentWave].normalSpawn.Length;
                    currentEnemykilled = 0;
                }
            }
        }
    }

    public void AddEnemyKill()
    {
        currentEnemykilled++;
    }

    public Vector3 GetCameraPostion()
    {
        return cameraAnchor.transform.position;
    }
}
