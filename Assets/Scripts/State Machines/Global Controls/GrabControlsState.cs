using System;
using UnityEngine;

[Serializable]

public class GrabControlState : StateControls
{
    private Transform objectToGrab;

    protected override void OnExit()
    {
        CharacterController.destinationReached -= GrabObject;
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        characterController.MoveToObject(raycastHit);
        CharacterController.destinationReached += GrabObject;

        objectToGrab = raycastHit.transform;
    }

    private void GrabObject()
    {
        if (objectToGrab.tag == "InteractableElement")
        {
            objectToGrab.SetParent(objectGrabbedParent);
            objectToGrab.transform.localPosition = Vector3.zero;
            objectToGrab.transform.localRotation = Quaternion.identity;
            objectToGrab.GetComponent<Rigidbody>().isKinematic = true;
            controlOptions.ChangeStateGrabbedObjectOptions();
        }

        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }
}