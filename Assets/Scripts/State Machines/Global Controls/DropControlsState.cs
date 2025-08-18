using System;
using UnityEngine;

[Serializable]

public class DropControlsState : CharacterStateControls
{
    private Transform objectToDrop;

    private Vector3 dropPosition;

    protected override void OnEntry()
    {
        objectToDrop = objectGrabbedParent.GetChild(0);
        controlsStateController.InstatiateDropObject(objectToDrop);
        MoveObjectOnMousePosition();
    }

    protected override void OnUpdate()
    {
        MoveObjectOnMousePosition();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        characterController.MoveToPosition(raycastHit);
        dropPosition = raycastHit.point;
        InteractableCharacterController.destinationReached += DropObject;
    }

    private void DropObject()
    {
        if (objectToDrop != null)
        {
            objectToDrop.transform.position = dropPosition;
            objectToDrop.transform.SetParent(controlsStateController.interactableElementsParent.transform);
            objectToDrop = null;
            controlsStateController.DestroyDisplayDropObject();
            controlOptions.ChangeStateGrabbedObjectOptions();
            controlsStateController.ChangeState(controlsStateController.globalControlsState);
        }
    }

    private void MoveObjectOnMousePosition()
    {
        //Display move cursor on ground where mouse is pointing
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit))
        {
            // Assuming the ground is tagged as "Ground"
            if (raycastHit.transform.CompareTag("Ground"))
            {
                // Set the position of the move cursor to the hit point
                controlsStateController.displayDropObject.transform.position = raycastHit.point;
            }
        }
    }
}