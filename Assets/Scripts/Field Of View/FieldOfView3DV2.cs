using System.Collections.Generic;
using UnityEngine;

public class FieldOfView3DV2 : MonoBehaviour
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
    

    public struct ViewCastInfo3DV2
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float horizontalAngle;

        public ViewCastInfo3DV2(bool _hit, Vector3 _point, float _distance, float _horizontalAngle)
        {
            point = _point;
            distance = _distance;
            horizontalAngle = _horizontalAngle;
            hit = _hit;
        }
    }

    public struct EdgeInfo3DV2
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo3DV2(Vector3 _pointA, Vector3 _pointB)
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


    void DrawFieldOfViewV2()
    {
        int horizontalStepCount = Mathf.RoundToInt(maxHorizontalAngleDetection * meshResolution);
        float horizontalStepAngleSize = maxHorizontalAngleDetection / horizontalStepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        List<Vector3> vertices = new List<Vector3>();

        // Store bottom and top viewcasts separately for comparison
        List<ViewCastInfo3DV2> bottomViewCasts = new List<ViewCastInfo3DV2>();
        List<ViewCastInfo3DV2> topViewCasts = new List<ViewCastInfo3DV2>();

        // Generate viewcasts for both vertical levels
        for (int i = -1; i <= 1; i += 2)
        {
            float verticalAngle = maxVerticalAngleDetection * Mathf.Sign(i);
            ViewCastInfo3DV2 oldViewCast = new ViewCastInfo3DV2();

            for (int j = 0; j <= horizontalStepCount; j++)
            {
                float horizontalAngle = viewGameObject.transform.eulerAngles.y - maxHorizontalAngleDetection / 2f + horizontalStepAngleSize * j;
                ViewCastInfo3DV2 newViewCast = ViewCast(horizontalAngle, verticalAngle);

                // Edge detection logic (keep existing code for this part)
                if (i > 0 && j > 0)
                {
                    bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                    {
                        EdgeInfo3DV2 edge = FindEdge(oldViewCast, newViewCast, verticalAngle);
                        if (edge.pointA != Vector3.zero) viewPoints.Add(edge.pointA);
                        if (edge.pointB != Vector3.zero) viewPoints.Add(edge.pointB);
                    }
                }

                // Store viewcasts separately
                if (i < 0) // Bottom level
                    bottomViewCasts.Add(newViewCast);
                else // Top level
                    topViewCasts.Add(newViewCast);

                oldViewCast = newViewCast;
            }
        }

        // Build vertices with XZ clamping logic
        for (int k = 0; k < bottomViewCasts.Count; k++)
        {
            var bottomVC = bottomViewCasts[k];
            var topVC = topViewCasts[k];

            // Add bottom vertex as usual
            viewPoints.Add(bottomVC.point);
            vertices.Add(viewGameObject.transform.InverseTransformPoint(bottomVC.point));

            // Clamp top vertex XZ to bottom vertex XZ if bottom hits but top doesn't
            Vector3 topPoint = topVC.point;
            if (bottomVC.hit && !topVC.hit)
            {
                // Keep the same X and Z as the bottom hit, but preserve the Y of the top point
                topPoint = new Vector3(bottomVC.point.x, topPoint.y, bottomVC.point.z);
            }

            viewPoints.Add(topPoint);
            vertices.Add(viewGameObject.transform.InverseTransformPoint(topPoint));
        }

        // Triangle generation (adjusted for paired vertex structure)
        List<int> triangles = new List<int>();
        vertices.Insert(0, Vector3.zero);

        for (int h = 0; h < horizontalStepCount; h++)
        {
            // Vertex indices adjusted for bottom-top pairs
            int bottomLeft = 1 + h * 2;
            int topLeft = bottomLeft + 1;
            int bottomRight = bottomLeft + 2;
            int topRight = topLeft + 2;

            // Skip if we don't have enough vertices for a complete quad
            if (topRight >= vertices.Count) continue;

            // Create triangles for the quad
            triangles.Add(bottomLeft);
            triangles.Add(topLeft);
            triangles.Add(bottomRight);

            triangles.Add(bottomRight);
            triangles.Add(topLeft);
            triangles.Add(topRight);

            // Edge triangles to center
            if (h == 0) // Left edge
            {
                triangles.Add(0);
                triangles.Add(bottomLeft);
                triangles.Add(topLeft);
            }
            if (h == horizontalStepCount - 1) // Right edge
            {
                triangles.Add(0);
                triangles.Add(topRight);
                triangles.Add(bottomRight);
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices.ToArray();
        viewMesh.triangles = triangles.ToArray();
        viewMesh.RecalculateNormals();
    }

    void DrawFieldOfView()
    {
        int horizontalStepCount = Mathf.RoundToInt(maxHorizontalAngleDetection * meshResolution);
        float horizontalStepAngleSize = maxHorizontalAngleDetection / horizontalStepCount;
        int verticalStepCount = Mathf.RoundToInt(maxVerticalAngleDetection * meshResolution);
        float verticalStepAngleSize = maxVerticalAngleDetection / verticalStepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo3DV2 oldViewCast = new ViewCastInfo3DV2();
        List<Vector3> vertices = new List<Vector3>();

        for (int i = -1; i <= 1; i = i+2)
        {
            float verticalAngle = maxVerticalAngleDetection * Mathf.Sign(i);
            
            for(int j = 0; j <= horizontalStepCount; j++)
            {
                float horizontalAngle = viewGameObject.transform.eulerAngles.y - maxHorizontalAngleDetection / 2f + horizontalStepAngleSize * j;
                ViewCastInfo3DV2 newViewCast = ViewCast(horizontalAngle, verticalAngle);

                if (i > 0 && j > 0)
                {
                    bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                    {
                        EdgeInfo3DV2 edge = FindEdge(oldViewCast, newViewCast, verticalAngle);
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

        for (int h = 0; h < horizontalStepCount - 1; h++)
        {
            // Current quad vertices
            int bottomLeft = 1 + h;
            int bottomRight = bottomLeft + 1;
            int topLeft = bottomLeft + (horizontalStepCount + 1);
            int topRight = topLeft + 1;

            // Create triangles for the quad (remove the redundant if condition)
            triangles.Add(0);
            triangles.Add(bottomLeft);
            triangles.Add(bottomRight);

            triangles.Add(0);
            triangles.Add(topLeft);
            triangles.Add(topRight);

            if(h == 0)
            {
                triangles.Add(0);
                triangles.Add(bottomLeft);
                triangles.Add(topLeft);
            }

            if(h == horizontalStepCount - 2)
            {
                triangles.Add(0);
                triangles.Add(bottomRight);
                triangles.Add(topRight);
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

    ViewCastInfo3DV2 ViewCast(float globalHorizontalAngle, float globalVerticalAngle)
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
                    return new ViewCastInfo3DV2(true, clampedPoint, newDistance, globalHorizontalAngle);
                }
            }

            return new ViewCastInfo3DV2(true, raycastHit.point, raycastHit.distance, globalHorizontalAngle);
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
            return new ViewCastInfo3DV2(false, defaultPoint, finalDistance, globalHorizontalAngle);
        }
    }

    EdgeInfo3DV2 FindEdge(ViewCastInfo3DV2 minViewCast, ViewCastInfo3DV2 maxViewCast, float verticalAngle)
    {
        float minAngle = minViewCast.horizontalAngle;
        float maxAngle = maxViewCast.horizontalAngle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo3DV2 newViewCast = ViewCast(angle, verticalAngle);

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
        return new EdgeInfo3DV2(minPoint, maxPoint);
    }
}
