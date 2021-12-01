using NodeEditorFramework;
using NodeEditorFramework.Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

    [ValueConnectionKnob("Input 1", Direction.In, "FrameKey")]
    public ValueConnectionKnob input1Knob;

    [ValueConnectionKnob("Output", Direction.Out, "FrameKey")]
    public ValueConnectionKnob output1Knob;

    public float oldPos;

#if UNITY_EDITOR
    private void OnDisable() {
    }
    public override void NodeGUI() {

        frameKey = AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys[frameKeyPair.frameKeyID];

        FrameEditor_FrameKeу.ShowFrameKeyData(frameKey);
        input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;

        if (output1Knob != null) {
            output1Knob.SetValue<FrameKey>(frameKey);
            foreach(ValueConnectionKnob node in outputKnobs) {
                node.SetValue(frameKey);
                node.maxConnectionCount = ConnectionCount.Single;
            }
            if (input1Knob.connected()) {
                frameKey.keySequence.previousKey = input1Knob.GetValue<FrameKey>();
                input1Knob.GetValue<FrameKey>().keySequence.nextKey = frameKey;
            }
        }
        foreach (var characterValue in frameKey.frameKeyValues.Where(ch => ch.Value is FrameCharacterValues)) {
            if (this.connectionKnobs.Count > 10) continue;

            var p = this.CreateValueConnectionKnob(new ValueConnectionKnobAttribute("Output", Direction.Out, "FrameKey"));
            p.SetPosition(oldPos + 20f);
            p.SetValue<FrameKey>(frameKey);
            oldPos = p.sidePosition;
            
            NodeEditorFramework.ConnectionPortManager.UpdateConnectionPorts(this);
            NodeEditorFramework.ConnectionPortManager.UpdatePortLists(this);
            NodeEditorFramework.ConnectionPortManager.UpdateRepresentativePortLists(this);

        }
    }
#endif
}