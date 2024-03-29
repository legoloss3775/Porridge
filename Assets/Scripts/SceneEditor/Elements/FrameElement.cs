﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameKey;

#region SERIALIZATION
[Serializable]
public class FrameElementValues : Values, IFrameElementSerialization
{
    public Vector2 position { get; set; }
    public bool activeStatus { get; set; }
    public FrameElementValues(FrameElement element)
    {
        position = element.position;
        activeStatus = element.activeStatus;
    }
    public FrameElementValues() { }
    [Serializable]
    public struct SerializedElementValues : IFrameElementSerialization
    {
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;

        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
    }
    [SerializeField]
    private SerializedElementValues serializedElementValues;

    public SerializedElementValues SetSerializedFrameKeyElementValues()
    {
        serializedElementValues.position = position;
        serializedElementValues.activeStatus = activeStatus;

        return serializedElementValues;
    }
    public static void LoadSerialzedFrameKeyElementValues(List<SerializedElementValues> serializedElementValues, List<Values> values)
    {
        foreach (var svalue in serializedElementValues)
        {
            values.Add(new FrameElementValues
            {
                position = svalue.position,
                activeStatus = svalue.activeStatus
            });
        }
    }
}
#endregion
public interface IFrameElementSerialization
{
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
}
[System.Serializable]
public abstract class FrameElement : MonoBehaviour, IFrameElementSerialization
{
    //VALUES
    public Vector2 position
    {
        get
        {
            if (this is FrameUIDialogue)
                try
                {
                    return this.GetComponent<RectTransform>().anchoredPosition;
                }
                catch (System.Exception)
                {
                    if (this != null)
                        return this.gameObject.transform.position;
                    else
                        return frameElementObject.prefab.transform.position;
                }
            else if (this != null)
                return this.gameObject.transform.position;
            else
                return frameElementObject.prefab.transform.position;
        }
        set
        {
            if (this is FrameUIDialogue)
                GetComponent<RectTransform>().anchoredPosition = value;
            else
                this.transform.position = value;
        }
    }
    public bool activeStatus
    {
        get
        {
            if (this != null)
                return this.gameObject.activeSelf;
            else return true;
        }
        set
        {
            this.gameObject.SetActive(value);
        }
    }
    //ID объекта задается при его создании, сохранение в ключе не нужно.
    public string id { get; set; }
    public FrameKey.Values frameKeyValues { get; set; }
    public FrameElementSO frameElementObject;

    #region VALUES_SETTINGS
    public virtual FrameKey.Values GetFrameKeyValuesType()
    {
        return new FrameElementValues(this);
    }
    public virtual void UpdateValuesFromKey(object frameKeyValues)
    {
        FrameElementValues values = (FrameElementValues)frameKeyValues;
        activeStatus = values.activeStatus;
        position = values.position;
    }

    public void SetKeyValuesWhileNotInPlayMode<T, V>()
        where T: Values
        where V: FrameElement
    {
        T values = null;
        if (!Application.isPlaying)
        {
            if (FrameManager.frame.currentKey.ContainsID(id))
            {
                values = (T)FrameManager.frame.currentKey.frameKeyValues[id];
                frameKeyValues = values.GetObject<T>((V)this);
            }
            if (frameKeyValues != null)
            {
                FrameManager.frame.currentKey.UpdateFrameKeyValues(id, frameKeyValues);
                UpdateValuesFromKey(frameKeyValues);
            }
            else
            {
                frameKeyValues = GetFrameKeyValuesType();
                FrameManager.frame.currentKey.AddFrameKeyValues(id, frameKeyValues);
                UpdateValuesFromKey(frameKeyValues);
            }
        }
    }

    #endregion

    private void Update()
    {
        //Debug.Log(id);
    }
    public string GetName()
    {
        return this.id.Split('_')[0];
    }
    #region EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(FrameElement))]
    [CanEditMultipleObjects]
    public abstract class FrameElementCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            FrameElement element = (FrameElement)target;
            Values values;
            if (FrameManager.frame.currentKey.ContainsID(element.id))
                values = FrameManager.frame.currentKey.frameKeyValues[element.id];
            else
                values = null;

            element.position = element.gameObject.transform.position;

            Debug.Log(element.id);
            if(targets.Length > 1)
            {
                foreach (var target in targets)
                {
                    FrameElement mTarget = (FrameElement)target;
                    mTarget.position = mTarget.gameObject.transform.position;
                    mTarget.SetKeyValuesWhileNotInPlayMode<FrameElementValues, FrameElement>();
                }
            }
        }
        public void SetElementInInspector<T>()
            where T : FrameElementSO
        {
            FrameElement element = (FrameElement)target;

            element.frameElementObject = (T)EditorGUILayout.ObjectField(
                element.frameElementObject,
                typeof(T),
                false
                );
        }
    }
#endif
    #endregion
}
