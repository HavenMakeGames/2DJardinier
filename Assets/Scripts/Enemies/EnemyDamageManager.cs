using System.Collections;
using UnityEngine;

public class EnemyDamageManager : MonoBehaviour
{
    [Header("Stats")]
    public float Health;
    private float maxHP;
    private float currentHP;
    private bool canTakeDamage;
    public float repulseForce;

    [Header("Components")]
    private GameObject enemy;
    private Rigidbody2D rb;
    private FlyingEnemy fe;
    

    private void Start()
    {
        enemy = gameObject;
        rb = GetComponent<Rigidbody2D>();
        Health = maxHP;
        currentHP = maxHP;
        fe = GetComponent<FlyingEnemy>();
    }

    private void GetHit(float damage)
    {
        currentHP -= damage;
        StartCoroutine(HitCooldown());
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(enemy);
    }

    IEnumerator HitCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(1f);
        canTakeDamage = true;
    }

    public void GetPushedAway(GameObject pushed /*l'individu poussé*/, GameObject from/*L'individu qui pousse*/ , float repulseForce/*La force du knockbacj*/)
    // On fait le knockback quand on prend un dégat
    {
        Rigidbody2D pushedRB = pushed.GetComponent<Rigidbody2D>(); // On prend le rigidbody du poussé
        Vector2 direction = pushed.transform.position - from.transform.position; // On regarde la direction dans laquelle le poussé doit etre pousser 
        pushedRB.linearVelocity = Vector2.zero; //On imobilise pour eviter les envols démesurés
        rb.AddForce(direction.normalized * repulseForce * 100);
        StartCoroutine(Stun());
    }
    IEnumerator Stun()
    {
        fe.canMove = false;
        yield return new WaitForSeconds(1);
        fe.canMove = true;
    }


    }
