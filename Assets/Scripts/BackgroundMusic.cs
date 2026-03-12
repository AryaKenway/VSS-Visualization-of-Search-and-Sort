using UnityEngine;
using DG.Tweening;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;
    private AudioSource audioSource;

    [Header("Settings")]
    public float maxVolume = 0.5f;
    public float fadeDuration = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            // Start the music
            audioSource.volume = 0;
            audioSource.Play();
            audioSource.DOFade(maxVolume, fadeDuration);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeOutAndStop(System.Action onComplete)
    {
        audioSource.DOFade(0, fadeDuration).OnComplete(() => {
            onComplete?.Invoke();
        });
    }

    public void FadeIn()
    {
        audioSource.DOFade(maxVolume, fadeDuration);
    }
}