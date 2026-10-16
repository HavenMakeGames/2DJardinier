using UnityEngine;

public class Grass : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" ||  collision.tag == "Enemy")
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb.linearVelocity != Vector2.zero)
            {
                animator.SetTrigger("Move");
            }
        }
    }
}