using UnityEngine;

public abstract class StateControls
{
    protected ControlsStateController controlsStateController;
    [SerializeField] private string test;

    public void OnStateEnter(ControlsStateController controller)
    {
        controlsStateController = controller;
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
