using System;
using UnityEngine;

public class DropControlsState : StateControls
{
    private Transform objectToDrop;

    private Vector3 dropPosition;

    protected override void OnEntry()
    {
        objectToDrop = controlsStateController.objectGrabbedParent.GetChild(0);
        controlsStateController.InstatiateDropObject(objectToDrop);
        MoveObjectOnMousePosition();
    }

    protected override void OnUpdate()
    {
        MoveObjectOnMousePosition();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        controlsStateController.characterController.MoveToPosition(raycastHit);
        dropPosition = raycastHit.point;
        CharacterController.destinationReached += DropObject;
    }

    private void DropObject()
    {
        if (objectToDrop != null)
        {
            objectToDrop.transform.position = dropPosition;
            objectToDrop.transform.SetParent(controlsStateController.interactableElementsParent.transform);
            objectToDrop = null;
            controlsStateController.DestroyDisplayDropObject();
            controlsStateController.controlOptions.ChangeStateGrabbedObjectOptions();
            controlsStateController.ChangeState(controlsStateController.globalControlsState);
        }
    }

    private void MoveObjectOnMousePosition()
    {
        //Display move cursor on ground where mouse is pointing
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Assuming the ground is tagged as "Ground"
            if (hit.transform.CompareTag("Ground"))
            {
                // Set the position of the move cursor to the hit point
                controlsStateController.displayDropObject.transform.position = hit.point;
            }
        }
    }
}