using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static FrameSO;
using UnityEditor;
using System;

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
    public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, T element, FrameKey.Values values)
    {
        element.frameElementObject = pair.elementObject;

        T elementClone = Instantiate(element.frameElementObject.prefab, FrameManager.UICanvas.transform).AddComponent<T>();
        elementClone.frameElementObject = pair.elementObject;
        elementClone.id = id;
        elementClone.frameKeyValues = values;
        EditorUtility.SetDirty(elementClone);
        FrameManager.AddElement(elementClone);
    }
    public override void CreateFrameElement<T>(FrameElementSO obj, T element, Vector2 position, out T elementClone)
    {
        elementClone = Instantiate(element.frameElementObject.prefab, position, new Quaternion(), FrameManager.UICanvas.transform).AddComponent<T>();
        elementClone.frameElementObject = obj;
        elementClone.id = obj.id + "_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        EditorUtility.SetDirty(elementClone);
        FrameManager.AddElement(elementClone);
    }
}
