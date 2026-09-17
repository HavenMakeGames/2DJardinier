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
    public float speed = 5;
    public float jumpForce = 5;
    public bool isGrounded = true;
    private Vector2 direction;

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


    private void Start()
    {
        // Lie l'input system avec les variables du script
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");

        // Lie les components avec les variables du script
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        player = GetComponent<GameObject>();
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
            isGrounded = false;
            // Ajoute une force vers le haut pour faire sauter le perso 
            // La force du saut / hauteur est fonction de la variable jumpForce
            rb.AddRelativeForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        }
    }

}
