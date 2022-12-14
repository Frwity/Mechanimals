using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

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

    [SerializeField] public new GameObject camera = null;

    [SerializeField] Sprite p1Arrow;
    [SerializeField] Sprite p2Arrow;

    [SerializeField] Sprite bearhud;
    [SerializeField] Sprite crabhud;
    [SerializeField] Sprite goathud;

    [SerializeField] GameObject spawn;

    // ui

    [SerializeField] GameObject hud;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject p1Arrowstart;
    [SerializeField] GameObject p2Arrowstart;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;

    [SerializeField] GameObject triggerWin;

    public static WorldManager worldManager;

    private Vector2 p1body;

    public void Start()
    {
        worldManager = this;

        firstDoor.SetActive(true);
        foreach (GameObject arena in arenas)
            arena.GetComponent<Arena>().worldMananger = this;
        foreach (GameObject scrollingZone in scrollingZones)
            scrollingZone.GetComponent<ScrollingZone>().worldMananger = this;

        //SummonNormalEnemyAt(transform.position + Vector3.right * 15);
        //SummonFlyingEnemyAt(transform.position + Vector3.right * 15);

    }

    public void Update()
    {
        if (triggerWin.GetComponent<TriggerZone>().isTrigger)
        {
            hud.SetActive(false);
            winMenu.SetActive(true);
        }

        if (player1 != null && player2 != null && currentArena == null && currentScrollingZone == null)
            camera.transform.position = Vector3.Lerp(camera.transform.position, new Vector3((player1.transform.position.x + player2.transform.position.x) / 2f, -28.59f, -20f), 0.01f); 
        else if (currentArena != null)
            camera.transform.position = Vector3.Lerp(camera.transform.position, currentArena.GetCameraPostion(), 0.01f);
        else if (currentScrollingZone)
            camera.transform.position = Vector3.Lerp(camera.transform.position,currentScrollingZone.GetCameraPostion(), 0.01f);

        if (player1 != null && player2 != null)
        {
            SetHealt1();
            SetHealt2();
        }

        if (currentScrollingZone != null && player1 != null && player2 != null)
        {
            Player p1 = player1.GetComponent<Player>();
            Player p2 = player2.GetComponent<Player>();

            if (!p1.isAlive)
            {
                p1.rb.velocity = Vector2.zero;
                player1.SetActive(true);
                p1.life = p1.maxLife;
                p1.isAlive = true;
                p1.transform.position = camera.transform.position;
            }
            if (!p2.isAlive)
            {
                p2.rb.velocity = Vector2.zero;
                player2.SetActive(true);
                p2.life = p2.maxLife;
                p2.isAlive = true;
                p2.transform.position = camera.transform.position;
            }
        }
        if (currentArena != null && player1 != null && player2 != null)
        {
            Player p1 = player1.GetComponent<Player>();
            Player p2 = player2.GetComponent<Player>();

            if (!p1.isAlive && !p2.isAlive)
            {
                loseMenu.SetActive(true);
                hud.SetActive(false);
                //p1.rb.velocity = Vector2.zero;
                //p1.life = p1.maxLife;
                //p1.isAlive = true;
                //p1.transform.position = transform.position;
                //player1.SetActive(true);
                //p2.rb.velocity = Vector2.zero;
                //p2.life = p2.maxLife;
                //p2.isAlive = true;
                //p2.transform.position = transform.position;
                //player2.SetActive(true);

                //foreach (GameObject arena in arenas)
                //    arena.GetComponent<Arena>().ResetArena();
                //foreach (GameObject scrollingZone in scrollingZones)
                //    scrollingZone.GetComponent<ScrollingZone>().ResetScrollingZone();

                //int r = Random.Range(0, 3);
                //Vector2 p1body = p1.RandomChangeBody(r, (r + Random.Range(1, 3)) % 3);
                //p2.RandomChangeBody((int)p1body.x, (int)p1body.y);

                //Enemy[] enemies = FindObjectsOfType<Enemy>();
                //foreach (Enemy en in enemies)
                //    Destroy(en.gameObject);

                //currentArena = null;
            }
        }
    }
    public void AssignSpawningPlayer(PlayerInput playerInput)
    {
        if (player1 == null)
        {
            player1 = playerInput.gameObject;
            player1.transform.position = transform.position;
            int r = Random.Range(0, 3);
            p1body = player1.GetComponent<Player>().RandomChangeBody(r, (r + Random.Range(1, 3)) % 3);
            player1.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = p1Arrow;
            p1Arrowstart.GetComponent<Image>().color = Color.green;

            hud.transform.GetChild(0).GetComponent<Image>().sprite = r == 0 ? goathud : (r == 1 ? bearhud : crabhud);
            SetMaxHealt1();
            return;
        }

        if (player2 == null)
        {
            player2 = playerInput.gameObject;
            player2.transform.position = transform.position;
            int x = (int)player2.GetComponent<Player>().RandomChangeBody((int)p1body.x, (int)p1body.y).x;
            player2.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = p2Arrow;
            p2Arrowstart.GetComponent<Image>().color = Color.green;
            hud.transform.GetChild(1).GetComponent<Image>().sprite = x == 0 ? goathud : ( x == 1 ? bearhud : crabhud);
            SetMaxHealt2();
        }

        firstDoor.SetActive(false);
    }

    public void RespawnPlayerIfDead()
    {
        if (player1 != null && player2 != null)
        {
            Player p1 = player1.GetComponent<Player>();
            Player p2 = player2.GetComponent<Player>();

            if (!p1.isAlive)
            {
                p1.rb.velocity = Vector2.zero;
                player1.SetActive(true);
                p1.life = p1.maxLife;
                p1.isAlive = true;
                p1.transform.position = camera.transform.position;
            }
            if (!p2.isAlive)
            {
                p2.rb.velocity = Vector2.zero;
                player2.SetActive(true);
                p2.life = p2.maxLife;
                p2.isAlive = true;
                p2.transform.position = camera.transform.position;
            }
        }
    }
    public GameObject GetClosestPlayer(Vector3 pos)
    {
        if (player1 == null)
            return null;
        if (player2 == null)
            return player1;

        if (!player1.GetComponent<Player>().isAlive)
            return player2;
        if (!player2.GetComponent<Player>().isAlive)
            return player1;

        if ((player1.transform.position - pos).magnitude < (player2.transform.position - pos).magnitude)
            return player1;
        else
            return player2;
    }

    public GameObject GetRandomPlayer(Vector3 pos)
    {
        if (player1 == null)
            return null;
        if (player2 == null)
            return player1;

        if (!player1.GetComponent<Player>().isAlive)
            return player2;
        if (!player2.GetComponent<Player>().isAlive)
            return player1;

        if (Random.Range(0, 101) % 2 == 0)
            return player1;
        else
            return player2;
    }

    public void SummonNormalEnemyAt(Vector3 pos)
    {
        Enemy enemy = Instantiate(enemyPrefabs[0], new Vector3(pos.x, pos.y), Quaternion.identity).GetComponent<Enemy>();
        enemy.worldMananger = this;
        enemy.arena = currentArena;
        enemy.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void SummonFlyingEnemyAt(Vector3 pos)
    {
        Enemy enemy = Instantiate(enemyPrefabs[1], new Vector3(pos.x, pos.y), Quaternion.identity).GetComponent<Enemy>();
        enemy.worldMananger = this;
        enemy.arena = currentArena;
        if (currentArena != null)
        {
            foreach (GameObject go in currentArena.waves[currentArena.currentWave].flyingPoint)
                enemy.waypoints.Add(go);
        }
    }

    public void SummonRandomEnemyAt(Vector3 pos)
    {
        Enemy enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], new Vector3(pos.x, pos.y), Quaternion.identity).GetComponent<Enemy>();
        enemy.worldMananger = this;
        enemy.arena = currentArena;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        if (player1 != null && player2 != null)
        {
            player1.transform.position = spawn.transform.position;
            player2.transform.position = spawn.transform.position;
            menu.SetActive(false);
            hud.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("LD_Thibaut");
    }

    void SetMaxHealt1()
    {
        hud.transform.GetChild(2).GetComponent<Slider>().maxValue = player1.GetComponent<Player>().maxLife;
        hud.transform.GetChild(2).GetComponent<Slider>().value = player1.GetComponent<Player>().maxLife;
    }
    void SetMaxHealt2()
    {
        hud.transform.GetChild(3).GetComponent<Slider>().maxValue = player2.GetComponent<Player>().maxLife;
        hud.transform.GetChild(3).GetComponent<Slider>().value = player2.GetComponent<Player>().maxLife;
    }
    void SetHealt1()
    {
        hud.transform.GetChild(2).GetComponent<Slider>().value = player1.GetComponent<Player>().life;
    }
    void SetHealt2()
    {
        hud.transform.GetChild(3).GetComponent<Slider>().value = player2.GetComponent<Player>().life;
    }

}
