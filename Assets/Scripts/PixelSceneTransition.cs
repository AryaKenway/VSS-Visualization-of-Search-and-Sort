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
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        pixels.Clear();

        Canvas.ForceUpdateCanvases();

        float screenWidth = gridParent.rect.width;
        float screenHeight = gridParent.rect.height;

        float cellWidth = screenWidth / (float)cols;

        float cellHeight = (screenHeight / (float)rows) * 1.25f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c <= cols + 1; c++)
            {
                GameObject go = Instantiate(pixelPrefab, gridParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                Image img = go.GetComponent<Image>();

                if (transitionColors != null && transitionColors.Length >= 4)
                {
                    float roll = Random.value;
                    if (roll > 0.97f)
                        img.color = transitionColors[3]; 
                    else if (roll > 0.90f)
                        img.color = transitionColors[Random.Range(1, 3)]; 
                    else
                    {
                        float brightnessVar = Random.Range(0.85f, 1f);
                        Color baseCol = transitionColors[0] * brightnessVar;
                        img.color = new Color(baseCol.r, baseCol.g, baseCol.b, 0.95f); 
                    }
                }

                rt.anchorMin = new Vector2(0, 1); 
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);

                rt.sizeDelta = new Vector2(cellWidth * 1.5f, cellWidth * 1.5f);

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
        StopAllCoroutines();
        StartCoroutine(ExecuteTransition(sceneName));
    }

    IEnumerator ExecuteTransition(string sceneName)
    {
        canvasGroup.alpha = 1f;

        if (glitchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        ShuffleList(pixels);
        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].DOScale(1f, pixelAnimDuration)
                     .SetEase(Ease.OutBack) 
                     .SetDelay(i * delayBetweenPixels);
        }

        yield return new WaitForSeconds((pixels.Count * delayBetweenPixels) + pixelAnimDuration);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;

        yield return new WaitForEndOfFrame();

        ShuffleList(pixels);
        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].DOScale(0f, pixelAnimDuration)
                     .SetEase(Ease.InSine)
                     .SetDelay(i * delayBetweenPixels);
        }

       
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