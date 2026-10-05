using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float bouncePower;
    private Collision2D collision;
    private bool boucing;
    private Rigidbody2D rb;

    private void FixedUpdate()
    {
        if (boucing)// regarde si le joueur rebondit 
        {
            rb.linearVelocity = Vector3.zero;// immobilise l'individu
            rb.AddForce(new Vector2(0f, bouncePower * 100 )); // fais rebondir l'individu
            boucing = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (!boucing) // regarde si il ne rebondit pas déja 
        {
            boucing = true;
            rb = coll.gameObject.GetComponent<Rigidbody2D>(); // récupere le rigidBody de l'individu qui rentre en contact avec le champignon 
        }
    }

}
