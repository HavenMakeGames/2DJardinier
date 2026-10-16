using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PlayerMovement : MonoBehaviour
{
    // Les infos qui regle comment le perso se deplace 
    [Header("Statistiques")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public float gravity;
    private Vector2 direction;

    public bool isFacingRight;
    private bool isJumping;
    private Vector2 moveValue;
    private float movingValue;
    private bool jumpValue;
    private bool isGrounded;
    public LayerMask jumpMask;
    private bool isAgainstWall;
    public LayerMask wallMask;
    public float slipSpeed;
    private bool canDetectWall = true;
    private bool isAccroched;
    private bool canMove = true;
    private bool canJump = true;
    // Les Inputs des Actions 
    [Header("Actions")]
    InputAction move;
    InputAction jump;


    // Les components qui controle et limite les deplacements du perso
    [Header("Components")]
    public Rigidbody2D rb;
    private Collider2D col;
    private GameObject player;
    public GameObject groundCheckL;
    public GameObject groundCheckR;
    public GameObject wallCheckL;
    public GameObject wallCheckR;

   


    // Catéforie pour quand y'aura des Prefabs
    [Header("Prefabs")]
    private string sertARien;



    //
    //Toutes les fonctions qui s'executent dans le jeu
    //
    private void Start()
    {
        // Lie l'input system avec les variables du script
        if (canMove)
        {
            move = InputSystem.actions.FindAction("Move");
        }
        if (canJump)
        {
            jump = InputSystem.actions.FindAction("Jump");
        }
        

        // Lie les components avec les variables du script
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        player = gameObject;
        isFacingRight = true;

    }

    private void Update()
    {
        // Lie les valeurs de déplacement . Ex: si le joueur appuie sur A ou D ou bien Espace 

        moveValue = move.ReadValue<Vector2>();
        movingValue = moveValue.x;
        jumpValue = jump.IsPressed();

        isGrounded = Physics2D.OverlapArea(groundCheckL.transform.position, groundCheckR.transform.position,jumpMask); // Regarde si la zone créée entre en contact avec le sol , si oui renvoie true
        if (canDetectWall) // si on autorise le joueur a aller contre un mur 
        {
            isAgainstWall = Physics2D.OverlapArea(wallCheckL.transform.position, wallCheckR.transform.position, wallMask); // Regarde si la zone crée touche un mur , qu'il soit à droite ou a gauche
        }
        
    }

    private void FixedUpdate()
    {
        // Appelle les fonctions pour faire bouger le perso 
        MovePlayer(moveValue, movingValue);
        JumpPlayer(jumpValue); 
    }




    //
    //Toutes les fonctions qui ne s'executent pas dans le jeu mais dans une fonction 
    //



    private void MovePlayer(Vector2 direction, float movingValue )
    {
        //Ajoute une force dans la direction du deplacement (A ou D) 
        // La vitesse dépend de la variable publique speed
        rb.linearVelocity = new Vector2(direction.x * speed , rb.linearVelocity.y);
        if (movingValue > 0)
        {
            isFacingRight = true;
        }
        else if (movingValue < 0)
        {
            isFacingRight = false;
        }
    }





    public void JumpPlayer(bool jumpValue)
    {
      
            if (!isJumping && jumpValue == true && isGrounded)
            {
                // Ajoute une force vers le haut pour faire sauter le perso 
                // La force du saut / hauteur est fonction de la variable jumpForce
                isJumping = true;
                rb.linearVelocityY = 0f;
                rb.AddForce(new Vector2(0f, jumpForce * 100));
            }
            else if (isAgainstWall && !isGrounded ) // Si le joueur touche un mur et n'est pas au sol , on active la capacité walljump
            {
                isAccroched = true;
                isJumping = false;
                // On imobilise le joueur pour montrer qu'il est accroché 
                rb.linearVelocity = new Vector2(0, 0);
                rb.angularVelocity = 0f;
                rb.gravityScale = slipSpeed; // On le fait glisser lentement pour pas que le joueur reste indefiniment sur le mur 

                if (jumpValue) // Si il saute en étant sur le mur
                {
                    StartCoroutine(WallJump()); // saute en étant sur le mur
                }
            }
            else if (!isAgainstWall) // Si le joueur n'est plu contre un mur le refaire tomber a la meme gravité que la normal
            {
                rb.gravityScale = gravity;
            }

        // Si le joueur appuie sur Espace et est au sol 
        isJumping = false;
    }





    IEnumerator DamageDelayTime(float invicibilityTime)
    {
        yield return new WaitForSecondsRealtime(invicibilityTime); // Le temps entre lequel le joueur ne peut pas se faire taper après un coup
    }


    

    IEnumerator WallJump()
    {
        
        rb.AddForce(new Vector2(0f, jumpForce * 100)); // Ajoute la force nécessaire au saut
        rb.gravityScale = gravity; // remet la gravité standard
        // Fais en sorte que le joueur soit concideré comme entrain de sauter et plu contre un mur pour pas qu'il bug
        isJumping = true;
        isAgainstWall = false;
        isAccroched = false;
        canDetectWall = false;
        yield return new WaitForSeconds(0.2f); // Un cooldown pour pas que le joueur se raccroche instantanement 
        canDetectWall = true;
    }

}
