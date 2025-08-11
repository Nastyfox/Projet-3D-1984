using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ControlOptions))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [SerializeField] private float distanceToGrabObject = 1.0f; // Distance to offset when grabbing an object
    private RaycastHit raycastHit;
    private bool tryGrabObject = false;
    private bool tryDropObject = false;
    public static event Action<RaycastHit> tryGrabObjectEvent;
    public static event Action<RaycastHit> tryDropObjectEvent;
    [SerializeField] private ControlsStateController controlsStateController;


    private void OnEnable()
    {
        MoveControlsState.clickToMoveEvent += MoveToPosition;
        GrabControlState.clickToGrabEvent += MoveToObject;
        DropControlsState.clickToDropObjectEvent += MoveToObject;
    }

    private void OnDisable()
    {
        MoveControlsState.clickToMoveEvent -= MoveToPosition;
        GrabControlState.clickToGrabEvent -= MoveToObject;
        DropControlsState.clickToDropObjectEvent -= MoveToObject;
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
                    if(tryGrabObject)
                    {
                        tryGrabObjectEvent?.Invoke(raycastHit);
                        tryGrabObject = false; // Reset the flag after trying to grab the object
                    }

                    if(tryDropObject)
                    {
                        tryDropObjectEvent?.Invoke(raycastHit); // Notify that the character is trying to drop an object
                        tryDropObject = false;
                    }
                    animator.SetBool("isWalking", false);
                    agent.ResetPath(); // Reset the path to stop the agent
                }
            }
        }
    }

    private void MoveToPosition(RaycastHit raycastHit, bool grab)
    {
        agent.SetDestination(raycastHit.point);
    }

    private void MoveToObject(RaycastHit raycastHit, bool grab)
    {
        Vector3 directionToObject = raycastHit.point - transform.position;
        Vector3 positionWithOffset = raycastHit.point - directionToObject.normalized * distanceToGrabObject; // Offset to avoid collision
        this.raycastHit = raycastHit;

        if(grab)
        {
            tryGrabObject = true;
        }
        else
        {
            tryDropObject = true;
        }

        agent.SetDestination(positionWithOffset);

        controlsStateController.ChangeState(controlsStateController.globalControlsState); // Switch back to GlobalControlsState
    }
}
