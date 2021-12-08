using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#region EXSTENTIONS
public static class RectTransformExtensions { public static void SetDefaultScale(this RectTransform trans) { trans.localScale = new Vector3(1, 1, 1); } public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec) { trans.pivot = aVec; trans.anchorMin = aVec; trans.anchorMax = aVec; } public static Vector2 GetSize(this RectTransform trans) { return trans.rect.size; } public static float GetWidth(this RectTransform trans) { return trans.rect.width; } public static float GetHeight(this RectTransform trans) { return trans.rect.height; } public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos) { trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z); } public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos) { trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z); } public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos) { trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z); } public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos) { trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z); } public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos) { trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z); } public static void SetSize(this RectTransform trans, Vector2 newSize) { Vector2 oldSize = trans.rect.size; Vector2 deltaSize = newSize - oldSize; trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y); trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y)); } public static void SetWidth(this RectTransform trans, float newSize) { SetSize(trans, new Vector2(newSize, trans.rect.size.y)); } public static void SetHeight(this RectTransform trans, float newSize) { SetSize(trans, new Vector2(trans.rect.size.x, newSize)); } }
#endregion

#if UNITY_EDITOR
public class AssetManager {
    public static void UpdateAssets() {
        UpdateFrameAssetsOfType<FrameCharacterSO>("Characters");
        UpdateFrameAssetsOfType<FrameBackgroundSO>("Backgrounds");
        UpdateFrameAssetsOfType<FrameUI_DialogueSO>("UI/Dialogue Windows");
        UpdateFrameAssetsOfType<FrameUI_DialogueAnswerSO>("UI/Dialogue Windows");
        UpdateFrames();
    }
    public static void UpdateFrames() {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        List<FrameSO> list = new List<FrameSO>();
        foreach (var obj in frameEditorSO.frames.FindAll(ch => ch is FrameSO)) {
            list.Add(obj);
        }
        list = list.Except(
            AssetManager.GetAtPath<FrameSO>("Frames/")
            )
            .ToList();
        if (list.Count > 0) {
            foreach (var obj in list)
                frameEditorSO.frames.Remove(obj);
        }
        for (int i = 0; i < frameEditorSO.frames.ToList().Count; i++) {
            if (frameEditorSO.frames[i] == null) {
                frameEditorSO.frames.RemoveAt(i);
            }
        }
        for (int i = 1; i < frameEditorSO.frames.ToList().Count; i++) {
            if (frameEditorSO.frames[i].name == frameEditorSO.frames[i - 1].name)
                frameEditorSO.frames.RemoveAt(i);
        }
    }
    public static void UpdateFrameAssetsOfType<T>(string folderName)
        where T : FrameElementSO {
        FrameEditorSO frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();
        List<T> list = new List<T>();
        foreach (var obj in frameEditorSO.frameElementsObjects.FindAll(ch => ch is T)) {
            list.Add(obj as T);
        }
        list = list.Except(
            AssetManager.GetAtPath<T>("Frames/" + folderName + "/")
            )
            .ToList();
        if (list.Count > 0) {
            foreach (var obj in list)
                frameEditorSO.frameElementsObjects.Remove(obj);
        }
        for (int i = 0; i < frameEditorSO.frameElementsObjects.ToList().Count; i++) {
            if (frameEditorSO.frameElementsObjects[i] == null)
                frameEditorSO.frameElementsObjects.RemoveAt(i);
        }
        for (int i = 1; i < frameEditorSO.frameElementsObjects.ToList().Count; i++) {
            if (frameEditorSO.frameElementsObjects[i].name == frameEditorSO.frameElementsObjects[i - 1].name)
                frameEditorSO.frameElementsObjects.RemoveAt(i);
        }
    }
    public static NodeEditorFramework.NodeCanvas[] GetFramesCanvases() {
        return AssetManager.GetAtPath<NodeEditorFramework.Standard.CalculationCanvasType>("Frames/NodeCanvases/");
    }
    public static FrameSO[] GetFrameAssets() {
        return AssetManager.GetAtPath<FrameSO>("Frames/");
    }
    public static T[] GetAtPath<T>(string path) {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
        foreach (string fileName in fileEntries) {
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
#endif
[Serializable]
public class FrameKeyDictionary : Dictionary<string, FrameKey.Values>, ISerializationCallbackReceiver {
    [SerializeField]
    private List<string> keys = new List<string>();

    [SerializeField]
    private List<FrameKey.Values> values = new List<FrameKey.Values>();

    [SerializeField]
    private List<FrameElementValues.SerializedElementValues> serializedElementValues =
        new List<FrameElementValues.SerializedElementValues>();
    [SerializeField]
    private List<FrameUI_DialogueValues.SerializedDialogueValues> serializedDialogueValues =
        new List<FrameUI_DialogueValues.SerializedDialogueValues>();
    [SerializeField]
    private List<FrameUI_DialogueAnswerValues.SerializedDialogueAnswerValues> serializedDialogueAnswerValues =
        new List<FrameUI_DialogueAnswerValues.SerializedDialogueAnswerValues>();
    [SerializeField]
    private List<FrameCharacterValues.SerializedFrameCharacterValues> serializedFrameCharacterValues =
        new List<FrameCharacterValues.SerializedFrameCharacterValues>();

    public virtual void OnBeforeSerialize() {
        keys.Clear();
        values.Clear();
        serializedElementValues.Clear();
        serializedDialogueValues.Clear();
        serializedDialogueAnswerValues.Clear();
        serializedFrameCharacterValues.Clear();

        foreach (var pair in this.Where(ch => ch.Value is FrameElementValues)) {
            FrameElementValues _sev = (FrameElementValues)pair.Value;
            _sev.SetSerializedFrameKeyElementValues();
            keys.Add(pair.Key);
            serializedElementValues.Add(_sev.serializedElementValues);
        }
        foreach (var pair in this.Where(ch => ch.Value is FrameUI_DialogueValues)) {
            FrameUI_DialogueValues _sdv = (FrameUI_DialogueValues)pair.Value;
            _sdv.SetSerializedDialogueValues();
            keys.Add(pair.Key);
            serializedDialogueValues.Add(_sdv.serializedDialogueValues);
        }
        foreach (var pair in this.Where(ch => ch.Value is FrameUI_DialogueAnswerValues)) {
            FrameUI_DialogueAnswerValues _sdav = (FrameUI_DialogueAnswerValues)pair.Value;
            _sdav.SetSerializedDialogueValues();
            keys.Add(pair.Key);
            serializedDialogueAnswerValues.Add(_sdav.serializedDialogueAnswerValues);
        }
        foreach (var pair in this.Where(ch => ch.Value is FrameCharacterValues)) {
            FrameCharacterValues _schv = (FrameCharacterValues)pair.Value;
            _schv.SetSerializedFrameCharacterValues();
            keys.Add(pair.Key);
            serializedFrameCharacterValues.Add(_schv.serializedFrameCharacterValues);
        }
    }

    public virtual void OnAfterDeserialize() {
        this.Clear();
        FrameElementValues
            .LoadSerialzedFrameKeyElementValues(serializedElementValues, values);
        FrameUI_DialogueValues
            .LoadSerialzedDialogueValues(serializedDialogueValues, values);
        FrameUI_DialogueAnswerValues
            .LoadSerialzedDialogueValues(serializedDialogueAnswerValues, values);
        FrameCharacterValues
            .LoadSerialzedFrameKeyCharacterElementValues(serializedFrameCharacterValues, values);


        for (int i = 0; i < keys.Count; i++) {
            this.Add(keys[i], values[i]);
        }
    }
}
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize() {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this) {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize() {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("oh shit, here we go again"));

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}

