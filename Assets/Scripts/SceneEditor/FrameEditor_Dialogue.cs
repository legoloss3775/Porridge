﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class FrameEditor_Dialogue {
    public static void FrameDialogueEditing() {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        foreach (var obj in frameEditorSO.GetFrameElementsOfType<FrameUI_DialogueSO>().ToList())
            if (FrameManager.frame.ContainsFrameElementObject(obj))
                foreach (var frameDialogueID in FrameManager.frame.GetFrameElementIDsByObject(obj).ToList()) {
                    var dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(frameDialogueID);
                    if (dialogue == null) FrameManager.ChangeFrame();
                    if (dialogue == null) return;

                    var keyValues = FrameElement.GetFrameKeyValues<FrameUI_DialogueValues>(frameDialogueID);
                    EditorUtility.SetDirty(dialogue);

                    if (!Application.isPlaying) {
                        keyValues.text = GUILayout.TextArea(keyValues.text, GUILayout.MaxHeight(100));
                        dialogue.text = keyValues.text;

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
        var keyValues = dialogue.GetFrameKeyValues<FrameUI_DialogueValues>();

        keyValues.type = (FrameUI_Dialogue.FrameDialogueElementType)EditorGUILayout.EnumPopup("Тип диалога:", keyValues.type);

        if (dialogue.type != keyValues.type) {
            dialogue.DialogueTypeChange(keyValues.type);
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
            if (i == keyValues.speakingCharacterIndex) {
                dialogue.currentConversationCharacterSO = frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>()[i];
            }
        }
        void OneSpeakingCharacterSelection() {
            keyValues.speakingCharacterIndex = EditorGUILayout.Popup(
                    "Собеседник:",
                    keyValues.speakingCharacterIndex,
                    frameEditorSO.GetFrameElementsObjectsNames<FrameCharacterSO>().ToArray()
                    );
            dialogue.speakingCharacterIndex = keyValues.speakingCharacterIndex;
        }
        void MuiltibleSpeakingCharactersSelection() {
            var names = new List<string>();
            foreach (var character in keyValues.conversationCharacters)
                names.Add(character.Value.Split('_')[0]);
            keyValues.speakingCharacterIndex = GUILayout.SelectionGrid(keyValues.speakingCharacterIndex, names.ToArray(), 6);
            dialogue.speakingCharacterIndex = keyValues.speakingCharacterIndex;
        }
        void AddMultibleCharacters() {
            if (GUILayout.Button("Добавить персонажа")) {

            }
        }
    }
    public static void UpdateFrameUIDialogueCharacter(FrameUI_Dialogue dialogue) {
        var keyValues = dialogue.GetFrameKeyValues<FrameUI_DialogueValues>();
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
                if (characterKeyValues.dialogueID == dialogue.id && characterID == keyValues.conversationCharacterID)
                    break;
            }

        if (keyValues.conversationCharacterID != null)
            firstCharacterCreated = true;


        switch (dialogue.type) {
            case FrameUI_Dialogue.FrameDialogueElementType.OneCharacter: {
                if (!firstCharacterCreated)
                    dialogue.SetConversationCharacter();

                foreach (string id in keyValues.conversationCharacters.Keys)
                    if (dialogue.currentConversationCharacterSO.id == id)
                        characterWasCreatedPreviously = true;

                if (dialogue.currentConversationCharacter != null &&
                    dialogue.currentConversationCharacterSO != null &&
                    dialogue.currentConversationCharacterSO != dialogue.currentConversationCharacter.frameElementObject)
                    selectionChange = true;

                if (selectionChange) {
                    if (characterWasCreatedPreviously)
                        foreach (var character in keyValues.conversationCharacters) {
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
#endif
