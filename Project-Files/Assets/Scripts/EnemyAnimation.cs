using UnityEngine;

usingSystem.Collections;

 

public class EnemyAnimation : MonoBehaviour {

 

         // Use this for initialization

    private Transform player;

    public float attackDistance = 2;//This is the distance to attack the target and also the target distance for pathfinding

    private Animator animator;

    public float speed;

    private CharacterController cc;

    public float attackTime = 3;   //Set the timer time to attack once in 3 seconds

    private float attackCounter = 0; //timer variable

         voidStart () {

        player = GameObject.FindGameObjectWithTag("Player").transform;

        cc = this.GetComponent<CharacterController>();

        animator = this.GetComponent<Animator>();//Control the running action call in the animation state machine

        attackCounter = attackTime;//At the beginning, as long as it reaches the target, it will attack immediately

         }

        

         // Update is called once per frame

         voidUpdate () {

        

        Vector3 targetPos =player.position;

        targetPos.y = transform.position.y;//The role here is to assign its own Y-axis value to the target Y value

        transform.LookAt(targetPos);//When rotating, ensure that the Y axis is the axis of rotation

        float distance = Vector3.Distance(targetPos,transform.position);

        if (distance <= attackDistance)

        {

            attackCounter += Time.deltaTime;

            if (attackCounter >attackTime)//Realization of timer function

            {

                int num = Random.Range(0, 2);//There are two types of attack animations, here we use random numbers ([0], [1]) to switch between the two animations

                if (num == 0)animator.SetTrigger("Attack01");

                else animator.SetTrigger("Attack02");

 

                attackCounter = 0;

            }

            else

            {

                animator.SetBool("Walk", false);

 

            }

        }

        else

        {

            attackCounter = attackTime;//Attacks immediately every time it moves to the minimum attack distance

            if(animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyWalk"))//EnemyRun the state of the walk in the animation state machine

            cc.SimpleMove(transform.forward*speed);

            animator.SetBool("Walk",true);//Play running animation while moving

        }

         }

}

