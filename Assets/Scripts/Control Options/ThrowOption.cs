using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowOption : MonoBehaviour
{
    private Scene simulationScene;
    private PhysicsScene physicsScene;
    [SerializeField] private List<Transform> elementsToCopy;
    private GameObject ghostObject;
    private Rigidbody ghostObjectRb;

    [SerializeField] private LineRenderer line;
    [SerializeField] private int maxPhysicsFrameIterations;

    [SerializeField] private GameObject objectGrabbedParent;
    private GameObject objectGrabbed;
    private GameObject displayThrowObject;
    private MeshRenderer objectToThrowRenderer;

    [SerializeField] private ControlsStateController controlsStateController;

    [SerializeField] private float throwForce;

    [SerializeField] private Transform interactableElementsParent;

    private bool isTrajectoryVisible = false;

    public static event Action<Transform> objectThrownEvent;

    [SerializeField] private GameObject character;

    private Dictionary<Transform, Transform> spawnedObjects = new Dictionary<Transform, Transform>();

    [SerializeField] private LineRenderer lineRenderer;

    private Vector3 throwVelocity;
    private Vector3 positionBeforeThrow;
    private Quaternion rotationBeforeThrow;

    private void OnEnable()
    {
        ThrowControlsState.clickToThrowObjectEvent += DeactivateThrowTrajectory;
    }

    private void OnDisable()
    {
        ThrowControlsState.clickToThrowObjectEvent -= DeactivateThrowTrajectory;
    }

    void CreatePhysicsScene()
    {
        simulationScene = SceneManager.CreateScene("SimulationScene", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        foreach (Transform element in elementsToCopy)
        {
            GameObject ghostObject = InstantiateGhostObject(element.gameObject, element.position, element.rotation);
            if (!ghostObject.isStatic)
            {
                spawnedObjects.Add(element, ghostObject.transform);
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreatePhysicsScene();
    }

    // Update is called once per frame
    void Update()
    {
        if(isTrajectoryVisible)
        {
            throwVelocity = character.transform.forward * throwForce;
            SimulateTrajectory(throwVelocity);
        }

        foreach (var item in spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    public void ActivateThrowTrajectory()
    {
        objectGrabbed = objectGrabbedParent.transform.GetChild(0).gameObject; // Assuming the grabbed object is the first child
        displayThrowObject = Instantiate(objectGrabbed);
        objectToThrowRenderer = displayThrowObject.GetComponent<MeshRenderer>();

        
        ghostObject = InstantiateGhostObject(objectGrabbed, objectGrabbed.transform.position, objectGrabbed.transform.rotation);
        ghostObjectRb = ghostObject.GetComponent<Rigidbody>();
        ghostObjectRb.isKinematic = false;

        positionBeforeThrow = objectGrabbed.transform.position; // Store the position before throwing
        rotationBeforeThrow = objectGrabbed.transform.rotation; // Store the rotation before throwing

        isTrajectoryVisible = true;
        lineRenderer.enabled = isTrajectoryVisible; // Enable the line renderer for trajectory visualization
        objectToThrowRenderer.enabled = isTrajectoryVisible; // Show the cursor
        controlsStateController.ChangeState(controlsStateController.throwControlsState); // Switch to MoveControlsState
    }

    private void DeactivateThrowTrajectory(RaycastHit raycastHit, bool grab)
    {
        isTrajectoryVisible = false;
        objectToThrowRenderer.enabled = isTrajectoryVisible; // Hide the cursor
        controlsStateController.ChangeState(controlsStateController.globalControlsState); // Switch to MoveControlsState

        objectGrabbed.GetComponent<Rigidbody>().isKinematic = false; // Make the object non-kinematic to allow physics interactions after throwing

        ThrowObject(objectGrabbed, throwVelocity);
        
        objectThrownEvent?.Invoke(objectGrabbed.transform);
        objectGrabbed.transform.SetParent(interactableElementsParent); // Detach the object from the parent
        objectGrabbed = null; // Clear the reference to the dropped object
        isTrajectoryVisible = false;
        lineRenderer.enabled = isTrajectoryVisible; // Disable the line renderer after throwing
        Destroy(displayThrowObject);
        Destroy(ghostObject); // Clean up the ghost object after throwing
    }

    public void SimulateTrajectory(Vector3 velocity)
    {
        ghostObjectRb.linearVelocity = Vector3.zero; // Reset the velocity of the ghost object

        ThrowObject(ghostObject, velocity);

        line.positionCount = maxPhysicsFrameIterations;

        for(int i = 0; i < maxPhysicsFrameIterations; i++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            line.SetPosition(i, ghostObject.transform.position);
        }

        displayThrowObject.transform.position = ghostObject.transform.position;
        displayThrowObject.transform.rotation = ghostObject.transform.rotation;
    }

    private GameObject InstantiateGhostObject(GameObject objectToThrow, Vector3 pos, Quaternion rotation)
    {
        var ghostObj = Instantiate(objectToThrow, pos, rotation);
        if(ghostObj.GetComponent<Renderer>())
        {
            ghostObj.GetComponent<Renderer>().enabled = false; // Disable rendering for ghost objects
        }
        SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);
        
        return ghostObj;
    }

    private void ThrowObject(GameObject objectToThrow, Vector3 velocity)
    {
        objectToThrow.transform.position = positionBeforeThrow;
        objectToThrow.transform.rotation = rotationBeforeThrow;
        objectToThrow.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);    
    }
}
