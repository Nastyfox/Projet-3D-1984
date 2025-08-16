using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FieldOfView2D : MonoBehaviour
{
    private GameObject viewGameObject;
    private float maxAngleDetection;
    [SerializeField] private float meshResolution;
    private Mesh viewMesh;
    [SerializeField] private MeshFilter viewMeshFilter;
    [SerializeField] float edgeResolveIterations;
    [SerializeField] float edgeDistanceThreshold;
    private float lookForDistance;
    private LayerMask obstacleMask;
    private LayerMask groundMask;

    public struct ViewCastInfo2D
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float horizontalAngle;

        public ViewCastInfo2D(bool _hit, Vector3 _point, float _distance, float _horizontalAngle)
        {
            point = _point;
            distance = _distance;
            horizontalAngle = _horizontalAngle;
            hit = _hit;
        }
    }

    public struct EdgeInfo2D
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo2D(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }
    
    public void SetViewParameters(GameObject viewObject, float angleDetection, float lookDistance, LayerMask obstacles, LayerMask ground)
    {
        viewGameObject = viewObject;
        maxAngleDetection = angleDetection;
        lookForDistance = lookDistance;
        obstacleMask = obstacles;
        groundMask = ground;

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += viewGameObject.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(maxAngleDetection * meshResolution);
        float stepAngleSize = maxAngleDetection / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo2D oldViewCast = new ViewCastInfo2D();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = viewGameObject.transform.eulerAngles.y - maxAngleDetection / 2f + stepAngleSize * i;
            ViewCastInfo2D newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo2D edge = FindEdge(oldViewCast, newViewCast);
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

        RaycastHit groundRaycastHit;
        Vector3 startPosition = Vector3.zero;

        if (Physics.Raycast(viewGameObject.transform.position, Vector3.down, out groundRaycastHit, Mathf.Infinity, groundMask))
        {
            startPosition = new Vector3(viewGameObject.transform.position.x, groundRaycastHit.point.y + 0.01f, viewGameObject.transform.position.z);
        }

        vertices[0] = viewGameObject.transform.InverseTransformPoint(startPosition);
        for (int i = 0; i < vertexCount - 1; i++)
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

    ViewCastInfo2D ViewCast(float globalAngle)
    {
        Vector3 direction = DirFromAngle(globalAngle, true);

        RaycastHit obstacleRaycastHit;

        if (Physics.Raycast(viewGameObject.transform.position, direction, out obstacleRaycastHit, lookForDistance, obstacleMask))
        {
            Vector3 hitPoint = obstacleRaycastHit.point;

            RaycastHit groundRaycastHit;

            if (Physics.Raycast(hitPoint, Vector3.down, out groundRaycastHit, Mathf.Infinity, groundMask))
            {
                hitPoint.y = groundRaycastHit.point.y;
            }

            float newDistance = Vector3.Distance(viewGameObject.transform.position, hitPoint);
            return new ViewCastInfo2D(true, hitPoint, newDistance, globalAngle);
        }
        else
        {
            Vector3 point = viewGameObject.transform.position + direction * lookForDistance;
            RaycastHit groundRaycastHit;

            if (Physics.Raycast(point, Vector3.down, out groundRaycastHit, Mathf.Infinity, groundMask))
            {
                point.y = groundRaycastHit.point.y;
            }

            float finalDistance = Vector3.Distance(viewGameObject.transform.position, point);
            return new ViewCastInfo2D(false, point, finalDistance, globalAngle);
        }
    }

    EdgeInfo2D FindEdge(ViewCastInfo2D minViewCast, ViewCastInfo2D maxViewCast)
    {
        float minAngle = minViewCast.horizontalAngle;
        float maxAngle = maxViewCast.horizontalAngle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo2D newViewCast = ViewCast(angle);

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
        return new EdgeInfo2D(minPoint, maxPoint);
    }
}
