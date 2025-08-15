using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ChaseState : StateNPC
{
    private Transform target;

    [SerializeField] private float chaseSpeed;

    protected override void OnEntry()
    {
        agent.speed = chaseSpeed;
        animator.speed = agent.speed / 8f;
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    protected override void OnUpdate()
    {
        if (!npcStateController.patrolState.FindVisibleTargets())
        {
            npcStateController.ChangeState(npcStateController.patrolState);
        }
        else
        {
            Chase();
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Chase()
    {
        agent.SetDestination(target.position);
    }
}
