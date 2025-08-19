using UnityEngine;

public abstract class CharacterStateControls
{
    protected CharacterControlsStateController controlsStateController;
    protected CameraManager cameraManager;
    protected ControlOptions controlOptions;
    protected InteractableCharacterController characterController;
    protected Transform objectGrabbedParent;
    protected Camera mainCamera;

    public void OnStateEnter(CharacterControlsStateController controller, CameraManager camManager, ControlOptions ctrlOptions, InteractableCharacterController charaController, Transform objGrabParent, Camera mainCam)
    {
        controlsStateController = controller;
        cameraManager = camManager;
        controlOptions = ctrlOptions;
        characterController = charaController;
        objectGrabbedParent = objGrabParent;
        mainCamera = mainCam;
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

    public void OnStateScroll(float scrollValue)
    {
        OnScroll(scrollValue);
    }

    protected virtual void OnScroll(float scrollValue)
    {
        cameraManager.OnZoom(scrollValue);
    }

    public void OnStateMouseMoveMiddleButtonPressed(float axisValue)
    {
        OnMouseMoveMiddleButtonPressed(axisValue);
    }

    protected virtual void OnMouseMoveMiddleButtonPressed(float axisValue)
    {
        cameraManager.OnRotate(axisValue);
    }

    public void OnStateControlPressed(bool isPressed)
    {
        OnControlPressed(isPressed);
    }

    protected virtual void OnControlPressed(bool isPressed)
    {

    }
}
