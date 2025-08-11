using UnityEngine;

public class NPCStateController : MonoBehaviour
{
    public IStateNPC currentState;

    public IdleState idleState = new IdleState();
    public PatrolState patrolState = new PatrolState();
    public ChaseState chaseState = new ChaseState();

    void Start()
    {
        ChangeState(patrolState);
    }

    public void ChangeState(IStateNPC newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = newState;
        currentState.OnEntry(this);
    }

    void Update()
    {
        currentState.OnUpdate(this);
    }
}