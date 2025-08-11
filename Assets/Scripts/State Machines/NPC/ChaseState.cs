using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ChaseState : IStateNPC
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private Transform myTransform;
    [HideInInspector] private Transform target;

    [SerializeField] private float loseDistance;

    public void OnEntry(NPCStateController controller)
    {
        Debug.Log("Chase State: Entering chase state");
        agent.speed *= 2f; // Increase speed for chase
        animator.speed = agent.speed / 8f;
        myTransform = controller.transform;
        agent.SetDestination(target.position);
    }

    public void OnUpdate(NPCStateController controller)
    {
        if (!controller.patrolState.LookForCharacter())
        {
            controller.ChangeState(controller.patrolState);
        }
        else
        {
            Chase();
        }
    }

    public void OnExit(NPCStateController controller)
    {
        Debug.Log("Chase State: Exiting chase state");
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Chase()
    {
        agent.SetDestination(target.position);
    }

    bool PlayerLost()
    {
        if (!target)
        {
            return true;
        }

        if (Vector3.Distance(myTransform.position, target.position) > loseDistance)
        {
            return true;
        }

        return false;
    }
}
