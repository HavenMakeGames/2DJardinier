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

    [Header("Stats")]
    public float speed;
    public float jumpForce; 
    private Vector2 direction;
    private int sens;
    public LayerMask groundMask;
    public LayerMask wallMask;
    private bool playerInLOS;
    public float rangeOfLOS;
    private bool knowPlayerPos;
    private bool isFacingRight;
   

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

    private float GetSens()
    {
        if (rb.linearVelocity.x > 0f)
        {
            isFacingRight = true;
            sens = 1;
            sr.flipX = false;
            return sens;
        }
        else if (rb.linearVelocity.x < 0f)
        {
            isFacingRight = false;
            sens = -1;
            sr.flipX = true;
            return sens;

        }
        return 0f;
    }

    private bool GetLOS()
    {
        Vector2 directionPlayer = new Vector2( player.transform.position.x - 1 * GetSens() - enemy.transform.position.x , player.transform.position.y - enemy.transform.position.y);
        RaycastHit2D hitPlayer = Physics2D.Raycast(new Vector2(enemy.transform.position.x + 1 * GetSens(), enemy.transform.position.y), directionPlayer, rangeOfLOS);
        if (directionPlayer.magnitude <= 5f)
        {
            return true;
        }
        else if (hitPlayer == true)
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

    IEnumerator lossLOSOnPLayer()
    {
        knowPlayerPos = true;
        yield return new WaitForSecondsRealtime(5f);
        knowPlayerPos = false;
    }
    
}
