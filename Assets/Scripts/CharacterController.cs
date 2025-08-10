using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ControlOptions))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        InteractionManager.clickToMoveEvent += MoveToPosition;
    }

    private void OnDisable()
    {
        InteractionManager.clickToMoveEvent -= MoveToPosition;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.speed = agent.speed / 8f;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.magnitude != 0f)
        {
            animator.SetBool("isWalking", true);
        }

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetBool("isWalking", false);
                    agent.ResetPath(); // Reset the path to stop the agent
                }
            }
        }
    }

    private void MoveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
