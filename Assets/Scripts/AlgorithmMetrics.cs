using UnityEngine;
using TMPro;
using System.Diagnostics;

public class AlgorithmMetrics : MonoBehaviour
{
    public static AlgorithmMetrics Instance;

    public TMP_Text metricsText;

    private Stopwatch stopwatch;
    private int stepCount;
    private long baseMemory;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        stopwatch = new Stopwatch();
    }

    public void StartTracking(int dataSize)
    {
        stepCount = 0;
        baseMemory = dataSize * sizeof(int);

        stopwatch.Reset();
        stopwatch.Start();

        UpdateUI();
    }

    public void AddStep()
    {
        stepCount++;
        UpdateUI();
    }

    public void StopTracking()
    {
        stopwatch.Stop();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (metricsText == null) return;

        float elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        float estimatedMemoryKB = baseMemory / 1024f;

        metricsText.text =
            "Real-Time Metrics\n" +
            "Time: " + elapsedMilliseconds + " ms\n" +
            "Space: " + estimatedMemoryKB.ToString("F2") + " KB\n" +
            "Steps: " + stepCount;
    }
}