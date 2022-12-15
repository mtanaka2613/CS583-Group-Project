using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.TextCore.Text;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    private float maxSpeed = 5.0f; //Player Movement Speed

    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private Camera playerCamera;
    private Animator animator;

    //Colliders
    public BoxCollider[] childColliders;
    private CapsuleCollider playerCollider;

    //Health
    public int maxHealth = 10;
    public int currentHealth;
    public HealthBar healthBar;

    //reference to SceneController
    public SceneController sc;

    //counts the current amount PowerGems collected in a level
    int powerGemsCount = 0;

    
    public AudioSource footstepsSound;
   
    public AudioSource swingSound;
    
    public AudioSource collectedGem;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerActionsAsset = new ThirdPersonActionAsset(); 
        animator = gameObject.GetComponent<Animator>();
        childColliders = GetComponentsInChildren<BoxCollider>(true); //Array of Colliders in Child Objects of Player Object
        playerCollider = GetComponent<CapsuleCollider>(); //Player Capsule Collider
        childColliders[0].enabled = false; //Shield Box Collider
        childColliders[1].enabled = false; //Sword Box Collider

        currentHealth = maxHealth; //Set health
        healthBar.SetMaxHealth(currentHealth);

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

        // if the player is moving and grounded play footsteps
        if (rb.velocity.magnitude > 0.5 && isGrounded()) 
        {
            footstepsSound.enabled = true;
        }
        else
        {
            footstepsSound.enabled = false;
        }

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

        // check if player's health is not zero
        if (currentHealth == 0)
        {
            SceneManager.LoadScene(10);
        }
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
        if (Time.timeScale != 0 ) //checks if the game is paused
        {
            childColliders[1].enabled = true; // Enables Sword Collider
            animator.SetTrigger("attack");
            swingSound.Play(); //plays swing sound
        }
    }

    //private void OnMouseDown()
    //{
    //    childColliders[1].enabled = true; //Enables Sword Collider
    //}

    private void OnMouseUp()
    {
        childColliders[1].enabled = false; //Disables Sword Collider
    }

    private void DoBlock(InputAction.CallbackContext obj)
    {
        //if not blocking, do blocking animation
        if (!animator.GetBool("block") & Time.timeScale != 0)
        {
            animator.SetBool("block", true);
            childColliders[0].enabled = true;
            return;
        }
        
        //if blocking, undo blocking animation
        if (animator.GetBool("block") & Time.timeScale != 0)
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
            Debug.Log("Speed Powerup Active");
            StartCoroutine(SpeedPowerup());
        }

        //Calls Big Sword Powerup
        if (PowerupName.Contains("Big Sword"))
        {
            Debug.Log("Big Sword Powerup Active");
            StartCoroutine(BigSwordPowerup());
        }

        //Calls Mega Jump Powerup
        if (PowerupName.Contains("Mega Jump"))
        {
            Debug.Log("Mega Jump Powerup Active");
            StartCoroutine(MegaJumpPowerup());
        }
    }

    IEnumerator BigSwordPowerup()
    {
        Vector3 temp = new Vector3(childColliders[1].size.x, childColliders[1].size.y, childColliders[1].size.z); //Store original value of sword collider size
        childColliders[1].size = temp * 2; //Doubles size of sword collider
        yield return new WaitForSecondsRealtime(10f);
        childColliders[1].size = temp; //Revert back to normal sword collider size
        Debug.Log("Big Sword Powerup Expired");
    }

    IEnumerator MegaJumpPowerup()
    {
        float temp = jumpForce; //Store original value of jump force
        jumpForce = 15f; //Jump height boost
        yield return new WaitForSecondsRealtime(5.0f);
        jumpForce = temp; //Revert back to normal jump force
        Debug.Log("Mega Jump Powerup Expired");
    }

    //Speed Powerup
    IEnumerator SpeedPowerup()
    {
        maxSpeed = 1000f; //Speed boost
        yield return new WaitForSecondsRealtime(10f); //Timer active for 10 seconds
        maxSpeed = 5.0f; //Revert back to normal speed
        Debug.Log("Speed Powerup Expired");
    }

    public void TakeDamage(int attackDamage)
    {
        //TODO: Check if player is blocking
        // create a bool variable that is set to true when blocking
        currentHealth -= attackDamage;
        healthBar.UpdateHealth(currentHealth); // updates the healthbar to the current health after taking damage
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerGem"))
        {
            collectedGem.Play();
            Destroy(other.gameObject);
            // play gem collected audio clip
            
            powerGemsCount += 1;

            // Check if player completed all levels
            if (GameManager.Instance.hasCompletedAllLevels() & powerGemsCount == 5)
            {
                SceneManager.LoadScene(9); //End Screen
                Cursor.lockState = CursorLockMode.Confined;
            }
            else if (powerGemsCount == 5) 
            {
                SceneManager.LoadScene(8); //Level cleared screen
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                sc.UpdateItemsText(powerGemsCount);
            }
        }
    }
}