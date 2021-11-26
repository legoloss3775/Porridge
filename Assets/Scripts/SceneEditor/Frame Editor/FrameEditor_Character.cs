using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
public class FrameEditor_Character : FrameEditor
{
    public static void FrameCharacterEditing() {
        Action<FrameCharacter> characterStateEditing = FrameCharacterStateEditing;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        foldouts[EditorType.CharacterEditor] = EditorGUILayout.Foldout(foldouts[EditorType.CharacterEditor], "Персонажи", EditorStyles.foldoutHeader);
        GUILayout.FlexibleSpace();
        ElementCreation(FrameEditor_CreationWindow.CreationType.FrameCharacter);
        GUILayout.EndHorizontal();

        if (foldouts[EditorType.CharacterEditor]) {
            GUILayout.BeginVertical("HelpBox");
            ElementEditing<FrameCharacterSO, FrameCharacter>(PositioningType.Vertical, EditorType.CharacterEditor, false, true, characterStateEditing);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndVertical();
    }
    private static void FrameCharacterStateEditing(FrameCharacter character) {
        var keyValues = character.GetFrameKeyValues<FrameCharacterValues>();
        var characterSO = (FrameCharacterSO)character.frameElementObject;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        ElementSelection(character);
        if(character.type != FrameCharacter.CharacterType.Conversation)
            ElementDeletion(character);
        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label(character.id, EditorStyles.largeLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (character.HasDialogue()) {
            GUILayout.FlexibleSpace();
            GUILayout.Label(character.dialogueID);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Настроение:", GUILayout.MaxWidth(300));
        keyValues.emotionState = (FrameCharacterSO.CharacterEmotionState)EditorGUILayout.EnumPopup(keyValues.emotionState, GUILayout.MaxWidth(145));
        GUILayout.EndHorizontal();

        var characterParts = new SerializableDictionary<int, CharacterPart>();
        foreach (var part in characterSO.characterParts.Where(ch => ch.state == keyValues.emotionState))
            characterParts.Add(characterSO.characterParts.IndexOf(part), part);

        if (characterParts.Count > 0)
            CharacterPartPrefabSelection(characterParts);
        else {
            GUILayout.Label("Не найдены пресеты.");
            //if (GUILayout.Button("Добавить")) {
                
            //}
        }

        void CharacterPartPrefabSelection(SerializableDictionary<int, CharacterPart> parts) {
            var icons = new SerializableDictionary<int, Texture>();
            foreach (var part in parts) {
                var icon = UnityEditor.AssetPreview.GetAssetPreview(part.Value.statePrefab);
                icons.Add(part.Key, icon);
            }

            keyValues.selectedPartIndex = GUILayout.SelectionGrid(keyValues.selectedPartIndex, icons.Values.ToArray(), 4, GUILayout.MaxWidth(450));
            if (!icons.ContainsKey(keyValues.selectedPartIndex))
                keyValues.selectedPartIndex = icons.Keys.First();
            var selectedPart = parts[keyValues.selectedPartIndex];
            if (character.emotionState != keyValues.emotionState || character.selectedPartIndex != keyValues.selectedPartIndex) {
                character.selectedPartIndex = keyValues.selectedPartIndex;
                character.CharacterPartChange(selectedPart,keyValues.emotionState);
            }
        }
    }
}
#endif