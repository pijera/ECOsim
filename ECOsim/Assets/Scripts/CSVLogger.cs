using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVLogger : MonoBehaviour
{
    public string fileName = "Data.csv";
    private string fullPath;
    private List<string> headers;
    private bool isInitialized = false;

    void Awake()
    {
        string dataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        fullPath = Path.Combine(dataFolder, fileName);

        try
        {
            if (File.Exists(fullPath))
            {
                using (var stream = new FileStream(fullPath, FileMode.Truncate)) { }
            }
            else
            {
                File.Create(fullPath).Close();
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to clear CSV file: " + e.Message);
        }

        Debug.Log("CSV initialized at: " + fullPath);
    }

    public void LogData(Dictionary<string, float> metrics, float timeStamp)
    {
        if (!isInitialized)
        {
            headers = new List<string> { "Time" };
            headers.AddRange(metrics.Keys);
            File.AppendAllText(fullPath, string.Join(";", headers) + "\n");
            isInitialized = true;
        }

        List<string> row = new List<string>() { timeStamp.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) };

        foreach (var key in metrics.Keys)
        {
            row.Add(metrics[key].ToString(System.Globalization.CultureInfo.InvariantCulture));
        }


        File.AppendAllText(fullPath, string.Join(";", row) + "\n");
    }
}