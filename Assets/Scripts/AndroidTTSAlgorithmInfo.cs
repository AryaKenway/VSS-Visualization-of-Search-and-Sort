using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AndroidTTSAlgorithmInfo : MonoBehaviour
{
    public GameObject infoPanel;

    public TMP_Text titleText;
    public TMP_Text bestText;
    public TMP_Text avgText;
    public TMP_Text worstText;
    public TMP_Text spaceText;

    private AndroidJavaObject tts;

    class AlgoData
    {
        public string best;
        public string avg;
        public string worst;
        public string space;
        public string speech;
    }

    Dictionary<string, AlgoData> algorithms = new Dictionary<string, AlgoData>()
    {
        {
        "Tree Sort",
        new AlgoData{
        best="O(n log n)",
        avg="O(n log n)",
        worst="O(n²)",
        space="O(n)",
        speech="Tree sort time complexity is n log n in the best and average case and n squared in the worst case. Space complexity is order n."
        }
        },

        {
        "Merge Sort",
        new AlgoData{
        best="O(n log n)",
        avg="O(n log n)",
        worst="O(n log n)",
        space="O(n)",
        speech="Merge sort time complexity is n log n in all cases. Space complexity is order n."
        }
        },

        {
        "Radix Sort",
        new AlgoData{
        best="O(nk)",
        avg="O(nk)",
        worst="O(nk)",
        space="O(n + k)",
        speech="Radix sort time complexity is n k where k is the number of digits. Space complexity is order n plus k."
        }
        },

        {
        "Linear Search",
        new AlgoData{
        best="O(1)",
        avg="O(n)",
        worst="O(n)",
        space="O(1)",
        speech="Linear search time complexity is order one in the best case and order n in the average and worst case. Space complexity is order one."
        }
        },

        {
        "Tree Search",
        new AlgoData{
        best="O(log n)",
        avg="O(log n)",
        worst="O(n)",
        space="O(1)",
        speech="Tree search time complexity is log n in the best and average case and order n in the worst case."
        }
        },

        {
        "Jump Search",
        new AlgoData{
        best="O(1)",
        avg="O(√n)",
        worst="O(√n)",
        space="O(1)",
        speech="Jump search time complexity is square root n in the average and worst case and order one in the best case."
        }
        }
    };

    void Start()
    {
        infoPanel.SetActive(false);

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity, null);

        AndroidJavaClass localeClass = new AndroidJavaClass("java.util.Locale");
        AndroidJavaObject locale = localeClass.GetStatic<AndroidJavaObject>("US");
        tts.Call<int>("setLanguage", locale);
#endif
    }

    public void ShowAlgorithmInfo(string algorithm)
    {
        if (!algorithms.ContainsKey(algorithm)) return;

        AlgoData data = algorithms[algorithm];

        infoPanel.SetActive(true);

        titleText.text = algorithm;
        bestText.text = "Best   : " + data.best;
        avgText.text = "Average: " + data.avg;
        worstText.text = "Worst  : " + data.worst;
        spaceText.text ="Space : " +data.space;

        Speak(data.speech);
    }

    public void CloseInfo()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (tts != null)
            tts.Call("stop");
#endif
        infoPanel.SetActive(false);
    }

    void Speak(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        tts.Call<int>("speak", message, 0, null, null);
#endif
    }
}