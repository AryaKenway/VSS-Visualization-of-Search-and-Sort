using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Debug = UnityEngine.Debug;

public class RadixSortVisualizer : MonoBehaviour
{
    public GameObject barPrefab;
    public Transform container;
    public int arraySize = 12;
    public float animationSpeed = 0.2f;

    private List<GameObject> bars = new List<GameObject>();
    private int[] data;

    private int[] snapshotData;

    void Start()
    {
        GenerateArray();
    }

    public void GenerateArray()
    {
        foreach (GameObject bar in bars) Destroy(bar);
        bars.Clear();
        data = new int[arraySize];

        HorizontalLayoutGroup layout = container.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.enabled = false;

        for (int i = 0; i < arraySize; i++)
        {
            data[i] = Random.Range(10, 999);

            GameObject newBar = Instantiate(barPrefab, container);
            RectTransform rt = newBar.GetComponent<RectTransform>();

            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 70);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, data[i]);
            newBar.transform.localScale = Vector3.one;

            newBar.GetComponentInChildren<TMP_Text>().text = data[i].ToString();
            bars.Add(newBar);
        }

        if (layout != null) layout.enabled = true;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        snapshotData = (int[])data.Clone();
        AlgorithmMetrics.Instance.StopTracking();
    }

    public void ResetToSnapshot()
    {
        if (snapshotData == null) return;

        StopAllCoroutines();

        HorizontalLayoutGroup layout = container.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.enabled = false;

        data = (int[])snapshotData.Clone();

        for (int i = 0; i < bars.Count; i++)
        {
            RectTransform rt = bars[i].GetComponent<RectTransform>();
            rt.DOKill();
            bars[i].GetComponent<Image>().DOKill();

            bars[i].GetComponent<Image>().color = Color.white;
            bars[i].GetComponentInChildren<TMP_Text>().text = data[i].ToString();

            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 70);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, data[i]);
            bars[i].transform.localScale = Vector3.one;
        }

        if (layout != null) layout.enabled = true;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        Debug.Log("<color=magenta>Radix Sort:</color> Reset to snapshot via shake.");
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
        {
            bars[i].GetComponent<Image>().DOKill();

            Highlight(i, Color.green);

            yield return new WaitForSeconds(0.05f);
        }

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

        for (int i = 0; i < n; i++)
        {
            int digit = (data[i] / exp) % 10;
            count[digit]++;

            AlgorithmMetrics.Instance.AddStep();

            Highlight(i, Color.yellow);
            AlgorithmAudioGenerator.Instance.PlayPing(data[i], 999f);
            yield return new WaitForSeconds(animationSpeed);

            bars[i].GetComponent<Image>().DOColor(Color.white, animationSpeed);
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

            rt.DOSizeDelta(new Vector2(70, data[i]), animationSpeed).SetEase(Ease.OutQuad);

            Highlight(i, Color.cyan);
            AlgorithmAudioGenerator.Instance.PlayPing(data[i], 999f);

            yield return new WaitForSeconds(animationSpeed);

            bars[i].GetComponent<Image>().DOColor(Color.white, animationSpeed);
        }
    }

    void Highlight(int index, Color color)
    {
        bars[index].GetComponent<Image>().DOColor(color, 0.15f);
    }
}