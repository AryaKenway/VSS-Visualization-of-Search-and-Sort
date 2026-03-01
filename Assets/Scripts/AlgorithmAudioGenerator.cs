using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AlgorithmAudioGenerator : MonoBehaviour
{
    public static AlgorithmAudioGenerator Instance;

    [Header("Audio Settings")]
    public float waveDuration = 0.15f;
    public float baseFrequency = 220f;
    public float maxFrequency = 880f;

    private float frequency;
    private float sampleRate;
    private double phase;
    private int sampleCount = 0;
    private int stopSample = 0;
    private bool isPlaying = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        sampleRate = AudioSettings.outputSampleRate;
    }

    void Start()
    {
        GetComponent<AudioSource>().Play();
    }

    public void PlayPing(float value, float maxValue)
    {
        frequency = Mathf.Lerp(baseFrequency, maxFrequency, value / maxValue);

        stopSample = sampleCount + (int)(waveDuration * sampleRate);
        isPlaying = true;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            float waveValue = 0;

            if (isPlaying && sampleCount < stopSample)
            {
                phase += 2 * Mathf.PI * frequency / sampleRate;
                waveValue = Mathf.Sin((float)phase);

                float remainingSamples = stopSample - sampleCount;
                float envelope = Mathf.Clamp01(remainingSamples / (waveDuration * sampleRate));
                waveValue *= envelope * 0.2f; 
            }
            else if (sampleCount >= stopSample)
            {
                isPlaying = false;
            }

            for (int j = 0; j < channels; j++)
            {
                data[i + j] = waveValue;
            }

            sampleCount++;

            if (phase > 2 * Mathf.PI) phase -= 2 * Mathf.PI;
        }
    }

    public void PlaySuccessSound() { StartCoroutine(SuccessRoutine()); }
    private IEnumerator SuccessRoutine()
    {
        PlayPing(80, 100);
        yield return new WaitForSeconds(0.1f);
        PlayPing(100, 100);
    }

    public void PlayFailSound() { PlayPing(10, 100); }
}