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
                    rotation = dialogue.rotation.eulerAngles,
                };
                dialogueTextData = new DialogueTextData {
                    text = dialogue.text,
                    conversationCharacterID = dialogue.conversationCharacterID,
                    speakingCharacterIndex = dialogue.speakingCharacterIndex,
                    conversationCharacters = dialogue.conversationCharacters,
                    type = dialogue.type,
                    textAnimationTime = dialogue.textAnimationTime,
                    autoContinue = dialogue.autoContinue,
                    transitionDelay = dialogue.transitionDelay,
                    textAnimationDelay = dialogue.textAnimationDelay,
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
        [ExecuteInEditMode]
        public class Dialogue : Window, IKeyTransition {
            public override Vector3 position {
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
            public override Quaternion rotation {
                get {
                    try {
                        return this.GetComponent<RectTransform>().localRotation;
                    }
                    catch (System.Exception) {
                        if (this != null)
                            return this.GetComponent<RectTransform>().localRotation;
                        else
                            return frameElementObject.prefab.GetComponent<RectTransform>().localRotation;
                    }
                }
                set {
                    this.GetComponent<RectTransform>().localRotation = value;
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
                        return this.gameObject.transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text;
                    else return frameElementObject.prefab.transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text;
                }
                set {
                    if (conversationCharacterID != null) {
                        this.gameObject.transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = value;
                    }
                }
            }
            public string conversationCharacterID { get; set; }
            public int speakingCharacterIndex { get; set; }
            public SerializableDictionary<string, string> conversationCharacters { get; set; }
            public FrameDialogueElementType type { get; set; }
            public Character currentConversationCharacter { get; set; }
            public CharacterSO currentConversationCharacterSO { get; set; }
            public float textAnimationTime = 0.0165f;
            public bool autoContinue { get; set; }
            public float transitionDelay = 1f;
            public float textAnimationDelay = 0;

            bool textPlayed = false;
            bool textColorChanged = false;

            Color32[] colors;
            float autoContinueWait = 0;

            public enum FrameDialogueElementType {
                Одинᅠперсонаж,
                Несколькоᅠперсонажей,
            }
            private void Start() {
                if (activeStatus == false) return;
                GetTextComponent().OnPreRenderText += HideText;
                textPlayed = false;
                textColorChanged = false;
            }
            private void Update() {
                if (!textColorChanged && !textPlayed) {
                    textColorChanged = true;
                }
                if (!textPlayed && textColorChanged) {
                    FrameController.AddAnimationToQueue(id, true);
                    StartCoroutine(TypeDialogue(text));
                    textPlayed = true;
                }

                KeyTransitionInput();

                UpdateText();
            }
            public override void OnKeyChanged() {
                if (activeStatus != false) {
                    textPlayed = false;
                    //StartCoroutine(HideText());
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
            public void UpdateText() {
                if (!Application.isPlaying) return;

                if (FrameController.animations.Count == 0 && autoContinue) {
                    if (nextKeyID != 0) {
                        autoContinueWait += Time.deltaTime;

                        if (autoContinueWait >= transitionDelay) {
                            FrameManager.SetKey(this.nextKeyID);
                            autoContinueWait = 0;
                        }
                    }
                }
            }
            //
           /** public void InsureHiddenText() {
                var TMProText = GetTextComponent();
                for (int i = 0; i < TMProText.textInfo.characterCount; i++) {

                    try {
                        int meshIndex = TMProText.textInfo.characterInfo[i].materialReferenceIndex;
                        int vertexIndex = TMProText.textInfo.characterInfo[i].vertexIndex;
                        Color32[] vertexColors = TMProText.textInfo.meshInfo[meshIndex].colors32;
                        if (!vertexColors[vertexIndex + 0].CompareRGB(new Color32(0, 255, 255, 50)) ||
                        vertexColors[vertexIndex + 1] != Color.clear ||
                        vertexColors[vertexIndex + 2] != Color.clear ||
                        vertexColors[vertexIndex + 3] != Color.clear) {
                            StartCoroutine(HideText());
                        }
                        TMProText.UpdateVertexData();
                    }
                    catch (System.Exception) { }
                }
            }**/
            public void HideText(TMPro.TMP_TextInfo textInfo) {
                Color32 blank = new Color32(0, 0, 0, 0);

                colors = new Color32[textInfo.characterCount];

                for (int i = 0; i < textInfo.characterCount; i++) {

                    try {
                        int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
                        int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                        if(textInfo.characterInfo[i].color != Color.clear)
                            colors[i] = textInfo.characterInfo[i].color;
                        Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;
                        vertexColors[vertexIndex + 0] = new Color32(0,255,255, 50);
                        vertexColors[vertexIndex + 1] = Color.clear;
                        vertexColors[vertexIndex + 2] = Color.clear;
                        vertexColors[vertexIndex + 3] = Color.clear;
                    }
                    catch (System.Exception) { }
                }
            }
            public IEnumerator TypeDialogue(string dialogueText){

                
                //text = "";
                characterNameField = "";
                /**StartCoroutine(HideText());**/

                var TMProText = GetTextComponent();
               //+ Color32[] colors = new Color32[TMProText.textInfo.characterCount];
                Color32 blank = new Color32(0, 0, 0, 0);

                yield return new WaitForSeconds(textAnimationDelay);

                switch (type) {
                    case FrameDialogueElementType.Одинᅠперсонаж:
                        characterNameField = conversationCharacters.Where(ch => ch.Value == conversationCharacterID).FirstOrDefault().Key;
                        break;
                    case FrameDialogueElementType.Несколькоᅠперсонажей:
                        characterNameField = conversationCharacters.Keys.ToList()[speakingCharacterIndex];
                        break;
                }
                /**for (int i = 0; i < TMProText.textInfo.characterCount; i++) {

                    try {
                        int meshIndex = TMProText.textInfo.characterInfo[i].materialReferenceIndex;
                        int vertexIndex = TMProText.textInfo.characterInfo[i].vertexIndex;
                        colors[i] = TMProText.textInfo.characterInfo[i].color;
                        Color32[] vertexColors = TMProText.textInfo.meshInfo[meshIndex].colors32;
                        vertexColors[vertexIndex + 0] = new Color32(0, 255, 255, 50);
                        vertexColors[vertexIndex + 1] = Color.clear;
                        vertexColors[vertexIndex + 2] = Color.clear;
                        vertexColors[vertexIndex + 3] = Color.clear;
                        TMProText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                    }
                    catch (System.Exception) { }
                }**/
                for (int i = 0; i < dialogueText.Length; i++) {
                    int meshIndex = TMProText.textInfo.characterInfo[i].materialReferenceIndex;
                    int vertexIndex = TMProText.textInfo.characterInfo[i].vertexIndex;
                    try {
                       // Debug.Log(colors[i]);
                        Color32[] vertexColors = TMProText.textInfo.meshInfo[meshIndex].colors32;
                        vertexColors[vertexIndex + 0] = colors[i];
                        vertexColors[vertexIndex + 1] = colors[i];
                        vertexColors[vertexIndex + 2] = colors[i];
                        vertexColors[vertexIndex + 3] = colors[i];
                        TMProText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                    }
                    catch (System.Exception) { }
                    yield return new WaitForSeconds(textAnimationTime);
                }

                /**foreach (char letter in dialogueText.ToCharArray()) {
                    text += letter;
                    yield return new WaitForSeconds(textAnimationTime);
                }**/

                yield return new WaitForSeconds(transitionDelay);
                textColorChanged = false;
                FrameController.RemoveAnimationFromQueue(id);
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
                //if (characterID == null)//
                if (currentConversationCharacter != null && type == FrameDialogueElementType.Одинᅠперсонаж && conversationCharacterID != characterID) RemovePreviousCharacterOnScene();

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

                    var values = (DialogueValues)key.GetFrameKeyValuesOfElement(id);
                    if (!values.dialogueTextData.conversationCharacters.ContainsKey(currentConversationCharacterSO.id)) {
                        values.dialogueTextData.conversationCharacters.Add(currentConversationCharacterSO.id, conversationCharacterID);
                    }
                   // key.UpdateFrameKeyValues(this.id, this.GetFrameKeyValues<Serialization.DialogueValues>());
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
                    switch (keyValues.dialogueTextData.type) {
                        case FrameDialogueElementType.Одинᅠперсонаж:
                            if (FrameManager.GetFrameElementOnSceneByID<Character>(character.Value))
                                FrameManager.RemoveElement(character.Value);
                            break;
                        case FrameDialogueElementType.Несколькоᅠперсонажей:
                            if (FrameManager.GetFrameElementOnSceneByID<Character>(character.Value))
                                FrameManager.RemoveElement(character.Value);
                            break;
                    }
                }
            }
            public void RemovePreviousCharacterOnScene() => FrameManager.RemoveElement(this.currentConversationCharacter.id);
            #endregion

            #region VALUES_SETTINGS
            public TextMeshProUGUI GetTextComponent() {
                if (this != null)
                    return this.gameObject.transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>();
                else return frameElementObject.prefab.transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>();
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
                rotation = Quaternion.Euler(keyValues.transformData.rotation);
                text = keyValues.dialogueTextData.text;

                conversationCharacterID = keyValues.dialogueTextData.conversationCharacterID;
                speakingCharacterIndex = keyValues.dialogueTextData.speakingCharacterIndex;
                conversationCharacters = keyValues.dialogueTextData.conversationCharacters;
                type = keyValues.dialogueTextData.type;
                textAnimationTime = keyValues.dialogueTextData.textAnimationTime;
                autoContinue = keyValues.dialogueTextData.autoContinue;
                transitionDelay = keyValues.dialogueTextData.transitionDelay;
                textAnimationDelay = keyValues.dialogueTextData.textAnimationDelay;

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
                    dialogue.rotation = dialogue.GetComponent<RectTransform>().localRotation;

                    dialogue.SetKeyValuesWhileNotInPlayMode();
                    if (targets.Length > 1) {
                        foreach (var target in targets) {
                            FrameElement mTarget = (Dialogue)target;
                            mTarget.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                            mTarget.size = mTarget.GetComponent<RectTransform>().sizeDelta;
                            mTarget.rotation = mTarget.GetComponent<RectTransform>().localRotation;
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