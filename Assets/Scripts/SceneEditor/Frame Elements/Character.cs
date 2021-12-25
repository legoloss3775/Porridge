using FrameCore.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace FrameCore {
    /// <summary>
    /// Для сериализации параметров.
    /// <see cref="FrameElement">
    /// </summary>
    namespace Serialization {
        #region SERIALIZATION
        [System.Serializable]
        public class CharacterValues : Values {
            public CharacterData characterData;
            public CharacterValues(Character character) {
                transformData = new TransformData {
                    position = character.position,
                    size = character.size,
                    rotation = character.rotation.eulerAngles,
                    activeStatus = character.activeStatus,
                };
                characterData = new CharacterData {
                    dialogueID = character.dialogueID,
                    type = character.type,
                    emotionState = character.emotionState,
                    selectedPartIndex = character.selectedPartIndex,
                };
            }
            public CharacterValues() { }
            [System.Serializable]
            public struct SerializedFrameCharacterValues {
                public TransformData transformData;
                public CharacterData characterData;
            }
            [SerializeField]
            public SerializedFrameCharacterValues serializedFrameCharacterValues {
                get {
                    return new SerializedFrameCharacterValues {
                        transformData = transformData,
                        characterData = characterData,
                    };
                }
            }
            public static void LoadSerialzedFrameKeyCharacterElementValues(List<SerializedFrameCharacterValues> serializedElementValues, List<Values> values) {
                foreach (var svalue in serializedElementValues) {
                    values.Add(new CharacterValues {
                        transformData = svalue.transformData,
                        characterData = svalue.characterData,
                    });
                }
            }
        }
        #endregion
    }
    public class Character : FrameElement {
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

            activeStatus = values.transformData.activeStatus;
            position = values.transformData.position;
            size = values.transformData.size;
            rotation = Quaternion.Euler(values.transformData.rotation);
            dialogueID = values.characterData.dialogueID;
            type = values.characterData.type;

            selectedPartIndex = values.characterData.selectedPartIndex;
            CharacterPartChange(characterSO.characterParts[selectedPartIndex], values.characterData.emotionState);
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
                    keyValues.transformData.position = character.gameObject.transform.position;
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