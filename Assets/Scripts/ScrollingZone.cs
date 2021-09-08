using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingZone : MonoBehaviour
{
    [HideInInspector] public WorldManager worldMananger;

    [SerializeField] GameObject[] enemySpawnPoint;
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
        }
    }

    public Vector3 GetCameraPostion()
    {
        return Vector3.Lerp(cameraAnchor1.transform.position, cameraAnchor2.transform.position, cameraLerpPos);
    }
}
