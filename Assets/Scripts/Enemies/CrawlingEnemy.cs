using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrawlingEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float jumpForce;
    public bool canWalkOnWalls;
    private Vector2 direction;
    private int sens;
    public LayerMask groundMask;
    public LayerMask wallMask;
    private bool playerInLOS;
    public float rangeOfLOS;

    [Header("Components")]
    private GameObject enemy ;
    private GameObject player ;
    private Rigidbody2D rb;
    public GameObject limit1;
    public GameObject limit2;
    public GameObject limit3;
    public GameObject limit4;

    private void Start()
    {
        enemy = gameObject;
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        direction = new Vector2(1, 0);
    }

    private void Update()
    {
        playerInLOS = GetLOS();
        Move();       
    }


    private void Move()
    {
        if (canWalkOnWalls == false) 
        {
            if (GetLOS() == false) {
                RaycastHit2D ground = Physics2D.Raycast(new Vector2(enemy.transform.position.x + 0.5f * GetSens(), enemy.transform.position.y - 1), new Vector2(0, -2f), 4f, groundMask);
                Debug.DrawRay(enemy.transform.position, new Vector2(0, -2f), Color.red);
                RaycastHit2D wall = Physics2D.Raycast(enemy.transform.position, new Vector2(GetSens(), 0), 1f, wallMask);
                Debug.DrawRay(enemy.transform.position, new Vector2(GetSens(), 0), Color.red);
                if (ground == false)
                {
                    direction = -direction;
                }
                if (wall.collider != null)
                {
                    direction = -direction;
                }
                rb.linearVelocity = direction * speed;
            }
            else if (playerInLOS == true) 
            {
                Vector2 directionPlayer = player.transform.position - enemy.transform.position;
                rb.linearVelocity = new Vector2 (directionPlayer.x * speed, 0f);
            }
        }
        else if (canWalkOnWalls == true)
        {

        }
        

    }

    private float GetSens()
    {
        if (rb.linearVelocity.x > 0f)
        {
            sens = 1;
            return sens;
        }
        else if (rb.linearVelocity.x < 0f)
        {
            sens = -1;
            return sens;
        }
        return 0f;
    }

    private bool GetLOS()
    {
        Vector2 directionPlayer = new Vector2( player.transform.position.x - 1 * GetSens() - enemy.transform.position.x , player.transform.position.y - enemy.transform.position.y);
        RaycastHit2D hitPlayer = Physics2D.Raycast(new Vector2(enemy.transform.position.x + 1 * GetSens(), enemy.transform.position.y), directionPlayer, rangeOfLOS);
        Debug.DrawRay(new Vector2(enemy.transform.position.x + 1 * GetSens(), enemy.transform.position.y), directionPlayer,Color.blue);
        if (hitPlayer == true)
        {
            Debug.Log("hit un truc");
            if (hitPlayer.collider.name == "Player")
            {
                Debug.Log(hitPlayer.collider.name);
                return true;
            }
            return false;
        }
        else
        {
            Debug.Log("hit rien");
            return false;
        }
       
    }
}
