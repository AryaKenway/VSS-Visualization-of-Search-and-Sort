using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PixelSceneTransition : MonoBehaviour
{
    public static PixelSceneTransition Instance;

    [Header("Settings")]
    public GameObject pixelPrefab;
    public RectTransform gridParent;
    public int rows = 20; // Increased for better coverage
    public int cols = 12;
    public float pixelAnimDuration = 0.4f;
    public float delayBetweenPixels = 0.005f;

    [Header("Colors")]
    public Color[] transitionColors = new Color[] {
        Color.black,
        new Color(0.1f, 0.1f, 0.1f), // Dark Grey
        new Color(0.2f, 0, 0.2f),    // Deep Purple
        new Color(0, 0.1f, 0.2f)     // Deep Blue
    };

    private List<RectTransform> pixels = new List<RectTransform>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupGrid();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupGrid()
    {
        // Clear existing just in case
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        pixels.Clear();

        // Use the RectTransform size instead of Screen.width/height 
        // to account for Canvas Scaling
        float width = gridParent.rect.width / (float)cols;
        float height = gridParent.rect.height / (float)rows;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject go = Instantiate(pixelPrefab, gridParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                Image img = go.GetComponent<Image>();

                // Set Random Color from our list
                if (transitionColors.Length > 0)
                    img.color = transitionColors[Random.Range(0, transitionColors.Length)];

                // Positioning
                rt.sizeDelta = new Vector2(width + 2, height + 2); // Overlap slightly to avoid gaps
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(c * width + (width / 2), -r * height - (height / 2));
                rt.localScale = Vector3.zero;

                pixels.Add(rt);
            }
        }
    }

    public void TransitionToScene(string sceneName)
    {
        StopAllCoroutines(); // Prevent overlapping transitions
        StartCoroutine(ExecuteTransition(sceneName));
    }

    IEnumerator ExecuteTransition(string sceneName)
    {
        // 1. Close the "Curtain"
        ShuffleList(pixels);

        for (int i = 0; i < pixels.Count; i++)
        {
            // We use SetDelay instead of a Sequence to avoid the "Lock" error
            pixels[i].DOScale(1f, pixelAnimDuration)
                     .SetEase(Ease.OutSine)
                     .SetDelay(i * delayBetweenPixels);
        }

        // Wait for the last pixel to finish scaling up
        yield return new WaitForSeconds((pixels.Count * delayBetweenPixels) + pixelAnimDuration);

        // 2. Load the actual scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;

        // 3. Open the "Curtain"
        ShuffleList(pixels);

        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].DOScale(0f, pixelAnimDuration)
                     .SetEase(Ease.InSine)
                     .SetDelay(i * delayBetweenPixels);
        }

        // Final wait to ensure everything is clear
        yield return new WaitForSeconds((pixels.Count * delayBetweenPixels) + pixelAnimDuration);
    }

    void ShuffleList(List<RectTransform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            RectTransform temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}