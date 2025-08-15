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
    
    public GameObject displayDropObject;
    public Transform interactableElementsParent;

    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ControlOptions controlOptions;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform objectGrabbedParent;

    void Start()
    {
        ChangeState(globalControlsState);
    }

    public void ChangeState(StateControls newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = newState;
        currentState.OnStateEnter(this, cameraManager, controlOptions, characterController, objectGrabbedParent);
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