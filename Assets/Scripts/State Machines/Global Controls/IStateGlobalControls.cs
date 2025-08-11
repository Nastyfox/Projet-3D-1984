using UnityEngine;
using UnityEngine.InputSystem;

public interface IStateControls
{
    void OnEntry(ControlsStateController controller);

    void OnUpdate(ControlsStateController controller);

    void OnExit(ControlsStateController controller);

    void OnMouseClick(ControlsStateController controller, RaycastHit raycastHit);
}
