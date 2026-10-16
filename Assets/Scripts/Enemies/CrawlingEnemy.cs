using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CrawlingEnemy : MonoBehaviour
{
    [Header("Type of Enemy")]
    public bool walkOnWallsPASFINI;
    public bool walkOnGround;
    public bool walkAroundWallsPASFINI;
    private int nbTypeOn;

    [Header("Stats")]
    public float speed;
    public float jumpForce; 
    private Vector2 direction;
    private int sens;
    public LayerMask groundMask;
    public LayerMask wallMask;
    private bool playerInLOS;
    public float rangeOfLOS;
    private int pointsIndex = 1;
    private float disBetweenPoints;
    private bool knowPlayerPos;
    private bool isFacingRight;
    private bool onWallRight;
   

    [Header("Components")]
    private GameObject enemy ;
    private GameObject player ;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public GameObject[] points;
    public GameObject leftGroundCheck;
    public GameObject rightGroundCheck;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        enemy = gameObject;
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        direction = new Vector2(1, 0);
    }

    private void Update()
    {
        
        Move();       
    }


    private void Move()
    {
        if (walkOnGround == true) 
        {
            float sens = GetSens();
            playerInLOS = GetLOS();
            if (!playerInLOS && knowPlayerPos == false) {
                RaycastHit2D ground = Physics2D.Raycast(new Vector2(enemy.transform.position.x + 0.5f * sens, enemy.transform.position.y - 1), new Vector2(0, -2f), 4f, groundMask);
                RaycastHit2D wall = Physics2D.Raycast(enemy.transform.position, new Vector2(sens, 0), 1f, wallMask);
                Debug.DrawRay(new Vector2(enemy.transform.position.x + 0.5f * sens, enemy.transform.position.y - 1), new Vector2(0, -2f), Color.blue);
                Debug.DrawRay(enemy.transform.position, new Vector2(sens, 0), Color.blue);
                if (ground == false)
                {
                    direction = -direction;
                }
                else if (wall== true)
                {
                    direction = -direction;
                }
                rb.linearVelocity = direction * speed / 2 ;
            }
            else if (playerInLOS == true || knowPlayerPos == true) 
            {
                knowPlayerPos = true;
                StartCoroutine(lossLOSOnPLayer());
                Vector2 directionPlayer = new Vector2(player.transform.position.x - 1 * sens - enemy.transform.position.x, player.transform.position.y - enemy.transform.position.y).normalized;
                if (directionPlayer.x > 0)
                {
                    rb.linearVelocity = new Vector2(1f * speed, 0f);
                }
                else if (directionPlayer.x < 0)
                {
                    rb.linearVelocity = new Vector2(-1f * speed, 0f);
                }
                else if (directionPlayer.x == 0)
                {
                    rb.linearVelocity = new Vector2(0f, 0f);
                }
                RaycastHit2D ground = Physics2D.Raycast(new Vector2(enemy.transform.position.x + 0.5f * sens, enemy.transform.position.y - 1), new Vector2(0, -2f), 4f, groundMask);
                RaycastHit2D wall = Physics2D.Raycast(enemy.transform.position, new Vector2(sens, 0), 1f, wallMask);
                if (ground == false || wall == true)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            } 
        }
        else if (walkOnWallsPASFINI == true)
        {
            direction = new Vector2(0, 1f);
            float sens = GetSensOnWall();
            rb.gravityScale = 0f;
            CheckWallSide();
            RaycastHit2D ground = Physics2D.Raycast(enemy.transform.position, Vector2.right * CheckWallSide(), 2.5f, wallMask);
            RaycastHit2D wall = Physics2D.Raycast(enemy.transform.position, Vector2.up * sens, 2f, groundMask);
            Debug.DrawRay(enemy.transform.position, Vector2.down * sens);
            Debug.DrawRay(enemy.transform.position, Vector2.left * sens);
            if (ground == false)
            {
                direction = -direction;
            }
            else if (wall == true )
            {
                direction = -direction;
            }
            rb.linearVelocity = direction * speed / 2;

        }
        else if (walkAroundWallsPASFINI == true)
        {
            disBetweenPoints = Vector2.Distance(enemy.transform.position, points[pointsIndex].transform.position);
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, points[pointsIndex].transform.position, speed * Time.deltaTime);
            if (disBetweenPoints < 0.2f)
            {
                TakeTurn();
            }
        }
        else
        {
            return;
        }
    }

    private float GetSens()
    {
        if (rb.linearVelocity.x > 0f)
        {
            isFacingRight = true;
            sens = 1;
            return sens;
        }
        else if (rb.linearVelocity.x < 0f)
        {
            isFacingRight = false;
            sens = -1;
            return sens;

        }
        return 0f;
    }

    private float GetSensOnWall()
    {
        if (onWallRight)
        {
            if(rb.linearVelocity.y > 0f)
            {
                isFacingRight = true;
                sens = 1;
                return sens;
            }
            else if (rb.linearVelocity.y < 0f)
            {
                isFacingRight = false;
                sens = -1;
                return sens;
            }
        }
        else if (!onWallRight)
        {
            if (rb.linearVelocity.y > 0f)
            {
                isFacingRight = false;
                sens = 1;
                return sens;
            }
            else if (rb.linearVelocity.y < 0f)
            {
                isFacingRight = true;
                sens = -1;
                return sens;
            }
        }
        return sens;
    }

   
    private bool GetLOS()
    {
        Vector2 directionPlayer = new Vector2( player.transform.position.x - 1 * GetSens() - enemy.transform.position.x , player.transform.position.y - enemy.transform.position.y);
        RaycastHit2D hitPlayer = Physics2D.Raycast(new Vector2(enemy.transform.position.x + 1 * GetSens(), enemy.transform.position.y), directionPlayer, rangeOfLOS);
        
        if (hitPlayer == true)
        {
            if (hitPlayer.collider.name == "Player")
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }


    private void TakeTurn()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z += points[pointsIndex].transform.eulerAngles.z;
        enemy.transform.eulerAngles = currentRotation;
        ChooseNextWaypoint();
    }


    private void ChooseNextWaypoint()
    {
        pointsIndex++;

        if(pointsIndex == points.Length)
        {
            pointsIndex = 0;
        }
    }

    IEnumerator lossLOSOnPLayer()
    {
        knowPlayerPos = true;
        yield return new WaitForSecondsRealtime(5f);
        knowPlayerPos = false;
    }

    float CheckWallSide()
    {
        Quaternion current = enemy.transform.rotation;
        RaycastHit2D leftWallCheck = Physics2D.Raycast(enemy.transform.position, Vector2.left, 2f, wallMask);
        RaycastHit2D rightWallCheck = Physics2D.Raycast(enemy.transform.position, Vector2.right, 2f, wallMask);
        float wallsens = 0f;
        if (leftWallCheck == true && rightWallCheck == false)
        {
            enemy.transform.rotation = Quaternion.Euler(0,0,-90);
            onWallRight = false;
            wallsens = -1;
        }
        else if (leftWallCheck == false && rightWallCheck == true)
        {
            enemy.transform.rotation = Quaternion.Euler(0, 0, 90);
            onWallRight = true;
            wallsens = 1;
        }
        return wallsens;
    }
    
}
