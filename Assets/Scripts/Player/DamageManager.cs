using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


public class DamageManager : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100;
    public float health = 100f;
    public float invicibilityTime;
    private bool canTakeDamage = true;
    public Slider healthSlider;
    private GameObject latestPoint;
    private HashSet<bool> effects = new HashSet<bool> { };
    public float repulseForce;
    private Vector2 startPos; 


    [Header("Components")]
    private GameObject player;
    private SpriteRenderer sr;
    private PlayerMovement pmouv;


    private void Start()
    {
        player = gameObject;
        pmouv = gameObject.GetComponent<PlayerMovement>();
        SetMaxHealth(health);  // On met max vie le perso (joueur ou npc)
        sr = GetComponent<SpriteRenderer>();
        startPos = player.transform.position;
    }

    public void OnCollisionEnter2D(Collision2D collision) // Quand l'individu rentre en contact avec un individu qui peut lui mettre des dégats , il prend des dégats
    {
        if (collision.collider.tag == "Enemy")
        {
            GetPushedAway(player, collision.gameObject,repulseForce);
            TakeDamage(10);
        }
    }
    
    public void OnTriggerEnter2D(Collider2D collision) // Quand le joueur touche un point de spawn , il l'enregistre
    {
        if (collision.gameObject.tag == "Spawn Point")
        {
            latestPoint = collision.gameObject ; // On enregistre le point de spawn
        }
    }

    public void TakeDamage(int amout)
    {
        if (canTakeDamage == true) // Si le joueur n'est pas intangible 
        {
            health = health - amout; // Retire la vie du joueur 
            healthSlider.value = health;
            if (health <= 0) // Si la vie du joueur arrive en dessous ou à 0 
            {
                Die(); // Meurs  
            }
            else
            {
                StartCoroutine(DamageDelayTime(invicibilityTime)); // Execute l'intangebilite 
            }
        }
    }

    public void Die()
    {
        if ( latestPoint )
        {
            player.transform.position = latestPoint.transform.position; // Teleporte le joueur au dernier point de réapparition
        }
        else if (latestPoint == null) // Si le character n'est pas le joueur , il ne peut donc pas avoir de point de spawn donc il meure sans réapparaitre
        {
            player.transform.position = startPos;
        }
        health = maxHealth;
        healthSlider.value = maxHealth; // On le remet full vie
    }

    private void SetMaxHealth(float health) // On ne fait pas commencer le joueur sans vie
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    IEnumerator DamageDelayTime(float invicibilityTime)
    {
        canTakeDamage = false;
        yield return new WaitForSecondsRealtime(invicibilityTime); // Le temps entre lequel le joueur ne peut pas se faire taper après un coup
        canTakeDamage = true;
    }


    public void GetPushedAway(GameObject pushed /*l'individu poussé*/, GameObject from/*L'individu qui pousse*/ , float repulseForce/*La force du knockbacj*/) 
        // On fait le knockback quand on prend un dégat
    {
        Rigidbody2D pushedRB = pushed.GetComponent<Rigidbody2D>(); // On prend le rigidbody du poussé
        Vector2 direction = pushed.transform.position - from.transform.position; // On regarde la direction dans laquelle le poussé doit etre pousser 
        pushedRB.linearVelocity = Vector2.zero; //On imobilise pour eviter les envols démesurés
        float sens = 1;
        if (direction.x < 0) // On regarde le sens dans lequel le poussé doit etre pousser
        {
            sens = -1; // gauche
        }
        else if(direction.x > 0)
        {
            sens = 1; // droite
        }

        if(direction.x < 1 && direction.x > -1) // dans les airs
        {
            // On le pousse en diagonal , ajusté selon la situation du poussé
            pushedRB.AddForceX( sens * repulseForce * 100);
            pushedRB.AddForceY(direction.y * repulseForce * 100 ); 
        }

        else // au sol
        {
            // On le pousse en diagonal , ajusté selon la situation du poussé
            pushedRB.AddForceX(direction.x * repulseForce * 100);
            pushedRB.AddForceY( repulseForce * 100);
        }
        StartCoroutine(littleStunOnHit()); // On empeche le poussé de bouger quand il est hit pour rendre punitif la prise de dégats
    }

    IEnumerator littleStunOnHit()
    {
        pmouv.canMove = false;
        yield return new WaitForSeconds(0.5f);
        pmouv.canMove = true;
    }
}
