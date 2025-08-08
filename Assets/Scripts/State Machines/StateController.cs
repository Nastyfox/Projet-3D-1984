using UnityEngine;

public class StateController : MonoBehaviour
{
    public IState currentState;

    public IdleState idleState = new IdleState();
    public PatrolState patrolState = new PatrolState();
    public ChaseState chaseState = new ChaseState();

    void Start()
    {
        ChangeState(idleState);
    }

    public void ChangeState(IState newState)
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