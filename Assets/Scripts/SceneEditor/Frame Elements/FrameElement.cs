using FrameCore.ScriptableObjects;
using FrameCore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace FrameCore {
    /// <summary>
    /// Добавление новых параметров в FrameKeyValues:
    /// 
    /// Добавление типа данных в FrameData<see cref="FrameData">
    /// 
    /// Добавление в основной класс, наследующий от Values, и в его конструктор
    /// <see cref="FrameElementValues">
    /// <see cref="FrameElementValues.FrameElementValues()">
    /// 
    /// Добавление в структуру SerializedElementValues
    /// <see cref="FrameElementValues.SerializedElementValues">
    /// 
    /// Добавление в метод распаковки структуры SerializedElementValues
    /// <see cref="FrameElementValues.LoadSerialzedFrameKeyElementValues(List{FrameElementValues.SerializedElementValues}, List{Values})">
    /// 
    /// Добавление поля параметра в элемент
    /// <see cref="FrameElement">
    /// 
    /// Добавление параметра в обновление ключей элемента
    /// <see cref="FrameElement.UpdateValuesFromKey(Values)">
    /// 
    /// Необязательно: добавление редактирования параметра через инспектор
    /// <see cref="FrameElement.FrameElementCustomInspector.OnInspectorGUI">
    /// 
    /// Если элемент новый, то добавление обработки сериализации в AssetManager<see cref="AssetManager">
    /// <see cref="FrameKeyDictionary">
    /// <see cref="FrameKeyDictionary.keys" cref="FrameKeyDictionary.values">
    /// <see cref="FrameKeyDictionary.OnBeforeSerialize" cref = "FrameKeyDictionary.OnAfterDeserialize">
    /// 
    /// Добавить загрузку элемента в FrameManager<see cref="FrameManager">
    /// <see cref="FrameManager.ChangeFrame">
    /// 
    /// </summary>
    namespace Serialization {
        #region SERIALIZATION
        [Serializable]
        public class FrameElementValues : Values {
            public FrameElementValues(FrameElement element) {
                transformData = new TransformData {
                    position = element.position,
                    activeStatus = element.activeStatus,
                    size = element.size,
                };
            }
            public FrameElementValues() { }
            [Serializable]
            public struct SerializedElementValues {
                public TransformData transformData;
            }
            [SerializeField]
            public SerializedElementValues serializedElementValues {
                get {
                    return new SerializedElementValues {
                        transformData = transformData,
                    };
                }
            }
            public static void LoadSerialzedFrameKeyElementValues(List<SerializedElementValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new FrameElementValues {
                        transformData = svalue.transformData
                    }
                    );
                }
            }
        }
        #endregion
    }
    [System.Serializable]
    public abstract class FrameElement : MonoBehaviour {
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
        public string id { get; set; }

        public FrameElementSO frameElementObject;
        public Animator animator {
            get {
                if (GetComponent<Animator>() != null)
                    return GetComponent<Animator>();
                else return null;
            }
        }

        /**private void Update() {

        }**/

        #region ELEMENT_SETUP
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
        public T GetFrameElementType<T>(T element) where T : FrameElement => GetComponent<T>();
        public string GetName() => this.id.Split('_')[0];
        #endregion

        #region VALUES_SETTINGS
        public virtual Values GetFrameKeyValuesType() {
            return new Serialization.FrameElementValues(this);
        }
        public virtual void UpdateValuesFromKey(Values frameKeyValues) {
            Serialization.FrameElementValues values = (Serialization.FrameElementValues)frameKeyValues;
            activeStatus = values.transformData.activeStatus;
            position = values.transformData.position;
            size = values.transformData.size;
        }

        public void SetKeyValuesWhileNotInPlayMode() {
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
                    keyValues.transformData.position = element.gameObject.transform.position;
                    keyValues.transformData.size = element.gameObject.transform.localScale;
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
}

