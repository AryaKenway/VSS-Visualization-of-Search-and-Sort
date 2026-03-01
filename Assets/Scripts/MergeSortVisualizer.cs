using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class MergeSortVisualizer : MonoBehaviour
{
    public GameObject barPrefab;
    public Transform container;
    private List<GameObject> visualBars = new List<GameObject>();
    private int[] data;

    void Start()
    {
        GenerateRandomArray();
    }

    public void GenerateRandomArray()
    {
        foreach (var bar in visualBars) Destroy(bar);
        visualBars.Clear();

        data = new int[10]; 
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Random.Range(10, 100);
            GameObject newBar = Instantiate(barPrefab, container);
            newBar.GetComponentInChildren<TMP_Text>().text = data[i].ToString();

            RectTransform rt = newBar.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(60, data[i] * 2.0f);
            visualBars.Add(newBar);
        }
    }

    public void StartMergeSort() => StartCoroutine(MergeSortCoroutine(0, data.Length - 1));

    IEnumerator MergeSortCoroutine(int left, int right)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;

            yield return MergeSortCoroutine(left, mid);
            yield return MergeSortCoroutine(mid + 1, right);
            yield return Merge(left, mid, right);
        }
    }

    IEnumerator Merge(int left, int mid, int right)
    {
        int n1 = mid - left + 1;
        int n2 = right - mid;

        int[] leftArray = new int[n1];
        int[] rightArray = new int[n2];

        for (int i = 0; i < n1; i++) leftArray[i] = data[left + i];
        for (int j = 0; j < n2; j++) rightArray[j] = data[mid + 1 + j];

        int k = left;
        int iIdx = 0, jIdx = 0;

        while (iIdx < n1 && jIdx < n2)
        {
            HighlightBar(left + iIdx, Color.yellow);
            HighlightBar(mid + 1 + jIdx, Color.yellow);
            AlgorithmAudioGenerator.Instance.PlayPing(data[left + iIdx], 100);

            yield return new WaitForSeconds(0.2f);

            if (leftArray[iIdx] <= rightArray[jIdx])
            {
                data[k] = leftArray[iIdx];
                UpdateBarVisual(k, data[k]);
                iIdx++;
            }
            else
            {
                data[k] = rightArray[jIdx];
                UpdateBarVisual(k, data[k]);
                jIdx++;
            }
            k++;
        }

        while (iIdx < n1)
        {
            data[k] = leftArray[iIdx];
            UpdateBarVisual(k, data[k]);
            iIdx++; k++;
            yield return new WaitForSeconds(0.1f);
        }

        while (jIdx < n2)
        {
            data[k] = rightArray[jIdx];
            UpdateBarVisual(k, data[k]);
            jIdx++; k++;
            yield return new WaitForSeconds(0.1f);
        }

        for (int x = left; x <= right; x++) HighlightBar(x, Color.green);
        AlgorithmAudioGenerator.Instance.PlaySuccessSound();
    }

    void UpdateBarVisual(int index, int value)
    {
        visualBars[index].GetComponentInChildren<TMP_Text>().text = value.ToString();
        RectTransform rt = visualBars[index].GetComponent<RectTransform>();
        rt.DOSizeDelta(new Vector2(60, value * 2.0f), 0.2f);
    }

    void HighlightBar(int index, Color color)
    {
        visualBars[index].GetComponent<Image>().DOColor(color, 0.1f);
    }
}