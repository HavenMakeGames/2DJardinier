using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Les infos qui regle comment le perso se deplace 
    [Header("Statistiques")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public float health = 100f;
    public float invicibilityTime;
    private bool canTakeDamage = true;
    private Vector2 direction;
    private GameObject latestPoint;


    // Les Inputs des Actions 
    [Header("Actions")]
    InputAction move;
    InputAction jump;

    // Les components qui controle et limite les deplacements du perso
    [Header("Components")]
    public Rigidbody2D rb;
    private Collider2D col;
    private GameObject player;

    // Catéforie pour quand y'aura des Prefabs
    [Header("Prefabs")]
    private string sertARien;



    //
    //Toutes les fonctions qui s'executent dans le jeu
    //
    private void Start()
    {
        // Lie l'input system avec les variables du script
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");

        // Lie les components avec les variables du script
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        player = gameObject;
    }

private void Update()
    {
        // Lie les valeurs de déplacement . Ex: si le joueur appuie sur A ou D ou bien Espace 
        Vector2 moveValue = move.ReadValue<Vector2>();
        bool jumpValue = jump.IsPressed();

        // Appelle les fonctions pour faire bouger le perso 
        MovePlayer(moveValue);
        JumpPlayer(jumpValue);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit");
        if (collision.collider.tag == "Enemy")
        {
            Debug.Log("Enemie touché");
            TakeDamage();
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Spawn Point")
        {
            latestPoint = collision.gameObject;
            Debug.Log("Nouveau point de réapparition " + latestPoint.name); 
        }
    }



    //
    //Toutes les fonctions qui ne s'executent pas dans le jeu mais dans une fonction 
    //
    private void MovePlayer(Vector2 direction)
    {
        //Ajoute une force dans la direction du deplacement (A ou D) 
        // La vitesse dépend de la variable publique speed
        rb.linearVelocity = new Vector2(direction.x * speed , rb.linearVelocity.y);
    }

    private bool GetIsGrounded()
    {
        // Verifie que le perso touche le sol pour pas pouvoir sauter a l'infini
        return Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
    }

    public void JumpPlayer(bool jumpValue)
    {
        // Si le joueur appuie sur Espace et est au sol 
        if (jumpValue == true && GetIsGrounded())
        {
            // Ajoute une force vers le haut pour faire sauter le perso 
            // La force du saut / hauteur est fonction de la variable jumpForce
            rb.AddRelativeForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        }
    }

    IEnumerator DamageDelayTime(float invicibilityTime)
    {
        yield return new WaitForSecondsRealtime(invicibilityTime); // Le temps entre lequel le joueur ne peut pas se faire taper après un coup
    }

    public void CanTakeDamage() // Utilise la coroutine pour que le joueur soit intangible pendant ce temps
    {
        canTakeDamage = false;
        StartCoroutine(DamageDelayTime(invicibilityTime));
        canTakeDamage = true;
    }

    public void TakeDamage()
    {
        if (canTakeDamage == true) // Si le joueur n'est pas intangible 
        {
            health = health - 10f; // Retire la vie du joueur 
            Debug.Log("Vous avez " + health);
            if (health <= 0) // Si la vie du joueur arrive en dessous ou à 0 
            {
                Die(); // Meurs  
            }
            CanTakeDamage(); // Execute l'intangebilite 
        }
    }

    public void Die()
    {
        if (latestPoint != null)
        {
            player.transform.position = latestPoint.transform.position;
        }

    }

}
