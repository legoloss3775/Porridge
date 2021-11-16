using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static FrameKey;

#region SERIALIZATION
[Serializable]
public class FrameCharacterValues : Values, IFrameCharacterSerialzation {
    public Vector2 position { get; set; }
    public bool activeStatus { get; set; }
    public string dialogueID { get; set; }
    public FrameCharacter.CharacterType type { get; set; }
    public FrameCharacterValues(FrameCharacter character) {
        position = character.position;
        activeStatus = character.activeStatus;
        dialogueID = character.dialogueID;
        type = character.type;
    }
    public FrameCharacterValues() { }
    [Serializable]
    public struct SerializedFrameCharacterValues : IFrameCharacterSerialzation {
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;
        [SerializeField]
        private string _dialogueID;
        [SerializeField]
        private FrameCharacter.CharacterType _type;

        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public string dialogueID { get => _dialogueID; set => _dialogueID = value; }
        public FrameCharacter.CharacterType type { get => _type; set => _type = value; }
    }
    [SerializeField]
    private SerializedFrameCharacterValues serializedFrameCharacterValues;

    public SerializedFrameCharacterValues SetSerializedFrameCharacterValues() {
        serializedFrameCharacterValues.position = position;
        serializedFrameCharacterValues.activeStatus = activeStatus;
        serializedFrameCharacterValues.dialogueID = dialogueID;
        serializedFrameCharacterValues.type = type;

        return serializedFrameCharacterValues;
    }
    public static void LoadSerialzedFrameKeyCharacterElementValues(List<SerializedFrameCharacterValues> serializedElementValues, List<Values> values) {
        foreach (var svalue in serializedElementValues) {
            values.Add(new FrameCharacterValues {
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                dialogueID = svalue.dialogueID,
                type = svalue.type,
            });
        }
    }
}
#endregion
public interface IFrameCharacterSerialzation {
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
    string dialogueID { get; set; }
    FrameCharacter.CharacterType type { get; set; }
}
public class FrameCharacter : FrameElement, IFrameCharacterSerialzation {
    public string dialogueID { get; set; }
    public CharacterType type { get; set; }
    public enum CharacterType {
        Standalone,
        Conversation,
    }

    public bool HasDialogue(string m_dialogueID) => m_dialogueID == dialogueID ? true : false;
    public bool HasDialogue() => dialogueID != null ? true : false;

    #region VALUES_SETTINGS
    public override FrameKey.Values GetFrameKeyValuesType() {
        return new FrameCharacterValues(this);
    }
    public override void UpdateValuesFromKey(Values frameKeyValues) {
        FrameCharacterValues values = (FrameCharacterValues)frameKeyValues;
        activeStatus = values.activeStatus;
        position = values.position;
        dialogueID = values.dialogueID;
        type = values.type;
    }

    #endregion

    #region EDITOR
#if UNITY_EDITOR

    [CustomEditor(typeof(FrameCharacter))]
    [CanEditMultipleObjects]
    public class FrameCharacterCustomInspector : FrameElementCustomInspector {
        public override void OnInspectorGUI() {
            FrameCharacter character = (FrameCharacter)target;
            var keyValues = GetFrameKeyValues<FrameCharacterValues>(character.id);

            if (keyValues != null) {
                keyValues.position = character.gameObject.transform.position;
                character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues>();

                if (targets.Length > 1) {
                    foreach (var target in targets) {
                        FrameElement mTarget = (FrameElement)target;
                        character.position = character.gameObject.transform.position;
                        mTarget.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues>();
                    }
                }
            }
            this.SetElementInInspector<FrameCharacterSO>();
        }
    }
#endif
    #endregion
}