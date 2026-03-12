using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    private Image icon;
    public AudioSource audioSource;

    public Color[] colors = new Color[]
    {
        new Color(0f, 1f, 1f),     // Neon Cyan
        new Color(1f, 0f, 1f),     // Neon Magenta
        new Color(1f, 0.85f, 0f)   // Neon Yellow
    };

    public float duration = 11f;

    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        icon = GetComponent<Image>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        Color startColor = colors[currentIndex];
        Color endColor = colors[(currentIndex + 1) % colors.Length];

        icon.color = Color.Lerp(startColor, endColor, timer / duration);

        if (timer >= duration)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % colors.Length;
        }
    }
    public void ToggleMute()
    {
        if (audioSource == null) return;

        audioSource.mute = !audioSource.mute;
    }
}