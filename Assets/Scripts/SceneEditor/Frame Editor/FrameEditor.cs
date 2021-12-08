using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using NodeEditorFramework;

#if UNITY_EDITOR
public static class FrameGUIUtility {
    internal static int hotControl;
    public static GUIStyle SetLabelIconColor(Color imageColor) {
        GUIStyle labelStyles = new GUIStyle(EditorStyles.label);
        GUI.contentColor = Color.white;
        GUI.color = Color.white;

        //Value Color
        labelStyles.normal.background = MakeTex(2, 2, imageColor);

        //Label Color
        EditorStyles.label.normal.textColor = imageColor;

        return labelStyles;
    }
    private static Texture2D MakeTex(int width, int height, Color col) {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i) {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    public static GUIStyle GetLabelStyle(Color textColor, int fontSize) {
        GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.largeLabel);
        GUI.contentColor = Color.white;
        GUI.color = Color.white;

        //Value Color
        TextFieldStyles.normal.textColor = textColor;

        //Label Color
        EditorStyles.label.normal.textColor = textColor;

        TextFieldStyles.fontSize = fontSize;

        return TextFieldStyles;
    }
    public static GUIStyle GetTextAreaStyle(Color textColor, int fontSize) {
        GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textArea);
        GUI.contentColor = Color.white;
        GUI.color = Color.white;

        //Value Color
        TextFieldStyles.normal.textColor = textColor;

        //Label Color
        EditorStyles.label.normal.textColor = textColor;

        TextFieldStyles.fontSize = fontSize;

        return TextFieldStyles;
    }

    public static void GuiLine(int i_height = 1) {

        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

    }
}
public abstract class FrameEditor : Editor
{
    #region VALUES
    public enum EditorType {
        DialogueEditor,
        CharacterEditor,
        BackgroundEditor,
    }
    public static SerializableDictionary<EditorType, Vector2> scrollPositions = new SerializableDictionary<EditorType, Vector2> {
        { EditorType.DialogueEditor, new Vector2()},
        { EditorType.CharacterEditor, new Vector2()},
        { EditorType.BackgroundEditor, new Vector2()},
    };
    public static SerializableDictionary<EditorType, string> searchTexts = new SerializableDictionary<EditorType, string> {
        { EditorType.DialogueEditor, ""},
        { EditorType.CharacterEditor, ""},
        { EditorType.BackgroundEditor, ""},
    };
    public static SerializableDictionary<EditorType, bool> foldouts = new SerializableDictionary<EditorType, bool> {
        { EditorType.DialogueEditor, true},
        { EditorType.CharacterEditor, true},
        { EditorType.BackgroundEditor, true},
    };
    public enum PositioningType {
        Horizontal,
        Vertical,
    }
    #endregion
    public static void ElementEditing<TElementSO, TElement>(PositioningType posType, EditorType editorType, bool scrollEnabled = false, bool searchEnabled = false, params Action<TElement>[] action)
    where TElementSO : FrameElementSO
    where TElement : FrameElement {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        switch (posType) {
            case PositioningType.Horizontal: {

                if(scrollEnabled)
                    scrollPositions[editorType] = GUILayout.BeginScrollView(scrollPositions[editorType]);

                if (searchEnabled) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Поиск: ", GUILayout.MaxWidth(275));
                    GUILayout.FlexibleSpace();
                    searchTexts[editorType] = GUILayout.TextArea(searchTexts[editorType], GUILayout.MaxWidth(200));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();

                foreach (var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is TElementSO)) {
                    if (FrameManager.frame.ContainsFrameElementObject((TElementSO)obj)) {
                        foreach (var elementID in FrameManager.frame.GetFrameElementIDsByObject((TElementSO)obj).ToList()) {

                            if(searchEnabled && searchTexts[editorType] != "") {
                                if (!elementID.Split('_')[0].Contains(searchTexts[editorType]))
                                    continue;
                            }

                            var element = FrameManager.GetFrameElementOnSceneByID<TElement>(elementID);
                            if (element == null) {
                                Debug.Log(elementID);
                                continue;
                            }

                            EditorUtility.SetDirty(element);

                            if (!Application.isPlaying) {
                                for(int i = 0; i < action.Length; i++) {
                                    action[i](element);
                                }
                            }
                            else {
                                //TODO: else?
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                if(scrollEnabled)
                    GUILayout.EndScrollView();

                break;
            }
            case PositioningType.Vertical: {

                if (scrollEnabled)
                    scrollPositions[editorType] = GUILayout.BeginScrollView(scrollPositions[editorType]);

                if (searchEnabled) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Поиск: ", GUILayout.MaxWidth(75));
                    GUILayout.FlexibleSpace();
                    searchTexts[editorType] = GUILayout.TextArea(searchTexts[editorType], GUILayout.MaxWidth(150));
                    GUILayout.EndHorizontal();
                    FrameGUIUtility.GuiLine();
                }

                foreach (var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is TElementSO))
                    if (FrameManager.frame.ContainsFrameElementObject((TElementSO)obj)) {
                        foreach (var elementID in FrameManager.frame.GetFrameElementIDsByObject((TElementSO)obj).ToList()) {

                            if (searchEnabled && searchTexts[editorType] != "") {
                                if (!elementID.Split('_')[0].Contains(searchTexts[editorType]))
                                    continue;
                            }

                            var element = FrameManager.GetFrameElementOnSceneByID<TElement>(elementID);
                            if (element == null) {
                                continue;
                            }
                            EditorUtility.SetDirty(element);

                            if (!Application.isPlaying) {
                                for (int i = 0; i < action.Length; i++) {
                                    action[i](element);
                                }
                                if(FrameManager.frame.usedElementsObjects.Where(ch => ch.elementObject is TElementSO).Count() > 1 ) {
                                    FrameGUIUtility.GuiLine();
                                }
                            }
                            else {
                                //TODO: else?
                            }
                        }
                    }
                //GUILayout.FlexibleSpace();
                if(scrollEnabled)
                    GUILayout.EndScrollView();

                break;
            }
        }
    }
    public static void ElementCreation(FrameEditor_CreationWindow.CreationType creationType) {
        if (creationType == FrameEditor_CreationWindow.CreationType.Frame) {
            if (GUILayout.Button("Выбрать фрейм", GUILayout.MaxWidth(282.5f))) {
                var editor = EditorWindow.GetWindow<FrameEditor_CreationWindow>();
                editor.type = creationType;
                editor.ShowPopup();
            }
        }
        else {
            if (GUILayout.Button("+", GUILayout.MaxWidth(25))) {
                var editor = EditorWindow.GetWindow<FrameEditor_CreationWindow>();
                editor.type = creationType;
                editor.ShowPopup();
            }
        }
    }
    public static void ElementSelection<TElement>(TElement element)
    where TElement : FrameElement {
        if(GUILayout.Button("•", GUILayout.MaxWidth(25))) {
            Selection.activeObject = element.gameObject;
            GameObject activeGO = Selection.activeGameObject;
            SceneView.lastActiveSceneView.rotation = activeGO.transform.rotation;

            Vector3 position = activeGO.transform.position + (activeGO.transform.forward * 10);
            SceneView.lastActiveSceneView.pivot = position;

            SceneView.lastActiveSceneView.Repaint();
        }
    }
    public static void ElementDeletion<TElement>(TElement element)
        where TElement: FrameElement {
        if (GUILayout.Button("X", GUILayout.MaxWidth(25))) {
            FrameManager.frame.RemoveElementFromCurrentKey(element.id);

            if(element is FrameUI_Dialogue) {
                foreach(var frameElement in FrameManager.frameElements.Where(ch => ch is FrameCharacter).ToList()) {
                    var dialogueCharacter = (FrameCharacter)frameElement;
                    if (dialogueCharacter.dialogueID == element.id)
                        FrameManager.frame.RemoveElementFromCurrentKey(dialogueCharacter.id);
                }
            }
            if (element is IInteractable) {
                foreach(var key in FrameManager.frame.frameKeys)
                    DeleteInteractableTransitionNode(element, key);
            }
        }
    }
    public static void ElementActiveStateChange<TElement>(TElement element)
        where TElement: FrameElement{
        if(GUILayout.Button("✔", GUILayout.MaxWidth(25))) {
            if (element.activeStatus)
                ChangeActiveState(FrameManager.frame.currentKey,element, false);
            else
                ChangeActiveState(FrameManager.frame.currentKey, element, true);
        }
    }
    public static void ChangeActiveState<TElement>(FrameKey key,TElement element, bool state)
        where TElement: FrameElement{

        var elementValues = key.GetFrameKeyValuesOfElement(element.id);

        if (state == false) {
            element.activeStatus = false;
            elementValues.activeStatus = false;
            if (element is IInteractable) {
                DeleteInteractableTransitionNode(element, key);
            }
        }
        else {
            element.activeStatus = true;
            elementValues.activeStatus = true;
            if (element is IInteractable) {
                CreateInteractableTransitionNode(element, key);
            }
        }

        element.SetKeyValuesWhileNotInPlayMode();
        
    }
    public static void DeleteInteractableTransitionNode(FrameElement element, FrameKey key) {
        if (NodeEditor.curNodeCanvas == null) return;
        foreach (KeyNode node in NodeEditor.curNodeCanvas.nodes) {
            if (node.frameKey == null || node.frameKeyPair.frameKeyID != key.id) continue;

                foreach (var n in node.outputKnobs.ToList()) {
                if (key.dialogueOutputKnobs.ContainsKey(element.id)) { 
                    var removeIndex = key.dialogueOutputKnobs[element.id];

                    key.dialogueOutputKnobs.Remove(element.id);
                    DestroyImmediate(n, true);

                    foreach (var killme in key.dialogueOutputKnobs.Keys.ToList()) {
                        if (key.dialogueOutputKnobs[killme] >= removeIndex) {
                            key.dialogueOutputKnobs[killme] -= 1;
                        }
                    }
                    NodeEditorFramework.ConnectionPortManager.UpdatePortLists(node);
                }
            }
        }
    }
    public static void CreateInteractableTransitionNode(FrameElement element, FrameKey key) {
        foreach (KeyNode node in NodeEditor.curNodeCanvas.nodes) {
            if (node.frameKey == null || node.frameKeyPair.frameKeyID != key.id) continue;
            if (key.dialogueOutputKnobs.ContainsKey(element.id)) continue;

            var knob = node.CreateValueConnectionKnob(new ValueConnectionKnobAttribute("Output", Direction.Out, "FrameKey"));
            knob.SetValue<FrameKey>(node.frameKey);
            node.oldPos = knob.sidePosition;

            if (element is FrameUI_Dialogue || element is FrameUI_DialogueAnswer)
                key.dialogueOutputKnobs.Add(element.id, node.connectionKnobs.IndexOf(knob) - 1);
            NodeEditorFramework.ConnectionPortManager.UpdatePortLists(node);
        }
    }
}
public class FrameEditor_CreationWindow : EditorWindow {
    public enum CreationType {
        Frame,
        FrameCharacter,
        FrameDialogue,
        FrameDialogueAnswer,
        FrameBackground,
    }
    public CreationType type;
    public static string createdElementID;
    public string searchText;
    public Vector2 scroll;
    private void OnEnable() {
        createdElementID = "";
    }
    private void OnGUI() {
        switch (type) {
            case CreationType.FrameCharacter:
                FrameElementCreationSelection<FrameCharacterSO, FrameCharacter>();
                break;
            case CreationType.FrameDialogue:
                FrameElementCreationSelection<FrameUI_DialogueSO, FrameUI_Dialogue>();
                break;
            case CreationType.FrameDialogueAnswer:
                FrameElementCreationSelection<FrameUI_DialogueAnswerSO, FrameUI_DialogueAnswer>();
                break;
            case CreationType.FrameBackground:
                FrameElementCreationSelection<FrameBackgroundSO, FrameBackground>();
                break;
            case CreationType.Frame:
                FrameSelection();
                break;
        }
    }
    private void FrameElementCreationSelection<TKey, TValue>()
    where TKey : global::FrameElementSO
    where TValue : global::FrameElement {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Поиск:", GUILayout.Width(50));
        searchText = GUILayout.TextArea(searchText, GUILayout.MaxWidth(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        scroll = GUILayout.BeginScrollView(scroll);
        foreach (var elementObject in frameEditorSO.frameElementsObjects.FindAll(el => el is TKey)) {

            if (searchText != null && searchText != "") {
                if (!elementObject.name.Split('_')[0].Contains(searchText))
                    continue;
            }

            GUILayout.BeginHorizontal();
            var icon = UnityEditor.AssetPreview.GetAssetPreview(elementObject.prefab);
            GUILayout.BeginVertical();
            if (GUILayout.Button(icon, GUILayout.MaxWidth(200))) {
                if(elementObject is FrameUI_DialogueSO || 
                    elementObject is FrameUI_DialogueAnswerSO) {
                    elementObject.CreateElementOnScene<TValue>(elementObject, Vector2.zero, elementObject.prefab.GetComponent<RectTransform>().sizeDelta, out string id);
                    createdElementID = id;
                }
                else {
                    elementObject.CreateElementOnScene<TValue>(elementObject, Vector2.zero, elementObject.prefab.gameObject.transform.localScale, out string id);
                    createdElementID = id;
                }

                var createdElement = FrameManager.GetFrameElementOnSceneByID<TValue>(createdElementID);
                //
                if(createdElement is IInteractable) {
                    foreach(KeyNode node in NodeEditor.curNodeCanvas.nodes) {
                        if(createdElement is FrameUI_Dialogue) {
                            if (node.frameKey.transitionType != FrameKey.TransitionType.DialogueLineContinue) continue;
                        }
                        if (createdElement is FrameUI_DialogueAnswer) {
                            if (node.frameKey.transitionType != FrameKey.TransitionType.DialogueAnswerSelection) continue;
                        }
                        var knob = node.CreateValueConnectionKnob(new ValueConnectionKnobAttribute("Output", Direction.Out, "FrameKey"));
                        knob.SetValue<FrameKey>(node.frameKey);

                        node.frameKey.dialogueOutputKnobs.Add(createdElementID, node.outputKnobs.IndexOf(knob) );
                        NodeEditorFramework.ConnectionPortManager.UpdatePortLists(node);
                    }
                }
                
                FrameManager.ChangeFrameKey();
                Close();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(75);
            GUILayout.Label(elementObject.name);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
    }
    private void FrameSelection() {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        List<FrameSO> frames = new List<FrameSO>();
        List<string> frameNames = new List<string>();

        if (AssetManager.GetFrameAssets().Length == 0) {
            CreateFrame();
        }
        if (GUILayout.Button("Создать новый фрейм", GUILayout.MaxWidth(250)))
            CreateFrame();

        foreach (var frame in AssetManager.GetFrameAssets()) {
            frames.Add(frame);
        }
        foreach(var frame in frames) {
            frameNames.Add(frame.name);
        }
        frameEditorSO.selectedFrameIndex = GUILayout.SelectionGrid(frameEditorSO.selectedFrameIndex, frameNames.ToArray(), 3 );
        for (int i = 0; i < frames.Count; i++) {
            if (i == frameEditorSO.selectedFrameIndex && FrameManager.frame != frames[i]) {
                if (FrameManager.frame.nodeCanvas != null){
                    NodeEditorFramework.Standard.NodeEditorWindow.editor.canvasCache.SaveNodeCanvas("Assets/Frames/NodeCanvases/Canvas_" + FrameManager.frame.id + ".asset");
                }

                FrameManager.frame = frames[i];

                FrameManager.ChangeFrame();
                FrameManager.ChangeFrameKey();
                Close();
            }
        }
    }
    public static void CreateFrame() {
        var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

        if (FrameManager.frame != null && FrameManager.frame.nodeCanvas != null) {
            NodeEditorFramework.Standard.NodeEditorWindow.editor.canvasCache.SaveNodeCanvas("Assets/Frames/NodeCanvases/Canvas_" + FrameManager.frame.id + ".asset");
        }

        int count = AssetManager.GetAtPath<FrameSO>("Frames/").Length;
        string path = "Assets/Frames/Frame " + count + ".asset";
        FrameSO frame = ScriptableObject.CreateInstance<FrameSO>();
        frame.selectedKeyIndex = 0;
        AssetDatabase.CreateAsset(frame, path);
        frame.id = "Frame_" + count;
        FrameManager.frame = frame;
        frameEditorSO.selectedFrameIndex = AssetManager.GetFrameAssets().Length - 1;
        frame.CreateNodeCanvas();
        frame.AddKey(new FrameKey());

        FrameManager.ChangeFrame();
        FrameManager.ChangeFrameKey();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
}
#endif