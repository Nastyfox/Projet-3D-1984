using System.Collections.Generic;
using UnityEngine;

public class ControlOptions : MonoBehaviour
{
    [SerializeField] private List<GameObject> controlOptions;
    [SerializeField] private List<GameObject> grabbedObjectControlOptions;
    [SerializeField] private Canvas canvasControlOptions;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private ControlsStateController controlsStateController;

    private void OnEnable()
    {
        GlobalControlsState.clickOnElementEvent += DisplayControlOptions;
        GrabOption.objectGrabbedEvent += DisplayGrabbedObjectOptions;
        ThrowOption.objectThrownEvent += DisplayGrabbedObjectOptions;
        DropOption.objectDroppedEvent += DisplayGrabbedObjectOptions;
        MoveControlsState.clickToMoveEvent += HideControlOptions;
        GrabControlState.clickToGrabEvent += HideControlOptions;
        DropControlsState.clickToDropObjectEvent += HideControlOptions;
        FollowControlsState.followEvent += HideControlOptions;
        ThrowControlsState.clickToThrowObjectEvent += HideControlOptions;
        DisplayControlsState.clickToCloseControls += HideControlOptions;
    }

    private void OnDisable()
    {
        GlobalControlsState.clickOnElementEvent -= DisplayControlOptions;
        GrabOption.objectGrabbedEvent -= DisplayGrabbedObjectOptions;
        ThrowOption.objectThrownEvent -= DisplayGrabbedObjectOptions;
        DropOption.objectDroppedEvent -= DisplayGrabbedObjectOptions;
        MoveControlsState.clickToMoveEvent -= HideControlOptions;
        GrabControlState.clickToGrabEvent -= HideControlOptions;
        DropControlsState.clickToDropObjectEvent -= HideControlOptions;
        FollowControlsState.followEvent -= HideControlOptions;
        ThrowControlsState.clickToThrowObjectEvent -= HideControlOptions;
        DisplayControlsState.clickToCloseControls -= HideControlOptions;
    }

    private void LateUpdate()
    {
        canvasControlOptions.transform.LookAt(canvasControlOptions.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    public void DisplayControlOptions(RaycastHit raycastHit)
    {
        if (raycastHit.transform != this.transform)
            return;

        foreach (GameObject controlOption in controlOptions)
        {
            if (controlOption != null)
            {
                canvasControlOptions.transform.SetParent(this.transform);
                canvasControlOptions.transform.localPosition = Vector3.zero;
                controlOption.SetActive(true);
            }
        }

        controlsStateController.ChangeState(controlsStateController.displayControlsState); // Switch to GlobalControlsState
    }

    private void HideControlOptions(RaycastHit raycastHit, bool grab)
    {
        foreach (GameObject controlOption in controlOptions)
        {
            if (controlOption != null)
            {
                controlOption.SetActive(false);
            }
        }

        controlsStateController.ChangeState(controlsStateController.globalControlsState); // Switch to GlobalControlsState
    }

    private void DisplayGrabbedObjectOptions(Transform grabbedObject)
    {
        foreach (GameObject grabbedObjectControlOption in grabbedObjectControlOptions)
        {
            if (grabbedObjectControlOption != null)
            {
                grabbedObjectControlOption.SetActive(!grabbedObjectControlOption.activeSelf); //Invert the active state of the control options for grabbed objects
            }
        }
    }
}
