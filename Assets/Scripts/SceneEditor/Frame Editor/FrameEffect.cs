using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameCore;
using FrameCore.ScriptableObjects;
using System;
using UnityEditor;
using FrameCore.FrameEffects;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс редактора эффектов
    /// </summary>
    public class FrameEffect : Core {

        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        public static void FrameEffectEditing() {
            Action<FrameCore.FrameEffect> effectEditing = EffectEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.FrameEffectEditor] = EditorGUILayout.Foldout(foldouts[EditorType.FrameEffectEditor], "Эффекты", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameEffect);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.FrameEffectEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<FrameEffectSO, FrameCore.FrameEffect>(PositioningType.Vertical, EditorType.FrameEffectEditor, false, false, effectEditing);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        public static void EffectEditing(FrameCore.FrameEffect frameEffect) {
            var keyValues = frameEffect.GetFrameKeyValues<FrameCore.Serialization.FrameEffectValues>();

            var icon = UnityEditor.AssetPreview.GetAssetPreview(frameEffect.frameElementObject.prefab);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(frameEffect);
            ElementActiveStateChange(frameEffect);
            ElementDeletion(frameEffect);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (frameEffect.activeStatus == false) {
                GUILayout.Label(frameEffect.GetName(), EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(frameEffect.GetName(), FrameGUIUtility.GetLabelStyle(Color.cyan, 15));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Параметры:", EditorStyles.boldLabel, GUILayout.MaxWidth(80));
            GUILayout.BeginVertical();
            GUILayout.Space(9);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            frameEffect.animationSpeed = EditorGUILayout.FloatField("Скорость анимации:", frameEffect.animationSpeed);
            if (frameEffect.animationSpeed != keyValues.frameEffectData.animationSpeed) keyValues.frameEffectData.animationSpeed = frameEffect.animationSpeed;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            frameEffect.animationDelay = EditorGUILayout.FloatField("Задержка анимации:", frameEffect.animationDelay);
            if (frameEffect.animationDelay != keyValues.frameEffectData.animationDelay) keyValues.frameEffectData.animationDelay = frameEffect.animationDelay;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (frameEffect.GetComponent<CameraTurn>() != null) {
                GUILayout.BeginHorizontal();
                frameEffect.cameraTurnAnimationData.degreesX = EditorGUILayout.FloatField("Поворот по горизонтали :", frameEffect.cameraTurnAnimationData.degreesX);
                if (frameEffect.cameraTurnAnimationData.degreesX != keyValues.cameraTurnAnimationData.degreesX) keyValues.cameraTurnAnimationData.degreesX = frameEffect.cameraTurnAnimationData.degreesX;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                frameEffect.cameraTurnAnimationData.degreesY = EditorGUILayout.FloatField("Поворот по вертикали :", frameEffect.cameraTurnAnimationData.degreesY);
                if (frameEffect.cameraTurnAnimationData.degreesY != keyValues.cameraTurnAnimationData.degreesY) keyValues.cameraTurnAnimationData.degreesY = frameEffect.cameraTurnAnimationData.degreesY;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            if (frameEffect.GetComponent<CameraMove>() != null) {
                GUILayout.BeginHorizontal();
                frameEffect.cameraTurnAnimationData.moveTo = EditorGUILayout.Vector3Field("Конечная позиция :", frameEffect.cameraTurnAnimationData.moveTo);
                if (frameEffect.cameraTurnAnimationData.moveTo != keyValues.cameraTurnAnimationData.moveTo) keyValues.cameraTurnAnimationData.moveTo = frameEffect.cameraTurnAnimationData.moveTo;
                GUILayout.BeginVertical();
                GUILayout.Space(20);
                if(GUILayout.Button("Скопировать позицию")) {
                    Selection.activeObject = Camera.main;
                    frameEffect.cameraTurnAnimationData.moveTo = Camera.main.transform.position;
                    keyValues.cameraTurnAnimationData.moveTo = Camera.main.transform.position;
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}
#endif
