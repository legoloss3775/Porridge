using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
public class FrameEditor_Character : FrameEditor_Element
{
    public static void FrameCharacterEditing() {
        Action<FrameCharacter> characterStateEditing = FrameCharacterStateEditing;

        GUILayout.BeginVertical();

        foldouts[EditorType.CharacterEditor] = EditorGUILayout.Foldout(foldouts[EditorType.CharacterEditor], "Персонажи");

        if (foldouts[EditorType.CharacterEditor]) {
            GUILayout.BeginVertical("HelpBox");
            ElementEditing<FrameCharacterSO, FrameCharacter>(PositioningType.Vertical, EditorType.CharacterEditor, false, true, characterStateEditing);
            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
    }
    private static void FrameCharacterStateEditing(FrameCharacter character) {
        var keyValues = character.GetFrameKeyValues<FrameCharacterValues>();
        var characterSO = (FrameCharacterSO)character.frameElementObject;

        GUILayout.BeginHorizontal();

        ElementSelection(character);
        GUILayout.Label(character.id);

        if (character.HasDialogue()) {
            GUILayout.FlexibleSpace();
            GUILayout.Label(character.dialogueID);
        }

        GUILayout.EndHorizontal();

        keyValues.emotionState = (FrameCharacterSO.CharacterEmotionState)EditorGUILayout.EnumPopup("Настроение:", keyValues.emotionState, GUILayout.MaxWidth(350));

        var characterParts = new SerializableDictionary<int, CharacterPart>();
        foreach (var part in characterSO.characterParts.Where(ch => ch.state == keyValues.emotionState))
            characterParts.Add(characterSO.characterParts.IndexOf(part), part);

        if (characterParts.Count > 0)
            CharacterPartPrefabSelection(characterParts);
        else {
            GUILayout.Label("Не найдены пресеты.");
            if (GUILayout.Button("Добавить")) {
                
            }
            var editor = CreateEditor(characterSO);
            var inspector = editor.CreateInspectorGUI();
            editor.OnInspectorGUI();
       
        }

        void CharacterPartPrefabSelection(SerializableDictionary<int, CharacterPart> parts) {
            var icons = new SerializableDictionary<int, Texture>();
            foreach (var part in parts) {
                var icon = UnityEditor.AssetPreview.GetAssetPreview(part.Value.statePrefab);
                icons.Add(part.Key, icon);
            }

            keyValues.selectedPartIndex = GUILayout.SelectionGrid(keyValues.selectedPartIndex, icons.Values.ToArray(), 3, GUILayout.MaxWidth(500));
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