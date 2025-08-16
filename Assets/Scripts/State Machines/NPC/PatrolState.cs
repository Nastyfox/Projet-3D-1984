using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatrolState : StateNPC
{
    [SerializeField] List<Vector3> movementPositions = new List<Vector3>();
    private Vector3 nextPosition;

    [SerializeField] private float patrolSpeed;

    [SerializeField] private float lookForDistance;
    [Range(0, 360)]
    [SerializeField] private float maxHorizontalAngleDetection;
    [Range(0, 360)]
    [SerializeField] private float maxVerticalAngleDetection;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private GameObject viewGameObject;

    private List<Transform> visibleTargets = new List<Transform>();

    private Transform targetDetected;

    [SerializeField] private FieldOfView3D fieldOfView3D;
    [SerializeField] private FieldOfView2D fieldOfView2D;

    protected override void OnEntry()
    {
        agent.speed = patrolSpeed;
        animator.speed = agent.speed / 8f;
        nextPosition = movementPositions[1];
        //agent.SetDestination(nextPosition);

        if(fieldOfView3D.enabled)
        {
            fieldOfView3D.SetViewParameters(viewGameObject, maxHorizontalAngleDetection, maxVerticalAngleDetection, lookForDistance, obstacleMask, groundMask);

        }
        if(fieldOfView2D.enabled)
        {
            fieldOfView2D.SetViewParameters(viewGameObject, maxHorizontalAngleDetection, lookForDistance, obstacleMask, groundMask);
        }
    }

    protected override void OnUpdate()
    {
        //Patrol();
        if (FindVisibleTargets())
        {
            npcStateController.chaseState.SetTarget(targetDetected);
            npcStateController.ChangeState(npcStateController.chaseState);
        }
    }

    void Patrol()
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
                    int nextIndex = Random.Range(0, movementPositions.Count);
                    nextPosition = movementPositions[nextIndex];
                    agent.SetDestination(nextPosition);
                }
            }
        }
    }

    public bool FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(viewGameObject.transform.position, lookForDistance, targetMask);

        float minDistanceToTarget = float.MaxValue;

        targetDetected = null;

        foreach (Collider hitCollider in hitColliders)
        {
            Vector3 directionToTarget = (hitCollider.transform.position - viewGameObject.transform.position).normalized;

            Vector3 directionToTargetHorizontal = new Vector3(directionToTarget.x, 0f, directionToTarget.z).normalized;
            float horizontalAngleToTarget = Vector3.Angle(viewGameObject.transform.forward, directionToTargetHorizontal); 
            float verticalAngleToTarget = Vector3.Angle(directionToTarget, directionToTargetHorizontal);

            bool horizontalInView = horizontalAngleToTarget < maxHorizontalAngleDetection / 2f;
            bool verticalInView = Mathf.Abs(verticalAngleToTarget) < maxVerticalAngleDetection / 2f;

            if (horizontalInView && verticalInView)
            {
                float distanceToTarget = Vector3.Distance(viewGameObject.transform.position, hitCollider.transform.position);

                if (!Physics.Raycast(viewGameObject.transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(targetDetected);
                    if (distanceToTarget < minDistanceToTarget)
                    {
                        minDistanceToTarget = distanceToTarget;
                        targetDetected = hitCollider.transform;
                    }
                }
            }
        }

        return targetDetected != null;
    }
}