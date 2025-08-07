using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ControlOptions))]
public class NPCController : MonoBehaviour
{
    [SerializeField] List<Vector3> movementPositions = new List<Vector3>();
    private Vector3 nextPosition;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private ControlOptions controlOptions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextPosition = movementPositions[0];
        animator.speed = agent.speed / 8f;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(nextPosition);

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
                    int nextIndex = Random.Range(0, movementPositions.Count);
                    nextPosition = movementPositions[nextIndex];
                }
            }
        }
    }
}
