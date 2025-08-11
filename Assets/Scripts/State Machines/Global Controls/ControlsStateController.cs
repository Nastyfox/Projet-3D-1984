using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsStateController : MonoBehaviour
{
    public IStateControls currentState;

    public GlobalControlsState globalControlsState = new GlobalControlsState();
    public MoveControlsState moveControlsState = new MoveControlsState();
    public GrabControlState grabControlState = new GrabControlState();
    public FollowControlsState followControlsState = new FollowControlsState();
    public DropControlsState dropControlsState = new DropControlsState();

    void Start()
    {
        ChangeState(globalControlsState);
    }

    public void ChangeState(IStateControls newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = newState;
        currentState.OnEntry(this);
    }

    public void OnMouseClick(RaycastHit raycastHit)
    {
        currentState.OnMouseClick(this, raycastHit);
    }

    void Update()
    {
        currentState.OnUpdate(this);
    }
}