using UnityEngine;
using UnityEngine.UI;

public class SSButton : MonoBehaviour
{
    public Image targetImage;

    public Color sceneColorA = new Color(0f, 1f, 1f, 1f);
    public Color sceneColorB = new Color(0.1f, 0.9f, 1f, 1f);

    public float transitionSpeed = 0.2f;
    public float glowStrength = 1.02f;

    void Start()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * transitionSpeed, 1f);

        Color blended = Color.Lerp(sceneColorA, sceneColorB, t);
        blended *= glowStrength;
        blended.a = 1f;

        targetImage.color = blended;
    }
}