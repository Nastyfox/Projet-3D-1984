using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatrolState : IState
{
    [SerializeField] private float patrolSpeed = 5;
    [SerializeField] private float lookForDistance = 3;
    [SerializeField] private int waypoint;
    [SerializeField] private List<Transform> waypoints;

    private Transform myTransform;
    private RaycastHit hitInfo;

    public void OnEntry(StateController controller)
    {
        myTransform = controller.transform;
    }

    public void OnUpdate(StateController controller)
    {
        Patrol();
        if (LookForPlayer())
        {
            controller.chaseState.SetTarget(hitInfo.transform);
            controller.ChangeState(controller.chaseState);
        }
    }

    public void OnExit(StateController controller)
    {
        // This will be called when first entering the state
    }

    bool LookForPlayer()
    {
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hitInfo, lookForDistance))
        {
            if (hitInfo.collider.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }

    void Patrol()
    {
        if (myTransform.position != waypoints[waypoint].position)
        {
            myTransform.position = Vector3.MoveTowards(myTransform.position, waypoints[waypoint].position, patrolSpeed * Time.deltaTime);
        }
        else
        {
            waypoint++;
            if (waypoint >= waypoints.Count)
            {
                waypoint = 0;
            }
        }
    }
}