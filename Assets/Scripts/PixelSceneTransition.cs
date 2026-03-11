using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PixelSceneTransition : MonoBehaviour
{
    public static PixelSceneTransition Instance;

    [Header("Settings")]
    public GameObject pixelPrefab;
    public RectTransform gridParent;
    public int rows = 35;
    public int cols = 20;
    public float pixelAnimDuration = 0.25f;
    public float delayBetweenPixels = 0.0015f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip glitchSound;

    [Header("Colors")]
    public Color[] transitionColors;

    private List<RectTransform> pixels = new List<RectTransform>();
    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            canvasGroup = GetComponent<CanvasGroup>();

            DOTween.SetTweensCapacity(1500, 100);
            // Auto-setup AudioSource if missing
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

            SetupGrid();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupGrid()
    {
        // 1. Clear old hexagons
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        pixels.Clear();

        // 2. Force Unity to recalculate UI layout before we do math
        Canvas.ForceUpdateCanvases();

        // 3. Get the ACTUAL dimensions of the container
        float screenWidth = gridParent.rect.width;
        float screenHeight = gridParent.rect.height;

        // Calculate basic cell size
        float cellWidth = screenWidth / (float)cols;

        // ADAPTIVE FIX: Calculate height based on actual screen height, 
        // then multiply by 1.25 to account for the honeycomb vertical nesting.
        float cellHeight = (screenHeight / (float)rows) * 1.25f;

        for (int r = 0; r < rows; r++)
        {
            // c <= cols + 1 ensures the right edge is covered during stagger
            for (int c = 0; c <= cols + 1; c++)
            {
                GameObject go = Instantiate(pixelPrefab, gridParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                Image img = go.GetComponent<Image>();

                // --- 4-COLOR CYBERPUNK DISTRIBUTION (Your stable logic) ---
                if (transitionColors != null && transitionColors.Length >= 4)
                {
                    float roll = Random.value;
                    if (roll > 0.97f)
                        img.color = transitionColors[3]; // Glitch
                    else if (roll > 0.90f)
                        img.color = transitionColors[Random.Range(1, 3)]; // Depth
                    else
                    {
                        float brightnessVar = Random.Range(0.85f, 1f);
                        Color baseCol = transitionColors[0] * brightnessVar;
                        img.color = new Color(baseCol.r, baseCol.g, baseCol.b, 0.95f); // Base
                    }
                }

                // --- ADAPTIVE POSITIONING ---
                rt.anchorMin = new Vector2(0, 1); // Top-Left anchor
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);

                // Use a generous overlap (1.5x) to prevent sub-pixel gaps on high-res displays
                rt.sizeDelta = new Vector2(cellWidth * 1.5f, cellWidth * 1.5f);

                float xOffset = (r % 2 == 0) ? (cellWidth / 2f) : 0;
                float xPos = (c * cellWidth) + xOffset;

                // 0.75f nesting factor "zips" the hexagons together vertically
                float yPos = -(r * (cellHeight * 0.75f));

                rt.anchoredPosition = new Vector2(xPos, yPos);
                rt.localScale = Vector3.zero;
                pixels.Add(rt);
            }
        }
    }

    public void TransitionToScene(string sceneName)
    {
        StopAllCoroutines();
        StartCoroutine(ExecuteTransition(sceneName));
    }

    IEnumerator ExecuteTransition(string sceneName)
    {
        canvasGroup.alpha = 1f;

        // --- TRIGGER CYBERPUNK AUDIO ---
        if (glitchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        // 1. Pixels POP IN (Closing Screen)
        ShuffleList(pixels);
        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].DOScale(1f, pixelAnimDuration)
                     .SetEase(Ease.OutBack) // "Pop" effect for Cyber feel
                     .SetDelay(i * delayBetweenPixels);
        }

        yield return new WaitForSeconds((pixels.Count * delayBetweenPixels) + pixelAnimDuration);

        // 2. LOAD SCENE
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;

        yield return new WaitForEndOfFrame();

        // 3. Pixels POP OUT (Opening Screen)
        ShuffleList(pixels);
        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].DOScale(0f, pixelAnimDuration)
                     .SetEase(Ease.InSine)
                     .SetDelay(i * delayBetweenPixels);
        }

        // --- SMOOTH FADE TO REVEAL ACTUAL SCENE ---
        // Start fading the whole canvas slightly before the last hexagon shrinks
        yield return new WaitForSeconds(0.15f);
        canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.5f);
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