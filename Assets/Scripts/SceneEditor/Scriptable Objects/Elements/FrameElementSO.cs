using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameSO;

public abstract class FrameElementSO : ScriptableObject, ISerializationCallbackReceiver {
    public string id { get; set; }

    [Header("Основной префаб")]
    public GameObject prefab;

    public virtual void OnAfterDeserialize() {

    }

    public virtual void OnBeforeSerialize() {

    }
    public virtual void OnEnable() {
        id = name;
#if UNITY_EDITOR
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        if (frameEditorSO != null && !frameEditorSO.frameElementsObjects.Contains(this))
            frameEditorSO.frameElementsObjects.Add(this);
#endif

    }
    public virtual void LoadElementOnScene<T>(FrameElementIDPair pair, string id, FrameKey.Values values)
    where T : FrameElement {
        T elementClone = Instantiate(pair.elementObject.prefab).AddComponent<T>();
        elementClone.frameElementObject = pair.elementObject;
        elementClone.id = id;
#if UNITY_EDITOR
        EditorUtility.SetDirty(elementClone);
#endif
        FrameManager.AddElement(elementClone);
    }
    public virtual void CreateElementOnScene<T>(FrameElementSO obj, Vector2 position, Vector2 size, out string elementID)
    where T : FrameElement {
        CreateFrameElement(obj, position, size, out T elementClone);

        bool hasElement = false;
        foreach (var pair in FrameManager.frame.usedElementsObjects) {
            if (pair.elementObject == obj) {
                pair.ids.Add(elementClone.id);
                hasElement = true;
            }
        }
        if (!hasElement) {
            FrameElementIDPair newPair = new FrameElementIDPair();
            newPair.ids = new List<string>();
            newPair.elementObject = obj;
            newPair.ids.Add(elementClone.id);
            FrameManager.frame.usedElementsObjects.Add(newPair);

        }

        elementID = elementClone.id;
    }
    public virtual void CreateElementOnScene<T>(FrameElementSO obj, Vector2 position, Vector2 size, string elementID)
    where T : FrameElement {
        T elementClone;
        CreateFrameElement(obj, position, size, out elementClone);

        elementClone.id = elementID;
        bool hasElement = false;
        foreach (var pair in FrameManager.frame.usedElementsObjects) {
            if (pair.elementObject == obj) {
                pair.ids.Add(elementClone.id);
                hasElement = true;
            }
        }
        if (!hasElement) {
            FrameElementIDPair newPair = new FrameElementIDPair();
            newPair.ids = new List<string>();
            newPair.elementObject = obj;
            newPair.ids.Add(elementClone.id);
            FrameManager.frame.usedElementsObjects.Add(newPair);

        }
    }
    public virtual void CreateFrameElement<T>(FrameElementSO obj, Vector2 position, Vector2 size, out T elementClone)
    where T : FrameElement {
        elementClone = Instantiate(obj.prefab, position, new Quaternion()).AddComponent<T>();
        elementClone.size = size;
        elementClone.frameElementObject = obj;
        elementClone.id = obj.id + "_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        foreach(var key in FrameManager.frame.frameKeys)
            key.AddFrameKeyValues(elementClone.id, elementClone.GetFrameKeyValuesType());
#if UNITY_EDITOR
        EditorUtility.SetDirty(elementClone);
#endif
        FrameManager.AddElement(elementClone);

    }
}