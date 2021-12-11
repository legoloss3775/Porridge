using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using FrameCore.ScriptableObjects;
using FrameCore.Serialization;


namespace FrameCore {
    /// <summary>
    /// Для сериализации параметров
    /// <see cref="FrameElement">
    /// </summary>
    namespace Serialization {
        #region SERIALIZATION
        [Serializable]
        public class CharacterValues : Values, IFrameCharacterSerialzation {
            public string dialogueID { get; set; }
            public Character.CharacterType type { get; set; }
            public CharacterSO.CharacterEmotionState emotionState { get; set; }
            public int selectedPartIndex { get; set; }
            public CharacterValues(Character character) {
                position = character.position;
                activeStatus = character.activeStatus;
                size = character.size;
                dialogueID = character.dialogueID;
                type = character.type;
                emotionState = character.emotionState;
                selectedPartIndex = character.selectedPartIndex;
            }
            public CharacterValues() { }
            [Serializable]
            public struct SerializedFrameCharacterValues : IFrameCharacterSerialzation {
                [SerializeField]
                private Vector2 _position;
                [SerializeField]
                private bool _activeStatus;
                [SerializeField]
                private Vector2 _size;
                [SerializeField]
                private string _dialogueID;
                [SerializeField]
                private Character.CharacterType _type;
                [SerializeField]
                private CharacterSO.CharacterEmotionState _emotionState;
                [SerializeField]
                private int _selectedPartIndex;

                public Vector2 position { get => _position; set => _position = value; }
                public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
                public Vector2 size { get => _size; set => _size = value; }
                public string dialogueID { get => _dialogueID; set => _dialogueID = value; }
                public Character.CharacterType type { get => _type; set => _type = value; }
                public CharacterSO.CharacterEmotionState emotionState { get => _emotionState; set => _emotionState = value; }
                public int selectedPartIndex { get => _selectedPartIndex; set => _selectedPartIndex = value; }
            }
            [SerializeField]
            public SerializedFrameCharacterValues serializedFrameCharacterValues;

            public void SetSerializedFrameCharacterValues() {
                serializedFrameCharacterValues.position = position;
                serializedFrameCharacterValues.activeStatus = activeStatus;
                serializedFrameCharacterValues.size = size;
                serializedFrameCharacterValues.dialogueID = dialogueID;
                serializedFrameCharacterValues.type = type;
                serializedFrameCharacterValues.emotionState = emotionState;
                serializedFrameCharacterValues.selectedPartIndex = selectedPartIndex;
            }
            public static void LoadSerialzedFrameKeyCharacterElementValues(List<SerializedFrameCharacterValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new CharacterValues {
                        position = svalue.position,
                        activeStatus = svalue.activeStatus,
                        size = svalue.size,
                        dialogueID = svalue.dialogueID,
                        type = svalue.type,
                        emotionState = svalue.emotionState,
                        selectedPartIndex = svalue.selectedPartIndex,
                    });
                }
            }
        }
        #endregion
        public interface IFrameCharacterSerialzation {
            Vector2 position { get; set; }
            bool activeStatus { get; set; }
            Vector2 size { get; set; }
            string dialogueID { get; set; }
            FrameCore.Character.CharacterType type { get; set; }
            CharacterSO.CharacterEmotionState emotionState { get; set; }
            int selectedPartIndex { get; set; }

        }
    }
    public class Character : FrameElement, Serialization.IFrameCharacterSerialzation {
        public string dialogueID { get; set; }
        public CharacterType type { get; set; }
        public CharacterSO.CharacterEmotionState emotionState { get; set; }
        public int selectedPartIndex { get; set; }
        public enum CharacterType {
            Standalone,
            Conversation,
        }
        public bool HasDialogue(string m_dialogueID) => m_dialogueID == dialogueID ? true : false;
        public bool HasDialogue() => dialogueID != null && dialogueID != "" ? true : false;

        #region CHARACTER_SETUP
        public void CharacterPartChange(CharacterPart part, CharacterSO.CharacterEmotionState state) {
            var frameCharacterSO = (CharacterSO)frameElementObject;

            this.emotionState = state;
            foreach (var partElement in GetCharacterParts()) {
                foreach (var child in part.statePrefab.GetComponentsInChildren<SpriteRenderer>()) {
                    if (partElement.Key == child.gameObject.name) {
                        partElement.Value.sprite = child.sprite;
                        break;
                    }
                    else partElement.Value.sprite = frameElementObject.prefab.GetComponentsInChildren<SpriteRenderer>()
                                                    .Where(ch => ch.gameObject.name == partElement.Key)
                                                    .First().sprite;
                }
            }
            SetKeyValuesWhileNotInPlayMode();
        }
        public SerializableDictionary<string, SpriteRenderer> GetCharacterParts() {
            var sprites = new SerializableDictionary<string, SpriteRenderer>();
            foreach (var child in transform.GetComponentsInChildren<SpriteRenderer>()) {
                if (!sprites.ContainsKey(child.gameObject.name))
                    sprites.Add(child.gameObject.name, child);
            }
            return sprites;
        }
        #endregion

        #region VALUES_SETTINGS
        public override Values GetFrameKeyValuesType() {
            return new Serialization.CharacterValues(this);
        }
        public override void UpdateValuesFromKey(Values frameKeyValues) {
            Serialization.CharacterValues values = (Serialization.CharacterValues)frameKeyValues;
            CharacterSO characterSO = (CharacterSO)frameElementObject;

            activeStatus = values.activeStatus;
            position = values.position;
            size = values.size;
            dialogueID = values.dialogueID;
            type = values.type;

            selectedPartIndex = values.selectedPartIndex;
            CharacterPartChange(characterSO.characterParts[selectedPartIndex], values.emotionState);
        }

        #endregion

        #region EDITOR
#if UNITY_EDITOR

        [CustomEditor(typeof(Character))]
        [CanEditMultipleObjects]
        public class FrameCharacterCustomInspector : FrameElementCustomInspector {
            public override void OnInspectorGUI() {
                Character character = (Character)target;
                var keyValues = GetFrameKeyValues<Serialization.CharacterValues>(character.id);

                Debug.Log(character.id);
                if (keyValues != null) {
                    keyValues.position = character.gameObject.transform.position;
                    character.SetKeyValuesWhileNotInPlayMode();

                    if (targets.Length > 1) {
                        foreach (var target in targets) {
                            FrameElement mTarget = (FrameElement)target;
                            character.position = character.gameObject.transform.position;
                            mTarget.SetKeyValuesWhileNotInPlayMode();
                        }
                    }
                }
                this.SetElementInInspector<CharacterSO>();
            }
        }
#endif
        #endregion
    }
}