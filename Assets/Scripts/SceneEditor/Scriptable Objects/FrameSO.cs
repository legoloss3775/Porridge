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
                if (element is FrameCharacter)
                    foreach (var id in pair.ids)
                    {
                        FrameCharacterValues values = null;
                        if (FrameManager.frame.currentKey.frameKeyValues.ContainsKey(id))
                            values = (FrameCharacterValues)FrameManager.frame.currentKey.frameKeyValues[id];
                        
                        if (values != null && values.dialogueID != null && values.dialogueID != "")
                        {
                            FrameUIDialogue dialogue = FrameManager.GetFrameElementOnSceneByID<FrameUIDialogue>(values.dialogueID);
                            UIDialogueValues dv = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[values.dialogueID];
                            if (dialogue.conversationCharacter == null && dv.conversationCharacterID == id)
                            {
                                LoadElementOnScene(pair, id, element, values);
                                FrameCharacter character = FrameManager.GetFrameElementOnSceneByID<FrameCharacter>(id);
                                dialogue.conversationCharacter = character;
                                dialogue.conversationCharacterID = id;
                            }
                        }
                        else 
                            LoadElementOnScene(pair, id, element, values);
                    }
                else if (element is FrameUIDialogue)
                    foreach (var id in pair.ids)
                    {
                        UIDialogueValues values = null;
                        if (FrameManager.frame.currentKey.frameKeyValues.ContainsKey(id))
                            values = (UIDialogueValues)FrameManager.frame.currentKey.frameKeyValues[id];
                        else throw new Exception("Не найден ID в ключе ");
                        LoadUIElementOnScene(pair, id, element, values);
                    }
                else
                    foreach (var id in pair.ids)
                    {
                        FrameElementValues values = null;
                        if (FrameManager.frame.currentKey.frameKeyValues.ContainsKey(id))
                            values = (FrameElementValues)FrameManager.frame.currentKey.frameKeyValues[id];
                        else throw new Exception("Не найден ID в ключе ");
                        LoadElementOnScene(pair, id, element, values);
                    }
            }
        }
    }
    public static void LoadElementOnScene<T>(FrameElementIDPair pair, string id, T element, FrameKey.Values values)
        where T: FrameElement
    {
        try
        {
            element.frameElementObject = pair.elementObject;

            T elementClone = Instantiate(element.frameElementObject.prefab).AddComponent<T>();
            elementClone.frameElementObject = pair.elementObject;
            elementClone.id = id;
            elementClone.frameKeyValues = values;
            EditorUtility.SetDirty(elementClone);
            FrameManager.AddElement(elementClone);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public static void LoadUIElementOnScene<T>(FrameElementIDPair pair, string id, T element, FrameKey.Values values)
    where T : FrameElement
    {
        try
        {
            element.frameElementObject = pair.elementObject;
            Debug.Log(element.frameElementObject.prefab.name);

            T elementClone = Instantiate(element.frameElementObject.prefab, FrameManager.UICanvas.transform).AddComponent<T>();
            elementClone.frameElementObject = pair.elementObject;
            elementClone.id = id;
            elementClone.frameKeyValues = values;
            EditorUtility.SetDirty(elementClone);
            FrameManager.AddElement(elementClone);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public static void CreateElementOnScene<T>(FrameElementSO obj, in T element, Vector2 position, out string elementID)
        where T : FrameElement
    {
        element.frameElementObject = obj;

        T elementClone;
        if (element is FrameUIWindow)
            CreateFrameUIElement(obj, element, position, out elementClone);
        else
            CreateFrameElement(obj, element, position, out elementClone);

        bool hasElement = false;
        foreach (var pair in FrameManager.frame.usedElementsObjects)
        {
            if (pair.elementObject == obj)
            {
                pair.ids.Add(elementClone.id);
                hasElement = true;
            }
        }
        if (!hasElement)
        {
            FrameElementIDPair newPair = new FrameElementIDPair();
            newPair.ids = new List<string>();
            newPair.elementObject = obj;
            newPair.ids.Add(elementClone.id);
            FrameManager.frame.usedElementsObjects.Add(newPair);

        }

        elementID = elementClone.id;
    }
    public static void CreateElementOnScene<T>(FrameElementSO obj, in T element, Vector2 position, string elementID)
    where T : FrameElement
    {
        element.frameElementObject = obj;

        T elementClone;
        if (element is FrameUIWindow)
            CreateFrameUIElement(obj, element, position, out elementClone);
        else
            CreateFrameElement(obj, element, position, out elementClone);

        elementClone.id = elementID;
        bool hasElement = false;
        foreach (var pair in FrameManager.frame.usedElementsObjects)
        {
            if (pair.elementObject == obj)
            {
                pair.ids.Add(elementClone.id);
                hasElement = true;
            }
        }
        if (!hasElement)
        {
            FrameElementIDPair newPair = new FrameElementIDPair();
            newPair.ids = new List<string>();
            newPair.elementObject = obj;
            newPair.ids.Add(elementClone.id);
            FrameManager.frame.usedElementsObjects.Add(newPair);

        }
    }
    public static void CreateFrameElement<T>(FrameElementSO obj, T element, Vector2 position, out T elementClone)
        where T: FrameElement
    {
        elementClone = Instantiate(element.frameElementObject.prefab, position, new Quaternion()).AddComponent<T>();
        elementClone.frameElementObject = obj;
        elementClone.id = obj.id + "_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        EditorUtility.SetDirty(elementClone);
        FrameManager.AddElement(elementClone);
    }
    public static void CreateFrameUIElement<T>(FrameElementSO obj, T element, Vector2 position, out T elementClone)
        where T: FrameElement
    {
        elementClone = Instantiate(element.frameElementObject.prefab, position, new Quaternion(), FrameManager.UICanvas.transform).AddComponent<T>();
        elementClone.frameElementObject = obj;
        elementClone.id = obj.id + "_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        EditorUtility.SetDirty(elementClone);
        FrameManager.AddElement(elementClone);
    }
}
