using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

[System.Serializable]
public class PatrolState : IState
{
    [SerializeField] List<Vector3> movementPositions = new List<Vector3>();
    private Vector3 nextPosition;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [SerializeField] private float lookForDistance;
    [SerializeField] private float maxAngleDetection;

    private Transform myTransform;
    private Transform targetDetected;

    private bool characterDetected;

    public void OnEntry(StateController controller)
    {
        animator.speed = agent.speed / 8f;
        myTransform = controller.transform;
        nextPosition = movementPositions[1];
        characterDetected = false;
    }

    public void OnUpdate(StateController controller)
    {
        Patrol();
        if (LookForCharacter() && !characterDetected)
        {
            controller.chaseState.SetTarget(targetDetected);
            controller.ChangeState(controller.chaseState);
            characterDetected = true;
        }
    }

    public void OnExit(StateController controller)
    {
        // This will be called when first entering the state
    }

    public bool LookForCharacter()
    {
        Vector3 startPositionRaycast = myTransform.position + Vector3.up * 0.5f;
        Vector3 raycastDirection = myTransform.forward;

        Collider[] hitColliders = Physics.OverlapSphere(startPositionRaycast, lookForDistance);

        for(int i = 0; i < hitColliders.Length; i++)
        {
            Vector3 hitPoint = hitColliders[i].transform.position;
            Vector3 directionToHit = hitPoint - startPositionRaycast;
            float angleToHit = Vector3.Angle(raycastDirection, directionToHit);

            if(angleToHit < maxAngleDetection)
            {
                if (hitColliders[i].tag == "ControllableCharacter")
                {
                    targetDetected = hitColliders[i].transform;
                    return true;
                }
            }
        }

        return false;
    }

    void Patrol()
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