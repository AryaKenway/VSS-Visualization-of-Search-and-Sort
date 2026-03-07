using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PixelSceneTransition : MonoBehaviour
{
    public Image overlay;
    public float glitchDuration = 0.5f;

    Color[] glitchColors =
    {
        new Color(1f,0f,1f,0.4f),   // magenta
        new Color(0f,1f,1f,0.4f),   // cyan
        new Color(1f,0.2f,0.6f,0.4f), // pink
        new Color(0.4f,0f,1f,0.4f)  // purple
    };

    void Start()
    {
        overlay.color = new Color(0, 0, 0, 0);
    }

    public void StartGlitchTransition(string sceneName)
    {
        StartCoroutine(Glitch(sceneName));
    }

    IEnumerator Glitch(string sceneName)
    {
        float timer = 0f;

        while (timer < glitchDuration)
        {
            timer += Time.deltaTime;

            overlay.color = glitchColors[Random.Range(0, glitchColors.Length)];

            overlay.rectTransform.anchoredPosition =
                new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));

            yield return new WaitForSeconds(0.03f);
        }

        overlay.rectTransform.anchoredPosition = Vector2.zero;
        overlay.color = Color.black;

        SceneManager.LoadScene(sceneName);
    }
}