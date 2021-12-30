using FrameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameEditor {
    public  class Cutscene : Editor {

        public static void CutsceneEditing() {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Cutscene", FrameGUIUtility.GetLabelStyle(FrameKeyNode.ORANGE, 30));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            FrameManager.frame.currentKey.cutscenePrefab = (GameObject)EditorGUILayout.ObjectField(FrameManager.frame.currentKey.cutscenePrefab, typeof(GameObject), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
    }
}
