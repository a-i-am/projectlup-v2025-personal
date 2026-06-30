using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class CSVDataSourceAdapter : IDataSourceAdapter
{
    public IEnumerator LoadData(string url, System.Action<string> onSuccess, System.Action<string> onError)
    {
        Debug.Log($"[CSVDataSourceAdapter] LoadData started. URL: {url}");

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        Debug.Log($"[CSVDataSourceAdapter] Request completed. Result: {www.result}");

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[CSVDataSourceAdapter] Request failed: {www.error}");
            onError?.Invoke(www.error);
        }
        else
        {

            if (www.downloadHandler == null)
            {
                Debug.LogError("[CSVDataSourceAdapter] DownloadHandler is null");
                onError?.Invoke("DownloadHandler is null");
            }
            else if (string.IsNullOrEmpty(www.downloadHandler.text))
            {
                Debug.LogError("[CSVDataSourceAdapter] Downloaded data is empty");
                onError?.Invoke("Downloaded data is empty");
            }
            else
            {
                Debug.Log($"[CSVDataSourceAdapter] Data loaded successfully. Size: {www.downloadHandler.text.Length} chars");
                onSuccess?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public List<T> ParseToObjects<T>(string csvData, int startRow) where T : new()
    {

        if (string.IsNullOrEmpty(csvData))
        {
            Debug.LogError("[CSVDataSourceAdapter] csvData is null or empty");
            return new List<T>();
        }

        Debug.Log($"[CSVDataSourceAdapter] ParseToObjects called with {csvData.Length} chars");
        Debug.Log($"[CSVDataSourceAdapter] START_ROW = {startRow}");


        string[] lines = CSVParser.SplitLines(csvData);
        Debug.Log($"[CSVDataSourceAdapter] Split into {lines.Length} lines");


        int headerIndex = startRow - 1;

        if (lines.Length <= headerIndex)
        {
            Debug.LogWarning($"[CSVDataSourceAdapter] Not enough lines in CSV (have {lines.Length}, need at least {headerIndex + 1})");
            return new List<T>();
        }


        string[] headers = CSVParser.ParseLine(lines[headerIndex]);
        Debug.Log($"[CSVDataSourceAdapter] Headers from row {startRow} (index {headerIndex}): {string.Join(", ", headers)}");

        Dictionary<string, int> headerMap = new Dictionary<string, int>();
        for (int i = 0; i < headers.Length; i++)
        {
            string headerName = headers[i].Trim();
            headerMap[headerName] = i;
        }

        List<T> dataList = new List<T>();
        int successCount = 0;
        int failCount = 0;


        for (int i = headerIndex + 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;


            string[] values = CSVParser.ParseLine(lines[i]);

            try
            {
                T data = ParseDataRow<T>(values, headerMap);
                if (data != null)
                {
                    dataList.Add(data);
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[CSVDataSourceAdapter] Failed to parse line {i}: {e.Message}");
                failCount++;
            }
        }

        Debug.Log($"[CSVDataSourceAdapter] Loaded {dataList.Count} entries (Success: {successCount}, Failed: {failCount})");
        return dataList;
    }

    private T ParseDataRow<T>(string[] values, Dictionary<string, int> headerMap) where T : new()
    {
        T instance = new T();
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);


        HashSet<string> mappedColumns = new HashSet<string>();

        foreach (FieldInfo field in fields)
        {
            ColumnAttribute columnAttr = field.GetCustomAttribute<ColumnAttribute>();

            if (columnAttr == null)
                continue;

            string headerName = columnAttr.HeaderName;

            if (!headerMap.ContainsKey(headerName))
            {
                if (columnAttr.Required)
                {
                    Debug.LogWarning($"[CSVDataSourceAdapter] Required column '{headerName}' not found in headers");
                }
                continue;
            }

            int columnIndex = headerMap[headerName];
            mappedColumns.Add(headerName);

            if (columnIndex >= values.Length)
            {
                Debug.LogWarning($"[CSVDataSourceAdapter] Column '{headerName}' index {columnIndex} out of range (values length: {values.Length})");
                continue;
            }

            string value = values[columnIndex].Trim();

            try
            {
                SetFieldValue(field, instance, value);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[CSVDataSourceAdapter] Failed to set field '{field.Name}': {e.Message}");
            }
        }


        if (instance is LUP.ICustomFieldSupport customFieldSupport)
        {
            foreach (var header in headerMap)
            {
                string columnName = header.Key;
                int columnIndex = header.Value;


                if (mappedColumns.Contains(columnName))
                    continue;


                if (columnIndex < values.Length)
                {
                    string value = values[columnIndex]?.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        customFieldSupport.SetCustomField(columnName, value);
                    }
                }
            }
        }

        return instance;
    }

    private void SetFieldValue(FieldInfo field, object instance, string value)
    {
        if (field.FieldType.IsEnum)
        {
            if (System.Enum.TryParse(field.FieldType, value, true, out object enumValue))
                field.SetValue(instance, enumValue);
        }
        else if (field.FieldType == typeof(string))
        {
            field.SetValue(instance, value);
        }
        else if (field.FieldType == typeof(int))
        {
            if (int.TryParse(value, out int intValue))
                field.SetValue(instance, intValue);
            else
                Debug.LogWarning($"[CSVDataSourceAdapter] Failed to parse '{value}' as int for field '{field.Name}'");
        }
        else if (field.FieldType == typeof(float))
        {
            if (float.TryParse(value, out float floatValue))
                field.SetValue(instance, floatValue);
            else
                Debug.LogWarning($"[CSVDataSourceAdapter] Failed to parse '{value}' as float for field '{field.Name}'");
        }
        else if (field.FieldType == typeof(bool))
        {
            if (bool.TryParse(value, out bool boolValue))
                field.SetValue(instance, boolValue);
            else
                Debug.LogWarning($"[CSVDataSourceAdapter] Failed to parse '{value}' as bool for field '{field.Name}'");
        }
        else if (field.FieldType == typeof(double))
        {
            if (double.TryParse(value, out double doubleValue))
                field.SetValue(instance, doubleValue);
            else
                Debug.LogWarning($"[CSVDataSourceAdapter] Failed to parse '{value}' as double for field '{field.Name}'");
        }
        else if (field.FieldType == typeof(long))
        {
            if (long.TryParse(value, out long longValue))
                field.SetValue(instance, longValue);
            else
                Debug.LogWarning($"[CSVDataSourceAdapter] Failed to parse '{value}' as long for field '{field.Name}'");
        }
        else
        {
            Debug.LogWarning($"[CSVDataSourceAdapter] Unsupported field type: {field.FieldType} for field '{field.Name}'");
        }
    }
}
