using UnityEngine;
using UnityEngine.AI;

public abstract class StateNPC
{
    protected NPCStateController npcStateController;
    protected Transform npcTransform;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

    public void OnStateEnter(NPCStateController controller)
    {
        npcStateController = controller;
        npcTransform = npcStateController.transform;
        OnEntry();
    }

    protected virtual void OnEntry()
    {

    }

    public void OnStateUpdate()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {

    }

    public void OnStateLateUpdate()
    {
        OnLateUpdate();
    }

    protected virtual void OnLateUpdate()
    {

    }

    public void OnStateExit()
    {
        OnExit();
    }

    protected virtual void OnExit()
    {

    }
}
