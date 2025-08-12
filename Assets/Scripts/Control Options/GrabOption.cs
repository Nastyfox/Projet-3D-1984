using System;
using UnityEngine;

public class GrabOption : MonoBehaviour
{
    [SerializeField] private ControlsStateController controlsStateController;
    [SerializeField] private GameObject objectGrabbedParent;

    public static event Action<Transform> objectGrabbedEvent;

    [SerializeField] private GameObject character;


    private void OnEnable()
    {
        CharacterController.tryGrabObjectEvent += GrabObject;
    }

    private void OnDisable()
    {
        CharacterController.tryGrabObjectEvent -= GrabObject;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateGrabControls()
    {
        controlsStateController.ChangeState(controlsStateController.grabControlState); // Switch to MoveControlsState
    }

    private void GrabObject(RaycastHit raycastHit)
    {
        Transform objectToGrab = raycastHit.transform;

        if (objectToGrab.tag == "InteractableElement")
        {
            objectToGrab.SetParent(objectGrabbedParent.transform);
            objectToGrab.transform.localPosition = Vector3.zero;
            objectToGrab.transform.localRotation = Quaternion.identity;
            objectToGrab.GetComponent<Rigidbody>().isKinematic = true; // Make the object kinematic to prevent physics interactions while grabbed
            objectGrabbedEvent?.Invoke(objectToGrab);
        }
    }
}
