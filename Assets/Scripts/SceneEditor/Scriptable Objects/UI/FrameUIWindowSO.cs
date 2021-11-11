using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Window", menuName = "Редактор Сцен/Окно" )]
public class FrameUIWindowSO : FrameElementSO
{
    public enum FrameUIWindowType
    {
        Default,
        Interactable,
    }
    public FrameUIWindowType windowType;
    public override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
    }

    public override void OnBeforeSerialize()
    {
        base.OnBeforeSerialize();
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
}
