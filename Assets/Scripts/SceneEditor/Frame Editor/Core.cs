using FrameCore;
using FrameCore.ScriptableObjects;
using FrameCore.ScriptableObjects.UI;
using FrameCore.UI;
using NodeEditorFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Дополнительные инструменты для отрисовки GUI
    /// </summary>
    public static class FrameGUIUtility {
        public static GUIStyle GetToggleStyle(Color color) {
            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.toggle);
            GUI.contentColor = Color.white;
            GUI.color = Color.white;

            //Value Color
            TextFieldStyles.normal.textColor = color;
            TextFieldStyles.hover.textColor = color;
            TextFieldStyles.active.textColor = color;
            TextFieldStyles.fontSize = 15;

            //TextFieldStyles.fontSize = fontSize;

            return TextFieldStyles;
        }
        public static GUIStyle GetPopupStyle(Color textColor) {
            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.popup);
            GUI.contentColor = Color.white;
            GUI.color = Color.white;

            //Value Color
            TextFieldStyles.normal.textColor = textColor;

            //Label Color
            //EditorStyles.label.normal.textColor = textColor;

            //TextFieldStyles.fontSize = fontSize;

            return TextFieldStyles;
        }
        public static GUIStyle SetLabelIconColor(Color imageColor) {
            GUIStyle labelStyles = new GUIStyle(EditorStyles.label);
            GUI.contentColor = Color.white;
            GUI.color = Color.white;

            //Value Color
            labelStyles.normal.background = MakeTex(2, 2, imageColor);

            //Label Color
            //EditorStyles.label.normal.textColor = imageColor;

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
            //EditorStyles.label.normal.textColor = textColor;

            TextFieldStyles.fontSize = fontSize;

            return TextFieldStyles;
        }

        public static void GuiLine(int i_height = 1) {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height);

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        }
    }
    /// <summary>
    /// Основной класс для редакторов фрейма,
    /// в нем содержатся методы для редактирования, удаления, создания элементов
    /// любого типа, наследующего от этого класса
    /// прим. при создании нового класса, необходимо добавить соответсвующие переменные в этом классе
    /// <see cref="EditorType">
    /// </summary>
    public abstract class Core : Editor {
        #region VALUES
        //Типы редакторов, необходимо добавлять новые при созданиии новых редакторов элементов
        public enum EditorType {
            DialogueEditor,     ///<see cref="FrameEditor.Dialogue">
            CharacterEditor,    ///<see cref="FrameEditor.Character">
            BackgroundEditor,   ///<see cref="FrameEditor.Background">
            FrameEffectEditor,  ///<see cref="FrameEditor.FrameEffect">
        }
        //Список для позиций ползунка для каждого из редакторов
        public static SerializableDictionary<EditorType, Vector2> scrollPositions = new SerializableDictionary<EditorType, Vector2> {
        { EditorType.DialogueEditor, new Vector2()},
        { EditorType.CharacterEditor, new Vector2()},
        { EditorType.BackgroundEditor, new Vector2()},
        { EditorType.FrameEffectEditor, new Vector2()},
    };
        //Список для поля поиска для каждого из редакторов
        public static SerializableDictionary<EditorType, string> searchTexts = new SerializableDictionary<EditorType, string> {
        { EditorType.DialogueEditor, ""},
        { EditorType.CharacterEditor, ""},
        { EditorType.BackgroundEditor, ""},
        { EditorType.FrameEffectEditor, ""},
    };
        //Список для опции сворачивания для каждого из редакторов
        public static SerializableDictionary<EditorType, bool> foldouts = new SerializableDictionary<EditorType, bool> {
        { EditorType.DialogueEditor, true},
        { EditorType.CharacterEditor, true},
        { EditorType.BackgroundEditor, true},
        { EditorType.FrameEffectEditor, true},
    };
        //Типы расположений элементов при их отрисовке в ElementEditing
        public enum PositioningType {
            Horizontal,
            Vertical,
        }
        #endregion
        /// <summary>
        /// Обобщенная функция для редактирования элементов, используемая всеми редакторами.
        /// </summary>
        /// <typeparam name="TElementSO"></typeparam>
        /// ScriptableObject элемента фрейма
        /// <typeparam name="TElement"></typeparam>
        /// MonoBehavior элемента фрейма
        /// <param name="posType"></param>
        /// Тип расположения отрисовки элементов по GUILayout
        /// <param name="editorType"></param>
        /// Требуется указывать конкретный тип редактора, заранее добавленный в enum
        /// 
        /// <see cref="EditorType">
        /// 
        /// <param name="scrollEnabled"></param>
        /// <param name="searchEnabled"></param>
        /// <param name="action"></param>
        /// Список параметров-действий, которые будут выполняться при редактировании элемента
        /// 

        public static void ElementEditing<TElementSO, TElement>(PositioningType posType, EditorType editorType, bool scrollEnabled = false, bool searchEnabled = false, params Action<TElement>[] action)
        where TElementSO : FrameElementSO
        where TElement : FrameElement {
            var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

            switch (posType) {
                case PositioningType.Horizontal: {

                    if (scrollEnabled)
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

                                if (searchEnabled && searchTexts[editorType] != "") {
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
                                    for (int i = 0; i < action.Length; i++) {
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

                    if (scrollEnabled)
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
                                    if (FrameManager.frame.usedElementsObjects.Where(ch => ch.elementObject is TElementSO).Count() > 1) {
                                        FrameGUIUtility.GuiLine();
                                    }
                                }
                                else {
                                    //TODO: else?
                                }
                            }
                        }
                    //GUILayout.FlexibleSpace();
                    if (scrollEnabled)
                        GUILayout.EndScrollView();

                    break;
                }
            }
        }
        /**public static void ElementAnimationSelection(FrameElement element){
            var keyValues = FrameElement.GetFrameKeyValues<Values>(element.id);
            var path = AssetDatabase.GetAssetPath(element.frameElementObject.prefab);
            path = path.Replace("Assets/", "");
            path = path.Replace(element.frameElementObject.prefab.name + ".prefab", "");
            if (element is FrameCore.Character) path = path.Replace(element.frameElementObject.prefab.name + ".psb", "");
            //Debug.Log(path);
            var animations = AssetManager.GetAtPath<AnimationClip>(path);
            var names = new List<string>();
            foreach(var animation in animations) {
                names.Add(animation.name);
            }
            if (names.Count == 0) return;

            int onStartAnimationSelected = 0;
            //int onActionAnimationSelected = 0;
            if(element.onStartAnimation != null && element.onStartAnimation != "") {
                onStartAnimationSelected = names.IndexOf(element.onStartAnimation);
            }
            if (element.onActionAnimation != null && element.onActionAnimation != "") {
                onActionAnimationSelected = names.IndexOf(element.onActionAnimation);
            }
            GUILayout.Space(10);
            GUILayout.Label("Анимация на старте:");
            onStartAnimationSelected = GUILayout.SelectionGrid(onStartAnimationSelected, names.ToArray(), 5, GUILayout.MaxWidth(450));
            GUILayout.Space(10);
             GUILayout.BeginHorizontal();
             GUILayout.Label("Анимация на старте:");
             onActionAnimationSelected = GUILayout.SelectionGrid(onActionAnimationSelected, names.ToArray(), 5);
             GUILayout.EndHorizontal();

            if (onStartAnimationSelected < names.Count)
                element.onStartAnimation = names[onStartAnimationSelected];
            /if (onActionAnimationSelected < names.Count)
                element.onActionAnimation = names[onActionAnimationSelected];

            if (element.onStartAnimation != keyValues.onStartAnimation)
                keyValues.onStartAnimation = element.onStartAnimation;
            if (element.onActionAnimation != keyValues.onActionAnimation)
                keyValues.onActionAnimation = element.onActionAnimation;
        }**/
        public static void ElementCreation(CreationWindow.CreationType creationType) {
            if (creationType == CreationWindow.CreationType.Frame) {
                if (GUILayout.Button("Выбрать фрейм", GUILayout.MaxWidth(282.5f))) {
                    var editor = EditorWindow.GetWindow<CreationWindow>("Создание элемента");
                    editor.type = creationType;
                    editor.ShowPopup();
                }
            }
            else {
                if (GUILayout.Button("+", GUILayout.MaxWidth(25))) {
                    var editor = EditorWindow.GetWindow<CreationWindow>();
                    editor.type = creationType;
                    editor.ShowPopup();
                }
            }
        }
        public static void ElementSelection<TElement>(TElement element)
        where TElement : FrameElement {
            if (GUILayout.Button("•", GUILayout.MaxWidth(25))) {
                Selection.activeObject = element.gameObject;
                GameObject activeGO = Selection.activeGameObject;
                SceneView.lastActiveSceneView.rotation = activeGO.transform.rotation;

                Vector3 position = activeGO.transform.position + (activeGO.transform.forward * 10);
                SceneView.lastActiveSceneView.pivot = position;

                SceneView.lastActiveSceneView.Repaint();
            }
        }
        public static void ElementDeletion<TElement>(TElement element)
            where TElement : FrameElement {
            if (GUILayout.Button("X", GUILayout.MaxWidth(25))) {

                if (!EditorUtility.DisplayDialog("Удаление элемента", "Вы точно хотите удалить элемент?", "Да", "Отмена")) return;

                FrameManager.frame.RemoveElementFromCurrentKey(element.id);

                if (element is FrameCore.UI.Dialogue) {
                    foreach (var frameElement in FrameManager.frameElements.Where(ch => ch is FrameCore.Character).ToList()) {
                        var dialogueCharacter = (FrameCore.Character)frameElement;
                        if (dialogueCharacter.dialogueID == element.id)
                            FrameManager.frame.RemoveElementFromCurrentKey(dialogueCharacter.id);
                    }
                }
                if (element is IKeyTransition) {
                    foreach (var key in FrameManager.frame.frameKeys)
                        DeleteInteractableTransitionNode(element, key);
                }
            }
        }
        /// <summary>
        /// Измение статуса активности объекта на сцене в текущем ключе
        /// При этом удаляются/создаются KeyNode соотвественно
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="element"></param>
        public static void ElementActiveStateChange<TElement>(TElement element)
            where TElement : FrameElement {
            if (GUILayout.Button("✔", GUILayout.MaxWidth(25))) {
                if (element.activeStatus)
                    ChangeActiveState(FrameManager.frame.currentKey, element, false);
                else
                    ChangeActiveState(FrameManager.frame.currentKey, element, true);
            }
        }
        public static void ChangeActiveState<TElement>(FrameKey key, TElement element, bool state)
            where TElement : FrameElement {

            var elementValues = key.GetFrameKeyValuesOfElement(element.id);

            if (state == false) {
                element.activeStatus = false;
                elementValues.transformData.activeStatus = false;
                if (element is IKeyTransition) {
                    DeleteInteractableTransitionNode(element, key);
                }
            }
            else {
                element.activeStatus = true;
                elementValues.transformData.activeStatus = true;
                if (element is IKeyTransition) {
                    CreateInteractableTransitionNode(element, key);
                }
            }

            element.SetKeyValuesWhileNotInPlayMode();

        }
        public static void DeleteInteractableTransitionNode(FrameElement element, FrameKey key) {
            if (NodeEditor.curNodeCanvas == null) return;
            foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
                if (node.frameKey == null || node.frameKeyPair.frameKeyID != key.id) continue;

                foreach (var n in node.outputKnobs.ToList()) {
                    if (key.frameKeyTransitionKnobs.ContainsKey(element.id)) {
                        //Удаление outputKnob из KeyNode
                        var removeIndex = key.frameKeyTransitionKnobs[element.id];

                        key.frameKeyTransitionKnobs.Remove(element.id);
                        DestroyImmediate(n, true);

                        //смещение индекса outputKnobs в словаре outputKnobs, привязанном к элементу, из-за которого
                        //они и удаляются
                        ///<see cref="FrameKey.frameKeyTransitionKnobs">
                        ///
                        foreach (var killme in key.frameKeyTransitionKnobs.Keys.ToList()) {
                            if (key.frameKeyTransitionKnobs[killme] >= removeIndex) {
                                key.frameKeyTransitionKnobs[killme] -= 1;
                            }
                        }
                        NodeEditorFramework.ConnectionPortManager.UpdatePortLists(node);
                    }
                }
            }
        }
        public static void CreateInteractableTransitionNode(FrameElement element, FrameKey key) {
            foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
                if (node.frameKey == null || node.frameKeyPair.frameKeyID != key.id) continue;
                if (key.frameKeyTransitionKnobs.ContainsKey(element.id)) continue;

                var knob = node.CreateValueConnectionKnob(new ValueConnectionKnobAttribute("Output", Direction.Out, "FrameKey"));
                knob.SetValue<FrameKey>(node.frameKey);

                if (element is FrameCore.UI.Dialogue || element is DialogueAnswer)
                    key.frameKeyTransitionKnobs.Add(element.id, node.connectionKnobs.IndexOf(knob) - 1);
                NodeEditorFramework.ConnectionPortManager.UpdatePortLists(node);
            }
        }
    }
    /// <summary>
    /// Окно создания элементов
    /// При добавлении нового элемента нужно пополнять список CreationType
    /// и добавлять его в switch в OnGUI()
    /// </summary>
    public class CreationWindow : EditorWindow {
        //Типы создаваемых элементов
        public enum CreationType {
            Frame,
            FrameCharacter,
            FrameDialogue,
            FrameDialogueAnswer,
            FrameBackground,
            FrameEffect,
        }
        public CreationType type;
        //ID создаваемого элемента записывается в эту статическую переменную, чтобы
        //при закрытии окна оно не обнулялось
        public static string createdElementID;
        public string searchText;
        public Vector2 scroll;
        private void OnEnable() {
            createdElementID = "";
        }
        private void OnGUI() {
            switch (type) {
                case CreationType.FrameCharacter:
                    FrameElementCreationSelection<CharacterSO, FrameCore.Character>();
                    break;
                case CreationType.FrameDialogue:
                    FrameElementCreationSelection<DialogueSO, FrameCore.UI.Dialogue>();
                    break;
                case CreationType.FrameDialogueAnswer:
                    FrameElementCreationSelection<DialogueAnswerSO, DialogueAnswer>();
                    break;
                case CreationType.FrameBackground:
                    FrameElementCreationSelection<BackgroundSO, FrameCore.Background>();
                    break;
                case CreationType.FrameEffect:
                    FrameElementCreationSelection<FrameEffectSO, FrameCore.FrameEffect>();
                    break;
                case CreationType.Frame: //для фрейма создана отдельная функция, потому что класс фрейма не относится к FrameElement
                    FrameSelection();
                    break;
            }
        }
        private void FrameElementCreationSelection<TKey, TValue>()
        where TKey : FrameElementSO
        where TValue : FrameElement {
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
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(icon, GUILayout.MaxWidth(200))) {
                    elementObject.CreateElementOnScene<TValue>(elementObject, Vector2.zero, elementObject.GetPrefabSize(), out string id);
                    createdElementID = id;

                    var createdElement = FrameManager.GetFrameElementOnSceneByID<TValue>(createdElementID);

                    /**if (createdElement is IKeyTransition) {
                        foreach (KeyNode node in NodeEditor.curNodeCanvas.nodes) {
                            if (createdElement is FrameCore.UI.Dialogue) {
                                if (node.frameKey.transitionType != FrameKey.TransitionType.DialogueLineContinue) continue;
                            }
                            if (createdElement is DialogueAnswer) {
                                if (node.frameKey.transitionType != FrameKey.TransitionType.DialogueAnswerSelection) continue;
                            }
                            var knob = node.CreateValueConnectionKnob(new ValueConnectionKnobAttribute("Output", Direction.Out, "FrameKey"));
                            knob.SetValue<FrameKey>(node.frameKey);

                            node.frameKey.dialogueOutputKnobs.Add(createdElementID, node.outputKnobs.IndexOf(knob) - 1);
                            NodeEditorFramework.ConnectionPortManager.UpdatePortLists(node);
                        }
                    }**/

                    foreach(var key in FrameManager.frame.frameKeys) {
                        Core.ChangeActiveState(key, createdElement, false);
                    }

                    FrameManager.ChangeFrameKey();
                    Close();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(elementObject.name);
                GUILayout.FlexibleSpace();
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
            foreach (var frame in frames) {
                frameNames.Add(frame.name);
            }
            frameEditorSO.selectedFrameIndex = GUILayout.SelectionGrid(frameEditorSO.selectedFrameIndex, frameNames.ToArray(), 3);
            for (int i = 0; i < frames.Count; i++) {
                if (i == frameEditorSO.selectedFrameIndex && FrameManager.frame != frames[i]) {
                    if (FrameManager.frame.nodeCanvas != null) {
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
}

#endif