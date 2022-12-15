using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent EnemyNav;
    private Animator animator;
    private Rigidbody rb;
    private AudioSource footStepsSound;

    public Transform Player;

    public float distanceBetween;
    public float sightRange = 20f;
    public float attackRange = 2.5f;

    public float timeBetweenAttacks = 2f;
    bool alreadyAttacked;

    //Health
    public int maxHealth = 15;
    public int currentHealth;

    private void Awake()
    {
        EnemyNav= GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        footStepsSound = GetComponent<AudioSource>();
        currentHealth = maxHealth; //Set health
    }

    private void Update()
    {
        distanceBetween = Vector3.Distance(Player.position, transform.position);

        if (distanceBetween <= sightRange & currentHealth != 0)
        {

            FollowPlayer();
        }

        if (distanceBetween <= attackRange & currentHealth != 0 )
        {

            Attack();
        }

        if (currentHealth == 0) // check if health is not zero
        {
            //Invoke the Destory gameobject after death animation
            //animator.SetTrigger("Death");
            //alreadyAttacked = true;
            //Invoke(nameof(DestoryEnemey), 5f);
            DestoryEnemey();
            
        }

        animator.SetFloat("speed", EnemyNav.velocity.magnitude / EnemyNav.speed);

        if (Time.timeScale == 0f)
        {
            footStepsSound.enabled = false;
        }
    }

    private void DestoryEnemey()
    {
        Destroy(this.gameObject);
    }

    private void FollowPlayer()
    {
        footStepsSound.enabled = true;
        EnemyNav.SetDestination(Player.position);
    }

    private void Attack()
    {
        footStepsSound.enabled = false;
        EnemyNav.SetDestination(transform.position);

        transform.LookAt(Player);

        if (!alreadyAttacked)
        {
            animator.SetTrigger("Attack");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
            
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Sword"))
        {
            TakeDamage(5);
        }
    }

    public void TakeDamage(int attackDamage)
    {
        currentHealth -= attackDamage;
    }

}
