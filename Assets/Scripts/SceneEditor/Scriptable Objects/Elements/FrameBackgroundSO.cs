using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Background", menuName = "Редактор Сцен/Бэкграунд")]
public class FrameBackgroundSO : FrameElementSO
{
    public enum BackgroundType
    {
        Default,
    }
    public BackgroundType type;

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
