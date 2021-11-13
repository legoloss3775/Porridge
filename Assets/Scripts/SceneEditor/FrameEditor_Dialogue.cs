using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public static class FrameEditor_Dialogue 
{
    public static void FrameDialogueEditing()
    {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        foreach (var obj in frameEditorSO.GetFrameElementsOfType<FrameUI_DialogueSO>().ToList())
            if (FrameManager.frame.ContainsFrameElementObject(obj))
                foreach (var frameDialogueID in FrameManager.frame.GetFrameElementIDsByObject(obj).ToList())
                    foreach (var pair in FrameManager.frame.currentKey.frameKeyValues.Where(ch => ch.Key == frameDialogueID).ToList())
                    {
                        FrameUI_Dialogue dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(frameDialogueID);
                        if (dialogue == null) FrameManager.ChangeFrame();
                        if (dialogue == null) return;

                        UIDialogueValues values = (UIDialogueValues)pair.Value;
                        EditorUtility.SetDirty(dialogue);

                        if (!Application.isPlaying)
                        {
                            values.text = GUILayout.TextArea(values.text, GUILayout.MaxHeight(100));
                            dialogue.text = values.text;

                            FrameUIDialogueCharacterSelection(dialogue);
                            UpdateFrameUIDialogueCharacter(dialogue);
                        }
                        else
                        {
                            dialogue.text = GUILayout.TextArea(dialogue.text, GUILayout.MaxHeight(100));
                        }
                    }
    }
    public static void FrameUIDialogueCharacterSelection(FrameUI_Dialogue dialogue)
    {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        UIDialogueValues values = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];

        values.selectedCharacterIndex = EditorGUILayout.Popup("Собеседник:", values.selectedCharacterIndex, frameEditorSO.GetFrameElementsObjectsNames<FrameCharacterSO>().ToArray());
        dialogue.selectedCharacterIndex = values.selectedCharacterIndex;
        
        for (int i = 0; i < frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>().Count; i++)
        {
            if (i == values.selectedCharacterIndex)
            {
                dialogue.conversationCharacterSO = frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>()[i];
            }
        }
    }
    public static void UpdateFrameUIDialogueCharacter(FrameUI_Dialogue dialogue)
    {
        var values = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[dialogue.id];
        var key = FrameManager.frame.currentKey;
        var characterIDs = new List<string>();
        FrameCharacterValues characterKeyValues = null;
        bool firstCharacterCreated = false;
        bool characterWasCreatedPreviously = false;
        bool selectionChange = false;

        if (FrameManager.frame.GetFrameElementIDsByObject(dialogue.conversationCharacterSO) != null)
            characterIDs = FrameManager.frame.GetFrameElementIDsByObject(dialogue.conversationCharacterSO);

        foreach (var characterID in characterIDs)
            if (key.ContainsID(characterID))
            {
                characterKeyValues = (FrameCharacterValues)key.frameKeyValues[characterID];
                if (characterKeyValues.dialogueID == dialogue.id)
                    break;
            }

        if (values.conversationCharacterID != null)
            firstCharacterCreated = true;

        if (!firstCharacterCreated)
            dialogue.SetConversationCharacter();

        foreach (string id in characterIDs)
            if (characterKeyValues != null && characterKeyValues.dialogueID == dialogue.id)
                characterWasCreatedPreviously = true;

        if (dialogue.conversationCharacterSO != null && dialogue.conversationCharacterSO != dialogue.conversationCharacter.frameElementObject)
            selectionChange = true;

        if (selectionChange)
        {
            if (characterWasCreatedPreviously)
                foreach (var characterID in FrameManager.frame.GetFrameElementIDsByObject(dialogue.conversationCharacterSO))
                {
                    dialogue.LoadConversationCharacter(characterID);
                    break;
                }
            else
            {
                RemovePreviousCharacter();
                dialogue.SetConversationCharacter();
            }
        }
        void RemovePreviousCharacter() => FrameManager.RemoveElement(dialogue.conversationCharacter.id);
    }
}
