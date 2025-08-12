using System;
using UnityEngine;

public class DropOption : MonoBehaviour
{
    private MeshRenderer objectToDropRenderer;
    private bool isCursorVisible = false;
    [SerializeField] private GameObject objectGrabbedParent;
    private GameObject objectGrabbed;
    private GameObject displayDropObject;

    [SerializeField] private ControlsStateController controlsStateController;

    [SerializeField] private Transform interactableElementsParent;

    public static event Action<Transform> objectDroppedEvent;

    private void OnEnable()
    {
        DropControlsState.clickToDropObjectEvent += DeactivateDropObject;
        CharacterController.tryDropObjectEvent += DropObject;
    }

    private void OnDisable()
    {
        DropControlsState.clickToDropObjectEvent -= DeactivateDropObject;
        CharacterController.tryDropObjectEvent -= DropObject;
    }

    void Update()
    {
        if(isCursorVisible)
        {
            MoveObjectOnMousePosition();
        }
    }

    public void ActivateDropObject()
    {
        objectGrabbed = objectGrabbedParent.transform.GetChild(0).gameObject; // Assuming the grabbed object is the first child
        displayDropObject = Instantiate(objectGrabbed);
        objectToDropRenderer = displayDropObject.GetComponent<MeshRenderer>();

        MoveObjectOnMousePosition();
        isCursorVisible = true;
        objectToDropRenderer.enabled = isCursorVisible; // Show the cursor
        controlsStateController.ChangeState(controlsStateController.dropControlsState); // Switch to MoveControlsState
    }

    public void DeactivateDropObject(RaycastHit raycastHit, bool grab)
    {
        isCursorVisible = false;
        objectToDropRenderer.enabled = isCursorVisible; // Hide the cursor
        controlsStateController.ChangeState(controlsStateController.globalControlsState); // Switch to MoveControlsState
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
                displayDropObject.transform.position = hit.point;
            }
        }
    }

    private void DropObject(RaycastHit raycastHit)
    {
        if (objectGrabbed != null)
        {
            objectDroppedEvent?.Invoke(objectGrabbed.transform);
            objectGrabbed.transform.position = raycastHit.point; // Set the position of the dropped object
            objectGrabbed.transform.SetParent(interactableElementsParent); // Detach the object from the parent
            objectGrabbed = null; // Clear the reference to the dropped object
            isCursorVisible = false;
            Destroy(displayDropObject);
        }
    }
}
