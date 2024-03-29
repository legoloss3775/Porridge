﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static FrameKey;

#region SERIALIZATION
[Serializable]
public class UIDialogueValues : Values, IFrameUIDialogueSerialization
{
    public Vector2 position { get; set; }
    public bool activeStatus { get; set; }
    public string text { get; set; }

    public string conversationCharacterID { get; set; }
    public int selectedCharacterIndex { get; set; }
    public UIDialogueValues(FrameUIDialogue dialogue)
    {
        position = dialogue.position;
        activeStatus = dialogue.activeStatus;
        text = dialogue.text;

        conversationCharacterID = dialogue.conversationCharacterID;
        selectedCharacterIndex = dialogue.selectedCharacterIndex;
    }
    public UIDialogueValues() { }
    [Serializable]
    public struct SerializedDialogueValues : IFrameUIDialogueSerialization
    {
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;
        [SerializeField]
        private string _text;
        [SerializeField]
        private string _conversationCharacterID;
        [SerializeField]
        private int _selectedCharacterIndex;

        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public string text { get => _text; set => _text = value; }
        public string conversationCharacterID { get => _conversationCharacterID; set => _conversationCharacterID = value; }
        public int selectedCharacterIndex { get => _selectedCharacterIndex; set => _selectedCharacterIndex = value; }
    }
    [SerializeField]
    private SerializedDialogueValues serializedDialogueValues;

    public SerializedDialogueValues SetSerializedDialogueValues()
    {
        serializedDialogueValues.text = text;
        serializedDialogueValues.position = position;
        serializedDialogueValues.activeStatus = activeStatus;
        serializedDialogueValues.selectedCharacterIndex = selectedCharacterIndex;
        serializedDialogueValues.conversationCharacterID = conversationCharacterID;

        return serializedDialogueValues;
    }
    public static void LoadSerialzedDialogueValues(List<SerializedDialogueValues> serializedElementValues, List<Values> values)
    {
        foreach (var svalue in serializedElementValues)
        {
            values.Add(new UIDialogueValues
            {
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                text = svalue.text,

                selectedCharacterIndex = svalue.selectedCharacterIndex,
                conversationCharacterID = svalue.conversationCharacterID,
            });
        }
    }
}
#endregion
public interface IFrameUIDialogueSerialization
{
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
    string text { get; set; }
    string conversationCharacterID { get; set; }
    int selectedCharacterIndex { get; set; }
}
public class FrameUIDialogue : FrameUIWindow, IFrameUIDialogueSerialization
{
    public string text
    {
        get
        {
            if (this != null)
                return this.gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
            else return frameElementObject.prefab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
        }
        set
        {
            this.gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = value;
        }
    }
    public string characterNameField
    {
        get
        {
            if (this != null)
                return this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
            else return frameElementObject.prefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        }
        set
        {
            if(conversationCharacterID != null)
                this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = conversationCharacterID.Split('_')[0];
        }
    }
    public FrameCharacter conversationCharacter { get; set; }
    public FrameCharacterSO conversationCharacterSO { get; set; }
    
    public string conversationCharacterID { get; set; }
    public int selectedCharacterIndex { get; set; }

    #region VALUES_SETTINGS
    public override FrameKey.Values GetFrameKeyValuesType()
    {
        return new UIDialogueValues(this);
    }
    public override void UpdateValuesFromKey(object frameKeyValues)
    {
        Debug.Log(frameKeyValues.GetType());
        UIDialogueValues values = (UIDialogueValues)frameKeyValues;
        activeStatus = values.activeStatus;
        position = values.position;
        text = values.text;

        conversationCharacterID = values.conversationCharacterID;
        selectedCharacterIndex = values.selectedCharacterIndex;

        characterNameField = characterNameField;
    }
    #endregion

    #region EDITOR
#if UNITY_EDITOR

    [CustomEditor(typeof(FrameUIDialogue))]
    [CanEditMultipleObjects]
    public class FrameUIDialogueWindowCustomInspector : FrameElementCustomInspector
    {
        public override void OnInspectorGUI()
        {
            FrameUIDialogue dialogue = (FrameUIDialogue)target;
            UIDialogueValues values;
            if (FrameManager.frame.currentKey.ContainsID(dialogue.id))
                values = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];
            else
                values = null;

            if (values != null)
            {
                values.position = dialogue.GetComponent<RectTransform>().anchoredPosition;
                Debug.Log(dialogue.GetComponent<RectTransform>().anchoredPosition + " " + values.position);
                dialogue.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUIDialogue>();
                Debug.Log(dialogue.id);
                if (targets.Length > 1)
                {
                    foreach (var target in targets)
                    {
                        FrameElement mTarget = (FrameElement)target;
                        dialogue.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                        mTarget.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUIDialogue>();
                    }
                }
            }
            this.SetElementInInspector<FrameUIDialogueSO>();
        }
    }
#endif
    #endregion
}