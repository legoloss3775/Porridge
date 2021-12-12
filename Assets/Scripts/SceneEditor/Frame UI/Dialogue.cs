using FrameCore.ScriptableObjects;
using FrameCore.ScriptableObjects.UI;
using FrameCore.Serialization;
using FrameCore.UI;
using System;
using System.Collections;
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
        public class DialogueValues : Values {
            public KeySequenceData keySequenceData;
            public DialogueTextData dialogueTextData;
            public DialogueValues(Dialogue dialogue) {
                keySequenceData = new KeySequenceData {
                    nextKeyID = dialogue.nextKeyID,
                    previousKeyID = dialogue.previousKeyID,
                };
                transformData = new TransformData {
                    position = dialogue.position,
                    activeStatus = dialogue.activeStatus,
                    size = dialogue.size,
                };
                dialogueTextData = new DialogueTextData {
                    text = dialogue.text,
                    conversationCharacterID = dialogue.conversationCharacterID,
                    speakingCharacterIndex = dialogue.speakingCharacterIndex,
                    conversationCharacters = dialogue.conversationCharacters,
                    type = dialogue.type,
                    textAnimationTime = dialogue.textAnimationTime,
                    autoContinue = dialogue.autoContinue,
                };
            }
            public DialogueValues() { }
            [Serializable]
            public struct SerializedDialogueValues {
                public TransformData transformData;
                public KeySequenceData keySequenceData;
                public DialogueTextData dialogueTextData;
            }
            [SerializeField]
            public SerializedDialogueValues serializedDialogueValues {
                get {
                    return new SerializedDialogueValues {
                        transformData = transformData,
                        keySequenceData = keySequenceData,
                        dialogueTextData = dialogueTextData,
                    };
                }
            }
            public static void LoadSerialzedDialogueValues(List<SerializedDialogueValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new DialogueValues {
                        transformData = svalue.transformData,
                        keySequenceData = svalue.keySequenceData,
                        dialogueTextData = svalue.dialogueTextData,
                    });
                }
            }
        }
        #endregion
    }
    namespace UI {
        public class Dialogue : Window, IKeyTransition {
            public override Vector2 position {
                get {
                    return this.GetComponent<RectTransform>().anchoredPosition;
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
            public float textAnimationTime = 0.065f;
            public bool autoContinue { get; set; }

            float autoContinueWait = 0;

            public enum FrameDialogueElementType {
                Одинᅠперсонаж,
                Несколькоᅠперсонажей,
            }
            private void Start() {
                if (activeStatus != false) {
                    FrameController.AddAnimationToQueue(id, true);
                    StartCoroutine(TypeDialogue(text));
                }
            }
            private void Update() {
                KeyTransitionInput();

                if (text != GetFrameKeyValues<DialogueValues>(id).dialogueTextData.text) {
                    FrameController.INPUT_BLOCK = true;
                }
                else FrameController.RemoveAnimationFromQueue(id);

                if(FrameController.animations.Count == 0 && autoContinue) {
                    if(nextKeyID != 0) {
                        autoContinueWait += Time.deltaTime;

                        if(autoContinueWait >= 1f) {
                            FrameManager.SetKey(this.nextKeyID);
                            autoContinueWait = 0;
                        }
                    }
                }
            }
            public void KeyTransitionInput() {
                if (FrameController.INPUT_BLOCK || autoContinue) return;

                if (Input.GetKeyDown(KeyCode.D)) {
                    if (nextKeyID != 0) {
                        FrameManager.SetKey(this.nextKeyID);
                    }
                }
            }
            public IEnumerator TypeDialogue(string dialogueText){
                text = "";
                foreach(char letter in dialogueText.ToCharArray()) {
                    text += letter;
                    yield return new WaitForSeconds(textAnimationTime);
                }
            }
            public bool HasCharacter(string characterID) {
                return characterID == conversationCharacterID ? true : false;
            }

            #region DIALOGUE_SETUP
            public void DialogueTypeChange(FrameDialogueElementType type) {
                var keyValues = GetFrameKeyValues<Serialization.DialogueValues>();
                switch (type) {
                    case FrameDialogueElementType.Одинᅠперсонаж: {
                        this.type = type;
                        RemoveAllConversationCharactersFromScene();
                        LoadConversationCharacter(keyValues.dialogueTextData.conversationCharacterID, FrameDialogueElementType.Одинᅠперсонаж);
                        break;
                    }
                    case FrameDialogueElementType.Несколькоᅠперсонажей: {
                        this.type = type;
                        RemoveAllConversationCharactersFromScene();
                        foreach (var characterID in keyValues.dialogueTextData.conversationCharacters.Values) {
                            LoadConversationCharacter(characterID, FrameDialogueElementType.Несколькоᅠперсонажей);
                        }
                        break;
                    }
                }
            }
            public void LoadConversationCharacter(string characterID, FrameDialogueElementType type) {
                //if (characterID == null)
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

                keyValues.dialogueTextData.conversationCharacterID = ID;
                this.conversationCharacterID = ID;
                this.currentConversationCharacter = character;

                character.SetKeyValuesWhileNotInPlayMode();
                this.SetKeyValuesWhileNotInPlayMode();
                keyValues.dialogueTextData.conversationCharacters.Add(this.currentConversationCharacterSO.id, currentConversationCharacter.id);
                foreach (var key in FrameManager.frame.frameKeys) {
                    key.UpdateFrameKeyValues(character.id, character.GetFrameKeyValues<CharacterValues>());
                    //key.UpdateFrameKeyValues(this.id, this.GetFrameKeyValues<FrameUI_DialogueValues>());
                }
            }
            public void SetConversationCharacterSO() {
                var keyValues = GetFrameKeyValues<DialogueValues>();
                var frameEditorSO = FrameManager.assetDatabase;

                for (int i = 0; i < frameEditorSO.GetFrameElementsOfType<CharacterSO>().Count; i++) {
                    if (i == keyValues.dialogueTextData.speakingCharacterIndex) {
                        this.currentConversationCharacterSO = frameEditorSO.GetFrameElementsOfType<CharacterSO>()[i];
                    }
                }
            }
            public void RemoveAllConversationCharactersFromScene() {
                var keyValues = GetFrameKeyValues<DialogueValues>();
                foreach (var character in keyValues.dialogueTextData.conversationCharacters) {
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

                nextKeyID = keyValues.keySequenceData.nextKeyID;
                previousKeyID = keyValues.keySequenceData.previousKeyID;
                activeStatus = keyValues.transformData.activeStatus;
                position = keyValues.transformData.position;
                size = keyValues.transformData.size;
                text = keyValues.dialogueTextData.text;

                conversationCharacterID = keyValues.dialogueTextData.conversationCharacterID;
                speakingCharacterIndex = keyValues.dialogueTextData.speakingCharacterIndex;
                conversationCharacters = keyValues.dialogueTextData.conversationCharacters;
                type = keyValues.dialogueTextData.type;
                textAnimationTime = keyValues.dialogueTextData.textAnimationTime;
                autoContinue = keyValues.dialogueTextData.autoContinue;

                characterNameField = characterNameField;
            }
            public void UpdateConversationCharacterFromKey(Values frameKeyValues) {
                var keyValues = (DialogueValues)frameKeyValues;

                switch (keyValues.dialogueTextData.type) {
                    case FrameDialogueElementType.Одинᅠперсонаж: {
                        RemoveAllConversationCharactersFromScene();
                        this.LoadConversationCharacter(keyValues.dialogueTextData.conversationCharacterID, keyValues.dialogueTextData.type);
                        break;
                    }
                    case FrameDialogueElementType.Несколькоᅠперсонажей: {
                        RemoveAllConversationCharactersFromScene();
                        foreach (var characterID in keyValues.dialogueTextData.conversationCharacters.Values) {
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