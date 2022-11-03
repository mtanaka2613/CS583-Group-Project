using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController: MonoBehaviour
{
    //input fields
    private ThirdPersonActionAsset playerActionsAsset;
    private InputAction move;

    //movement fields
    private Rigidbody rb;
    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private float maxSpeed = 5f; //Player Movement Speed
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private Camera playerCamera;
    private Animator animator;

    //Colliders
    private BoxCollider[] childColliders;
    private CapsuleCollider playerCollider;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerActionsAsset = new ThirdPersonActionAsset(); 
        animator = gameObject.GetComponent<Animator>();
        childColliders = GetComponentsInChildren<BoxCollider>(true); //Array of Colliders in Child Objects of Player Object
        playerCollider = GetComponent<CapsuleCollider>(); //Player Capsule Collider
        childColliders[0].enabled = false; //Shield Box Collider
        childColliders[1].enabled = false; //Sword Box Collider


    }

    private void OnEnable()
    {
        playerActionsAsset.Player.Jump.started += DoJump;
        playerActionsAsset.Player.Attack.started += DoAttack;
        playerActionsAsset.Player.Block.started += DoBlock;
        move = playerActionsAsset.Player.Move;
        playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Player.Jump.started -= DoJump;
        playerActionsAsset.Player.Attack.started -= DoAttack;
        playerActionsAsset.Player.Block.started -= DoBlock;
        playerActionsAsset.Player.Disable();
    }

    private void FixedUpdate()
    {
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;  

        //adds increased velocity when falling after jumping
        if (rb.velocity.y < 0f)
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
        }
        
        //restricts the max velocity the player can move
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

        LookAt();
    }


    //Controls the direction the player is looking
    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            rb.rotation = Quaternion.LookRotation(direction,Vector3.up);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if (isGrounded())
        {
            forceDirection += Vector3.up * jumpForce;
            // animator trigger for jump?
        }
    }

    private bool isGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        animator.SetTrigger("attack");
    }

    private void OnMouseDown()
    {
        childColliders[1].enabled = true; //Enables Sword Collider
    }

    private void OnMouseUp()
    {
        childColliders[1].enabled = false; //Disables Sword Collider
    }

    private void DoBlock(InputAction.CallbackContext obj)
    {
        //if not blocking, do blocking animation
        if (!animator.GetBool("block"))
        {
            animator.SetBool("block", true);
            childColliders[0].enabled = true;
            return;
        }
        
        //if blocking, undo blocking animation
        if (animator.GetBool("block"))
        {
            animator.SetBool("block", false);
            childColliders[0].enabled = false;
            return;
        }
    }

   //Player Collides with another Collider
    private void OnCollisionEnter(Collision collision)
    {
        //Player Collides with a powerup
        if (collision.collider.CompareTag("Powerup"))
        {
            Debug.Log("Picked up Powerup");
            Destroy(collision.collider.gameObject); //Deletes the object
            DeterminePowerupType(collision.collider.gameObject.name); //Helper Function
        }
    }

    //Help Determines the Powerup Type
    private void DeterminePowerupType(String PowerupName)
    {
        //Calls Speed Powerup
        if (PowerupName.Contains("Speed"))
        {
            StartCoroutine(SpeedPowerup());
        }

    }

    //Speed Powerup
    IEnumerator SpeedPowerup()
    {
        maxSpeed = 1000f; //Speed boost
        yield return new WaitForSecondsRealtime(10f); //Timer active for 10 seconds
        maxSpeed = 5f; //Revert back to normal speed
    }
}