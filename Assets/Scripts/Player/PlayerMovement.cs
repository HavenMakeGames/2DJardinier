using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
    [HideInInspector] public bool isGrounded;
    public LayerMask jumpMask;
    private bool isAgainstWall;
    public LayerMask wallMask;
    public float slipSpeed;
    private bool canDetectWall = true;
    private bool isAccroched;
    [HideInInspector] public bool canMove = true;
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
    private SpriteRenderer sr;
    private RecentsPosition rp;
   


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
        sr = GetComponent<SpriteRenderer>();
        rp = GetComponent<RecentsPosition>();

    }

    private void Update()
    {
        // Lie les valeurs de déplacement . Ex: si le joueur appuie sur A ou D ou bien Espace 

        moveValue = move.ReadValue<Vector2>();
        jumpValue = jump.IsPressed();

        isGrounded = Physics2D.OverlapArea(groundCheckL.transform.position, groundCheckR.transform.position,jumpMask); // Regarde si la zone créée entre en contact avec le sol , si oui renvoie true
        if (isGrounded) 
        { 
            canDetectWall = true;
            isJumping = false;
        }
        if (canDetectWall) // si on autorise le joueur a aller contre un mur 
        {
            isAgainstWall = Physics2D.OverlapArea(wallCheckL.transform.position, wallCheckR.transform.position, wallMask); // Regarde si la zone crée touche un mur , qu'il soit à droite ou a gauche
        }
        
    }

    private void FixedUpdate()
    {
        // Appelle les fonctions pour faire bouger le perso 
        MovePlayer(moveValue);
        JumpPlayer(jumpValue); 
    }




    //
    //Toutes les fonctions qui ne s'executent pas dans le jeu mais dans une fonction 
    //



    private void MovePlayer(Vector2 direction)
    {
        //Ajoute une force dans la direction du deplacement (A ou D) 
        // La vitesse dépend de la variable publique speed
        if (move.IsPressed() && canMove)
        {
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);//Fais avancer le joueur
            if (rb.linearVelocity.x > 0)// si le joueur va a droite 
            {
                isFacingRight = true;
                sr.flipX = false; // On le fait regarder à droite (visuellement)
            }
            else if (rb.linearVelocity.x < 0)// Si le joueur va a gauche
            {
                isFacingRight = false;
                sr.flipX = true;// On le fait regarder à gauche (visuellement)
            }
        }

    }

    float CheckWall()
    {
        RaycastHit2D left = Physics2D.Raycast(player.transform.position, Vector2.left, 3f, wallMask); // On regarde si à gauche du joueur il y a un mur
        if (left.collider != null ) // Si il y a un mur 
        {
            return 1;
        }
        else if(left.collider == null) // Si il y a pas de mur à gauche il y en a un a droite 
        {
            return -1;
        }
        return 0f;
    }



    public void JumpPlayer(bool jumpValue)
    {
      
            if (!isJumping && jumpValue == true && isGrounded)
            {
                // Ajoute une force vers le haut pour faire sauter le perso 
                // La force du saut / hauteur est fonction de la variable jumpForce
                isJumping = true;
                rb.linearVelocityY =0f;
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
                    WallJump(); // saute en étant sur le mur
                }
            }
            else if (!isAgainstWall) // Si le joueur n'est plu contre un mur le refaire tomber a la meme gravité que la normal
            {
                rb.gravityScale = gravity;
            }
    }

    private void WallJump()
    {
        float wallSide = CheckWall(); // On regarde de quel cote est le mur

        isJumping = true;
        rb.linearVelocity = Vector2.zero; // On l'imobilise
        rb.angularVelocity = 0f;
        rb.gravityScale = gravity; // remet la gravité standard
        isGrounded = false;

        rb.AddForce(new Vector2(50f* jumpForce * wallSide, jumpForce * 100f)); // On envoie le joueur en diagonal à droite ou a gauche selon le signe de wallside

        // Fais en sorte que le joueur soit concideré comme entrain de sauter et plu contre un mur pour pas qu'il bug
        isAgainstWall = false;
        isAccroched = false;
        StartCoroutine(StopWallJump()); // On réinitialise l'état après un certain temps
    }
    IEnumerator StopWallJump()
    {
        canDetectWall = false;
        canMove = false;
        yield return new WaitForSeconds(0.2f); // Un cooldown pour pas que le joueur se raccroche instantanement et bouge 
        canDetectWall = true;
        canMove = true;
    }

    public void MoveToLastAvailablePos()
    {
        player.transform.position = rp.GetLastPos();
    }
    
}
