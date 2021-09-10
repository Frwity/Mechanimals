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

    [SerializeField] GameObject nextScrollingZone;


    [System.Serializable]
    public class Wave
    {
        [SerializeField] public GameObject[] normalSpawn;
        [SerializeField] public GameObject[] flyingSpawn;
        [SerializeField] public GameObject[] flyingPoint;

    };
    [SerializeField] public Wave[] waves;

    [SerializeField] float timeBetweenWaves = 5f;
    bool isFighting = false;
    float timerBetweenWaves = 0f;
    bool isWaveFinished = false;
    bool isWaveFinishedSpawning = false;

    [HideInInspector] public int currentWave = 0;
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
            if (!isWaveFinishedSpawning) // summon
            {
                for (int i = 0; i < waves[currentWave].normalSpawn.Length; ++i) // spawn normal
                    worldMananger.SummonNormalEnemyAt(waves[currentWave].normalSpawn[i].transform.position);

                for (int i = 0; i < waves[currentWave].flyingSpawn.Length; ++i) // spawn normal
                    worldMananger.SummonFlyingEnemyAt(waves[currentWave].flyingSpawn[i].transform.position);
                isWaveFinishedSpawning = true;
            }


            // check wave ending 
            if (!isWaveFinished && currentEnemykilled >= totalWaveEnemy)
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
                    if (currentWave >= waves.Length) // check arena finished
                    {
                        worldMananger.currentArena = null;
                        worldMananger.RespawnPlayerIfDead();
                        isFighting = false;
                        secondDoor.SetActive(false);
                        nextScrollingZone.GetComponent<ScrollingZone>().ActivateScrollingZone();
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

    public void ResetArena()
    {
        timerBetweenWaves = 0f;
        isWaveFinished = false;
        isWaveFinishedSpawning = false;

        currentWave = 0;
        currentEnemykilled = 0;
        firstDoor.SetActive(false);
    }
}
