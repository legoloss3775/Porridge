using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static FrameSO;

[CreateAssetMenu(fileName = "Character", menuName = "Редактор Сцен/Персонаж")]
public class FrameCharacterSO : FrameElementSO
{
    public enum CharacterEmotionState
    {
        Default,
        Angry,
        Sad,
        Thinking,
        Ashamed
    }

    [HideInInspector]
    public CharacterEmotionState state;

    [OneLine.OneLine]
    public List<CharacterPart> characterParts;

    public override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
    }

    public override void OnBeforeSerialize()
    {
        base.OnBeforeSerialize();
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void LoadElementOnScene<T>(FrameElementIDPair pair, string id, T element, FrameKey.Values values)
    {
        FrameCharacterValues cValues = (FrameCharacterValues)values;
        if (element.frameElementObject == null)
            element.frameElementObject = pair.elementObject;

        T elementClone = Instantiate(element.frameElementObject.prefab).AddComponent<T>();
        elementClone.frameElementObject = pair.elementObject;
        elementClone.id = id;
        elementClone.frameKeyValues = cValues;
        EditorUtility.SetDirty(elementClone);
        FrameManager.AddElement(elementClone);
        
        if (cValues != null && cValues.dialogueID != null && cValues.dialogueID != "")
        {
            FrameUI_Dialogue dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(cValues.dialogueID);
            UIDialogueValues dv = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[cValues.dialogueID];
            if (dialogue.conversationCharacter == null && dv.conversationCharacterID == id)
            {
                dialogue.conversationCharacter = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(id);
                dialogue.conversationCharacterID = id;
            }
        }
    }
}
[System.Serializable]
public class CharacterPart
{
    public GameObject statePrefab;
    public FrameCharacterSO.CharacterEmotionState state;
}
