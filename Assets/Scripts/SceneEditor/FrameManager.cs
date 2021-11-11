using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class FrameManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static FrameSO frame;
    public static Canvas UICanvas;
    [SerializeField]
    public static List<FrameElement> frameElements = new List<FrameElement>();
    public List<FrameElement> serializedFrameElementsList = new List<FrameElement>();


    int selectedFrame = 0;
    int selectedFrameKey = 0;

    private void Awake()
    {
#if UNITY_EDITOR
        //UpdateDirty();
#endif
    }
    private void Start()
    {
        SetDefaultFrame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedFrame > 0)
                selectedFrame--;

            SetFrame(selectedFrame, selectedFrameKey);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (selectedFrame < AssetManager.GetAtPath<FrameSO>("Frames/").Length)
                selectedFrame++;

            SetFrame(selectedFrame, selectedFrameKey);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (selectedFrameKey > 0)
                selectedFrameKey--;

            SetFrame(selectedFrame, selectedFrameKey);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (frame.frameKeys.Count - 1 > selectedFrameKey)
                selectedFrameKey++;

            SetFrame(selectedFrame, selectedFrameKey);
        }
        Debug.Log(frame.frameKeys.IndexOf(frame.currentKey));
    }
    public void SetFrame(int frameIndex, int keyIndex)
    {
        if (AssetManager.GetAtPath<FrameSO>("Frames/").Length > frameIndex)
        {
            UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
            frame = AssetManager.GetAtPath<FrameSO>("Frames/")[frameIndex];

            frame.currentKey = frame.frameKeys[keyIndex];

            ChangeFrame();
            ChangeFrameKey();
        }
    }
    public void SetDefaultFrame()
    {
        if(AssetManager.GetAtPath<FrameSO>("Frames/").Length > 0)
        {
            UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
            frame = AssetManager.GetAtPath<FrameSO>("Frames/")[0];
            frame.currentKey = frame.frameKeys[0];
            ChangeFrame();
            ChangeFrameKey();
        }
    }
    public static T GetFrameElementOnSceneByID<T>(string id)
    where T : FrameElement
    {
        foreach (var element in FrameManager.frameElements)
            if (element.id == id)
                return (T)element;
        return null;
    }
    public static bool ContainsElementInManagerByID(string id)
    {
        foreach (var element in FrameManager.frameElements)
            if (element.id == id)
                return true;
        return false;
    }
    public static void AddElement(FrameElement element)
    {
        frameElements.Add(element);
    }
    public static void RemoveElement(string id)
    {
        for(int i = 0; i < frameElements.Count; i++)
        {
            if (frameElements[i].id == id)
            {
                DestroyImmediate(frameElements[i].gameObject);
                frameElements.RemoveAt(i);
            }
        }
    }
    public static void ChangeFrameKey()
    {
        foreach(var element in frameElements)
        {
            if (element == null)
                continue;
            if (element.id == null)
            {
                ClearElements();
                ChangeFrame();
            }

            if (frame.currentKey.frameKeyValues.ContainsKey(element.id))
            {
                frame.currentKey.UpdateFrameElementKeyValues(element);
                element.UpdateValuesFromKey(frame.currentKey.frameKeyValues[element.id]);
            }
            else 
            {
                frame.currentKey.AddFrameKeyValues(element.id, element.GetFrameKeyValuesType());
            }
        }
    }
    public static void ChangeFrame()
    {
        ClearElements();
        FrameSO.LoadElementsOnScene<FrameUIDialogueSO, FrameUIDialogue>(frame.usedElementsObjects, new FrameUIDialogue());
        FrameSO.LoadElementsOnScene<FrameCharacterSO, FrameCharacter>(frame.usedElementsObjects, new FrameCharacter());
    }
    public static void ClearElements()
    {
        foreach (var element in frameElements.ToList())
        {
            if (element != null)
                DestroyImmediate(element.gameObject);
        }
        frameElements.Clear();
    }
#if UNITY_EDITOR
    public void UpdateDirty()
    {
        foreach (var element in frameElements)
            if (EditorUtility.IsDirty(element))
                EditorUtility.ClearDirty(element);
    }
    public void OnBeforeSerialize()
    {
        serializedFrameElementsList.Clear();
        foreach(var element in frameElements)
        {
            if(element != null)
                serializedFrameElementsList.Add(element);
        }
    }
    public void OnAfterDeserialize()
    {
        frameElements.Clear();
        foreach(var element in serializedFrameElementsList)
        {
            
            if (element != null)
                frameElements.Add(element);

        }
    }
}
[CustomEditor(typeof(FrameManager))]
public class FrameManagerCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Debug.Log(target.GetInstanceID());
    }
#endif
}
