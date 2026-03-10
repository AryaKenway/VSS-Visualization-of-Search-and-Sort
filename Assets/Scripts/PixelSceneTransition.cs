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
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        pixels.Clear();

        float gridWidth = gridParent.rect.width;
        float gridHeight = gridParent.rect.height;

        float cellWidth = gridWidth / (float)cols;
        float cellHeight = (gridHeight / (float)rows) * 1.25f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c <= cols; c++)
            {
                GameObject go = Instantiate(pixelPrefab, gridParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                Image img = go.GetComponent<Image>();

                // --- 4-COLOR CYBERPUNK DISTRIBUTION ---
                if (transitionColors != null && transitionColors.Length >= 4)
                {
                    float roll = Random.value; // Get a random number between 0 and 1

                    if (roll > 0.97f)
                    {
                        // 3% chance: THE GLITCH (Color 3) - Brightest neon
                        img.color = transitionColors[3];
                    }
                    else if (roll > 0.90f)
                    {
                        // 7% chance: THE DEPTH (Color 1 or 2) - Muted secondary neons
                        img.color = transitionColors[Random.Range(1, 3)];
                    }
                    else
                    {
                        // 90% chance: THE BASE (Color 0) - Very dark background
                        float brightnessVar = Random.Range(0.85f, 1f);
                        Color baseCol = transitionColors[0] * brightnessVar;
                        img.color = new Color(baseCol.r, baseCol.g, baseCol.b, 0.95f);
                    }
                }

                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);

                // Slightly wider overlap to ensure total coverage
                rt.sizeDelta = new Vector2(cellWidth * 1.45f, cellWidth * 1.45f);

                float xOffset = (r % 2 == 0) ? (cellWidth / 2f) : 0;
                float xPos = (c * cellWidth) + xOffset;
                float yPos = -(r * (cellHeight * 0.75f));

                rt.anchoredPosition = new Vector2(xPos, yPos);
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
        // Re-shuffle for randomness
        ShuffleList(pixels);

        // 1. Pixels POP IN
        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].DOScale(1f, pixelAnimDuration)
                     .SetEase(Ease.OutSine)
                     .SetDelay(i * delayBetweenPixels);
        }

        // Wait for the full grid to be visible
        yield return new WaitForSeconds((pixels.Count * delayBetweenPixels) + pixelAnimDuration);

        // 2. LOAD SCENE
        SceneManager.LoadScene(sceneName);

        // Give the new scene a frame to settle
        yield return new WaitForEndOfFrame();

        // 3. Pixels POP OUT
        ShuffleList(pixels); // Shuffle again for a different pattern
        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].DOScale(0f, pixelAnimDuration)
                     .SetEase(Ease.InSine)
                     .SetDelay(i * delayBetweenPixels);
        }
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