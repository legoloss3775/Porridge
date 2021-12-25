using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager 
{
    public static void SavePlayer() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar +"Game.save";

        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData {
            currentFrame = FrameCore.FrameManager.assetDatabase.frames.IndexOf(FrameCore.FrameManager.frame),
            currentKey = FrameCore.FrameManager.frame.currentKey.id,
            flags = FrameCore.FrameKey.frameCoreFlags,
        };
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayer() {
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + "Game.save";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = null;
            try {
                data = formatter.Deserialize(stream) as PlayerData;
                FrameCore.FrameKey.frameCoreFlags = data.flags;
            }
            catch (System.Exception ex) { Debug.Log(ex.Message); }
            stream.Close();

            return data;
        }
        else {
            //Debug.LogError("Файл сохранения не найден " + path);
            return null;
        }
    }
    public static void SaveFlagsFile() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.streamingAssetsPath + "/FrameGlobalFlags.save";

        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData {
            flags = FrameCore.FrameKey.frameCoreFlags,
        };
        foreach(var flag in data.flags.keys.ToList()) {
            data.flags.SetValue(flag, false);
        }
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadFlagsFile() {
        string path = Application.streamingAssetsPath + "/FrameGlobalFlags.save";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = null;
            try {
                data = formatter.Deserialize(stream) as PlayerData;
                FrameCore.FrameKey.frameCoreFlags = data.flags;
            }
            catch (System.Exception ex) { Debug.Log(ex.Message); }
            stream.Close();

            return data;
        }
        else {
            //Debug.LogError("Файл сохранения не найден " + path);
            return null;
        }
    }
}
