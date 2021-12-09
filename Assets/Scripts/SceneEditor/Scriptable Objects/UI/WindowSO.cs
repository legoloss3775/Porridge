using System;
using UnityEditor;
using UnityEngine;
using static FrameCore.ScriptableObjects.FrameSO;

namespace FrameCore {
    namespace ScriptableObjects {
        namespace UI {

            [CreateAssetMenu(fileName = "Window", menuName = "Редактор Сцен/Окно")]
            public class WindowSO : FrameElementSO {
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
                public override Vector2 GetPrefabPosition() {
                    return prefab.GetComponent<RectTransform>().anchoredPosition;
                }
                public override Vector2 GetPrefabSize() {
                    return prefab.GetComponent<RectTransform>().sizeDelta;
                }
                public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, Values values) {
                    T elementClone = Instantiate(pair.elementObject.prefab, FrameManager.UICanvas.transform).AddComponent<T>();
                    elementClone.frameElementObject = pair.elementObject;
                    elementClone.id = id;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(elementClone);
#endif
                    FrameManager.AddElement(elementClone);
                }
                public override void CreateFrameElement<T>(FrameElementSO obj, Vector2 position, Vector2 size, out T elementClone) {
                    elementClone = Instantiate(obj.prefab, position, new Quaternion(), FrameManager.UICanvas.transform).AddComponent<T>();
                    elementClone.size = size;
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
        }
    }
}
