using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using FrameCore;
using FrameCore.ScriptableObjects;
using FrameCore.Serialization;
using FrameCore.UI;
using FrameCore.ScriptableObjects.UI;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс редактора диалоговых окон
    /// </summary>
    public class Dialogue : Core {

        public static bool characterCreationButtonPressed = false; //проверка нажатия на кнопку создания элемента, без нее создавалось бы бесконечное количество привязанных
                                                                   //к диалогу персонажей

        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        /// В данном случае этот редактор работает одновременно для 
        /// <see cref="FrameCore.UI.Dialogue">
        /// <see cref="FrameCore.UI.DialogueAnswer">
        public static void FrameDialogueEditing() {
            Action<FrameCore.UI.Dialogue> dialogueCharacterSelection = FrameUI_DialogueCharacterSelection;
            Action<FrameCore.UI.Dialogue> textEditing = FrameUI_TextEditing;

            Action<DialogueAnswer> answerEditing = FrameUI_DialogueAnswerEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.DialogueEditor] = EditorGUILayout.Foldout(foldouts[EditorType.DialogueEditor], "Диалоги", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameDialogue);
            GUILayout.Label("Ответы");
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameDialogueAnswer);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.DialogueEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<DialogueSO, FrameCore.UI.Dialogue>(PositioningType.Vertical, EditorType.DialogueEditor, false, false, dialogueCharacterSelection, textEditing);
                GUILayout.EndVertical();
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<DialogueAnswerSO, DialogueAnswer>(PositioningType.Vertical, EditorType.DialogueEditor, false, false, answerEditing);
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
        //Редактирование конкретно окна ответов
        private static void FrameUI_DialogueAnswerEditing(DialogueAnswer dialogueAnswer) {
            var keyValues = dialogueAnswer.GetFrameKeyValues<DialogueAnswerValues>();
            if (keyValues == null) return;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(dialogueAnswer); //кнопка выделения элемента
            ElementActiveStateChange<DialogueAnswer>(dialogueAnswer); //кнопка изменения статуса активности элемента
            ElementDeletion(dialogueAnswer); //кнопка удаления элемента
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Label(dialogueAnswer.id, EditorStyles.largeLabel);
            if (dialogueAnswer.activeStatus == false) {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Ответ:", GUILayout.MaxWidth(95));
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            keyValues.dialogueAnswerTextData.text = GUILayout.TextArea(keyValues.dialogueAnswerTextData.text, FrameGUIUtility.GetTextAreaStyle(Color.white, 15), GUILayout.MaxWidth(450), GUILayout.MaxHeight(150));
            if (dialogueAnswer.text != keyValues.dialogueAnswerTextData.text) dialogueAnswer.text = keyValues.dialogueAnswerTextData.text;
            FrameGUIUtility.GuiLine();
        }
        private static void FrameUI_TextEditing(FrameCore.UI.Dialogue dialogue) {
            if (dialogue.activeStatus == false) return;

            var keyValues = dialogue.GetFrameKeyValues<DialogueValues>();
            if (keyValues == null)
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Текст диалога:", GUILayout.MaxWidth(95));
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            keyValues.dialogueTextData.text = GUILayout.TextArea(keyValues.dialogueTextData.text, FrameGUIUtility.GetTextAreaStyle(Color.white, 15), GUILayout.MaxWidth(450), GUILayout.MaxHeight(150));
            if (dialogue.text != keyValues.dialogueTextData.text) dialogue.text = keyValues.dialogueTextData.text;

            GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Параметры:", EditorStyles.boldLabel, GUILayout.MaxWidth(80));
            GUILayout.BeginVertical();
            GUILayout.Space(9);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            keyValues.dialogueTextData.autoContinue = GUILayout.Toggle(keyValues.dialogueTextData.autoContinue, "Автопродолжение");
            if (dialogue.autoContinue != keyValues.dialogueTextData.autoContinue) dialogue.autoContinue = keyValues.dialogueTextData.autoContinue;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            keyValues.dialogueTextData.textAnimationTime = EditorGUILayout.FloatField("Время анимации текста: ",keyValues.dialogueTextData.textAnimationTime);
            if (dialogue.textAnimationTime != keyValues.dialogueTextData.textAnimationTime) dialogue.textAnimationTime = keyValues.dialogueTextData.textAnimationTime;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();
        }
        //не ну там такой пиздец снизу я не буду это описывать
        //оно просто работает и все
        //НЕ ТРОГАТЬ ЭТОТ КОД
        private static void FrameUI_DialogueCharacterSelection(FrameCore.UI.Dialogue dialogue) {

            var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
            var keyValues = dialogue.GetFrameKeyValues<DialogueValues>();
            if (keyValues == null) return;

            float dialogueTypeSelectionWidth;
            if (dialogue.type == FrameCore.UI.Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей)
                dialogueTypeSelectionWidth = 425;
            else
                dialogueTypeSelectionWidth = 450;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(dialogue); //кнопка выделения элемента
            ElementActiveStateChange(dialogue); //кнопка изменения статуса активности элемента
            ElementDeletion(dialogue); //кнопка удаления элемента
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Label(dialogue.id, EditorStyles.largeLabel);
            if (dialogue.activeStatus == false) {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            FrameGUIUtility.GuiLine();


            GUILayout.BeginHorizontal();

            keyValues.dialogueTextData.type = (FrameCore.UI.Dialogue.FrameDialogueElementType)EditorGUILayout.EnumPopup(keyValues.dialogueTextData.type, GUILayout.MaxWidth(dialogueTypeSelectionWidth));
            if (keyValues.dialogueTextData.type == FrameCore.UI.Dialogue.FrameDialogueElementType.Одинᅠперсонаж) {
                GUILayout.EndHorizontal();
                FrameGUIUtility.GuiLine();
            }

            if (keyValues.dialogueTextData.type == FrameCore.UI.Dialogue.FrameDialogueElementType.Одинᅠперсонаж) {
                EditorGUILayout.Separator();
            }

            if (dialogue.type != keyValues.dialogueTextData.type) {
                dialogue.DialogueTypeChange(keyValues.dialogueTextData.type);
                dialogue.SetKeyValuesWhileNotInPlayMode();
            }

            switch (dialogue.type) {
                case FrameCore.UI.Dialogue.FrameDialogueElementType.Одинᅠперсонаж: {
                    OneSpeakingCharacterSelection();
                    break;
                }
                case FrameCore.UI.Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей: {
                    AddMultibleCharacters();

                    GUILayout.EndHorizontal();
                    FrameGUIUtility.GuiLine();

                    EditorGUILayout.Separator();

                    MuiltibleSpeakingCharactersSelection();
                    break;
                }
            }

            for (int i = 0; i < frameEditorSO.GetFrameElementsOfType<CharacterSO>().Count; i++) {
                if (i == keyValues.dialogueTextData.speakingCharacterIndex) {
                    dialogue.currentConversationCharacterSO = frameEditorSO.GetFrameElementsOfType<CharacterSO>()[i];
                }
            }
            void OneSpeakingCharacterSelection() {
                keyValues.dialogueTextData.speakingCharacterIndex = EditorGUILayout.Popup(
                        "Собеседник:",
                        keyValues.dialogueTextData.speakingCharacterIndex,
                        frameEditorSO.GetFrameElementsObjectsNames<CharacterSO>().ToArray(),
                        FrameGUIUtility.GetPopupStyle(Color.white),
                        GUILayout.MaxWidth(450)
                        );
                dialogue.speakingCharacterIndex = keyValues.dialogueTextData.speakingCharacterIndex;
            }
            void MuiltibleSpeakingCharactersSelection() {
                var names = new List<string>();
                foreach (var character in keyValues.dialogueTextData.conversationCharacters)
                    names.Add(character.Value.Split('_')[0]);
                GUILayout.Label("Выбор говорящего:");
                keyValues.dialogueTextData.speakingCharacterIndex = GUILayout.SelectionGrid(
                    keyValues.dialogueTextData.speakingCharacterIndex,
                    names.ToArray(),
                    4,
                    GUILayout.MaxWidth(450)
                    );
                dialogue.speakingCharacterIndex = keyValues.dialogueTextData.speakingCharacterIndex;
                dialogue.conversationCharacterID = keyValues.dialogueTextData.conversationCharacters.Values.ToList()[keyValues.dialogueTextData.speakingCharacterIndex];
                if (keyValues.dialogueTextData.conversationCharacterID != dialogue.conversationCharacterID)
                    keyValues.dialogueTextData.conversationCharacterID = dialogue.conversationCharacterID;
            }
            void AddMultibleCharacters() {
                if (GUILayout.Button("+", GUILayout.MaxWidth(22.5f))) {
                    var editor = EditorWindow.GetWindow<CreationWindow>();
                    editor.type = CreationWindow.CreationType.FrameCharacter;
                    characterCreationButtonPressed = true;
                }
                if (characterCreationButtonPressed && CreationWindow.createdElementID != "") {
                    var character = FrameManager.GetFrameElementOnSceneByID<FrameCore.Character>(CreationWindow.createdElementID);
                    if (character != null && !dialogue.conversationCharacters.ContainsKey(character.frameElementObject.name)) {
                        character.type = FrameCore.Character.CharacterType.Conversation;
                        character.dialogueID = dialogue.id;
                        dialogue.conversationCharacters.Add(character.frameElementObject.name, character.id);
                        dialogue.currentConversationCharacterSO = (CharacterSO)character.frameElementObject;
                        dialogue.conversationCharacterID = character.id;

                        character.SetKeyValuesWhileNotInPlayMode();
                        dialogue.SetKeyValuesWhileNotInPlayMode();

                        CreationWindow.createdElementID = "";
                        characterCreationButtonPressed = false;
                    }
                    else {
                        FrameManager.frame.RemoveElementFromCurrentKey(CreationWindow.createdElementID);
                        characterCreationButtonPressed = false;
                    }
                }
            }

            UpdateFrameUI_DialogueCharacter(dialogue);
        }
        //НЕ ТРОГАТЬ ЭТОТ КОД
        //TODO: кто-нибудь, умоляю, исправьте эту функцию
        private static void UpdateFrameUI_DialogueCharacter(FrameCore.UI.Dialogue dialogue) {
            if (dialogue.activeStatus == false) return;

            var keyValues = dialogue.GetFrameKeyValues<DialogueValues>();
            if (keyValues == null)
                return;

            var key = FrameManager.frame.currentKey;
            var characterIDs = new List<string>();

            bool firstCharacterCreated = false;
            bool characterWasCreatedPreviously = false;
            bool selectionChange = false;

            CharacterValues characterKeyValues = null;

            characterIDs = FrameManager.frame.GetFrameElementIDsByObject(dialogue.currentConversationCharacterSO) ?? new List<string>();

            foreach(var characterID in dialogue.conversationCharacters) {
                if (!FrameManager.frame.currentKey.ContainsID(characterID.Value)) {
                    try {
                        FrameManager.frame.currentKey.AddFrameKeyValues(
                            characterID.Value,
                            FrameManager.GetFrameElementOnSceneByID<FrameCore.Character>(characterID.Value).GetFrameKeyValuesType()
                            );
                    }
                    catch (System.Exception) { }
                }
            }

            foreach (var characterID in characterIDs)
                if (key.ContainsID(characterID)) {
                    characterKeyValues = FrameElement.GetFrameKeyValues<CharacterValues>(characterID);
                    if (characterKeyValues.characterData.dialogueID == dialogue.id && characterID == keyValues.dialogueTextData.conversationCharacterID)
                        break;
                }

            if (keyValues != null && keyValues.dialogueTextData.conversationCharacterID != null)
                firstCharacterCreated = true;


            switch (dialogue.type) {
                case FrameCore.UI.Dialogue.FrameDialogueElementType.Одинᅠперсонаж: {
                    if (!firstCharacterCreated)
                        dialogue.SetConversationCharacter();

                    foreach (string id in keyValues.dialogueTextData.conversationCharacters.Keys)
                        if (dialogue.currentConversationCharacterSO.id == id)
                            characterWasCreatedPreviously = true;

                    FrameCore.Character dialogueCharacter = null;
                    foreach(var characterID in dialogue.conversationCharacters) {
                        if (FrameManager.GetFrameElementOnSceneByID<FrameCore.Character>(characterID.Value)?.id == dialogue.conversationCharacterID)
                            dialogueCharacter = FrameManager.GetFrameElementOnSceneByID<FrameCore.Character>(characterID.Value);
                    }
                    if (dialogue.currentConversationCharacter != null &&
                        dialogue.currentConversationCharacterSO != null &&
                        dialogue.currentConversationCharacterSO != dialogue.currentConversationCharacter.frameElementObject)
                        selectionChange = true;


                    if (selectionChange) {
                        if (characterWasCreatedPreviously)
                            foreach (var character in keyValues.dialogueTextData.conversationCharacters) {
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
                    if(dialogueCharacter == null && dialogue.conversationCharacters.Count > 0) {
                        foreach (var character in keyValues.dialogueTextData.conversationCharacters) {
                            if (character.Key == dialogue.currentConversationCharacterSO.id) {
                                if (!FrameManager.frame.currentKey.ContainsID(character.Value)) {
                                    var newValues = new CharacterValues {
                                        transformData = new TransformData {
                                            activeStatus = true,
                                            position = dialogue.currentConversationCharacterSO.GetPrefabPosition(),
                                            size = dialogue.currentConversationCharacterSO.GetPrefabSize(),
                                        },
                                        characterData = new CharacterData {
                                            dialogueID = dialogue.id,
                                            type = FrameCore.Character.CharacterType.Conversation,
                                        },
                                    };
                                    FrameManager.frame.currentKey.AddFrameKeyValues(character.Value, newValues);
                                    
                                }
                                dialogue.LoadConversationCharacter(character.Value, dialogue.type);
                                SetPreviousCharacterValues();
                                break;
                            }
                        }
                    }
                    break;
                }
                case FrameCore.UI.Dialogue.FrameDialogueElementType.Несколькоᅠперсонажей: {
                    if (!firstCharacterCreated)
                        dialogue.SetConversationCharacter();

                    break;
                }
            }
            void SetPreviousCharacterValues() {
                foreach (var previousCharacterValue in FrameManager.frame.frameKeys.Where(ch => !ch.ContainsID(dialogue.conversationCharacterID))) {
                    if (characterKeyValues == null) characterKeyValues = (CharacterValues)FrameManager.GetFrameElementOnSceneByID<FrameCore.Character>(dialogue.conversationCharacterID).GetFrameKeyValuesType();
                    previousCharacterValue.AddFrameKeyValues(dialogue.conversationCharacterID, characterKeyValues);
                }
            }
        }
    }
}
#endif
