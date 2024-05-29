using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class SaveSystem
{
    private static string filePath;
    private static byte[] key = new byte[32];
    private static byte[] iv = new byte[16];

    static SaveSystem()
    {
        // Set the file path
        filePath = Application.persistentDataPath + "/data.dat";
    }

    public static void SaveToJson(object data)
    {
        // Convert the data to JSON format
        string jsonData = JsonUtility.ToJson(data);


        // Write the encrypted data to a file
        File.WriteAllText(filePath, jsonData);
    }

    public static T LoadFromJson<T>()
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Convert the decrypted data back to a string
            string jsonData = File.ReadAllText(filePath);

            // Convert the JSON data back to an object of type T
            return JsonUtility.FromJson<T>(jsonData);
        }
        else
        {
            // If the file doesn't exist, return the default value for type T
            Debug.LogWarning("No File Found.");
            return default(T);
        }
    }

    public static void DeleteFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
