using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Behaviour")]
    public bool followPoints; // Mode de l'ennemi
    public GameObject[] points;
    public bool stayStill; // Mode de l'ennemi
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
    private bool playerInLos;
    public float repulseForce;

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
        startPos = enemy.transform.position;// On enregistre la position ou le volant est au debut 
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
            // Quand le volant touche un mur il se cogne et est immobilisé
            rb.linearVelocity = Vector2.zero;
        }
        else if (collision.collider.tag == "Player")
        {
            // Si il touche le joueur on pousse le volant 
            Vector2 direction = rb.transform.position - collision.gameObject.transform.position;
            rb.linearVelocity = Vector2.zero; // On evite la démesure de force en mettant à zero
            rb.AddForce(direction * repulseForce); // On pousse
        }
    }

    
    private void Move()
    {
        playerInLos = GetLOS(); // On regarde si l'ennemi voit le joueur
        if (canMove)
        {
            // Mode : On fait bouger le volant comme si il fait une ronde / patrouille pour le rendre vivant 
            if (followPoints) // On regarde le mode de l'ennemi
            {
                if (playerInLos == false && knowPlayerPos == false) // Si le volant ne sait pas ou est le joueur 
                {
                    Vector2 direction = points[currentPoints].transform.position - enemy.transform.position; // On prend la direction de l'ennemi vers le prochain point
                    rb.linearVelocity = direction * speed / 2; // On le fait avancer vers le point 
                    if (points[currentPoints].transform.position == enemy.transform.position) // Quand le volant a atteint le point on pass au suivant 
                    {
                        currentPoints += 1;
                    }
                    if (currentPoints >= points.Length) // On reinitialise quand le tour est fini
                    {
                        currentPoints = 0;
                    }
                }
                else if (playerInLos == true || knowPlayerPos == true) // si le joueur sait ou est le joueur
                {
                    StartCoroutine(lossLOSOnPLayer());// On lui permet de se rappeler de ou le joueur est si il le voit , il l'oublie après 5 sec
                    Vector2 direction = player.transform.position - enemy.transform.position; // On regarde la direction vers le joueur
                    rb.linearVelocity = direction * speed; // Le volant avance vers le joueur
                    int i = Random.Range(0, 100); // On choisi aléatoirement si le volant dash 
                    if (i < 5) // Si il a été choisi qu'il dash
                    {
                        if (direction.magnitude <= 10f) // Si le volant est à une bonne distance pour dash
                        {
                            if (canDash) // et qu'il peut dash
                            {
                                StartCoroutine(Dash()); // Il dash
                            }
                        }
                    }
                    if (enemy.transform.position.y < player.transform.position.y + 6f) // Si le volant n'est pas au dessus du joueur
                    {
                        rb.AddRelativeForceY(speed); // On le fait monter
                    }
                    else if (enemy.transform.position.y > player.transform.position.y + 6f)// Si le volant est au dessus de sa position 
                    {
                        rb.AddForceY(-speed); // On le fait descendre
                    }

                    else { rb.linearVelocityY = 0f; } // sinon il avance en ligne droite au dessus du joueur
                }
            }

            // Mode : Le volant est immobile 
            else if (stayStill) // On regarde le mode de l'ennemi
            {
                Vector2 currentPos = new Vector2(enemy.transform.position.x, enemy.transform.position.y); // On prend la position actuelle du volant
                if (currentPos != startPos && playerInLos == false && knowPlayerPos == false) // Si le volant n'est pas à sa position de départ et qu'il ne voit pas le joueur
                {
                    Vector2 direction = new Vector2(startPos.x - currentPos.x , startPos.y - currentPos.y); // on regarde la direction vers son point d'origine
                    rb.linearVelocity = direction * speed; // Il va vers ce point
                }
                else if (currentPos == startPos && playerInLos == false && knowPlayerPos == false) 
                {
                    rb.linearVelocity = Vector2.zero; // Une fois qu'il atteint le point , il s'arrete 
                }
                else if (playerInLos == true || knowPlayerPos == true) // Si il sait ou est le joueur
                {
                    StartCoroutine(lossLOSOnPLayer());// On lui permet de se rappeler de ou le joueur est si il le voit , il l'oublie après 5 sec
                    Vector2 direction = player.transform.position - enemy.transform.position; // On regarde la direction vers le joueur
                    rb.linearVelocity = direction * speed; // On le fait avancer vers le joueur
                    int i = Random.Range(0, 100); // On choisit si il dash
                    if (i < 5) // si il dash
                    {
                        if (direction.magnitude <= 10f) // Si le volant est à une bonne distance pour dash
                        {
                            if (canDash) // et qu'il peut dash
                            {
                                StartCoroutine(Dash()); // Il dash
                            }
                        }
                    }
                    if (enemy.transform.position.y < player.transform.position.y + 6f)  // Si le volant est en dessous de sa position 
                    {
                        rb.AddRelativeForceY(speed); // On le fait monter
                    }
                    else if (enemy.transform.position.y > player.transform.position.y + 6f)// Si le volant est au dessus de sa position 
                    {
                        rb.AddForceY(-speed); // On le fait descendre
                    }

                    else { rb.linearVelocityY = 0f; } // On le fait bouger en ligne droite 
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
        canMove = false;
        canDash = false;
        Vector2 direction = player.transform.position - enemy.transform.position;// On prend la direction vers le joueur
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);// On l'imobilise pour faire comme si le volant charge le dash
        rb.AddForce(direction * dashPower * 10);// il dash
        yield return new WaitForSeconds(1f);// On le laisse avancer
        rb.linearVelocity = Vector2.zero;// On l'imobilise , fin du dash
        yield return new WaitForSeconds(cooldownStunt); // On le stun qui si il était stun
        canMove = true;
        canDash = true;
    }
}