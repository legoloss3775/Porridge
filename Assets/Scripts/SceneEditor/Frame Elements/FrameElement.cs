using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameKey;

#region SERIALIZATION
[Serializable]
public class FrameElementValues : Values, IFrameElementSerialization {
    public FrameElementValues(FrameElement element) {
        position = element.position;
        activeStatus = element.activeStatus;
        size = element.size;
    }
    public FrameElementValues() { }
    [Serializable]
    public struct SerializedElementValues : IFrameElementSerialization {
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;
        [SerializeField]
        private Vector2 _size;

        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public Vector2 size { get => _size; set => _size = value; }
    }
    [SerializeField]
    public SerializedElementValues serializedElementValues;

    public void SetSerializedFrameKeyElementValues() {
        serializedElementValues.position = position;
        serializedElementValues.activeStatus = activeStatus;
        serializedElementValues.size = size;
    }
    public static void LoadSerialzedFrameKeyElementValues(List<SerializedElementValues> serializedElementValues, List<Values> values) {
        foreach (var svalue in serializedElementValues) {
            values.Add(new FrameElementValues {
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                size = svalue.size,
            });
        }
    }
}
#endregion
public interface IFrameElementSerialization {
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
    Vector2 size { get; set; }
}
[System.Serializable]
public abstract class FrameElement : MonoBehaviour, IFrameElementSerialization {
    //VALUES
    public virtual Vector2 position {
        get {
            if (this != null)
                return this.gameObject.transform.position;
            else
                return frameElementObject.prefab.transform.position;
        }
        set {
                this.transform.position = value;
        }
    }
    public bool activeStatus {
        get {
            if (this != null)
                return this.gameObject.activeSelf;
            else return true;
        }
        set {
            this.gameObject.SetActive(value);
        }
    }
    public virtual Vector2 size {
        get {
            if (this != null)
                return this.gameObject.transform.localScale;
            else return frameElementObject.prefab.transform.localScale;
        }
        set {
            this.gameObject.transform.localScale = value;
        }
    }
    //ID объекта задается при его создании, сохранение в ключе не нужно.
    public string id { get; set; }

    public FrameElementSO frameElementObject;
    private void Update() {

    }
    public T GetFrameKeyValues<T>() where T : Values => (T)FrameManager.frame.currentKey.GetFrameKeyValuesOfElement(id);
    public static T GetFrameKeyValues<T>(string id) 
        where T : Values {
        try {
            return (T)FrameManager.frame.currentKey.GetFrameKeyValuesOfElement(id);
        }
        catch (System.Exception) {
            T values = null;

            try {
                values = (T)FrameManager.frame.frameKeys.Where(ch => ch.ContainsID(id)).Last().frameKeyValues[id];
            }
            catch (System.Exception) {

            }

            if (values == null) {
                values = Values.GetObject<T>();
                FrameManager.frame.currentKey.AddFrameKeyValues(id, values);
                return values;
            }
            FrameManager.frame.currentKey.AddFrameKeyValues(id, values);
            return values;
        }
    }
    public void SetFrameKeyValues() => FrameManager.frame.currentKey.UpdateFrameKeyValues(id, GetFrameKeyValuesType());
    public T GetFrameElementType<T>(T element) where T: FrameElement => GetComponent<T>();
    public string GetName() => this.id.Split('_')[0];

    #region VALUES_SETTINGS
    public virtual FrameKey.Values GetFrameKeyValuesType() {
        return new FrameElementValues(this);
    }
    public virtual void UpdateValuesFromKey(Values frameKeyValues) {
        FrameElementValues values = (FrameElementValues)frameKeyValues;
        activeStatus = values.activeStatus;
        position = values.position;
        size = values.size;
    }

    public void SetKeyValuesWhileNotInPlayMode(){
        if (!Application.isPlaying) {
            if (FrameManager.frame.currentKey.ContainsID(id)) {
                SetFrameKeyValues();
            }
        }
    }

    #endregion

    #region EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(FrameElement))]
    [CanEditMultipleObjects]
    public abstract class FrameElementCustomInspector : Editor {
        public override void OnInspectorGUI() {
            FrameElement element = (FrameElement)target;
            var keyValues = GetFrameKeyValues<FrameElementValues>(element.id);

            Debug.Log(element.id);
            if (keyValues != null) {
                keyValues.position = element.gameObject.transform.position;
                keyValues.size = element.gameObject.transform.localScale;
                element.SetKeyValuesWhileNotInPlayMode();

                if (targets.Length > 1) {
                    foreach (var target in targets) {
                        FrameElement mTarget = (FrameElement)target;
                        element.position = element.gameObject.transform.position;
                        element.size = element.gameObject.transform.localScale;
                        mTarget.SetKeyValuesWhileNotInPlayMode();
                    }
                }
            }
        }
        public void SetElementInInspector<T>()
            where T : FrameElementSO {
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
