using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveDataEditor : EditorWindow
{
    private string filePath;

    [MenuItem("Window/Save Data")]
    static void Init()
    {
        SaveDataEditor window = (SaveDataEditor)EditorWindow.GetWindow(typeof(SaveDataEditor));
        window.Show();
    }

    void OnEnable()
    {
        filePath = Path.Combine(Application.persistentDataPath, "data.dat");
    }

    void OnGUI()
    {
        GUILayout.Label("Save Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Delete Saved Data"))
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Saved data deleted.");
            }
            else
            {
                Debug.Log("Saved data file not found.");
            }
        }
    }
}
