using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class JumpSearchVisualizer : MonoBehaviour
{
    public GameObject barPrefab;
    public Transform container;
    public TMP_InputField targetInputField;

    public int arraySize = 10;
    public float animationDelay = 0.4f;

    private int[] data;
    private List<GameObject> bars = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        GenerateRandomArray();
    }

    public void GenerateRandomArray()
    {
        foreach (var bar in bars) Destroy(bar);
        bars.Clear();
        originalScales.Clear();

        data = new int[arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            data[i] = Random.Range(5, 80);

            GameObject newBar = Instantiate(barPrefab, container);
            newBar.GetComponentInChildren<TMP_Text>().text = data[i].ToString();

            RectTransform rt = newBar.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(60, data[i] * 5);

            bars.Add(newBar);
            originalScales[newBar] = newBar.transform.localScale;
        }
    }

    public void OnJumpSearchClicked()
    {
        if (int.TryParse(targetInputField.text, out int target))
        {
            StopAllCoroutines();
            ResetBars();
            StartCoroutine(JumpSearchCoroutine(target));
        }
    }

    IEnumerator JumpSearchCoroutine(int target)
    {
       
        System.Array.Sort(data);
        RegenerateSortedVisuals();

        yield return new WaitForSeconds(0.5f);

        AlgorithmMetrics.Instance.StartTracking(data.Length);

        int n = data.Length;
        int step = Mathf.FloorToInt(Mathf.Sqrt(n));
        int prev = 0;
        float maxVal = 80f;

        while (prev < n && data[Mathf.Min(step, n) - 1] < target)
        {
            AlgorithmMetrics.Instance.AddStep();

            int checkIndex = Mathf.Min(step, n) - 1;

            Highlight(checkIndex, Color.cyan);
            AlgorithmAudioGenerator.Instance.PlayPing(data[checkIndex], maxVal);

            yield return new WaitForSeconds(animationDelay);

            ResetBar(checkIndex);

            prev = step;
            step += Mathf.FloorToInt(Mathf.Sqrt(n));

            if (prev >= n)
            {
                AlgorithmMetrics.Instance.StopTracking();
                yield break;
            }
        }

        for (int i = prev; i < Mathf.Min(step, n); i++)
        {
            AlgorithmMetrics.Instance.AddStep();

            Highlight(i, Color.yellow);
            AlgorithmAudioGenerator.Instance.PlayPing(data[i], maxVal);

            yield return new WaitForSeconds(animationDelay);

            if (data[i] == target)
            {
                Highlight(i, Color.green);
                AlgorithmAudioGenerator.Instance.PlaySuccessSound();
                AlgorithmMetrics.Instance.StopTracking();
                yield break;
            }

            Highlight(i, Color.red);
            yield return new WaitForSeconds(0.2f);
            ResetBar(i);
        }

        AlgorithmMetrics.Instance.StopTracking();
    }

    void RegenerateSortedVisuals()
    {
        for (int i = 0; i < data.Length; i++)
        {
            bars[i].GetComponentInChildren<TMP_Text>().text = data[i].ToString();

            RectTransform rt = bars[i].GetComponent<RectTransform>();
            rt.DOSizeDelta(new Vector2(60, data[i] * 5), 0.3f);
        }
    }

    void Highlight(int index, Color color)
    {
        bars[index].GetComponent<Image>().DOColor(color, 0.2f);
        bars[index].transform.DOScale(originalScales[bars[index]] * 1.2f, 0.2f);
    }

    void ResetBar(int index)
    {
        bars[index].GetComponent<Image>().DOColor(Color.white, 0.2f);
        bars[index].transform.DOScale(originalScales[bars[index]], 0.2f);
    }

    void ResetBars()
    {
        foreach (var bar in bars)
        {
            bar.GetComponent<Image>().color = Color.white;
            bar.transform.localScale = originalScales[bar];
        }
    }
}