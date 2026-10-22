using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Types of Behavior")]
    public bool stayStill;
    private Vector2 startPos;
    public bool havePatern;
    public List<GameObject> followingPoints = new List<GameObject>();
    private int currentPoint =1;
    

    [Header("Stats")]
    public float speed;
    public float rangeLOS;
    private bool playerInLOS;
    private bool playerWasInLOS;
    public float rangeOfSafety;
    public float memoryTime;
    private bool isFacingRight = true;
    private bool knowPlayerPos;
    private bool canDash = true;
    private bool isDashing;
    [HideInInspector]public bool canMove = true;
    private bool wasDashing;


    [Header("Components")]
    private GameObject enemy;
    private GameObject player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private RecentsPosition rp;
    private EnemyDamageManager dm;

    private void Start()
    {
        enemy = gameObject;
        player = GameObject.FindWithTag("Player");
        rb = enemy.GetComponent<Rigidbody2D>();
        sr = enemy.GetComponent<SpriteRenderer>();
        startPos = enemy.transform.position;
        rp = player.GetComponent<RecentsPosition>();
        dm = enemy.GetComponent <EnemyDamageManager>();
    }

    private void Update()
    {
        GetFlip();
        playerInLOS = GetLOS();
        StartCoroutine(WasInLOS());
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            if (playerInLOS || knowPlayerPos)
            {
                Chase();
            }
            else if (!playerInLOS && !knowPlayerPos)
            {
                Idle();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && isDashing)
        {
            dm.GetPushedAway(enemy , player , dm.repulseForce);
            StartCoroutine(Stun(0.5f));
        }
        else if (collision.collider.tag != "Player" && isDashing )
        {
            StartCoroutine(Stun(2));
        }
    }

    private void Idle()
    {
        if (havePatern)
        {
            if (enemy.transform.position != followingPoints[currentPoint].transform.position)
            {
                Vector2 direction = followingPoints[currentPoint].transform.position - enemy.transform.position;
                rb.linearVelocity = direction.normalized * speed / 2;
            }
            else if (enemy.transform.position == followingPoints[currentPoint].transform.position)
            {
                currentPoint += 1;
                if (currentPoint < followingPoints.Count)
                {
                    currentPoint = 0;
                }
            }
        }
        else if (stayStill)
        {
            Vector2 currentPos = enemy.transform.position;
            if (Vector2.Distance(currentPos, startPos) > 0.5f)
            {
                Vector2 direction = startPos - currentPos;
                rb.linearVelocity = direction.normalized * speed / 2;
            }
            else if (Vector2.Distance(currentPos, startPos) < 0.5f)
            {
                rb.linearVelocity = Vector2.zero;
                isFacingRight = true;
                sr.flipX = false;
            }
        }
    }

    private void Chase()
    {
       
    }

    private void GetFlip()
    {
        if (rb.linearVelocity.x > 0)
        {
            isFacingRight = true;
            sr.flipX = false;
        }
        if (rb.linearVelocity.x < 0)
        {
            isFacingRight = false;
            sr.flipX = true;
        }
    }

    private float CheckFacingSide()
    {
        if (!isFacingRight)
        {
            return -1;
        }
        else if (isFacingRight)
        {
            return 1;
        }
        return 1;
    }
    private bool GetLOS()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 origin = new Vector2(enemy.transform.position.x + 1 * CheckFacingSide(), enemy.transform.position.y);
        Vector2 direction = playerPos - origin;
        RaycastHit2D hitPlayer = Physics2D.Raycast(origin, direction, rangeLOS);
        Debug.DrawRay(origin, direction, Color.red);
        if(hitPlayer.collider != null)
        {
            if (hitPlayer.collider.CompareTag("Player"))
            {
                return true;
            }
            else { return false; }
        }
        else { return false; }
    }

    IEnumerator WasInLOS()
    {
        if (playerInLOS != playerWasInLOS)
        {
            playerWasInLOS = playerInLOS;
            if(playerInLOS == false)
            {
                knowPlayerPos = true;
                yield return new WaitForSeconds(memoryTime);
                Debug.Log("Oublie");
                knowPlayerPos = false;
            }
        }
    }

    private void Dash(Vector2 direction)
    {
       
    }

    IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(3f);
    }

    IEnumerator DashPreparation()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator Stun(float duration)
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        canMove = true;

    }

    private bool ChooseToDash()
    {
        int i = Random.Range(0, 1000);
        if (i < 30)
        {
            return true;
        }
        return false; 
    }

    IEnumerator DashCooldown()
    {
        if (isDashing != wasDashing)
        {
            wasDashing = isDashing;
            if (isDashing == false)
            {
                canDash = true;
                yield return new WaitForSeconds(memoryTime);
                canDash = false;
            }
        }
    }
 

    private void GetBackToPosition()
    {

    }

    
}