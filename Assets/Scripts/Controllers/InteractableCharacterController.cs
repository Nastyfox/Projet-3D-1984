using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ControlOptions))]
public class InteractableCharacterController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [SerializeField] private float distanceToGrabObject = 1.0f; // Distance to offset when grabbing an object

    public static event Action destinationReached;

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
                    destinationReached?.Invoke();

                    animator.SetBool("isWalking", false);
                    agent.ResetPath(); // Reset the path to stop the agent
                }
            }
        }
    }

    public void MoveToPosition(RaycastHit raycastHit)
    {
        agent.SetDestination(raycastHit.point);
    }

    public void MoveToObject(RaycastHit raycastHit)
    {
        Vector3 directionToObject = raycastHit.point - transform.position;
        Vector3 positionWithOffset = raycastHit.point - directionToObject.normalized * distanceToGrabObject; // Offset to avoid collision

        agent.SetDestination(positionWithOffset);
    }

    public void FollowMoveCursor(Vector3 pointToLookAt)
    {
        this.transform.LookAt(new Vector3(pointToLookAt.x, this.transform.position.y, pointToLookAt.z));
    }
}
