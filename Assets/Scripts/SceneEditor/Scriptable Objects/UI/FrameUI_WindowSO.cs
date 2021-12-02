using System;
using UnityEditor;
using UnityEngine;
using static FrameSO;

[CreateAssetMenu(fileName = "Window", menuName = "Редактор Сцен/Окно")]
public class FrameUI_WindowSO : FrameElementSO {
    public enum FrameUIWindowType {
        Default,
        Interactable,
    }
    public FrameUIWindowType windowType;
    public override void OnAfterDeserialize() {
        base.OnAfterDeserialize();
    }

    public override void OnBeforeSerialize() {
        base.OnBeforeSerialize();
    }
    public override void OnEnable() {
        base.OnEnable();
    }
    public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, FrameKey.Values values) {
        T elementClone = Instantiate(pair.elementObject.prefab, FrameManager.UICanvas.transform).AddComponent<T>();
        elementClone.frameElementObject = pair.elementObject;
        elementClone.id = id;
#if UNITY_EDITOR
        EditorUtility.SetDirty(elementClone);
#endif
        FrameManager.AddElement(elementClone);
    }
    public override void CreateFrameElement<T>(FrameElementSO obj, Vector2 position, out T elementClone) {
        elementClone = Instantiate(obj.prefab, position, new Quaternion(), FrameManager.UICanvas.transform).AddComponent<T>();
        elementClone.frameElementObject = obj;
        elementClone.id = obj.id + "_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        foreach (var key in FrameManager.frame.frameKeys)
            key.AddFrameKeyValues(elementClone.id, elementClone.GetFrameKeyValuesType());
#if UNITY_EDITOR
        EditorUtility.SetDirty(elementClone);
#endif
        FrameManager.AddElement(elementClone);
    }
}
