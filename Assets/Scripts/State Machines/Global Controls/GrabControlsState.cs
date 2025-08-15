using System;
using UnityEngine;

public class GrabControlState : StateControls
{
    private Transform objectToGrab;

    protected override void OnExit()
    {
        CharacterController.destinationReached -= GrabObject;
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        controlsStateController.characterController.MoveToObject(raycastHit);
        CharacterController.destinationReached += GrabObject;

        objectToGrab = raycastHit.transform;
    }

    private void GrabObject()
    {
        if (objectToGrab.tag == "InteractableElement")
        {
            objectToGrab.SetParent(controlsStateController.objectGrabbedParent);
            objectToGrab.transform.localPosition = Vector3.zero;
            objectToGrab.transform.localRotation = Quaternion.identity;
            objectToGrab.GetComponent<Rigidbody>().isKinematic = true;
            controlsStateController.controlOptions.ChangeStateGrabbedObjectOptions();
        }

        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }
}