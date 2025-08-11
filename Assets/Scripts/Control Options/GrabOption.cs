using System;
using UnityEngine;
using UnityEngine.AI;

public class GrabOption : MonoBehaviour
{
    [SerializeField] private ControlsStateController controlsStateController;
    [SerializeField] private GameObject objectGrabbedParent;

    public static event Action<Transform> objectGrabbedEvent;


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
            objectGrabbedEvent?.Invoke(objectToGrab);
        }
    }
}
