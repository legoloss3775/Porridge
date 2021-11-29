using NodeEditorFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Node(false, "FrameKey/Display")]
public class KeyNodeDisplay : Node
{
    public const string ID = "keyNodeDisplay";
    public override string GetID { get { return ID; } }

    public override string Title { get { return "KeyFrame Display Node"; } }
    public override Vector2 DefaultSize { get { return new Vector2(200, 100); } }
}