using NodeEditorFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Node(false, "FrameKey/Test"), System.Serializable]
public class KeyNode : Node
{
    public FrameKey frameKey;
    public FrameIDKeyPair frameKeyPair;

    [System.Serializable]
    public struct FrameIDKeyPair {
        public string frameID;
        public int frameKeyID;
    }

    public const string ID = "keyNode";
    public override string GetID { get { return ID; } }

    public override string Title { get { return "KeyFrame Node"; } }
    public override Vector2 DefaultSize { get { return new Vector2(200, 200); } }

    [ValueConnectionKnob("Input 1", Direction.In, "Int")]
    public ValueConnectionKnob input1Knob;

    [ValueConnectionKnob("Output", Direction.Out, "Int")]
    public ValueConnectionKnob output1Knob;

    public List<int> inputs = new List<int>();
    public int output;
    public Vector2 scroll;

#if UNITY_EDITOR
    public override void NodeGUI() {

        frameKey = AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys[frameKeyPair.frameKeyID];

        FrameEditor_FrameKeу.ShowFrameKeyData(frameKey);
    }
#endif
}