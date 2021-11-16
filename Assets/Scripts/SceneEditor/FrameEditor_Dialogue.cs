using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FrameEditor_Dialogue {
    public static void FrameDialogueEditing() {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        foreach (var obj in frameEditorSO.GetFrameElementsOfType<FrameUI_DialogueSO>().ToList())
            if (FrameManager.frame.ContainsFrameElementObject(obj))
                foreach (var frameDialogueID in FrameManager.frame.GetFrameElementIDsByObject(obj).ToList()) {
                    FrameUI_Dialogue dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(frameDialogueID);
                    if (dialogue == null) FrameManager.ChangeFrame();
                    if (dialogue == null) return;

                    FrameUI_DialogueValues values = FrameElement.GetFrameKeyValues<FrameUI_DialogueValues>(frameDialogueID);
                    EditorUtility.SetDirty(dialogue);

                    if (!Application.isPlaying) {
                        values.text = GUILayout.TextArea(values.text, GUILayout.MaxHeight(100));
                        dialogue.text = values.text;

                        FrameUIDialogueCharacterSelection(dialogue);
                        UpdateFrameUIDialogueCharacter(dialogue);
                    }
                    else {
                        dialogue.text = GUILayout.TextArea(dialogue.text, GUILayout.MaxHeight(100));
                    }
                }
    }
    public static void FrameUIDialogueCharacterSelection(FrameUI_Dialogue dialogue) {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        var values = dialogue.GetFrameKeyValues<FrameUI_DialogueValues>();

        values.type = (FrameUI_Dialogue.FrameDialogueElementType)EditorGUILayout.EnumPopup("Тип диалога:", values.type);

        if (dialogue.type != values.type) {
            dialogue.DialogueTypeChange(values.type);
            dialogue.SetKeyValuesWhileNotInPlayMode<FrameUI_DialogueValues>();
        } 

        switch (dialogue.type) {
            case FrameUI_Dialogue.FrameDialogueElementType.OneCharacter: {
                OneSpeakingCharacterSelection();
                break;
            }
            case FrameUI_Dialogue.FrameDialogueElementType.MultibleCharacters: {
                AddMultibleCharacters();
                MuiltibleSpeakingCharactersSelection();
                break;
            }
        }

        for (int i = 0; i < frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>().Count; i++) {
            if (i == values.speakingCharacterIndex) {
                dialogue.currentConversationCharacterSO = frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>()[i];
            }
        }
        void OneSpeakingCharacterSelection() {
            values.speakingCharacterIndex = EditorGUILayout.Popup(
                    "Собеседник:",
                    values.speakingCharacterIndex,
                    frameEditorSO.GetFrameElementsObjectsNames<FrameCharacterSO>().ToArray()
                    );
            dialogue.speakingCharacterIndex = values.speakingCharacterIndex;
        }
        void MuiltibleSpeakingCharactersSelection() {
            var names = new List<string>();
            foreach (var character in values.conversationCharacters)
                names.Add(character.Value.Split('_')[0]);
            values.speakingCharacterIndex = GUILayout.SelectionGrid(values.speakingCharacterIndex, names.ToArray(), 6);
            dialogue.speakingCharacterIndex = values.speakingCharacterIndex;
        }
        void AddMultibleCharacters() {
            if (GUILayout.Button("Добавить персонажа")) {

            }
        }
    }
    public static void UpdateFrameUIDialogueCharacter(FrameUI_Dialogue dialogue) {
        var values = dialogue.GetFrameKeyValues<FrameUI_DialogueValues>();
        var key = FrameManager.frame.currentKey;
        var characterIDs = new List<string>();

        bool firstCharacterCreated = false;
        bool characterWasCreatedPreviously = false;
        bool selectionChange = false;

        FrameCharacterValues characterKeyValues = null;

        characterIDs = FrameManager.frame.GetFrameElementIDsByObject(dialogue.currentConversationCharacterSO) ?? new List<string>();

        foreach (var characterID in characterIDs)
            if (key.ContainsID(characterID)) {
                characterKeyValues = FrameElement.GetFrameKeyValues<FrameCharacterValues>(characterID);
                if (characterKeyValues.dialogueID == dialogue.id && characterID == values.conversationCharacterID)
                    break;
            }

        if (values.conversationCharacterID != null)
            firstCharacterCreated = true;


        switch (dialogue.type) {
            case FrameUI_Dialogue.FrameDialogueElementType.OneCharacter: {
                if (!firstCharacterCreated)
                    dialogue.SetConversationCharacter();

                foreach (string id in values.conversationCharacters.Keys)
                    if (dialogue.currentConversationCharacterSO.id == id)
                        characterWasCreatedPreviously = true;

                if (dialogue.currentConversationCharacter != null &&
                    dialogue.currentConversationCharacterSO != null &&
                    dialogue.currentConversationCharacterSO != dialogue.currentConversationCharacter.frameElementObject)
                    selectionChange = true;

                if (selectionChange) {
                    if (characterWasCreatedPreviously)
                        foreach (var character in values.conversationCharacters) {
                            if (character.Key == dialogue.currentConversationCharacterSO.id) {
                                dialogue.LoadConversationCharacter(character.Value, dialogue.type);
                                SetPreviousCharacterValues();
                                break;
                            }
                        }
                    else {
                        dialogue.RemovePreviousCharacterOnScene();
                        dialogue.SetConversationCharacter();
                        SetPreviousCharacterValues();
                    }
                }
                break;
            }
            case FrameUI_Dialogue.FrameDialogueElementType.MultibleCharacters: {
                if (!firstCharacterCreated)
                    dialogue.SetConversationCharacter();

                break;
            }
        }
        void SetPreviousCharacterValues() {
            foreach (var previousCharacterValue in FrameManager.frame.frameKeys.Where(ch => !ch.ContainsID(dialogue.conversationCharacterID))) {
                if (characterKeyValues == null) characterKeyValues = (FrameCharacterValues)FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(dialogue.conversationCharacterID).GetFrameKeyValuesType();
                previousCharacterValue.AddFrameKeyValues(dialogue.conversationCharacterID, characterKeyValues);
            }
        }
    }
}
