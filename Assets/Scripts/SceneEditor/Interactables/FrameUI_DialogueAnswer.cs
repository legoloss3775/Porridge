using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static FrameKey;

#region SERIALIZATION

[Serializable]
public class FrameUI_DialogueAnswerValues : Values, IFrameUI_DialogueAnswerSerialization {
    public int nextKeyID { get; set; }
    public int previousKeyID { get; set; }
    public string text { get; set; }
    public FrameUI_DialogueAnswerValues(FrameUI_DialogueAnswer dialogue) {
        nextKeyID = dialogue.nextKeyID;
        previousKeyID = dialogue.previousKeyID;
        position = dialogue.position;
        activeStatus = dialogue.activeStatus;
        size = dialogue.size;
        text = dialogue.text;
    }
    public FrameUI_DialogueAnswerValues() { }
    [Serializable]
    public struct SerializedDialogueAnswerValues : IFrameUI_DialogueAnswerSerialization {
        [SerializeField]
        private int _nextKeyID;
        [SerializeField]
        private int _previousKeyID;
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;
        [SerializeField]
        private Vector2 _size;
        [SerializeField]
        private string _text;

        public int nextKeyID { get => _nextKeyID; set => _nextKeyID = value; }
        public int previousKeyID { get => _previousKeyID; set => _previousKeyID = value; }
        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public Vector2 size { get => _size; set => _size = value; }
        public string text { get => _text; set => _text = value; }
    }
    [SerializeField]
    public SerializedDialogueAnswerValues serializedDialogueAnswerValues;

    public void SetSerializedDialogueValues() {
        serializedDialogueAnswerValues.nextKeyID = nextKeyID;
        serializedDialogueAnswerValues.previousKeyID = previousKeyID;
        serializedDialogueAnswerValues.text = text;
        serializedDialogueAnswerValues.position = position;
        serializedDialogueAnswerValues.activeStatus = activeStatus;
        serializedDialogueAnswerValues.size = size;
    }
    public static void LoadSerialzedDialogueValues(List<SerializedDialogueAnswerValues> serializedElementValues, List<Values> values) {
        foreach (var svalue in serializedElementValues) {
            values.Add(new FrameUI_DialogueAnswerValues {
                nextKeyID = svalue.nextKeyID,
                previousKeyID = svalue.previousKeyID,
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                size = svalue.size,
                text = svalue.text,
            });
        }
    }
}
#endregion
public interface IFrameUI_DialogueAnswerSerialization {
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
    Vector2 size { get; set; }
    string text { get; set; }
}
public class FrameUI_DialogueAnswer : FrameUI_Window, IInteractable {
    public override Vector2 position {
        get {
            try {
                return this.GetComponent<RectTransform>().anchoredPosition;
            }
            catch (System.Exception) {
                if (this != null)
                    return this.gameObject.transform.position;
                else
                    return frameElementObject.prefab.transform.position;
            }
        }
        set {
            GetComponent<RectTransform>().anchoredPosition = value;
        }
    }
    public override Vector2 size {
        get {
            try {
                return RectTransformExtensions.GetSize(this.GetComponent<RectTransform>());
            }
            catch (System.Exception) {
                if (this != null)
                    return RectTransformExtensions.GetSize(this.GetComponent<RectTransform>());
                else
                    return RectTransformExtensions.GetSize(frameElementObject.prefab.GetComponent<RectTransform>());
            }
        }
        set {
            RectTransformExtensions.SetWidth(this.GetComponent<RectTransform>(), value.x);
            RectTransformExtensions.SetHeight(this.GetComponent<RectTransform>(), value.y);
        }
    }
    public int nextKeyID { get; set; }
    public int previousKeyID { get; set; }
    public string text {
        get {
            return GetTextComponent().text;
        }
        set {
            GetTextComponent().text = value;
        }
    }

    #region VALUES_SETTINGS
    public TextMeshProUGUI GetTextComponent() {
        if (this != null)
            return this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        else return frameElementObject.prefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
    }
    public override FrameKey.Values GetFrameKeyValuesType() {
        return new FrameUI_DialogueAnswerValues(this);
    }
    public override void UpdateValuesFromKey(Values frameKeyValues) {
        var keyValues = (FrameUI_DialogueAnswerValues)frameKeyValues;

        nextKeyID = keyValues.nextKeyID;
        previousKeyID = keyValues.previousKeyID;
        activeStatus = keyValues.activeStatus;
        position = keyValues.position;
        size = keyValues.size;
        text = keyValues.text;
    }
    #endregion
    #region EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(FrameUI_DialogueAnswer))]
    [CanEditMultipleObjects]
    public class FrameUIDialogueWindowCustomInspector : FrameElementCustomInspector {
        public override void OnInspectorGUI() {
            FrameUI_DialogueAnswer answer = (FrameUI_DialogueAnswer)target;
            EditorUtility.SetDirty(answer);

            answer.position = answer.GetComponent<RectTransform>().anchoredPosition;
            answer.size = answer.GetComponent<RectTransform>().sizeDelta;

            answer.SetKeyValuesWhileNotInPlayMode();
            Debug.Log(answer.id);
            if (targets.Length > 1) {
                foreach (var target in targets) {
                    FrameElement mTarget = (FrameElement)target;
                    answer.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                    answer.size = answer.GetComponent<RectTransform>().sizeDelta;
                    mTarget.SetKeyValuesWhileNotInPlayMode();
                }
            }

            this.SetElementInInspector<FrameUI_DialogueAnswerSO>();
        }
    }
#endif
    #endregion
}
