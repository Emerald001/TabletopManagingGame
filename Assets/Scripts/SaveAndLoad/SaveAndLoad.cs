using System.IO;
using UnityEngine;

public class SaveAndLoad {
    public string fileName = "SaveData.txt";

    public void Save(SaveData manager) {
        string path;
        if (Application.isEditor) {
            path = Application.dataPath + "/" + fileName;
        }
        else {
            path = Application.persistentDataPath + "/" + fileName;
        }

        StreamWriter writer = new(path, false);
        writer.Write(JsonUtility.ToJson(manager));
        writer.Dispose();
        writer.Close();
    }

    public SaveData Load() {
        string path;
        if (Application.isEditor) {
            path = Application.dataPath + "/" + fileName;
        }
        else {
            path = Application.persistentDataPath + "/" + fileName;
        }

        if (File.Exists(path)) {
            StreamReader reader = new(path);

            SaveData loadedData = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());
            reader.Dispose();
            reader.Close();

            return loadedData;
        }
        else {
            return null;
        }
    }
}