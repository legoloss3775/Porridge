using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameSO;

public abstract class FrameElementSO : ScriptableObject, ISerializationCallbackReceiver
{
    public string id { get; set; }

    public GameObject prefab;

    public virtual void OnAfterDeserialize()
    {
        
    }

    public virtual void OnBeforeSerialize()
    {

    }
    public virtual void OnEnable()
    {
        id = name;
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        if(!frameEditorSO.frameElementsObjects.Contains(this))
            frameEditorSO.frameElementsObjects.Add(this);
    }
    public virtual void LoadElementOnScene<T>(FrameElementIDPair pair, string id, T element, FrameKey.Values values)
    where T : FrameElement
    {
        element.frameElementObject = pair.elementObject;

        T elementClone = Instantiate(element.frameElementObject.prefab).AddComponent<T>();
        elementClone.frameElementObject = pair.elementObject;
        elementClone.id = id;
        elementClone.frameKeyValues = values;
        EditorUtility.SetDirty(elementClone);
        FrameManager.AddElement(elementClone);
    }
    public virtual void CreateElementOnScene<T>(FrameElementSO obj, in T element, Vector2 position, out string elementID)
    where T : FrameElement
    {
        element.frameElementObject = obj;

        T elementClone;
        CreateFrameElement(obj, element, position, out elementClone);

        bool hasElement = false;
        foreach (var pair in FrameManager.frame.usedElementsObjects)
        {
            if (pair.elementObject == obj)
            {
                pair.ids.Add(elementClone.id);
                hasElement = true;
            }
        }
        if (!hasElement)
        {
            FrameElementIDPair newPair = new FrameElementIDPair();
            newPair.ids = new List<string>();
            newPair.elementObject = obj;
            newPair.ids.Add(elementClone.id);
            FrameManager.frame.usedElementsObjects.Add(newPair);

        }

        elementID = elementClone.id;
    }
    public virtual void CreateElementOnScene<T>(FrameElementSO obj, in T element, Vector2 position, string elementID)
    where T : FrameElement
    {
        element.frameElementObject = obj;

        T elementClone;
        CreateFrameElement(obj, element, position, out elementClone);

        elementClone.id = elementID;
        bool hasElement = false;
        foreach (var pair in FrameManager.frame.usedElementsObjects)
        {
            if (pair.elementObject == obj)
            {
                pair.ids.Add(elementClone.id);
                hasElement = true;
            }
        }
        if (!hasElement)
        {
            FrameElementIDPair newPair = new FrameElementIDPair();
            newPair.ids = new List<string>();
            newPair.elementObject = obj;
            newPair.ids.Add(elementClone.id);
            FrameManager.frame.usedElementsObjects.Add(newPair);

        }
    }
    public virtual void CreateFrameElement<T>(FrameElementSO obj, T element, Vector2 position, out T elementClone)
    where T : FrameElement
    {
        elementClone = Instantiate(element.frameElementObject.prefab, position, new Quaternion()).AddComponent<T>();
        elementClone.frameElementObject = obj;
        elementClone.id = obj.id + "_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        EditorUtility.SetDirty(elementClone);
        FrameManager.AddElement(elementClone);
    }
}