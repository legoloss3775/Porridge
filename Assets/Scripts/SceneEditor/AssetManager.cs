using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetManager
{
    public static void UpdateAssets()
    {
        UpdateFrameAssetsOfType<FrameCharacterSO>("Characters");
        UpdateFrameAssetsOfType<FrameBackgroundSO>("Backgrounds");
        UpdateFrameAssetsOfType<FrameUIDialogueSO>("UI/Dialogue Windows");
    }
    public static void UpdateFrameAssetsOfType<T>(string folderName)
        where T : FrameElementSO
    {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        List<T> list = new List<T>();
        foreach (var obj in frameEditorSO.frameElementsObjects.FindAll(ch => ch is T))
        {
            list.Add(obj as T);
        }
        list = list.Except(
            AssetManager.GetAtPath<T>("Frames/" + folderName + "/")
            )
            .ToList();
        if (list.Count > 0)
        {
            foreach (var obj in list)
                frameEditorSO.frameElementsObjects.Remove(obj);
        }
        for (int i = 1; i < frameEditorSO.frameElementsObjects.ToList().Count; i++)
        {
            if (frameEditorSO.frameElementsObjects[i] == null)
                frameEditorSO.frameElementsObjects.RemoveAt(i);
            if (frameEditorSO.frameElementsObjects[i].name == frameEditorSO.frameElementsObjects[i - 1].name)
                frameEditorSO.frameElementsObjects.RemoveAt(i);
        }
    }
    public static FrameSO[] GetFrameAssets()
    {
        return AssetManager.GetAtPath<FrameSO>("Frames/");
    }
    public static T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
        foreach (string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("/");
            string localPath = "Assets/" + path;

            if (index > 0)
                localPath += fileName.Substring(index);

            UnityEngine.Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];


        return result;
    }
}
[Serializable]
public class FrameKeyDictionary: Dictionary<string, FrameKey.Values>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<string> keys = new List<string>();

    [SerializeField]
    private List<FrameKey.Values> values = new List<FrameKey.Values>();

    [SerializeField]
    private List<FrameElementValues.SerializedElementValues> serializedElementValues = 
        new List<FrameElementValues.SerializedElementValues>();
    [SerializeField]
    private List<UIDialogueValues.SerializedDialogueValues> serializedDialogueValues = 
        new List<UIDialogueValues.SerializedDialogueValues>();
    [SerializeField]
    private List<FrameCharacterValues.SerializedFrameCharacterValues> serializedFrameCharacterValues = 
        new List<FrameCharacterValues.SerializedFrameCharacterValues>();
    public virtual void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        serializedElementValues.Clear();
        serializedDialogueValues.Clear();
        serializedFrameCharacterValues.Clear();

        foreach (var pair in this.Where(ch => ch.Value is FrameElementValues))
        {
            FrameElementValues _sev = (FrameElementValues)pair.Value;
            keys.Add(pair.Key);
            serializedElementValues.Add(_sev.SetSerializedFrameKeyElementValues());
        }
        foreach(var pair in this.Where(ch => ch.Value is UIDialogueValues))
        {
            UIDialogueValues _sdv = (UIDialogueValues)pair.Value;
            keys.Add(pair.Key);
            serializedDialogueValues.Add(_sdv.SetSerializedDialogueValues());
        }
        foreach (var pair in this.Where(ch => ch.Value is FrameCharacterValues))
        {
            FrameCharacterValues _schv = (FrameCharacterValues)pair.Value;
            keys.Add(pair.Key);
            serializedFrameCharacterValues.Add(_schv.SetSerializedFrameCharacterValues());
        }
    }

    public virtual void OnAfterDeserialize()
    {
        this.Clear();
        FrameElementValues
            .LoadSerialzedFrameKeyElementValues(serializedElementValues, values);
        UIDialogueValues
            .LoadSerialzedDialogueValues(serializedDialogueValues, values);
        FrameCharacterValues
            .LoadSerialzedFrameKeyCharacterElementValues(serializedFrameCharacterValues, values);


        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
