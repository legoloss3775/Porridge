using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static FrameKey;
using System;

#region SERIALIZATION
[Serializable]
public class FrameCharacterValues : Values, IFrameCharacterSerialzation
{
    public Vector2 position { get; set; }
    public bool activeStatus { get; set; }
    public string dialogueID { get; set; }
    public FrameCharacterValues(FrameCharacter character)
    {
        position = character.position;
        activeStatus = character.activeStatus;
        dialogueID = character.dialogueID;
    }
    public FrameCharacterValues() { }
    [Serializable]
    public struct SerializedFrameCharacterValues : IFrameCharacterSerialzation
    {
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;
        [SerializeField]
        private string _dialogueID;

        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public string dialogueID { get => _dialogueID; set => _dialogueID = value; }
    }
    [SerializeField]
    private SerializedFrameCharacterValues serializedFrameCharacterValues;

    public SerializedFrameCharacterValues SetSerializedFrameCharacterValues()
    {
        serializedFrameCharacterValues.position = position;
        serializedFrameCharacterValues.activeStatus = activeStatus;
        serializedFrameCharacterValues.dialogueID = dialogueID;

        return serializedFrameCharacterValues;
    }
    public static void LoadSerialzedFrameKeyCharacterElementValues(List<SerializedFrameCharacterValues> serializedElementValues,  List<Values> values)
    {
        foreach (var svalue in serializedElementValues)
        {
            values.Add(new FrameCharacterValues
            {
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                dialogueID = svalue.dialogueID,
            });
        }
    }
}
#endregion
public interface IFrameCharacterSerialzation
{
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
    string dialogueID { get; set; }
}
public class FrameCharacter : FrameElement, IFrameCharacterSerialzation
{
    public string dialogueID { get; set; }

    public bool HasDialogue(string m_dialogueID)
    {
        return m_dialogueID == dialogueID ? true : false;
    }

    #region VALUES_SETTINGS
    public override FrameKey.Values GetFrameKeyValuesType()
    {
        return new FrameCharacterValues(this);
    }
    public override void UpdateValuesFromKey(object frameKeyValues)
    {
        FrameCharacterValues values = (FrameCharacterValues)frameKeyValues;
        activeStatus = values.activeStatus;
        position = values.position;
        dialogueID = values.dialogueID;
    }

    #endregion

    public bool HasDialogue()
    {
        if (dialogueID != null)
            return true;
        return false;
    }
    #region EDITOR
#if UNITY_EDITOR

    [CustomEditor(typeof(FrameCharacter))]
    [CanEditMultipleObjects]
    public class FrameCharacterCustomInspector : FrameElementCustomInspector
    {
        public override void OnInspectorGUI()
        {
            FrameCharacter character = (FrameCharacter)target;
            FrameCharacterValues values;
            if (FrameManager.frame.currentKey.ContainsID(character.id))
                values = (FrameCharacterValues)FrameManager.frame.currentKey.frameKeyValues[character.id];
            else
                values = null;

            if (values != null)
            {
                values.position = character.gameObject.transform.position;
                character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues, FrameCharacter>();
                Debug.Log(character.id);
                if (targets.Length > 1)
                {
                    foreach (var target in targets)
                    {
                        FrameElement mTarget = (FrameElement)target;
                        character.position = character.gameObject.transform.position;
                        mTarget.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues, FrameCharacter>();
                    }
                }
            }
            this.SetElementInInspector<FrameCharacterSO>();
        }
    }
#endif
    #endregion
}