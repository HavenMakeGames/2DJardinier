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


    [Header("Components")]
    public GameObject character;


    private void Start()
    {
        character = gameObject;
        SetMaxHealth(health);  // On met max vie le perso (joueur ou npc)
    }

    public void OnCollisionEnter2D(Collision2D collision) // Quand l'individu rentre en contact avec un individu qui peut lui mettre des dégats , il prend des dégats
    {
        if (collision.collider.tag == "Enemy" && character.tag == "Player")
        {
            TakeDamage();
        }
        else if (collision.collider.tag == "Player" && character.tag == "Enemy")
        {
            TakeDamage();
        }
    }
    


    public void OnTriggerEnter2D(Collider2D collision) // Quand le joueur touche un point de spawn , il l'enregistre
    {
        if (collision.gameObject.tag == "Spawn Point")
        {
            if (character.tag== "Player")
            {
                latestPoint = collision.gameObject;
                Debug.Log("Nouveau point de réapparition " + latestPoint.name);
            }
        }
    }


    public void CanTakeDamage() // Utilise la coroutine pour que le joueur soit intangible pendant ce temps
    {
        canTakeDamage = false;
        StartCoroutine(DamageDelayTime(invicibilityTime));
        canTakeDamage = true;
    }





    private void TakeDamage()
    {
        if (canTakeDamage == true) // Si le joueur n'est pas intangible 
        {
            health = health - 10f; // Retire la vie du joueur 
            Debug.Log("Vous avez " + health);
            if (health <= 0) // Si la vie du joueur arrive en dessous ou à 0 
            {
                Die(); // Meurs  
            }
            healthSlider.value = health;
            CanTakeDamage(); // Execute l'intangebilite 

        }
    }


    public void Die()
    {
        if (latestPoint != null)
        {
            character.transform.position = latestPoint.transform.position; // Teleporte le joueur au dernier point de réapparition
        }
        else if (latestPoint == null) // Si le character n'est pas le joueur , il ne peut donc pas avoir de point de spawn donc il meure sans réapparaitre
        {
            Destroy(character.transform);
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
        yield return new WaitForSecondsRealtime(invicibilityTime); // Le temps entre lequel le joueur ne peut pas se faire taper après un coup
    }


}
