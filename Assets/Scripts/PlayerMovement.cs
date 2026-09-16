using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Statistiques")]
    public float speed = 5;
    public float jumpForce = 5;
    public float gravity = 5;
    public bool isGrounded = true;
    private Vector2 direction;


    [Header("Actions")]
    InputAction move;
    InputAction jump;


    

    [Header("Components")]
    public Rigidbody2D rb;
    private Collider2D col;
    private GameObject player;

    [Header("Prefabs")]
    private string eviteBug;


    private void Start()
    {
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");



        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        player = GetComponent<GameObject>();



    }

    private void Update()
    {


        Vector2 moveValue = move.ReadValue<Vector2>();
        bool jumpValue = jump.IsPressed();

        MovePlayer(moveValue);

        JumpPlayer(jumpValue);


    }

    private void MovePlayer(Vector2 direction)
    {
        rb.linearVelocity = new Vector2(direction.x * speed , rb.linearVelocity.y);
    }

    private bool GetIsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
        
    }
    public void JumpPlayer(bool jumpValue) 
    {
        
        if ( jumpValue == true && GetIsGrounded())
        {
            isGrounded = false;
            rb.AddRelativeForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        }
    }

   


}
