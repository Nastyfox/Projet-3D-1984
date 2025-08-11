using System.Collections.Generic;
using UnityEngine;

public class ControlOptions : MonoBehaviour
{
    [SerializeField] private List<GameObject> controlOptions;
    [SerializeField] private List<GameObject> grabbedObjectControlOptions;
    [SerializeField] private Canvas canvasControlOptions;
    [SerializeField] private Camera mainCamera;

    private void OnEnable()
    {
        GlobalControlsState.clickOnElementEvent += DisplayControlOptions;
        MoveControlsState.clickToMoveEvent += HideControlOptions;
        GrabControlState.clickToGrabEvent += HideControlOptions;
        DropControlsState.clickToDropObjectEvent += HideControlOptions;
        FollowControlsState.followEvent += HideControlOptions;
        GrabOption.objectGrabbedEvent += DisplayGrabbedObjectOptions;
    }

    private void OnDisable()
    {
        GlobalControlsState.clickOnElementEvent -= DisplayControlOptions;
        MoveControlsState.clickToMoveEvent -= HideControlOptions;
        GrabControlState.clickToGrabEvent -= HideControlOptions;
        DropControlsState.clickToDropObjectEvent -= HideControlOptions;
        FollowControlsState.followEvent -= HideControlOptions;
        GrabOption.objectGrabbedEvent -= DisplayGrabbedObjectOptions;
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
