using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

#if UNITY_EDITOR 
public abstract class FrameEditor_Element : Editor
{
    #region VALUES
    public enum EditorType {
        DialogueEditor,
        CharacterEditor,
    }
    public static SerializableDictionary<EditorType, Vector2> scrollPositions = new SerializableDictionary<EditorType, Vector2> {
        { EditorType.DialogueEditor, new Vector2()},
        { EditorType.CharacterEditor, new Vector2()},
    };
    public static SerializableDictionary<EditorType, string> searchTexts = new SerializableDictionary<EditorType, string> {
        { EditorType.DialogueEditor, ""},
        { EditorType.CharacterEditor, ""},
    };
    public static SerializableDictionary<EditorType, bool> foldouts = new SerializableDictionary<EditorType, bool> {
        { EditorType.DialogueEditor, false},
        { EditorType.CharacterEditor, false},
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
                    searchTexts[editorType] = GUILayout.TextArea(searchTexts[editorType], GUILayout.MaxWidth(200));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }
                GUILayout.BeginHorizontal();

                foreach (var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is TElementSO)) {
                    if (FrameManager.frame.ContainsFrameElementObject((TElementSO)obj)) {
                        foreach (var elementID in FrameManager.frame.GetFrameElementIDsByObject((TElementSO)obj)) {

                            if(searchEnabled && searchTexts[editorType] != "") {
                                if (!elementID.Split('_')[0].Contains(searchTexts[editorType]))
                                    continue;
                            }

                            //GUILayout.BeginVertical("HelpBox");

                            var element = FrameManager.GetFrameElementOnSceneByID<TElement>(elementID);
                            if (element == null) {
                                Debug.Log(elementID);
                                continue;
                            }

                            EditorUtility.SetDirty(element);

                            if (!Application.isPlaying) {
                                for(int i = 0; i < action.Length; i++) {
                                    ElementSelection(element);
                                    action[i](element);
                                }
                            }
                            else {
                                //TODO: else?
                            }
                            //GUILayout.EndVertical();
                        }
                    }
                }
                GUILayout.EndHorizontal();

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
                    searchTexts[editorType] = GUILayout.TextArea(searchTexts[editorType], GUILayout.MaxWidth(275));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }

                foreach (var obj in frameEditorSO.frameElementsObjects.Where(ch => ch is TElementSO))
                    if (FrameManager.frame.ContainsFrameElementObject((TElementSO)obj)) {
                        foreach (var elementID in FrameManager.frame.GetFrameElementIDsByObject((TElementSO)obj)) {

                            if (searchEnabled && searchTexts[editorType] != "") {
                                if (!elementID.Split('_')[0].Contains(searchTexts[editorType]))
                                    continue;
                            }
                            //GUILayout.BeginVertical("HelpBox");

                            var element = FrameManager.GetFrameElementOnSceneByID<TElement>(elementID);
                            if (element == null) {
                                continue;
                            }
                            EditorUtility.SetDirty(element);

                            if (!Application.isPlaying) {
                                for (int i = 0; i < action.Length; i++) {
                                    action[i](element);
                                }
                            }
                            else {
                                //TODO: else?
                            }
                            //GUILayout.EndVertical();
                        }
                    }
                GUILayout.FlexibleSpace();
                if(scrollEnabled)
                    GUILayout.EndScrollView();

                break;
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
}
#endif