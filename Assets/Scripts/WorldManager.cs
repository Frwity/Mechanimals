using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class WorldManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] GameObject[] arenas;
    [SerializeField] GameObject[] scrollingZones;

    [SerializeField] public Arena currentArena = null;
    [SerializeField] public ScrollingZone currentScrollingZone = null;

    [SerializeField] GameObject firstDoor;

    [HideInInspector] public GameObject player1 = null;
    [HideInInspector] public GameObject player2 = null;

    [SerializeField] new Camera camera = null;

    public void Start()
    {
        firstDoor.SetActive(true);
        foreach (GameObject arena in arenas)
            arena.GetComponent<Arena>().worldMananger = this;
        foreach (GameObject scrollingZone in scrollingZones)
            scrollingZone.GetComponent<ScrollingZone>().worldMananger = this;
    }

    public void Update()
    {
        if (player1 != null && player2 != null && currentArena == null && currentScrollingZone == null)
            camera.transform.position = Vector3.Lerp(camera.transform.position, new Vector3((player1.transform.position.x + player2.transform.position.x) / 2f, 7f, -20f), 0.01f); 
        else if (currentArena != null)
            camera.transform.position = Vector3.Lerp(camera.transform.position, currentArena.GetCameraPostion(), 0.01f);
        else if (currentScrollingZone)
            camera.transform.position = Vector3.Lerp(camera.transform.position,currentScrollingZone.GetCameraPostion(), 0.01f);
    }

    public void AssignSpawningPlayer(PlayerInput playerInput)
    {
        if (player1 == null)
        {
            player1 = playerInput.gameObject;
            player1.transform.position = transform.position;
            return;
        }

        if (player2 == null)
        {
            player2 = playerInput.gameObject;
            player2.transform.position = transform.position;
        }

        firstDoor.SetActive(false);
    }

    public GameObject GetClosestPlayer(Vector3 pos)
    {
        if (player1 == null)
            return null;
        if (player2 == null)
            return player1;

        if ((player1.transform.position - pos).magnitude < (player2.transform.position - pos).magnitude)
            return player1;
        else
            return player2;
    }

    public void SummonNormalEnemyAt(Vector3 pos)
    {
        Enemy enemy = Instantiate(enemyPrefabs[0], new Vector3(pos.x, pos.y), Quaternion.identity).GetComponent<Enemy>();
        enemy.worldMananger = this;
        enemy.arena = currentArena;
    }

    public void SummonFlyingEnemyAt(Vector3 pos)
    {
        //Enemy enemy = Instantiate(enemyPrefabs[1], new Vector3(pos.x, pos.y), Quaternion.identity).GetComponent<Enemy>();
        //enemy.worldMananger = this;
        //enemy.arena = currentArena;
    }

    public void SummonRandomEnemyAt(Vector3 pos)
    {
        Enemy enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], new Vector3(pos.x, pos.y), Quaternion.identity).GetComponent<Enemy>();
        enemy.worldMananger = this;
        enemy.arena = currentArena;
    }
}
