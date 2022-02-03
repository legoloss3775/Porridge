using FrameCore.ScriptableObjects;
using FrameCore.Serialization;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс редактора персонажей
    /// </summary>
    public class Character : Core {

        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        public static void FrameCharacterEditing() {
            Action<FrameCore.Character> characterStateEditing = FrameCharacterStateEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.CharacterEditor] = EditorGUILayout.Foldout(foldouts[EditorType.CharacterEditor], "Персонажи", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameCharacter);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.CharacterEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<CharacterSO, FrameCore.Character>(PositioningType.Vertical, EditorType.CharacterEditor, false, true, characterStateEditing);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }
        private static void FrameCharacterStateEditing(FrameCore.Character character) {
            var keyValues = character.GetFrameKeyValues<CharacterValues>();
            var characterSO = (CharacterSO)character.frameElementObject;

            if (keyValues == null) return;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(character); //кнопка выбора элемента
            ElementActiveStateChange(character); //кнопка изменения статуса активности элемента
            ElementDeletion(character);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            if(character.activeStatus)
                GUILayout.Label(character.id, FrameGUIUtility.GetLabelStyle(FrameKeyNode.ORANGE, 15));
            else GUILayout.Label(character.id);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (character.HasDialogue()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label(character.dialogueID);
            }
            if (character.activeStatus == false) {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive");
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Настроение:", GUILayout.MaxWidth(300));
            keyValues.characterData.emotionState = (CharacterSO.CharacterEmotionState)EditorGUILayout.EnumPopup(keyValues.characterData.emotionState, GUILayout.MaxWidth(145));
            GUILayout.EndHorizontal();

            var characterParts = new SerializableDictionary<int, CharacterPart>();
            for (int i = 0; i < characterSO.characterParts.Count; i++) {
                if (characterSO.characterParts[i].state == keyValues.characterData.emotionState) {
                    characterParts.Add(i, characterSO.characterParts[i]);
                }
            }

            if (characterParts.Count > 0)
                CharacterPartPrefabSelection(characterParts);
            else {
                GUILayout.Label("Не найдены пресеты.");
                //if (GUILayout.Button("Добавить")) {

                //}
            }

            void CharacterPartPrefabSelection(SerializableDictionary<int, CharacterPart> parts) {
                if (character.activeStatus == false) return;

                //отрисовка иконок взависимости от отображения превью префаба в Assets
                var icons = new SerializableDictionary<int, Texture>();
                foreach (var part in parts) {
                    var icon = UnityEditor.AssetPreview.GetAssetPreview(part.Value.statePrefab);
                    if (!icons.ContainsValue(icon))
                        icons.Add(part.Key, icon);
                }
                int index = 0;
                for (int i = 0; i < parts.Count; i++) {
                    if (parts.Keys.ToArray()[i] == keyValues.characterData.selectedPartIndex) {
                        index = i;
                        break;
                    }
                }

                index = GUILayout.SelectionGrid(index, icons.Values.ToArray(), 4, GUILayout.MaxWidth(450));

                keyValues.characterData.selectedPartIndex = parts.Keys.ToArray()[index];

                if (!icons.ContainsKey(keyValues.characterData.selectedPartIndex))
                    keyValues.characterData.selectedPartIndex = icons.Keys.First();

                var selectedPart = parts[keyValues.characterData.selectedPartIndex];
                if (character.emotionState != keyValues.characterData.emotionState || character.selectedPartIndex != keyValues.characterData.selectedPartIndex) {
                    character.selectedPartIndex = keyValues.characterData.selectedPartIndex;
                    character.CharacterPartChange(selectedPart, keyValues.characterData.emotionState);

                }
            }
        }
    }
}

#endif