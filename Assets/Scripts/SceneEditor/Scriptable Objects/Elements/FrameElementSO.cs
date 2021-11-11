using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
}