using FrameCore.ScriptableObjects;
using FrameCore.ScriptableObjects.UI;
using FrameCore.Serialization;
using FrameCore.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;


namespace FrameCore {
    /// <summary>
    /// Для сериализации параметров
    /// <see cref="FrameElement">
    /// </summary>
    namespace Serialization {
        #region SERIALIZATION
        [Serializable]
        public class DialogueValues : Values, IFrameUI_DialogueSerialization {
            public int nextKeyID { get; set; }
            public int previousKeyID { get; set; }
            public string text { get; set; }
            public string conversationCharacterID { get; set; }
            public int speakingCharacterIndex { get; set; }
            public SerializableDictionary<string, string> conversationCharacters { get; set; }
            public Dialogue.FrameDialogueElementType type { get; set; }
            public DialogueValues(Dialogue dialogue) {
                nextKeyID = dialogue.nextKeyID;
                previousKeyID = dialogue.previousKeyID;
                position = dialogue.position;
                activeStatus = dialogue.activeStatus;
                size = dialogue.size;
                text = dialogue.text;

                conversationCharacterID = dialogue.conversationCharacterID;
                speakingCharacterIndex = dialogue.speakingCharacterIndex;
                conversationCharacters = dialogue.conversationCharacters;
                type = dialogue.type;
            }
            public DialogueValues() { }
            [Serializable]
            public struct SerializedDialogueValues : IFrameUI_DialogueSerialization {
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
                [SerializeField]
                private string _conversationCharacterID;
                [SerializeField]
                private int _speakingCharacterIndex;
                [SerializeField]
                private SerializableDictionary<string, string> _conversationCharacters;
                [SerializeField]
                private Dialogue.FrameDialogueElementType _type;

                public int nextKeyID { get => _nextKeyID; set => _nextKeyID = value; }
                public int previousKeyID { get => _previousKeyID; set => _previousKeyID = value; }
                public Vector2 position { get => _position; set => _position = value; }
                public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
                public Vector2 size { get => _size; set => _size = value; }
                public string text { get => _text; set => _text = value; }
                public string conversationCharacterID { get => _conversationCharacterID; set => _conversationCharacterID = value; }
                public int speakingCharacterIndex { get => _speakingCharacterIndex; set => _speakingCharacterIndex = value; }
                public SerializableDictionary<string, string> conversationCharacters { get => _conversationCharacters; set => _conversationCharacters = value; }
                public Dialogue.FrameDialogueElementType type { get => _type; set => _type = value; }
            }
            [SerializeField]
            public SerializedDialogueValues serializedDialogueValues;

            public void SetSerializedDialogueValues() {
                serializedDialogueValues.nextKeyID = nextKeyID;
                serializedDialogueValues.previousKeyID = previousKeyID;
                serializedDialogueValues.text = text;
                serializedDialogueValues.position = position;
                serializedDialogueValues.activeStatus = activeStatus;
                serializedDialogueValues.size = size;
                serializedDialogueValues.speakingCharacterIndex = speakingCharacterIndex;
                serializedDialogueValues.conversationCharacterID = conversationCharacterID;
                serializedDialogueValues.conversationCharacters = conversationCharacters;
                serializedDialogueValues.type = type;
            }
            public static void LoadSerialzedDialogueValues(List<SerializedDialogueValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new DialogueValues {
                        nextKeyID = svalue.nextKeyID,
                        previousKeyID = svalue.previousKeyID,
                        position = svalue.position,
                        activeStatus = svalue.activeStatus,
                        size = svalue.size,
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
        public interface IFrameUI_DialogueSerialization {
            Vector2 position { get; set; }
            bool activeStatus { get; set; }
            Vector2 size { get; set; }
            string text { get; set; }
            string conversationCharacterID { get; set; }
            int speakingCharacterIndex { get; set; }
            SerializableDictionary<string, string> conversationCharacters { get; set; }
            Dialogue.FrameDialogueElementType type { get; set; }
        }
    }
    namespace UI {
        public class Dialogue : Window, Serialization.IFrameUI_DialogueSerialization, IKeyTransition {
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
                        return this.GetComponent<RectTransform>().sizeDelta;
                    }
                    catch (System.Exception) {
                        if (this != null)
                            return this.GetComponent<RectTransform>().sizeDelta;
                        else
                            return frameElementObject.prefab.GetComponent<RectTransform>().sizeDelta;
                    }
                }
                set {
                    this.GetComponent<RectTransform>().sizeDelta = value;
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
            public string characterNameField {
                get {
                    if (this != null)
                        return this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
                    else return frameElementObject.prefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
                }
                set {
                    if (conversationCharacterID != null)
                        this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = conversationCharacters.Values.ToList()[speakingCharacterIndex].Split('_')[0];
                }
            }
            public string conversationCharacterID { get; set; }
            public int speakingCharacterIndex { get; set; }
            public SerializableDictionary<string, string> conversationCharacters { get; set; }
            public FrameDialogueElementType type { get; set; }
            public Character currentConversationCharacter { get; set; }
            public CharacterSO currentConversationCharacterSO { get; set; }

            public enum FrameDialogueElementType {
                Одинᅠперсонаж,
                Несколькоᅠперсонажей,
            }
            public enum FrameDialogueState {
                CharacterLine,
                PlayerAnswer,
            }
            private void Update() {
                KeyTransitionInput();
            }
            public void KeyTransitionInput() {
                if (Input.GetKeyDown(KeyCode.D)) {
                    if (nextKeyID != 0) FrameManager.SetKey(this.nextKeyID);
                }
            }
            public bool HasCharacter(string characterID) {
                return characterID == conversationCharacterID ? true : false;
            }

            #region DIALOGUE_SETUP
            public void DialogueStateChange(FrameDialogueState state) {

            }
            public void DialogueTypeChange(FrameDialogueElementType type) {
                var keyValues = GetFrameKeyValues<Serialization.DialogueValues>();
                switch (type) {
                    case FrameDialogueElementType.Одинᅠперсонаж: {
                        this.type = type;
                        RemoveAllConversationCharactersFromScene();
                        LoadConversationCharacter(keyValues.conversationCharacterID, FrameDialogueElementType.Одинᅠперсонаж);
                        break;
                    }
                    case FrameDialogueElementType.Несколькоᅠперсонажей: {
                        this.type = type;
                        RemoveAllConversationCharactersFromScene();
                        foreach (var characterID in keyValues.conversationCharacters.Values) {
                            LoadConversationCharacter(characterID, FrameDialogueElementType.Несколькоᅠперсонажей);
                        }
                        break;
                    }
                }
            }
            public void LoadConversationCharacter(string characterID, FrameDialogueElementType type) {
                if (characterID == null) return;
                if (currentConversationCharacter != null && type == FrameDialogueElementType.Одинᅠперсонаж) RemovePreviousCharacterOnScene();

                var characterKeyValues = FrameElement.GetFrameKeyValues<Serialization.CharacterValues>(characterID);

                this.SetConversationCharacterSO();

                this.currentConversationCharacterSO.LoadElementOnScene<Character>(
                    FrameManager.frame.GetPair(FrameManager.frame.GetFrameElementObjectByID(characterID)), characterID, characterKeyValues);

                var character = FrameManager.GetFrameElementOnSceneByID<Character>(characterID);
                if (character != null) {
                    character.dialogueID = this.id;
                    this.currentConversationCharacter = character;
                    this.conversationCharacterID = characterID;

                    character.UpdateValuesFromKey(characterKeyValues);

                    character.SetKeyValuesWhileNotInPlayMode();
                    this.SetKeyValuesWhileNotInPlayMode();
                }
            }
            public void SetConversationCharacter() {
                var keyValues = GetFrameKeyValues<DialogueValues>();
                if (keyValues == null)
                    return;

                this.currentConversationCharacterSO.CreateElementOnScene<Character>(this.currentConversationCharacterSO, this.currentConversationCharacterSO.prefab.transform.position, this.currentConversationCharacterSO.prefab.transform.localScale, out string ID);
                if (FrameManager.GetFrameElementOnSceneByID<Character>(ID) == null)
                    return;
                Character character = FrameManager.GetFrameElementOnSceneByID<Character>(ID);
                character.dialogueID = this.id;
                character.type = Character.CharacterType.Conversation;

                keyValues.conversationCharacterID = ID;
                this.conversationCharacterID = ID;
                this.currentConversationCharacter = character;

                character.SetKeyValuesWhileNotInPlayMode();
                this.SetKeyValuesWhileNotInPlayMode();
                keyValues.conversationCharacters.Add(this.currentConversationCharacterSO.id, currentConversationCharacter.id);
                foreach (var key in FrameManager.frame.frameKeys) {
                    key.UpdateFrameKeyValues(character.id, character.GetFrameKeyValues<CharacterValues>());
                    //key.UpdateFrameKeyValues(this.id, this.GetFrameKeyValues<FrameUI_DialogueValues>());
                }
            }
            public void SetConversationCharacterSO() {
                var keyValues = GetFrameKeyValues<DialogueValues>();
                var frameEditorSO = FrameManager.assetDatabase;

                for (int i = 0; i < frameEditorSO.GetFrameElementsOfType<CharacterSO>().Count; i++) {
                    if (i == keyValues.speakingCharacterIndex) {
                        this.currentConversationCharacterSO = frameEditorSO.GetFrameElementsOfType<CharacterSO>()[i];
                    }
                }
            }
            public void RemoveAllConversationCharactersFromScene() {
                var keyValues = GetFrameKeyValues<DialogueValues>();
                foreach (var character in keyValues.conversationCharacters) {
                    if (FrameManager.GetFrameElementOnSceneByID<Character>(character.Value))
                        FrameManager.RemoveElement(character.Value);
                }
            }
            public void RemovePreviousCharacterOnScene() => FrameManager.RemoveElement(this.currentConversationCharacter.id);
            #endregion

            #region VALUES_SETTINGS
            public TextMeshProUGUI GetTextComponent() {
                if (this != null)
                    return this.gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
                else return frameElementObject.prefab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            }
            public override Values GetFrameKeyValuesType() {
                return new DialogueValues(this);
            }
            public override void UpdateValuesFromKey(Values frameKeyValues) {
                var keyValues = (DialogueValues)frameKeyValues;

                nextKeyID = keyValues.nextKeyID;
                previousKeyID = keyValues.previousKeyID;
                activeStatus = keyValues.activeStatus;
                position = keyValues.position;
                size = keyValues.size;
                text = keyValues.text;

                conversationCharacterID = keyValues.conversationCharacterID;
                speakingCharacterIndex = keyValues.speakingCharacterIndex;
                conversationCharacters = keyValues.conversationCharacters;
                type = keyValues.type;

                characterNameField = characterNameField;
            }
            public void UpdateConversationCharacterFromKey(Values frameKeyValues) {
                var keyValues = (DialogueValues)frameKeyValues;

                switch (keyValues.type) {
                    case FrameDialogueElementType.Одинᅠперсонаж: {
                        RemoveAllConversationCharactersFromScene();
                        this.LoadConversationCharacter(keyValues.conversationCharacterID, keyValues.type);
                        break;
                    }
                    case FrameDialogueElementType.Несколькоᅠперсонажей: {
                        RemoveAllConversationCharactersFromScene();
                        foreach (var characterID in keyValues.conversationCharacters.Values) {
                            LoadConversationCharacter(characterID, FrameDialogueElementType.Несколькоᅠперсонажей);
                        }
                        break;
                    }
                }
            }
            #endregion

            #region EDITOR
#if UNITY_EDITOR
            [CustomEditor(typeof(Dialogue))]
            [CanEditMultipleObjects]
            public class FrameUIDialogueWindowCustomInspector : FrameElementCustomInspector {
                public override void OnInspectorGUI() {
                    Dialogue dialogue = (Dialogue)target;
                    EditorUtility.SetDirty(dialogue);

                    dialogue.position = dialogue.GetComponent<RectTransform>().anchoredPosition;
                    dialogue.size = dialogue.GetComponent<RectTransform>().sizeDelta;

                    dialogue.SetKeyValuesWhileNotInPlayMode();
                    if (targets.Length > 1) {
                        foreach (var target in targets) {
                            FrameElement mTarget = (FrameElement)target;
                            dialogue.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                            dialogue.size = dialogue.GetComponent<RectTransform>().sizeDelta;
                            mTarget.SetKeyValuesWhileNotInPlayMode();
                        }
                    }

                    this.SetElementInInspector<DialogueSO>();
                }
            }
#endif
            #endregion
        }
    }
}