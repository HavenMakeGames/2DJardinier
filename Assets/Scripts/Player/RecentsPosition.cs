using UnityEngine;
using System.Collections.Generic;
public class RecentsPosition : MonoBehaviour
{
    [Header("Stats")]
    public float intervalle;
    private float timer;
    [HideInInspector]public Stack<Vector2> recentPos = new Stack<Vector2>();

    [Header("Components")]
    private GameObject character;
    private PlayerMovement pm;

    private void Start()
    {
        character = gameObject;
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Timer();
        RegisterPos(recentPos);
    }

    private void RegisterPos(Stack<Vector2> pos)
    {
        if (Timer() == true && pm.isGrounded)
        {
            pos.Push(character.transform.position);
        }

    }

    public Vector2 GetLastPos()
    {
        return recentPos.Pop();
    }

    private bool Timer()
    {
        timer += Time.deltaTime;
        if (timer >= intervalle)
        {
            timer = 0;
            return true;
        }
        return false;
    }
}
