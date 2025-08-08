using UnityEngine;

[System.Serializable]
public class ChaseState : IState
{
    [SerializeField] private float chaseSpeed = 8;
    [SerializeField] private float loseDistance = 3;

    private Transform myTransform;
    [HideInInspector] private Transform target;

    public void OnEntry(StateController controller)
    {
        myTransform = controller.transform;
    }

    public void OnUpdate(StateController controller)
    {
        if (PlayerLost())
        {
            controller.ChangeState(controller.patrolState);
        }
        else
        {
            Chase();
        }
    }

    public void OnExit(StateController controller)
    {
        // "Must've been the wind"
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Chase()
    {
        myTransform.position = Vector3.MoveTowards(myTransform.position, target.position, chaseSpeed * Time.deltaTime);
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
