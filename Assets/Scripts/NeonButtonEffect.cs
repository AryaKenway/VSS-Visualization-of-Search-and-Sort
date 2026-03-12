using UnityEngine;
using UnityEngine.UI;

public class NeonButtonEffect : MonoBehaviour
{
    public Image targetImage;

    public Color colorA = new Color(0f, 1f, 1f, 1f);   // cyan
    public Color colorB = new Color(0.7f, 0f, 1f, 1f); // purple

    public float transitionSpeed = 0.5f;
    public float glowStrength = 1.05f;

    void Start()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * transitionSpeed) + 1f) * 0.5f;

        Color blended = Color.Lerp(colorA, colorB, t);
        blended *= glowStrength;
        blended.a = 1f;

        targetImage.color = blended;
    }
}