﻿using NodeEditorFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ValueType (FrameKey) для KeyNode
/// </summary>
public class FrameKeyConnectionType : ValueConnectionType {
    public override string Identifier { get { return "FrameKey"; } }
    public override Color Color { get { return Color.cyan; } }
    public override Type Type { get { return typeof(FrameCore.FrameKey); } }
}