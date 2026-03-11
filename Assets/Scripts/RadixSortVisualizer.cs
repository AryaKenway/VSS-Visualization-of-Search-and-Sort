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

    // --- NEW: Stores the original unsorted state ---
    private int[] snapshotData;

    void Start()
    {
        GenerateArray();
    }

    public void GenerateArray()
    {
        // 1. Clean up and setup
        foreach (GameObject bar in bars) Destroy(bar);
        bars.Clear();
        data = new int[arraySize];

        // 2. Temporarily disable layout to prevent "Big/Unorganized" glitch
        HorizontalLayoutGroup layout = container.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.enabled = false;

        for (int i = 0; i < arraySize; i++)
        {
            data[i] = Random.Range(10, 999);

            GameObject newBar = Instantiate(barPrefab, container);
            RectTransform rt = newBar.GetComponent<RectTransform>();

            // 3. APPLY THE "GOOD SCALE" RULES IMMEDIATELY
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);

            // Match the scale you liked in the snapshot
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 70);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, data[i]);
            newBar.transform.localScale = Vector3.one;

            newBar.GetComponentInChildren<TMP_Text>().text = data[i].ToString();
            bars.Add(newBar);
        }

        // 4. Force immediate organized layout
        if (layout != null) layout.enabled = true;
        Canvas.ForceUpdateCanvases(); // Force UI recalculation
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

            // 5. USE THE EXACT SAME SCALE RULES AS GENERATE
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

        // --- THE FIX: Highlight ALL bars green ---
        // Ensure i goes from 0 to exactly data.Length - 1
        for (int i = 0; i < data.Length; i++)
        {
            // Kill any cyan/yellow tweens still running on this bar
            bars[i].GetComponent<Image>().DOKill();

            Highlight(i, Color.green);

            // Optional: Add a tiny delay for a "scanning" success effect
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

        // --- PHASE 1: OCCURRENCE COUNTING ---
        for (int i = 0; i < n; i++)
        {
            int digit = (data[i] / exp) % 10;
            count[digit]++;

            AlgorithmMetrics.Instance.AddStep();

            // Highlight current digit being checked
            Highlight(i, Color.yellow);
            AlgorithmAudioGenerator.Instance.PlayPing(data[i], 999f);
            yield return new WaitForSeconds(animationSpeed);

            // Return to white so the yellow doesn't bleed into the next step
            bars[i].GetComponent<Image>().DOColor(Color.white, animationSpeed);
        }

        // Accumulate counts
        for (int i = 1; i < 10; i++)
            count[i] += count[i - 1];

        // Build output array
        for (int i = n - 1; i >= 0; i--)
        {
            int digit = (data[i] / exp) % 10;
            output[count[digit] - 1] = data[i];
            count[digit]--;
        }

        // --- PHASE 2: VISUAL UPDATE (THE ADAPTIVE FIX) ---
        for (int i = 0; i < n; i++)
        {
            data[i] = output[i];
            AlgorithmMetrics.Instance.AddStep();

            // Update Text
            bars[i].GetComponentInChildren<TMP_Text>().text = data[i].ToString();

            RectTransform rt = bars[i].GetComponent<RectTransform>();

            // STABILITY CHANGE: Instead of DOSizeDelta(Vector2), we target just the Height.
            // This ensures the width (70) never changes, even for a split second.
            rt.DOSizeDelta(new Vector2(70, data[i]), animationSpeed).SetEase(Ease.OutQuad);

            // Cyberpunk Cyan Highlight
            Highlight(i, Color.cyan);
            AlgorithmAudioGenerator.Instance.PlayPing(data[i], 999f);

            yield return new WaitForSeconds(animationSpeed);

            // Optional: Fade back to white for the next EXP pass
            bars[i].GetComponent<Image>().DOColor(Color.white, animationSpeed);
        }
    }

    void Highlight(int index, Color color)
    {
        bars[index].GetComponent<Image>().DOColor(color, 0.15f);
    }
}