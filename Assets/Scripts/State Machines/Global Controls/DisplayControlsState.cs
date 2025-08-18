using System;
using UnityEngine;

[Serializable]
public class DisplayControlsState : CharacterStateControls
{
    public static event Action<RaycastHit, bool> clickToCloseControls;

    protected override void OnEntry()
    {

    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        clickToCloseControls?.Invoke(raycastHit, false);
    }
}