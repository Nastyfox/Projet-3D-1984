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
    [SerializeField] private float maxAngleDetection;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private GameObject viewGameObject;

    [SerializeField] private float meshResolution;
    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    [SerializeField] float edgeResolveIterations;
    [SerializeField] float edgeDistanceThreshold;

    private List<Transform> visibleTargets = new List<Transform>();

    private Transform targetDetected;

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            point = _point;
            distance = _distance;
            angle = _angle;
            hit = _hit;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    protected override void OnEntry()
    {
        agent.speed = patrolSpeed;
        animator.speed = agent.speed / 8f;
        nextPosition = movementPositions[1];
        agent.SetDestination(nextPosition);

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    protected override void OnUpdate()
    {
        Patrol();
        if (FindVisibleTargets())
        {
            npcStateController.chaseState.SetTarget(targetDetected);
            npcStateController.ChangeState(npcStateController.chaseState);
        }
    }

    protected override void OnLateUpdate()
    {
        DrawFieldOfView();
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

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += viewGameObject.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public bool FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(viewGameObject.transform.position, lookForDistance, targetMask);

        float minDistanceToTarget = float.MaxValue;

        targetDetected = null;

        foreach (Collider hitCollider in hitColliders)
        {
            targetDetected = hitCollider.transform;

            Vector3 directionToTarget = (hitCollider.transform.position - viewGameObject.transform.position).normalized;
            float angleToTarget = Vector3.Angle(viewGameObject.transform.forward, directionToTarget);

            if (angleToTarget < maxAngleDetection / 2f)
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

        if (targetDetected != null)
        {
            return true;
        }

        return false;
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(maxAngleDetection * meshResolution);
        float stepAngleSize = maxAngleDetection / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = viewGameObject.transform.eulerAngles.y - maxAngleDetection / 2f + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if(i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = viewGameObject.transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirFromAngle(globalAngle, true);

        RaycastHit raycastHit;

        if (Physics.Raycast(viewGameObject.transform.position, direction, out raycastHit, lookForDistance, obstacleMask))
        {
            return new ViewCastInfo(true, raycastHit.point, raycastHit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, viewGameObject.transform.position + direction * lookForDistance, lookForDistance, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }
}