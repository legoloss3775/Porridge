using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FrameKey
{
    public FrameKeyDictionary frameKeyValues = new FrameKeyDictionary();

    [Serializable]
    public abstract class Values
    {
        public T GetObject<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
    }

    public void UpdateFrameElementKeyValues(FrameElement element)
    {
        element.UpdateValuesFromKey(frameKeyValues[element.id]);
    }
    public void AddFrameKeyValues(string id, Values values)
    {
        frameKeyValues.Add(id, values);
    }
    public void UpdateFrameKeyValues(string id, Values values)
    {
        frameKeyValues[id] = values;
    }
    public bool ContainsID(string id)
    {
        foreach (var value in frameKeyValues)
        {
            if (value.Key == id)
                return true;
        }
        return false;
    }
}
