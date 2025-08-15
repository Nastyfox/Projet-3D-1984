using UnityEngine;

public abstract class StateControls
{
    protected ControlsStateController controlsStateController;
    protected CameraManager cameraManager;
    protected ControlOptions controlOptions;
    protected CharacterController characterController;
    protected Transform objectGrabbedParent;

    public void OnStateEnter(ControlsStateController controller, CameraManager camManager, ControlOptions ctrlOptions, CharacterController charaController, Transform objGrabParent)
    {
        controlsStateController = controller;
        cameraManager = camManager;
        controlOptions = ctrlOptions;
        characterController = charaController;
        objectGrabbedParent = objGrabParent;
        OnEntry();
    }

    protected virtual void OnEntry()
    {

    }

    public void OnStateUpdate()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {

    }

    public void OnStateExit()
    {
        OnExit();
    }

    protected virtual void OnExit()
    {

    }

    public void OnStateMouseClick(RaycastHit raycastHit)
    {
        OnMouseClick(raycastHit);
    }

    protected virtual void OnMouseClick(RaycastHit raycastHit)
    {

    }
}
