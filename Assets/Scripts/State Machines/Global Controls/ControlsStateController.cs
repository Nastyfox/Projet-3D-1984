using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControlsStateController : MonoBehaviour
{
    public StateControls currentState;

    public GlobalControlsState globalControlsState = new GlobalControlsState();
    public MoveControlsState moveControlsState = new MoveControlsState();
    public GrabControlState grabControlsState = new GrabControlState();
    public FollowControlsState followControlsState = new FollowControlsState();
    public DropControlsState dropControlsState = new DropControlsState();
    public ThrowControlsState throwControlsState = new ThrowControlsState();
    public DisplayControlsState displayControlsState = new DisplayControlsState();

    public GameObject moveCursor;
    public Transform objectGrabbedParent;
    public Transform interactableElementsParent;
    public GameObject displayDropObject;
    public CharacterController characterController;
    public ControlOptions controlOptions;
    public CameraManager cameraManager;
    public ThrowController throwController;

    public List<UnityEvent> buttonsEvents;

    void Start()
    {
        ChangeState(globalControlsState);
        characterController = this.gameObject.GetComponent<CharacterController>();
    }

    public void ChangeState(StateControls newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = newState;
        currentState.OnStateEnter(this);
    }
    public void OnMouseClick(RaycastHit raycastHit)
    {
        currentState.OnStateMouseClick(raycastHit);
    }

    void Update()
    {
        currentState.OnStateUpdate();
    }

    public void OnMoveButtonClicked()
    {
        ChangeState(moveControlsState);
    }

    public void OnGrabButtonClicked()
    {
        ChangeState(grabControlsState);
    }

    public void OnFollowButtonClicked()
    {
        ChangeState(followControlsState);
    }

    public void OnDropButtonClicked()
    {
        ChangeState(dropControlsState);
    }

    public void OnThrowButtonClicked()
    {
        ChangeState(throwControlsState);
    }

    public void DestroyDisplayDropObject()
    {
        if (displayDropObject != null)
        {
            Destroy(displayDropObject);
        }
    }

    public void InstatiateDropObject(Transform objectToDrop)
    {
        if (displayDropObject != null)
        {
            Destroy(displayDropObject);
        }
        displayDropObject = Instantiate(objectToDrop.gameObject, interactableElementsParent);
    }
}