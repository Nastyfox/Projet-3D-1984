using System.Collections.Generic;
using UnityEngine;

public class FieldOfView3D : MonoBehaviour
{
    private GameObject viewGameObject;
    private float maxHorizontalAngleDetection;
    private float maxVerticalAngleDetection;
    [SerializeField] private float meshResolution;
    private Mesh viewMesh;
    [SerializeField] private MeshFilter viewMeshFilter;
    [SerializeField] float edgeResolveIterations;
    [SerializeField] float edgeDistanceThreshold;
    private float lookForDistance;
    private LayerMask obstacleMask;
    private LayerMask groundMask;
    

    public struct ViewCastInfo3D
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float horizontalAngle;
        public float verticalAngle;

        public ViewCastInfo3D(bool _hit, Vector3 _point, float _distance, float _horizontalAngle, float _verticalAngle)
        {
            point = _point;
            distance = _distance;
            horizontalAngle = _horizontalAngle;
            verticalAngle = _verticalAngle;
            hit = _hit;
        }
    }

    public struct EdgeInfo3D
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo3D(Vector3 _pointA, Vector3 _pointB)
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
    
    public void SetViewParameters(GameObject viewObject, float horizontalAngleDetection, float verticalAngleDetection, float lookDistance, LayerMask obstacles, LayerMask ground)
    {
        viewGameObject = viewObject;
        maxHorizontalAngleDetection = horizontalAngleDetection;
        maxVerticalAngleDetection = verticalAngleDetection;
        lookForDistance = lookDistance;
        obstacleMask = obstacles;
        groundMask = ground;

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }


    void DrawFieldOfView()
    {
        int horizontalStepCount = Mathf.RoundToInt(maxHorizontalAngleDetection * meshResolution);
        float horizontalStepAngleSize = maxHorizontalAngleDetection / horizontalStepCount;
        int verticalStepCount = Mathf.RoundToInt(maxVerticalAngleDetection * meshResolution);
        float verticalStepAngleSize = maxVerticalAngleDetection / verticalStepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo3D oldViewCast = new ViewCastInfo3D();
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i <= verticalStepCount; i++)
        {
            float verticalAngle = viewGameObject.transform.eulerAngles.x - maxVerticalAngleDetection / 2f + verticalStepAngleSize * i;
            
            for(int j = 0; j <= horizontalStepCount; j++)
            {
                float horizontalAngle = viewGameObject.transform.eulerAngles.y - maxHorizontalAngleDetection / 2f + horizontalStepAngleSize * j;
                ViewCastInfo3D newViewCast = ViewCast(horizontalAngle, verticalAngle);

                if (i > 0 && j > 0)
                {
                    bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                    {
                        EdgeInfo3D edge = FindEdge(oldViewCast, newViewCast, verticalAngle);
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
                vertices.Add(viewGameObject.transform.InverseTransformPoint(newViewCast.point));
                oldViewCast = newViewCast;
            }
        }

        int vertexCount = viewPoints.Count + 1;
        List<int> triangles = new List<int>();

        vertices[0] = Vector3.zero;

        for (int v = 0; v < verticalStepCount - 1; v++)
        {
            for (int h = 0; h < horizontalStepCount - 1; h++)
            {
                // Current quad vertices
                int bottomLeft = 1 + v * (horizontalStepCount + 1) + h;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + (horizontalStepCount + 1);
                int topRight = topLeft + 1;

                // Create triangles for the quad (remove the redundant if condition)
                triangles.Add(bottomLeft);
                triangles.Add(topLeft);
                triangles.Add(bottomRight);

                triangles.Add(bottomRight);
                triangles.Add(topLeft);
                triangles.Add(topRight);

                // Connect to center for edge triangles
                if (v == 0) // Bottom edge
                {
                    triangles.Add(0);
                    triangles.Add(bottomLeft);
                    triangles.Add(bottomRight);
                }

                if (v == verticalStepCount - 2) // Top edge (changed from -1 to -2)
                {
                    triangles.Add(0);
                    triangles.Add(topRight);
                    triangles.Add(topLeft);
                }

                if (h == 0) // Left edge
                {
                    triangles.Add(0);
                    triangles.Add(topLeft);
                    triangles.Add(bottomLeft);
                }

                if (h == horizontalStepCount - 2) // Right edge (changed from -1 to -2)
                {
                    triangles.Add(0);
                    triangles.Add(bottomRight);
                    triangles.Add(topRight);
                }
            }
        }


        viewMesh.Clear();
        viewMesh.vertices = vertices.ToArray();
        viewMesh.triangles = triangles.ToArray();
        viewMesh.RecalculateNormals();
    }

    private Vector3 DirFromAngles(float horizontalAngleInDegrees, float verticalAngleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            horizontalAngleInDegrees += viewGameObject.transform.eulerAngles.y;
            verticalAngleInDegrees += viewGameObject.transform.eulerAngles.x;
        }
        return new Vector3(Mathf.Sin(horizontalAngleInDegrees * Mathf.Deg2Rad) * Mathf.Cos(verticalAngleInDegrees * Mathf.Deg2Rad),
                           Mathf.Sin(verticalAngleInDegrees * Mathf.Deg2Rad),
                           Mathf.Cos(horizontalAngleInDegrees * Mathf.Deg2Rad) * Mathf.Cos(verticalAngleInDegrees * Mathf.Deg2Rad));
    }

    ViewCastInfo3D ViewCast(float globalHorizontalAngle, float globalVerticalAngle)
    {
        Vector3 direction = DirFromAngles(globalHorizontalAngle, globalVerticalAngle, true);

        RaycastHit raycastHit;

        if (Physics.Raycast(viewGameObject.transform.position, direction, out raycastHit, lookForDistance, obstacleMask))
        {
            RaycastHit groundRaycastHit;
            Vector3 hitPoint = raycastHit.point;

            if (Physics.Raycast(viewGameObject.transform.position, direction, out groundRaycastHit, lookForDistance, groundMask))
            {
                float groundHeight = groundRaycastHit.point.y;
                Vector3 clampedPoint = raycastHit.point;

                // If hit point is below ground, clamp it to ground level
                if (clampedPoint.y < groundHeight)
                {
                    clampedPoint.y = groundHeight;
                    float newDistance = Vector3.Distance(viewGameObject.transform.position, clampedPoint);
                    return new ViewCastInfo3D(true, clampedPoint, newDistance, globalHorizontalAngle, globalVerticalAngle);
                }
            }

            return new ViewCastInfo3D(true, raycastHit.point, raycastHit.distance, globalHorizontalAngle, globalVerticalAngle);
        }
        else
        {
            RaycastHit groundRaycastHit;
            Vector3 defaultPoint = viewGameObject.transform.position + direction * lookForDistance;

            if (Physics.Raycast(viewGameObject.transform.position, direction, out groundRaycastHit, lookForDistance, groundMask))
            {
                // Clamp to ground height if point would be below ground
                defaultPoint.y = Mathf.Max(defaultPoint.y, groundRaycastHit.point.y);
            }

            float finalDistance = Vector3.Distance(viewGameObject.transform.position, defaultPoint);
            return new ViewCastInfo3D(false, defaultPoint, finalDistance, globalHorizontalAngle, globalVerticalAngle);
        }
    }

    EdgeInfo3D FindEdge(ViewCastInfo3D minViewCast, ViewCastInfo3D maxViewCast, float verticalAngle)
    {
        float minAngle = minViewCast.horizontalAngle;
        float maxAngle = maxViewCast.horizontalAngle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo3D newViewCast = ViewCast(angle, verticalAngle);

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
        return new EdgeInfo3D(minPoint, maxPoint);
    }
}
