using FrameCore;
using FrameCore.ScriptableObjects;
using NodeEditorFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Node(false, "Frame/Flag/Set"), System.Serializable]
public class FlagSetNode : FrameKeyNode
{

    public const string ID = "flagSet";
    public override string GetID { get { return ID; } }

    public override string Title { get { return "FlagSet" + " " + frameKey?.id.ToString(); } }
    public override Vector2 DefaultSize { get { return new Vector2(300, 150); } }

    [ValueConnectionKnob("Output 1", Direction.Out, "FrameKey")]
    public ValueConnectionKnob output1Knob;


#if UNITY_EDITOR
    private void Awake() {
        frameEditorSO = FrameManager.assetDatabase;
    }
    public override void NodeGUI() {

        if (Application.isPlaying) return;
        if (EditorApplication.isCompiling) return;
        if (!NodeEditor.curEditorState.drawing) return;

        UpdateFrameKey();

        InsureFrameKeySelection();

        input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;
        input1Knob.SetPosition(75);
        output1Knob.maxConnectionCount = ConnectionCount.Single;
        output1Knob.SetPosition(75);

        this.backgroundColor = new Color32(135, 135, 135, 125);

        if (frameKey.flagData.keys != null) {
            foreach (var flag in frameKey.flagData.keys) {
                if (frameKey.flagData.GetValue(flag).ToString() == "True") {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(flag, FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 25));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(frameKey.flagData.GetValue(flag).ToString(), FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 25));
                    GUILayout.EndHorizontal();
                }
                else {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(flag, FrameEditor.FrameGUIUtility.GetLabelStyle(Color.red, 25));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(frameKey.flagData.GetValue(flag).ToString(), FrameEditor.FrameGUIUtility.GetLabelStyle(Color.red, 25));
                    GUILayout.EndHorizontal();
                }
            }
        }

        if (output1Knob.connected()) {
            var body = (FrameKeyNode)output1Knob.connection(0).body;
            frameKey.flagSequenceData.nextKeyID = body.frameKey.id;
        }

        FrameKeySelection();

        foreach (var knob in outputKnobs) {
            knob.color = Color.cyan;
        }

        if (GUI.changed)
            NodeEditor.curNodeCanvas.OnNodeChange(this);
    }
    protected override void OnCreate() {
        var key = new FrameKey();
        FrameManager.frame.frameKeys.Add(key);
        key = FrameManager.frame.frameKeys.Last();
        key.id = FrameManager.frame.frameKeys.IndexOf(key);

        key.keyType = FrameKey.KeyType.FlagChange;

        this.frameKey = key;
        this.frameKeyPair.frameID = FrameManager.frame.id;
        this.frameKeyPair.frameKeyID = key.id;
        this.input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;

        //Debug.Log(key.id);

        FrameManager.frame.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
        FrameManager.frame.currentKey = key;
        frameEditorSO.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
        FrameManager.ChangeFrameKey();

    }
    protected override void OnDelete() {
        var id = FrameManager.frame.frameKeys.IndexOf(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
        FrameManager.frame.frameKeys.Remove(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
        foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
            if (node.frameKeyPair.frameKeyID > id)
                node.frameKeyPair.frameKeyID -= 1;
        }
        foreach (var addkey in FrameManager.frame.frameKeys)
            addkey.id = FrameManager.frame.frameKeys.IndexOf(addkey);

    }
#endif
}
