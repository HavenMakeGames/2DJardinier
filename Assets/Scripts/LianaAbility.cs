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
   Créer un Joint => Maintenir un distance entre un objet A et un objet B , ici le joueur et le point d'accroche du grappin
   Besoin de :
   La longueur max du grappin 
   Un layerMask pour pas s'accrocher a tout  
   un Vector2 pour la position du point d'accroche

   Organisation du code :
   Raycast2D pour trouver le point d'accroche 
   Creer le point d'accroche 
   Ajouter une force dans la direction 
   */


    [Header("Grappling ")]
    InputAction grapplingInput;
    private bool canUseAbility; 
    private PlayerMovement playerMovement;
    private Mouse mouse;
    public float maxLianaLenght;
    public LayerMask mask;

    [Header("Force")]
    private GameObject player;
    private Rigidbody2D rb;

    private void Start()
    {
        mouse = Mouse.current;
        player = gameObject;
        grapplingInput = InputSystem.actions.FindAction("Liane Ability");
        rb = gameObject.GetComponent<Rigidbody2D>();
        canUseAbility = true;
    }

    private void FixedUpdate()
    {

        if (grapplingInput.WasPressedThisFrame())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            Vector2 playerPos = player.transform.position;
            Vector2 direction = (mousePos - playerPos).normalized ;

            RaycastHit2D hit = Physics2D.Raycast(playerPos, direction , maxLianaLenght , mask);
            Debug.DrawRay(playerPos, hit.point - playerPos , Color.red, 2f);
            Debug.Log(direction);

            if (hit.collider != null)
            {
                Debug.Log("Le raycast a touché" + hit.collider.name);
                Vector2 impulsion = (hit.point - playerPos).normalized ;
                rb.AddForce(impulsion * 250);
                StartCoroutine(Cooldown());
                
            }

        }
    }



    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(2);
    }
}

