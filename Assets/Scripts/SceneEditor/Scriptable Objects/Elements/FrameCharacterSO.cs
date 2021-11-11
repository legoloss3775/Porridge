using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
}
[System.Serializable]
public class CharacterPart
{
    public GameObject statePrefab;
    public FrameCharacterSO.CharacterEmotionState state;
}
