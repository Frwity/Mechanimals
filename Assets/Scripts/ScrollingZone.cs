using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingZone : MonoBehaviour
{
    [HideInInspector] public WorldManager worldMananger;

    [System.Serializable]
    public class Wave
    {
        [SerializeField] public GameObject waveTriggerZone;
        [SerializeField] public GameObject[] normalSpawn;
        [SerializeField] public GameObject[] flyingSpawn;
        [SerializeField] public GameObject[] flyingPoint;

    };
    [SerializeField] public Wave[] waves;


    [SerializeField] GameObject triggerZone;
    [SerializeField] GameObject cameraAnchor1;
    [SerializeField] GameObject cameraAnchor2;
    [SerializeField] float speed;
    float lerpSpeed;
    float cameraLerpPos = 0;
    bool isFighting = false;

    public void Start()
    {
        lerpSpeed = 1f / (cameraAnchor1.transform.position - cameraAnchor2.transform.position).magnitude * speed;
    }

    public void Update()
    {
        if (triggerZone.GetComponent<TriggerZone>().isTrigger)
        {
            worldMananger.currentScrollingZone = this;
            triggerZone.SetActive(false);
            isFighting = true;
        }
        if (isFighting)
        {
            cameraLerpPos += lerpSpeed * Time.deltaTime;
            
            foreach (Wave wave in waves)
            {
                if (wave.waveTriggerZone.activeSelf && wave.waveTriggerZone.GetComponent<TriggerZone>().isTrigger)
                {
                    for (int i = 0; i < wave.normalSpawn.Length; ++i) // spawn normal
                        worldMananger.SummonNormalEnemyAt(wave.normalSpawn[i].transform.position);

                    for (int i = 0; i < wave.flyingSpawn.Length; ++i) // spawn flying
                        worldMananger.SummonFlyingEnemyAt(wave.flyingSpawn[i].transform.position);

                    wave.waveTriggerZone.SetActive(false);
                }
            }
        }
    }

    public Vector3 GetCameraPostion()
    {
        return Vector3.Lerp(cameraAnchor1.transform.position, cameraAnchor2.transform.position, cameraLerpPos);
    }

    public void ResetScrollingZone()
    {
        isFighting = false;

        foreach (Wave wave in waves)
        {
            wave.waveTriggerZone.SetActive(true);
            wave.waveTriggerZone.GetComponent<TriggerZone>().isTrigger = false;
        }
    }
}
