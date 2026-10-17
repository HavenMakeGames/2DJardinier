using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Behaviour")]
    public bool followPoints;
    public GameObject[] points;
    public bool stayStill;
    private Vector2 startPos;

    [Header("Stats")]
    public float speed;
    public float rangeOfLOS;
    private bool isFacingRight;
    private float sens;
    private int currentPoints = 1;
    private bool knowPlayerPos;
    private bool canDash = true;
    public float dashPower;
    public float cooldownStunt;
    public float slowingSpeed;
    private bool canMove = true;

    [Header("Components")]
    private GameObject player;
    private GameObject enemy;
    private Rigidbody2D rb;
    private SpriteRenderer sr;



    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        enemy = gameObject;
        rb = enemy.GetComponent<Rigidbody2D>();
        sr = enemy.GetComponent<SpriteRenderer>();
        startPos = enemy.transform.position;
    }
    private void Update()
    {
        GetSens();
        Move();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "Wall" || collision.collider.tag == "Plantable")
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    private void Move()
    {
        if (canMove)
        {
            if (followPoints)
            {
                if (GetLOS() == false && knowPlayerPos == false)
                {
                    Vector2 direction = points[currentPoints].transform.position - enemy.transform.position;
                    rb.linearVelocity = direction * speed / 2;
                    currentPoints += 1;
                    if (currentPoints >= points.Length)
                    {
                        currentPoints = 0;
                    }
                }
                else if (GetLOS() == true || knowPlayerPos == true)
                {
                    StartCoroutine(lossLOSOnPLayer());
                    Vector2 direction = player.transform.position - enemy.transform.position;
                    rb.linearVelocity = direction * speed;
                    int i = Random.Range(0, 100);
                    if (i < 10)
                    {
                        if (canDash)
                        {
                            StartCoroutine(Dash());
                        }
                    }
                    if (enemy.transform.position.y < player.transform.position.y + 5f) 
                    {
                        rb.AddRelativeForceY(speed);
                    }
                    else { rb.linearVelocityY = 0f; }
                }
            }
            else if (stayStill)
            {
                Vector2 currentPos = player.transform.position;
                while (currentPos != startPos && GetLOS() == false && knowPlayerPos == false)
                {
                    rb.linearVelocity = Vector2.zero;
                    Vector2 direction = startPos - currentPos;
                    rb.linearVelocity = direction * speed;
                    Debug.Log("Va a origine" +"origine :" + startPos + "Current :" + currentPos);
                }
                if (GetLOS() == true || knowPlayerPos == true)
                {
                    StartCoroutine(lossLOSOnPLayer());
                    Vector2 direction = player.transform.position - enemy.transform.position;
                    rb.linearVelocity = direction * speed;
                    int i = Random.Range(0, 100);
                    if (i < 10)
                    {
                        if (direction.magnitude <= 7f)
                        {
                            if (canDash)
                            {
                                StartCoroutine(Dash());
                            }
                        }
                    }
                    if (enemy.transform.position.y < player.transform.position.y + 5f)
                    {
                        rb.AddRelativeForceY(speed);
                    }
                    else { rb.linearVelocityY = 0f; }
                }
            }
        }
    }
    private bool GetLOS()
    {
        Vector2 directionPlayer = new Vector2(player.transform.position.x - 1 * GetSens() - enemy.transform.position.x, player.transform.position.y - enemy.transform.position.y);
        RaycastHit2D hitPlayer = Physics2D.Raycast(new Vector2(enemy.transform.position.x + 2 * GetSens(), enemy.transform.position.y), directionPlayer, rangeOfLOS );
        Debug.DrawRay(new Vector2(enemy.transform.position.x + 2 * GetSens(), enemy.transform.position.y), directionPlayer, Color.blue);
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
        return 1f;
    }

    IEnumerator lossLOSOnPLayer()
    {
        knowPlayerPos = true;
        yield return new WaitForSecondsRealtime(5f);
        knowPlayerPos = false;
    }

    IEnumerator Dash()
    {
        Debug.Log("Dash");
        canMove = false;
        canDash = false;
        Vector2 direction = player.transform.position - enemy.transform.position;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        rb.AddForce(direction * dashPower * 10);
        yield return new WaitForSeconds(1f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(cooldownStunt);
        canMove = true;
        canDash = true;
    }
}