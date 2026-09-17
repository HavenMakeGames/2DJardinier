using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;
public class LianaAbility : MonoBehaviour
{
    /*
    Grappin : 
    Tire le joueur dans la direction de la sourie avec une certaine force 

   */


    // Toutes les stats qui regle le grappin
    [Header("Grappling ")]
    InputAction grapplingInput;
    private bool canUseAbility; 
    private PlayerMovement playerMovement;
    private Mouse mouse;
    public float maxLianaLenght;
    public float cooldownTime;
    public LayerMask mask;
    public Vector2 raycastOffset ;
    public float grapPower;

    // Les components qui vont faire bouger le perso 
    [Header("Component")]
    private GameObject player;
    private Rigidbody2D rb;

    private void Start()
    { 
        mouse = Mouse.current; // Connecte la variable avec le peripherique 

        // Connecte les components avec les variables 
        player = gameObject;  
        rb = gameObject.GetComponent<Rigidbody2D>();

        grapplingInput = InputSystem.actions.FindAction("Liane Ability"); // Lie l'input system avec la variable Input
        canUseAbility = true; //Sert a reguler l'utilisation de la capacité 
    }

    private void Update()
    {
        // Si la touche est appuyé et la régulation l'autorise 
        if (grapplingInput.WasPressedThisFrame() && canUseAbility == true)
        {
            canUseAbility = false;

            // Récupere les positions nécessaire à l'utilisation du grappin 
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());// La ou la sourie est 
            Vector2 playerPos = player.transform.position;// La ou le joueur est 
            Vector2 direction = (mousePos - playerPos).normalized;// La direction dans laquelle le grappin doit aller , du joueur vers la sourie 

            // Utilise un Raycast ("laser") pour simuler le grappin et detecter les collisions 
            RaycastHit2D hit = Physics2D.Raycast(playerPos, direction, maxLianaLenght, mask);// Limiter par la longueur maximale du grappin et un filtre pour ne pas s'accrocher à tout 
            Debug.DrawRay(playerPos + raycastOffset, direction * 10, Color.green, 2f); // Affiche le grappin temporaire 
            Debug.Log(direction);

            // Si le grappin touche bel et bien un object valide 
            if (hit.collider != null)
            {
                // Effectue l'animation et l'action du grappin 
                rb.simulated = false;
                StartCoroutine(ImobilisationAnimation(hit, playerPos, grapPower));
            }

            // Cooldown de la capacite pour ne pas spam
            StartCoroutine(Cooldown());

        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(cooldownTime); // Attend le temps de la variable 
        canUseAbility = true;
    }

    IEnumerator ImobilisationAnimation(RaycastHit2D hit, Vector2 playerPos , float grapPower)
    {

        yield return new WaitForSecondsRealtime(0.25f); // Imobilise le perso dans les airs 
        rb.simulated = true;
        Debug.Log("Le raycast a touché" + hit.collider.name); 
        Vector2 impulsion = (hit.point - playerPos).normalized; // Calcule la puissance du saut 
        rb.AddRelativeForce(impulsion * grapPower * 100); // Ajuste et effectue le saut selon la variable grapPower

    }

}

