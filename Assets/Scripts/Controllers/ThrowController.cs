using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowController : MonoBehaviour
{
    private Scene simulationScene;
    private PhysicsScene physicsScene;
    [SerializeField] private List<Transform> elementsToCopy;

    private GameObject ghostObject;
    private Rigidbody ghostObjectRb;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int maxPhysicsFrameIterations;

    [SerializeField] private GameObject objectGrabbedParent;
    private GameObject objectGrabbed;
    private Rigidbody objectGrabbedRb;
    private GameObject displayThrowObject;
    private MeshRenderer objectToThrowRenderer;

    [SerializeField] private Transform interactableElementsParent;

    private Dictionary<Transform, Transform> spawnedObjects = new Dictionary<Transform, Transform>();

    private Vector3 positionBeforeThrow;
    private Quaternion rotationBeforeThrow;

    private Vector3 mousePosition;
    private float gravity;

    [SerializeField] private float maxHeightThrow;

    [SerializeField] private float maxDistanceThrow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer.material.SetFloat("_TilingAmount", maxPhysicsFrameIterations / 2f);
        gravity = Physics.gravity.y;
        CreatePhysicsScene();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    void CreatePhysicsScene()
    {
        if (SceneManager.GetSceneByName("SimulationScene").IsValid())
        {
            simulationScene = SceneManager.GetSceneByName("SimulationScene");
            physicsScene = simulationScene.GetPhysicsScene();
        }
        else
        {
            simulationScene = SceneManager.CreateScene("SimulationScene", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            physicsScene = simulationScene.GetPhysicsScene();
        }

        foreach (Transform element in elementsToCopy)
        {
            GameObject ghostObject = InstantiateGhostObject(element.gameObject, element.position, element.rotation);
            if (!ghostObject.isStatic)
            {
                spawnedObjects.Add(element, ghostObject.transform);
            }
        }
    }

    public void ActivateThrowTrajectory()
    {
        objectGrabbed = objectGrabbedParent.transform.GetChild(0).gameObject;
        objectGrabbedRb = objectGrabbed.GetComponent<Rigidbody>();
        displayThrowObject = Instantiate(objectGrabbed);
        objectToThrowRenderer = displayThrowObject.GetComponent<MeshRenderer>();

        ghostObject = InstantiateGhostObject(objectGrabbed, objectGrabbed.transform.position, objectGrabbed.transform.rotation);
        ghostObjectRb = ghostObject.GetComponent<Rigidbody>();
        ghostObjectRb.isKinematic = false;

        positionBeforeThrow = objectGrabbed.transform.position;
        rotationBeforeThrow = objectGrabbed.transform.rotation;

        lineRenderer.enabled = true;
        objectToThrowRenderer.enabled = true;
    }

    public void SimulateTrajectory()
    {
        ghostObjectRb.linearVelocity = Vector3.zero;
        ghostObject.transform.position = positionBeforeThrow;
        ghostObject.transform.rotation = rotationBeforeThrow;

        ghostObjectRb.linearVelocity = CalculateLaunchVelocity();

        lineRenderer.positionCount = maxPhysicsFrameIterations;

        for(int i = 0; i < maxPhysicsFrameIterations; i++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, ghostObject.transform.position);
        }

        displayThrowObject.transform.position = ghostObject.transform.position;
        displayThrowObject.transform.rotation = ghostObject.transform.rotation;
    }

    private GameObject InstantiateGhostObject(GameObject objectToThrow, Vector3 pos, Quaternion rotation)
    {
        var ghostObj = Instantiate(objectToThrow, pos, rotation);
        if(ghostObj.GetComponent<Renderer>())
        {
            ghostObj.GetComponent<Renderer>().enabled = false;
        }
        SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);
        
        return ghostObj;
    }

    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mousePosition = hit.point;
        }

        return mousePosition;
    }

    private Vector3 CalculateLaunchVelocity()
    {
        float displacementY = mousePosition.y - positionBeforeThrow.y;

        float displacementX = Mathf.Clamp(mousePosition.x - positionBeforeThrow.x, -maxDistanceThrow, maxDistanceThrow);
        float displacementZ = Mathf.Clamp(mousePosition.z - positionBeforeThrow.z, -maxDistanceThrow, maxDistanceThrow);
        Vector3 displacementXZ = new Vector3(displacementX, 0, displacementZ);

        float time = Mathf.Sqrt(-2 * maxHeightThrow / gravity) + Mathf.Sqrt(2 * (displacementY - maxHeightThrow) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * maxHeightThrow);
        Vector3 velocityXZ = displacementXZ / time;

        return velocityXZ + velocityY;
    }

    public void LaunchObject()
    {
        lineRenderer.enabled = false;
        objectToThrowRenderer.enabled = false;

        objectGrabbedRb.isKinematic = false;
        objectGrabbedRb.linearVelocity = CalculateLaunchVelocity();

        objectGrabbed.transform.SetParent(interactableElementsParent);
        objectGrabbed = null;
        Destroy(displayThrowObject);
        Destroy(ghostObject);
    }
}
