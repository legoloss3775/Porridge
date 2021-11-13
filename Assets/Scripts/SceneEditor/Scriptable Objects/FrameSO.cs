using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FrameSO : ScriptableObject
{
    public enum FrameType
    {
        Default,
    }
    public FrameType type;

    [SerializeField]
    public string id;
    [SerializeField]
    public int selectedKeyIndex;
    [SerializeField]
    public FrameKey currentKey;
    public List<FrameKey> frameKeys = new List<FrameKey>();
    [Serializable]
    public struct FrameElementIDPair
    {
        public FrameElementSO elementObject;
        public List<string> ids;
    }
    public List<FrameElementIDPair> usedElementsObjects = new List<FrameElementIDPair>();
    private void OnEnable()
    {
        if (currentKey == null)
        {
            if(frameKeys.Count > 0)
                currentKey = frameKeys[0];
            else
            {
                currentKey = new FrameKey();
                this.AddKey(currentKey);
            }
        }
    }
    public void AddKey(FrameKey key)
    {
        frameKeys.Add(key);
    }
    public FrameElementIDPair GetPair<T>(T frameElementObject)
        where T: FrameElementSO
    {
        foreach(var pair in usedElementsObjects)
        {
            if (pair.elementObject == frameElementObject)
                return pair;
        }
        return new FrameElementIDPair();
    }
    public bool ContainsFrameElementID(string id)
    {
        foreach(var pair in usedElementsObjects)
        {
            foreach (var pairedID in pair.ids)
                if (id == pairedID)
                    return true;
        }
        return false;
    }
    public bool ContainsFrameElementObject<T>(T obj)
        where T: FrameElementSO
    {
        foreach (var pair in usedElementsObjects)
        {
            if (pair.elementObject == obj)
                return true;
        }
        return false;
    }
    public List<string> GetFrameElementIDsByObject<T>(T obj)
        where T: FrameElementSO
    {
        foreach (var pair in usedElementsObjects)
        {
            if (pair.elementObject == obj)
                return pair.ids;
        }
        return null;
    }
    public FrameElementSO GetFrameElementObjectByID(string id)
    {
        foreach(var pair in usedElementsObjects)
        {
            if (pair.ids.Contains(id))
                return pair.elementObject;
        }
        throw new Exception("Не найдет элемент по ID");
    }
    public void RemoveElementFromCurrentKey(string frameElementID)
    {
        foreach(var pair in usedElementsObjects.ToList())
        {
            if (pair.ids.Contains(frameElementID))
            {
                pair.ids.Remove(frameElementID);
                DestroyElementOnScene(frameElementID);
                if (pair.ids.Count <= 0)
                    usedElementsObjects.Remove(pair);
            }
        }
        foreach (var value in currentKey.frameKeyValues.Where(ch => ch.Key == frameElementID).ToList())
            currentKey.frameKeyValues.Remove(value.Key);
    }
    public void DestroyElementOnScene(string frameElementID)
    {
        FrameElement element = FrameManager.GetFrameElementOnSceneByID<FrameElement>(frameElementID);
        DestroyImmediate(element.gameObject);
    }
    public static void LoadElementsOnScene<TKey, TValue>(List<FrameElementIDPair> pairs,TValue element)
    where TValue : global::FrameElement
    {
        foreach (var pair in pairs)
        {
            if (pair.elementObject.GetType() == typeof(TKey))
            {
                foreach (var id in pair.ids)
                {
                    FrameKey.Values values = null;
                    if (FrameManager.frame.currentKey.frameKeyValues.ContainsKey(id))
                        values = FrameManager.frame.currentKey.frameKeyValues[id];

                    if (element is FrameCharacter)
                    {
                        FrameCharacterValues chValues = (FrameCharacterValues)values;

                        if (chValues != null && chValues.dialogueID != null && chValues.dialogueID != "")
                        {
                            FrameUI_Dialogue dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUI_Dialogue>(chValues.dialogueID);
                            UIDialogueValues dv = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[chValues.dialogueID];
                            if (dialogue.conversationCharacter == null && dv.conversationCharacterID == id)
                            {
                                pair.elementObject.LoadElementOnScene(pair, id, element, chValues);
                                FrameCharacter character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(id);
                                dialogue.conversationCharacter = character;
                                dialogue.conversationCharacterID = id;
                            }
                        }
                        else
                            pair.elementObject.LoadElementOnScene(pair, id, element, values);

                    }
                    else
                        pair.elementObject.LoadElementOnScene(pair, id, element, values);
                }
            }
        }
    }
}
