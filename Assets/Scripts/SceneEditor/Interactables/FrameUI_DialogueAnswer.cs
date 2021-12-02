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
    public Vector2 position { get; set; }
    public bool activeStatus { get; set; }
    public string text { get; set; }
    public FrameUI_DialogueAnswerValues(FrameUI_DialogueAnswer dialogue) {
        nextKeyID = dialogue.nextKeyID;
        previousKeyID = dialogue.previousKeyID;
        position = dialogue.position;
        activeStatus = dialogue.activeStatus;
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
        private string _text;

        public int nextKeyID { get => _nextKeyID; set => _nextKeyID = value; }
        public int previousKeyID { get => _previousKeyID; set => _previousKeyID = value; }
        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public string text { get => _text; set => _text = value; }
    }
    [SerializeField]
    public SerializedDialogueAnswerValues serializedDialogueValues;

    public void SetSerializedDialogueValues() {
        serializedDialogueValues.nextKeyID = nextKeyID;
        serializedDialogueValues.previousKeyID = previousKeyID;
        serializedDialogueValues.text = text;
        serializedDialogueValues.position = position;
        serializedDialogueValues.activeStatus = activeStatus;
    }
    public static void LoadSerialzedDialogueValues(List<SerializedDialogueAnswerValues> serializedElementValues, List<Values> values) {
        foreach (var svalue in serializedElementValues) {
            values.Add(new FrameUI_DialogueAnswerValues {
                nextKeyID = svalue.nextKeyID,
                previousKeyID = svalue.previousKeyID,
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                text = svalue.text,
            });
        }
    }
}
#endregion
public interface IFrameUI_DialogueAnswerSerialization {
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
            return this.gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        else return frameElementObject.prefab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
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

            answer.SetKeyValuesWhileNotInPlayMode();
            Debug.Log(answer.id);
            if (targets.Length > 1) {
                foreach (var target in targets) {
                    FrameElement mTarget = (FrameElement)target;
                    answer.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                    mTarget.SetKeyValuesWhileNotInPlayMode();
                }
            }

            this.SetElementInInspector<FrameUI_DialogueAnswerSO>();
        }
    }
#endif
    #endregion
}
