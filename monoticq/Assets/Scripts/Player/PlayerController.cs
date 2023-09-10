using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Takes and handles input and movement for a player character
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    public Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if(canMove) {
            // If movement input is not 0, try to move
            if(movementInput != Vector2.zero)
            {
                
                bool success = TryMove(movementInput);

                int lastX;
                int lastY;

                if(!success) 
                {
                    lastY = (int) movementInput.y;
                    success = TryMove(new Vector2(movementInput.x, lastY));
                }

                if(!success) 
                {
                    lastX = (int) movementInput.x;
                    success = TryMove(new Vector2(lastX, movementInput.y));
                }

                if (movementInput.y > 0)
                {
                    animator.SetBool("isGoingVertical", success);
                    animator.SetBool("isMoving", false);
                    animator.SetBool("isGoingDown", false);
                }
                else if (movementInput.y < 0)
                {
                    animator.SetBool("isGoingDown", success);
                    animator.SetBool("isGoingVertical", false);
                    animator.SetBool("isMoving", false);
                }
                else
                {
                    animator.SetBool("isMoving", success);
                    animator.SetBool("isGoingVertical", false);
                    animator.SetBool("isGoingDown", false);
                }

            } 


            // Set direction of sprite to movement direction
            if(movementInput.x < 0) 
            {
                spriteRenderer.flipX = true;
            } 
            else if (movementInput.x > 0) 
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction) {
        if(direction != Vector2.zero) {
            // Check for potential collisions
            int count = rb.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

            if(count == 0){
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
        } else {
            // Can't move if there's no direction to move in
            return false;
        }
        
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
}
