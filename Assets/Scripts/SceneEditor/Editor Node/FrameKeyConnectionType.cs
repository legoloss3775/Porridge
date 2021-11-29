using NodeEditorFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameKeyConnectionType : ValueConnectionType {
    public override string Identifier { get { return "Int"; } }
    public override Color Color { get { return Color.cyan; } }
    public override Type Type { get { return typeof(int); } }
}