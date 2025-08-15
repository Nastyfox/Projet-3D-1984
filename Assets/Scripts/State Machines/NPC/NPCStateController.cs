using UnityEngine;

public class NPCStateController : MonoBehaviour
{
    public StateNPC currentState;

    public IdleState idleState = new IdleState();
    public PatrolState patrolState = new PatrolState();
    public ChaseState chaseState = new ChaseState();

    void Start()
    {
        ChangeState(patrolState);
    }

    public void ChangeState(StateNPC newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = newState;
        currentState.OnStateEnter(this);
    }

    void Update()
    {
        currentState.OnStateUpdate();
    }

    void LateUpdate()
    {
        currentState.OnStateLateUpdate();
    }
}