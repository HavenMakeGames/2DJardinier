using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlantSeedAbility : MonoBehaviour
{
    // Pouvoir planter des graines devant nous 

    [Header("Stats")]
    public float distance;
    public float durationPlant;
    public LayerMask groundMask;
    public LayerMask plantMask;
    private GameObject plantEquiped;
    public int maxPlant;
    private int nombrePlant;



 
    [Header("Prefabs")]
    public GameObject bounceMushroom;


    [Header("Components")]
    PlayerMovement playerMovement;
    private GameObject player;


    [Header("Inputs")]
    InputAction plantSeed;
    InputAction destroySeed;

    public void Start()
    {
        // va récuperer les touches de la capacité 
        plantSeed = InputSystem.actions.FindAction("Plant");
        destroySeed = InputSystem.actions.FindAction("Destroy Plant");
        player = gameObject;
        plantEquiped = bounceMushroom;
        playerMovement = GetComponentInParent<PlayerMovement>(); // recupere le script PlayerMovement pour avoir acces a des fonctions et des variables qui y sont
    }

    public void Update()
    {

        if (plantSeed.WasPressedThisFrame() == true) // Si la touche de la capacité est appuyée 
        {

            if (nombrePlant < maxPlant) // Limite le nombre de plantes posée simultanément 
            {
                Plant(plantEquiped);//Plante la graine equipée
            }
            else
            {
                Debug.Log("Nombre Max de Plant");
            }
            
        }
    }

    public void Plant(GameObject plantEquiped)
    {
        Vector3 pos = CalculPos(distance); // On récupère la position
        if (pos == Vector3.zero) { return; }
        nombrePlant += 1;
        GameObject plantToDestroy = Instantiate(plantEquiped, position: pos, rotation: Quaternion.identity); // On crée la plante ou point retourner par la fonction CalculPos()
        StartCoroutine(DestroyPlant(plantToDestroy));
    }

    public Vector3 CalculPos(float distance)
    {
        if (playerMovement.isFacingRight != true) // On regarde la direction vers laquelle le joueur regarde
        {
            distance = -1f * distance; // Si il regarde a gauche on met en negatif 
        }

        Vector2 sens = new Vector2(distance, 0); // On traduit la direction en un vector2
        float disX = player.transform.position.x + distance; // On calcule le point ou la plante sera plantée
        RaycastHit2D ray = Physics2D.Raycast(player.transform.position, sens, Mathf.Abs(distance) , groundMask); // On regarde si il n'y a pas de mur devant le joueur
        if (ray == false) // Si il n'y en a pas , on plante
        {
            Vector2 dis = new Vector2(disX, player.transform.position.y); // On prend la position ou la plante sera crée 
            RaycastHit2D hit = Physics2D.Raycast(dis, Vector2.down, 4, layerMask: plantMask); // On regarde si il n'y a pas de vide 
            Debug.DrawRay(dis, Vector2.down, Color.green, 4f); // On affiche le Raycast
            if (hit != false)
            {
                Vector3 pos = hit.point; // La position ou la plante sera crée
                return pos; // Renvoie cette position 
            }
        }
        return Vector3.zero;
    }

    IEnumerator DestroyPlant(GameObject plant)
    {
        yield return new WaitForSecondsRealtime(5f); // Fait disparaitre les plantes après un certain temps
        Destroy(plant);
        nombrePlant = nombrePlant - 1;
    }






}
