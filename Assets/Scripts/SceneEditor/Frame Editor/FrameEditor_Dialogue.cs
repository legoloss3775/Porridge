using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class FrameEditor_Dialogue : FrameEditor {

    public static bool characterCreationButtonPressed = false;
    public static void FrameDialogueEditing() {
        Action<FrameUI_Dialogue> dialogueCharacterSelection = FrameUI_DialogueCharacterSelection;
        Action<FrameUI_Dialogue> textEditing = FrameUI_TextEditing;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        foldouts[EditorType.DialogueEditor] = EditorGUILayout.Foldout(foldouts[EditorType.DialogueEditor], "Диалоги", EditorStyles.foldoutHeader);
        GUILayout.FlexibleSpace();
        ElementCreation(FrameEditor_CreationWindow.CreationType.FrameDialogue);
        GUILayout.EndHorizontal();

        if (foldouts[EditorType.DialogueEditor]) {
            GUILayout.BeginVertical("HelpBox");
            ElementEditing<FrameUI_DialogueSO, FrameUI_Dialogue>(PositioningType.Vertical, EditorType.DialogueEditor, false, false, dialogueCharacterSelection, textEditing);
            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();
    }
    private static void FrameUI_TextEditing(FrameUI_Dialogue dialogue) {
        var keyValues = dialogue.GetFrameKeyValues<FrameUI_DialogueValues>();
        if (keyValues == null)
            return;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Текст диалога:", GUILayout.MaxWidth(95));
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        FrameGUIUtility.GuiLine();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        keyValues.text = GUILayout.TextArea(keyValues.text, GUILayout.MaxWidth(450), GUILayout.MaxHeight(150));
        if (dialogue.text != keyValues.text) dialogue.text = keyValues.text;
        FrameGUIUtility.GuiLine();
    }
    private static void FrameUI_DialogueCharacterSelection(FrameUI_Dialogue dialogue) {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        var keyValues = dialogue.GetFrameKeyValues<FrameUI_DialogueValues>();
        if (keyValues == null) return;

        float dialogueTypeSelectionWidth;
        if (dialogue.type == FrameUI_Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей)
            dialogueTypeSelectionWidth = 425;
        else
            dialogueTypeSelectionWidth = 450;

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        ElementSelection(dialogue);
        ElementDeletion(dialogue);
        GUILayout.EndVertical();
        GUILayout.Label(dialogue.id, EditorStyles.largeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        keyValues.type = (FrameUI_Dialogue.FrameDialogueElementType)EditorGUILayout.EnumPopup(keyValues.type, GUILayout.MaxWidth(dialogueTypeSelectionWidth));
        if(keyValues.type == FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж) {
            GUILayout.EndHorizontal();
        }

        if(keyValues.type == FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж) {
            EditorGUILayout.Separator();
        }

        if (dialogue.type != keyValues.type) {
            dialogue.DialogueTypeChange(keyValues.type);
            dialogue.SetKeyValuesWhileNotInPlayMode<FrameUI_DialogueValues>();
        } 

        switch (dialogue.type) {
            case FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж: {
                OneSpeakingCharacterSelection();
                break;
            }
            case FrameUI_Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей: {
                AddMultibleCharacters();

                GUILayout.EndHorizontal();

                EditorGUILayout.Separator();

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
                    frameEditorSO.GetFrameElementsObjectsNames<FrameCharacterSO>().ToArray(),
                    GUILayout.MaxWidth(450)
                    );
            dialogue.speakingCharacterIndex = keyValues.speakingCharacterIndex;
        }
        void MuiltibleSpeakingCharactersSelection() {
            var names = new List<string>();
            foreach (var character in keyValues.conversationCharacters)
                names.Add(character.Value.Split('_')[0]);
            GUILayout.Label("Выбор говорящего:");
            keyValues.speakingCharacterIndex = GUILayout.SelectionGrid(
                keyValues.speakingCharacterIndex, 
                names.ToArray(), 
                4,
                GUILayout.MaxWidth(450)
                );
            dialogue.speakingCharacterIndex = keyValues.speakingCharacterIndex;
        }
        void AddMultibleCharacters() {
            if (GUILayout.Button("+", GUILayout.MaxWidth(22.5f))) {
                var editor = EditorWindow.GetWindow<FrameEditor_CreationWindow>();
                editor.type = FrameEditor_CreationWindow.CreationType.FrameCharacter;
                characterCreationButtonPressed = true;
            }
            if (characterCreationButtonPressed && FrameEditor_CreationWindow.createdElementID != "") {
                var character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(FrameEditor_CreationWindow.createdElementID);
                if (character != null && !dialogue.conversationCharacters.ContainsKey(character.frameElementObject.name)) {
                    character.type = FrameCharacter.CharacterType.Conversation;
                    character.dialogueID = dialogue.id;
                    dialogue.conversationCharacters.Add(character.frameElementObject.name, character.id);
                    dialogue.currentConversationCharacterSO = (FrameCharacterSO)character.frameElementObject;
                    dialogue.conversationCharacterID = character.id;

                    character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues>();
                    dialogue.SetKeyValuesWhileNotInPlayMode<FrameUI_DialogueValues>();

                    FrameEditor_CreationWindow.createdElementID = "";
                    characterCreationButtonPressed = false;
                }
                else {
                    FrameManager.frame.RemoveElementFromCurrentKey(FrameEditor_CreationWindow.createdElementID);
                    characterCreationButtonPressed = false;
                }
            }
        }

        UpdateFrameUI_DialogueCharacter(dialogue);
    }
    private static void UpdateFrameUI_DialogueCharacter(FrameUI_Dialogue dialogue) {
        var keyValues = dialogue.GetFrameKeyValues<FrameUI_DialogueValues>();
        if (keyValues == null)
            return;

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

        if (keyValues != null && keyValues.conversationCharacterID != null)
            firstCharacterCreated = true;


        switch (dialogue.type) {
            case FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж: {
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
            case FrameUI_Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей: {
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
