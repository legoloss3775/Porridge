﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using static FrameKey;

#region SERIALIZATION
[Serializable]
public class FrameUI_DialogueValues : Values, IFrameUIDialogueSerialization {
    public Vector2 position { get; set; }
    public bool activeStatus { get; set; }
    public string text { get; set; }
    public string conversationCharacterID { get; set; }
    public int speakingCharacterIndex { get; set; }
    public SerializableDictionary<string, string> conversationCharacters { get; set; }
    public FrameUI_Dialogue.FrameDialogueElementType type { get; set; }
    public FrameUI_DialogueValues(FrameUI_Dialogue dialogue) {
        position = dialogue.position;
        activeStatus = dialogue.activeStatus;
        text = dialogue.text;

        conversationCharacterID = dialogue.conversationCharacterID;
        speakingCharacterIndex = dialogue.speakingCharacterIndex;
        conversationCharacters = dialogue.conversationCharacters;
        type = dialogue.type;
    }
    public FrameUI_DialogueValues() { }
    [Serializable]
    public struct SerializedDialogueValues : IFrameUIDialogueSerialization {
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;
        [SerializeField]
        private string _text;
        [SerializeField]
        private string _conversationCharacterID;
        [SerializeField]
        private int _speakingCharacterIndex;
        [SerializeField]
        private SerializableDictionary<string, string> _conversationCharacters;
        [SerializeField]
        private FrameUI_Dialogue.FrameDialogueElementType _type;

        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public string text { get => _text; set => _text = value; }
        public string conversationCharacterID { get => _conversationCharacterID; set => _conversationCharacterID = value; }
        public int speakingCharacterIndex { get => _speakingCharacterIndex; set => _speakingCharacterIndex = value; }
        public SerializableDictionary<string, string> conversationCharacters { get => _conversationCharacters; set => _conversationCharacters = value; }
        public FrameUI_Dialogue.FrameDialogueElementType type { get => _type; set => _type = value; }
    }
    [SerializeField]
    private SerializedDialogueValues serializedDialogueValues;

    public SerializedDialogueValues SetSerializedDialogueValues() {
        serializedDialogueValues.text = text;
        serializedDialogueValues.position = position;
        serializedDialogueValues.activeStatus = activeStatus;
        serializedDialogueValues.speakingCharacterIndex = speakingCharacterIndex;
        serializedDialogueValues.conversationCharacterID = conversationCharacterID;
        serializedDialogueValues.conversationCharacters = conversationCharacters;
        serializedDialogueValues.type = type;

        return serializedDialogueValues;
    }
    public static void LoadSerialzedDialogueValues(List<SerializedDialogueValues> serializedElementValues, List<Values> values) {
        foreach (var svalue in serializedElementValues) {
            values.Add(new FrameUI_DialogueValues {
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                text = svalue.text,

                speakingCharacterIndex = svalue.speakingCharacterIndex,
                conversationCharacterID = svalue.conversationCharacterID,
                conversationCharacters = svalue.conversationCharacters,
                type = svalue.type,
            });
        }
    }
}
#endregion
public interface IFrameUIDialogueSerialization {
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
    string text { get; set; }
    string conversationCharacterID { get; set; }
    int speakingCharacterIndex { get; set; }
    SerializableDictionary<string, string> conversationCharacters { get; set; }
    FrameUI_Dialogue.FrameDialogueElementType type { get; set; }
}
public class FrameUI_Dialogue : FrameUI_Window, IFrameUIDialogueSerialization {
    public string text {
        get {
            return GetTextComponent().text;
        }
        set {
            GetTextComponent().text = value;
        }
    }
    public string characterNameField {
        get {
            if (this != null)
                return this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
            else return frameElementObject.prefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        }
        set {
            if (conversationCharacterID != null)
                this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = conversationCharacterID.Split('_')[0];
        }
    }
    public string conversationCharacterID { get; set; }
    public int speakingCharacterIndex { get; set; }
    public SerializableDictionary<string, string> conversationCharacters { get; set; }
    public FrameDialogueElementType type { get; set; }
    public FrameCharacter currentConversationCharacter { get; set; }
    public FrameCharacterSO currentConversationCharacterSO { get; set; }

    public enum FrameDialogueElementType {
        OneCharacter,
        MultibleCharacters,
    }


    public bool HasCharacter(string characterID) {
        return characterID == conversationCharacterID ? true : false;
    }
    public void DialogueTypeChange(FrameDialogueElementType type) {
        var keyValues = GetFrameKeyValues<FrameUI_DialogueValues>();
        switch (type) {
            case FrameDialogueElementType.OneCharacter: {
                this.type = type;
                RemoveAllConversationCharactersFromScene();
                LoadConversationCharacter(keyValues.conversationCharacterID, FrameDialogueElementType.OneCharacter);
                break;
            }
            case FrameDialogueElementType.MultibleCharacters: {
                this.type = type;
                RemoveAllConversationCharactersFromScene();
                foreach (var characterID in keyValues.conversationCharacters.Values) {
                    LoadConversationCharacter(characterID, FrameDialogueElementType.MultibleCharacters);
                }
                break;
            }
        }
    }
    public void LoadConversationCharacter(string characterID, FrameDialogueElementType type) {
        if (characterID == null) return;
        if (currentConversationCharacter != null && type == FrameDialogueElementType.OneCharacter) RemovePreviousCharacterOnScene();

        var character = new FrameCharacter();
        var characterKeyValues = FrameElement.GetFrameKeyValues<FrameCharacterValues>(characterID, character);

        this.SetConversationCharacterSO();

        this.currentConversationCharacterSO.LoadElementOnScene<FrameCharacter>(
            FrameManager.frame.GetPair(FrameManager.frame.GetFrameElementObjectByID(characterID)), characterID, characterKeyValues);

        character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(characterID);
        if (character != null) {
            character.dialogueID = this.id;
            this.currentConversationCharacter = character;
            this.conversationCharacterID = characterID;

            character.UpdateValuesFromKey(characterKeyValues);

            character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues>();
            this.SetKeyValuesWhileNotInPlayMode<FrameUI_DialogueValues>();
        }
    }
    public void SetConversationCharacter() {
        var keyValues = GetFrameKeyValues<FrameUI_DialogueValues>();

        this.currentConversationCharacterSO.CreateElementOnScene<FrameCharacter>(this.currentConversationCharacterSO, this.currentConversationCharacterSO.prefab.transform.position, out string ID);
        if (FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(ID) == null)
            return;
        FrameCharacter character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(ID);
        character.dialogueID = this.id;
        character.type = FrameCharacter.CharacterType.Conversation;

        keyValues.conversationCharacterID = ID;
        this.conversationCharacterID = ID;
        this.currentConversationCharacter = character;


        character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues>();
        this.SetKeyValuesWhileNotInPlayMode<FrameUI_DialogueValues>();
        keyValues.conversationCharacters.Add(this.currentConversationCharacterSO.id, currentConversationCharacter.id);
    }
    public void SetConversationCharacterSO() {
        var keyValues = GetFrameKeyValues<FrameUI_DialogueValues>();
        var frameEditorSO = FrameManager.assetDatabase;

        for (int i = 0; i < frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>().Count; i++) {
            if (i == keyValues.speakingCharacterIndex) {
                this.currentConversationCharacterSO = frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>()[i];
            }
        }
    }
    public void RemoveAllConversationCharactersFromScene() {
        var keyValues = GetFrameKeyValues<FrameUI_DialogueValues>();
        foreach (var character in keyValues.conversationCharacters) {
            if (FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(character.Value))
                FrameManager.RemoveElement(character.Value);
        }
    }
    public void RemovePreviousCharacterOnScene() => FrameManager.RemoveElement(this.currentConversationCharacter.id);

    #region VALUES_SETTINGS
    public TextMeshProUGUI GetTextComponent() {
        if (this != null)
            return this.gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        else return frameElementObject.prefab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
    }
    public override FrameKey.Values GetFrameKeyValuesType() {
        return new FrameUI_DialogueValues(this);
    }
    public override void UpdateValuesFromKey(Values frameKeyValues) {
        var keyValues = (FrameUI_DialogueValues)frameKeyValues;
        activeStatus = keyValues.activeStatus;
        position = keyValues.position;
        text = keyValues.text;

        conversationCharacterID = keyValues.conversationCharacterID;
        speakingCharacterIndex = keyValues.speakingCharacterIndex;
        conversationCharacters = keyValues.conversationCharacters;
        type = keyValues.type;

        characterNameField = characterNameField;
    }
    public void UpdateConversationCharacterFromKey(Values frameKeyValues) {
        var keyValues = (FrameUI_DialogueValues)frameKeyValues;

        switch (keyValues.type) {
            case FrameDialogueElementType.OneCharacter: {
                RemoveAllConversationCharactersFromScene();
                this.LoadConversationCharacter(keyValues.conversationCharacterID, keyValues.type);
                break;
            }
            case FrameDialogueElementType.MultibleCharacters: {
                RemoveAllConversationCharactersFromScene();
                foreach (var characterID in keyValues.conversationCharacters.Values) {
                    LoadConversationCharacter(characterID, FrameDialogueElementType.MultibleCharacters);
                }
                break;
            }
        }
    }
    #endregion
    #region EDITOR
#if UNITY_EDITOR
    [CustomEditor(typeof(FrameUI_Dialogue))]
    [CanEditMultipleObjects]
    public class FrameUIDialogueWindowCustomInspector : FrameElementCustomInspector {
        public override void OnInspectorGUI() {
            FrameUI_Dialogue dialogue = (FrameUI_Dialogue)target;
            EditorUtility.SetDirty(dialogue);

            dialogue.position = dialogue.GetComponent<RectTransform>().anchoredPosition;

            dialogue.SetKeyValuesWhileNotInPlayMode<FrameUI_DialogueValues>();
            Debug.Log(dialogue.id);
            if (targets.Length > 1) {
                foreach (var target in targets) {
                    FrameElement mTarget = (FrameElement)target;
                    dialogue.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                    mTarget.SetKeyValuesWhileNotInPlayMode<FrameUI_DialogueValues>();
                }
            }

            this.SetElementInInspector<FrameUI_DialogueSO>();
        }
    }
#endif
    #endregion
}