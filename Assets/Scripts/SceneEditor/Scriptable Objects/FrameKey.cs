using System;

[Serializable]
public class FrameKey {
    public FrameKeyDictionary frameKeyValues = new FrameKeyDictionary();

    [Serializable]
    public abstract class Values {
        public T GetObject<T>(params object[] args) {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
    }
    public FrameKeyDictionary GetFrameKeyValuesOfType<T>()
        where T : Values {
        var exitDictionary = new FrameKeyDictionary();
        foreach (var pair in frameKeyValues) {
            if (pair.Value is T) {
                exitDictionary.Add(pair.Key, pair.Value);
            }
        }
        return exitDictionary;
    }
    public Values GetFrameKeyValuesOfElement(string id) {
        if (ContainsID(id)) return frameKeyValues[id];
        else throw new Exception("Не найдены значения объекта " + id + " в ключе");
    }
    public void UpdateFrameKeyValues(string id, Values values) {
        if (ContainsID(id)) frameKeyValues[id] = values;
        else AddFrameKeyValues(id, values);
    }
    public void AddFrameKeyValues(string id, Values values) => frameKeyValues.Add(id, values);
    public bool ContainsID(string id) {
        if (id != null)
            return frameKeyValues.ContainsKey(id);
        else return false;
    }
}
