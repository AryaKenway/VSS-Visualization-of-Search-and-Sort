using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class RadixSortVisualizer : MonoBehaviour
{
    public GameObject barPrefab;
    public Transform container;
    public int arraySize = 12;
    public float animationSpeed = 0.2f;

    private List<GameObject> bars = new List<GameObject>();
    private int[] data;

    void Start()
    {
        GenerateArray();
    }

    public void GenerateArray()
    {
        foreach (GameObject bar in bars)
            Destroy(bar);

        bars.Clear();
        data = new int[arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            data[i] = Random.Range(10, 999);

            GameObject newBar = Instantiate(barPrefab, container);
            RectTransform rt = newBar.GetComponent<RectTransform>();

            rt.sizeDelta = new Vector2(70, data[i]);
            newBar.GetComponentInChildren<TMP_Text>().text = data[i].ToString();

            bars.Add(newBar);
        }
        AlgorithmMetrics.Instance.StopTracking();
    }

    public void StartRadixSort()
    {
        AlgorithmMetrics.Instance.StartTracking(data.Length);
        StartCoroutine(RadixSort());
    }

    IEnumerator RadixSort()
    {
        int max = GetMax();

        for (int exp = 1; max / exp > 0; exp *= 10)
        {
            yield return CountingSort(exp);
        }

        for (int i = 0; i < data.Length; i++)
            Highlight(i, Color.green);
        AlgorithmMetrics.Instance.StopTracking();
        AlgorithmAudioGenerator.Instance.PlaySuccessSound();

    }

    int GetMax()
    {
        int max = data[0];
        for (int i = 1; i < data.Length; i++)
            if (data[i] > max)
                max = data[i];

        return max;
    }

    IEnumerator CountingSort(int exp)
    {
        int n = data.Length;
        int[] output = new int[n];
        int[] count = new int[10];

        // Count digits
        for (int i = 0; i < n; i++)
        {
            int digit = (data[i] / exp) % 10;
            count[digit]++;
            AlgorithmMetrics.Instance.AddStep();
            Highlight(i, Color.yellow);
            AlgorithmAudioGenerator.Instance.PlayPing(data[i], 999f);
            yield return new WaitForSeconds(animationSpeed);
        }

        for (int i = 1; i < 10; i++)
            count[i] += count[i - 1];

        for (int i = n - 1; i >= 0; i--)
        {
            int digit = (data[i] / exp) % 10;
            output[count[digit] - 1] = data[i];
            count[digit]--;
        }

        for (int i = 0; i < n; i++)
        {
            data[i] = output[i];
            AlgorithmMetrics.Instance.AddStep();
            bars[i].GetComponentInChildren<TMP_Text>().text = data[i].ToString();

            RectTransform rt = bars[i].GetComponent<RectTransform>();
            rt.DOSizeDelta(new Vector2(70, data[i]), animationSpeed);

            Highlight(i, Color.cyan);
            AlgorithmAudioGenerator.Instance.PlayPing(data[i], 999f);
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    void Highlight(int index, Color color)
    {
        bars[index].GetComponent<Image>().DOColor(color, 0.15f);
    }
}