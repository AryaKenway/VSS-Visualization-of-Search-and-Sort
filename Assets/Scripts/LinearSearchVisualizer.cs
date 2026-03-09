using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Debug = UnityEngine.Debug;

public class LinearSearchVisualizer : MonoBehaviour
{
    public GameObject barPrefab;
    public Transform container;
    public int[] data;
    public TMP_InputField targetInputField;

    private List<GameObject> visualBars = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    // --- NEW VARIABLE FOR RESET FUNCTIONALITY ---
    private int[] snapshotData;

    void Start()
    {
        GenerateArray(new int[] { 10, 25, 5, 40, 15, 30 });
    }

    public void GenerateArray(int[] newData)
    {
        foreach (var bar in visualBars) Destroy(bar);
        visualBars.Clear();
        originalScales.Clear();

        data = newData;

        // --- TAKE SNAPSHOT ---
        snapshotData = (int[])newData.Clone();

        foreach (int val in newData)
        {
            GameObject newBar = Instantiate(barPrefab, container);
            newBar.GetComponentInChildren<TMP_Text>().text = val.ToString();

            RectTransform rt = newBar.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, val * 5);

            visualBars.Add(newBar);
            originalScales[newBar] = newBar.transform.localScale;
        }
    }

    // --- NEW METHOD: RESET TO SNAPSHOT ---
    public void ResetToSnapshot()
    {
        if (snapshotData == null || snapshotData.Length == 0) return;

        StopAllCoroutines();
        // Re-generate using the stored snapshot
        GenerateArray(snapshotData);

        Debug.Log("<color=yellow>Linear Search:</color> Reset to snapshot via shake.");
    }

    public void StartLinearSearch(int target)
    {
        StartCoroutine(LinearSearchCoroutine(target));
    }

    IEnumerator LinearSearchCoroutine(int target)
    {
        float maxVal = 50f;
        AlgorithmMetrics.Instance.StartTracking(data.Length);

        for (int i = 0; i < visualBars.Count; i++)
        {
            AlgorithmMetrics.Instance.AddStep();
            Image barImage = visualBars[i].GetComponent<Image>();

            barImage.DOColor(Color.yellow, 0.3f);
            visualBars[i].transform.DOScale(originalScales[visualBars[i]] * 1.2f, 0.2f);

            int currentVal = int.Parse(visualBars[i].GetComponentInChildren<TMP_Text>().text);
            AlgorithmAudioGenerator.Instance.PlayPing(currentVal, maxVal);

            yield return new WaitForSeconds(0.5f);

            if (currentVal == target)
            {
                barImage.DOColor(Color.green, 0.3f);
                AlgorithmAudioGenerator.Instance.PlaySuccessSound();
                AlgorithmMetrics.Instance.StopTracking();
                yield break;
            }
            else
            {
                barImage.DOColor(Color.red, 0.2f);
                visualBars[i].transform.DOScale(originalScales[visualBars[i]], 0.2f);
                yield return new WaitForSeconds(0.2f);
                barImage.DOColor(Color.white, 0.3f);
            }
        }
        AlgorithmMetrics.Instance.StopTracking();
    }

    public void OnSearchButtonClicked()
    {
        StopAllCoroutines();
        foreach (var bar in visualBars)
        {
            bar.GetComponent<Image>().color = Color.white;
            bar.transform.localScale = originalScales[bar];
        }

        if (int.TryParse(targetInputField.text, out int target))
        {
            StartLinearSearch(target);
        }
        else
        {
            Debug.Log("Please enter a valid number!");
        }
    }

    public void OnRandomizeButtonClicked()
    {
        StopAllCoroutines();
        int[] randomData = new int[6];
        for (int i = 0; i < randomData.Length; i++)
        {
            randomData[i] = Random.Range(5, 50);
        }
        GenerateArray(randomData);
    }
}