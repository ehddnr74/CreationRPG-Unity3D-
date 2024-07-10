using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Transform player;
    public float stopDistance = 3.5f; // 플레이어 앞에서 멈출 거리
    public float attackRange = 3.5f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    float timer;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        agent.speed = 2.0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        agent.SetDestination(player.position);
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > 15f)
        {
            animator.SetBool("isChasing", false);
        }
        else if (distance > stopDistance)
        {
            Vector3 direction = (player.position - animator.transform.position).normalized;
            Vector3 stopPosition = player.position - direction * stopDistance;
            agent.SetDestination(stopPosition);
        }
        if(distance <= attackRange)
        {
            if (timer > 1f)
            {
                animator.SetBool("isChasing", false);
                agent.isStopped = true;
                animator.SetTrigger("Attack"); // Attack 트리거 설정
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
