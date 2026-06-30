using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseRuntimeData
{
    [SerializeField]
    public string filename;

    public event Action OnValueChanged;


    private static MonoBehaviour coroutineRunner;
    private Coroutine saveCoroutine;
    private float saveDelay = 0.5f;

    public static void SetCoroutineRunner(MonoBehaviour runner)
    {
        coroutineRunner = runner;
    }

    public void SetSaveDelay(float delay)
    {
        saveDelay = delay;
    }



    protected void SetValue<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            NotifyValueChanged();
        }
    }

    protected void NotifyValueChanged()
    {
        OnValueChanged?.Invoke();
        SaveData();
    }

    public abstract void ResetData();

    public void SaveData()
    {
        if (saveCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(saveCoroutine);
        }

        if (coroutineRunner != null)
        {
            saveCoroutine = coroutineRunner.StartCoroutine(SaveAfterDelay());
        }
        else
        {
            SaveDataImmediate();
            Debug.LogWarning($"[{GetType().Name}] 코루틴 러너가 설정되지 않아 즉시 저장합니다.");
        }
    }

    private IEnumerator SaveAfterDelay()
    {
        yield return new WaitForSeconds(saveDelay);
        SaveDataImmediate();

    }

    private void SaveDataImmediate()
    {
        JsonDataHelper.SaveData(this, filename);
    }


    public void AddToList<T>(List<T> list, T item)
    {
        list.Add(item);
        NotifyValueChanged();
    }


    public bool RemoveFromList<T>(List<T> list, T item)
    {
        bool removed = list.Remove(item);
        if (removed)
        {
            NotifyValueChanged();
        }
        return removed;
    }


    public void ClearList<T>(List<T> list)
    {
        list.Clear();
        NotifyValueChanged();
    }
}
