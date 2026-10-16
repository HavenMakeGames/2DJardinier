using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // modifie la profondeur pour avoir du recul sur la scène 

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset; // évite que la caméra soit collée au joueur 

        transform.position = Vector3.SmoothDamp( // va progressivement faire glisser la caméra vers le joueur pour rendre fluide le mouvement de la caméra
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
