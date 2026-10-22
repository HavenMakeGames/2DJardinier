using System.Collections;
using UnityEngine;
public class DamagaZone : MonoBehaviour
{
    [Header("Stats")]
    public int damageInflicted;

    [Header("Components")]
    private GameObject zone;
    private Collider2D col;
    private DamageManager dm;
    private PlayerMovement pm;

    private void Start()
    {
        zone = GetComponent<GameObject>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dm = collision.gameObject.GetComponent<DamageManager>();
            pm = collision.gameObject.GetComponent<PlayerMovement>();
            dm.TakeDamage(damageInflicted);
            DamageAnimation();
            pm.MoveToLastAvailablePos();
        }

        else if (collision.gameObject.CompareTag("Enemy"))
        {
            dm = collision.gameObject.GetComponent<DamageManager>();
            dm.Die();
        }
    }

    IEnumerator DamageAnimation()
    {
        yield return new WaitForSeconds(1);
    }
}
