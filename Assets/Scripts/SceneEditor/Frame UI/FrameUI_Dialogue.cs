using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using static FrameKey;

#region SERIALIZATION
[Serializable]
public class UIDialogueValues : Values, IFrameUIDialogueSerialization
{
    public Vector2 position { get; set; }
    public bool activeStatus { get; set; }
    public string text { get; set; }

    public string conversationCharacterID { get; set; }
    public int selectedCharacterIndex { get; set; }
    public UIDialogueValues(FrameUI_Dialogue dialogue)
    {
        position = dialogue.position;
        activeStatus = dialogue.activeStatus;
        text = dialogue.text;

        conversationCharacterID = dialogue.conversationCharacterID;
        selectedCharacterIndex = dialogue.selectedCharacterIndex;
    }
    public UIDialogueValues() { }
    [Serializable]
    public struct SerializedDialogueValues : IFrameUIDialogueSerialization
    {
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private bool _activeStatus;
        [SerializeField]
        private string _text;
        [SerializeField]
        private string _conversationCharacterID;
        [SerializeField]
        private int _selectedCharacterIndex;

        public Vector2 position { get => _position; set => _position = value; }
        public bool activeStatus { get => _activeStatus; set => _activeStatus = value; }
        public string text { get => _text; set => _text = value; }
        public string conversationCharacterID { get => _conversationCharacterID; set => _conversationCharacterID = value; }
        public int selectedCharacterIndex { get => _selectedCharacterIndex; set => _selectedCharacterIndex = value; }
    }
    [SerializeField]
    private SerializedDialogueValues serializedDialogueValues;

    public SerializedDialogueValues SetSerializedDialogueValues()
    {
        serializedDialogueValues.text = text;
        serializedDialogueValues.position = position;
        serializedDialogueValues.activeStatus = activeStatus;
        serializedDialogueValues.selectedCharacterIndex = selectedCharacterIndex;
        serializedDialogueValues.conversationCharacterID = conversationCharacterID;

        return serializedDialogueValues;
    }
    public static void LoadSerialzedDialogueValues(List<SerializedDialogueValues> serializedElementValues, List<Values> values)
    {
        foreach (var svalue in serializedElementValues)
        {
            values.Add(new UIDialogueValues
            {
                position = svalue.position,
                activeStatus = svalue.activeStatus,
                text = svalue.text,

                selectedCharacterIndex = svalue.selectedCharacterIndex,
                conversationCharacterID = svalue.conversationCharacterID,
            });
        }
    }
}
#endregion
public interface IFrameUIDialogueSerialization
{
    Vector2 position { get; set; }
    bool activeStatus { get; set; }
    string text { get; set; }
    string conversationCharacterID { get; set; }
    int selectedCharacterIndex { get; set; }
}
public class FrameUI_Dialogue : FrameUI_Window, IFrameUIDialogueSerialization
{
    public string text
    {
        get
        {
            return GetTextComponent().text;
        }
        set
        {
            GetTextComponent().text = value;
        }
    }
    public string characterNameField
    {
        get
        {
            if (this != null)
                return this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
            else return frameElementObject.prefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        }
        set
        {
            if(conversationCharacterID != null)
                this.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = conversationCharacterID.Split('_')[0];
        }
    }
    public FrameCharacter conversationCharacter { get; set; }
    public FrameCharacterSO conversationCharacterSO { get; set; }
    
    public string conversationCharacterID { get; set; }
    public int selectedCharacterIndex { get; set; }
    
    public bool HasCharacter(string characterID)
    {
        return characterID == conversationCharacterID ? true : false;
    }
    public void LoadConversationCharacter(string characterID)
    {
        FrameCharacter character = new FrameCharacter();
        FrameKey key = FrameManager.frame.currentKey;
        FrameCharacterValues chValues = (FrameCharacterValues)key.frameKeyValues[characterID];

        RemovePreviousCharacter();
        this.SetConversationCharacterSO();

        this.conversationCharacterSO.LoadElementOnScene(FrameManager.frame.GetPair(FrameManager.frame.GetFrameElementObjectByID(characterID)), characterID, character, chValues);
        character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(characterID);
        character.dialogueID = this.id;
        this.conversationCharacter = character; 
        this.conversationCharacterID = characterID;
        this.conversationCharacterID = characterID;

        key.UpdateFrameElementKeyValues(character);

        character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues, FrameCharacter>();
        this.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUI_Dialogue>();

        void RemovePreviousCharacter() => FrameManager.RemoveElement(this.conversationCharacter.id);
    }
    public void SetConversationCharacter()
    {
        FrameCharacter character = new FrameCharacter();

        this.conversationCharacterSO.CreateElementOnScene(this.conversationCharacterSO, character, this.conversationCharacterSO.prefab.transform.position, out string ID);
        if (FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(ID) == null)
            return;
        character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(ID);
        character.dialogueID = this.id;

        this.conversationCharacterID = ID;
        this.conversationCharacter = character;
        this.conversationCharacterID = ID;

        character.SetKeyValuesWhileNotInPlayMode<FrameCharacterValues, FrameCharacter>();
        this.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUI_Dialogue>();
    }
    public void SetConversationCharacterSO()
    {
        UIDialogueValues values = (UIDialogueValues)frameKeyValues;
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        for (int i = 0; i < frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>().Count; i++)
        {
            if (i == values.selectedCharacterIndex)
            {
                this.conversationCharacterSO = frameEditorSO.GetFrameElementsOfType<FrameCharacterSO>()[i];
            }
        }
    }

    #region VALUES_SETTINGS
    public TextMeshProUGUI GetTextComponent()
    {
        if (this != null)
            return this.gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        else return frameElementObject.prefab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
    }
    public override FrameKey.Values GetFrameKeyValuesType()
    {
        return new UIDialogueValues(this);
    }
    public override void UpdateValuesFromKey(object frameKeyValues)
    {
        UIDialogueValues values = (UIDialogueValues)frameKeyValues;
        activeStatus = values.activeStatus;
        position = values.position;
        text = values.text;

        conversationCharacterID = values.conversationCharacterID;
        selectedCharacterIndex = values.selectedCharacterIndex;

        characterNameField = characterNameField;

        UpdateConversationCharacterFromKey(frameKeyValues);
    }
    public void UpdateConversationCharacterFromKey(object frameKeyValues)
    {
        UIDialogueValues values = (UIDialogueValues)frameKeyValues;

        if (this.conversationCharacter.id != values.conversationCharacterID)
        {
            this.LoadConversationCharacter(values.conversationCharacterID);
        }
    }
    #endregion

    #region EDITOR
#if UNITY_EDITOR

    [CustomEditor(typeof(FrameUI_Dialogue))]
    [CanEditMultipleObjects]
    public class FrameUIDialogueWindowCustomInspector : FrameElementCustomInspector
    {
        public override void OnInspectorGUI()
        {
            FrameUI_Dialogue dialogue = (FrameUI_Dialogue)target;
            EditorUtility.SetDirty(dialogue);

            dialogue.position = dialogue.GetComponent<RectTransform>().anchoredPosition;

            dialogue.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUI_Dialogue>();

            if (targets.Length > 1)
            {
                foreach (var target in targets)
                {
                    FrameElement mTarget = (FrameElement)target;
                    dialogue.position = mTarget.GetComponent<RectTransform>().anchoredPosition;
                    mTarget.SetKeyValuesWhileNotInPlayMode<UIDialogueValues, FrameUI_Dialogue>();
                }
            }

            this.SetElementInInspector<FrameUI_DialogueSO>();
        }
    }
#endif
    #endregion
}