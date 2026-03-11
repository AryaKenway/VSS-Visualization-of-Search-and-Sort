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
            new AlgoData {
                best="O(n log n)",
                avg="O(n log n)",
                worst="O(n²)",
                space="O(n)",
                speech="Tree Sort works by building a Binary Search Tree from the input elements and then performing an in-order traversal. It is uniquely useful because it keeps data sorted as it is inserted, though it can become inefficient if the tree becomes unbalanced, leading to a worst-case quadratic time complexity."
            }
        },
        {
            "Merge Sort",
            new AlgoData {
                best="O(n log n)",
                avg="O(n log n)",
                worst="O(n log n)",
                space="O(n)",
                speech="Merge Sort is a classic divide-and-conquer algorithm. It recursively splits the array into halves until they are single elements, then merges them back together in perfect order. It is highly stable and guaranteed to perform in N-Log-N time, making it a top choice for sorting large, complex data sets."
            }
        },
        {
            "Radix Sort",
            new AlgoData {
                best="O(nk)",
                avg="O(nk)",
                worst="O(nk)",
                space="O(n + k)",
                speech="Unlike comparison-based sorts, Radix Sort avoids comparing numbers directly. Instead, it distributes elements into buckets based on their individual digits, starting from the least significant. This makes it incredibly fast for integers, often outperforming general-purpose algorithms in specific hardware scenarios."
            }
        },
        {
            "Linear Search",
            new AlgoData {
                best="O(1)",
                avg="O(n)",
                worst="O(n)",
                space="O(1)",
                speech="Linear Search is the most intuitive search method. it checks every single element one by one until a match is found. while it is slow for large data sets, it is the only way to search through unsorted data without a pre-processing step."
            }
        },
        {
            "Tree Search",
            new AlgoData {
                best="O(log n)",
                avg="O(log n)",
                worst="O(n)",
                space="O(1)",
                speech="Binary Tree Search leverages the structured nature of a search tree to discard half of the remaining data with every single comparison. This logarithmic efficiency allows it to find a specific item among millions of records in just a few steps, provided the tree remains well-balanced."
            }
        },
        {
            "Jump Search",
            new AlgoData {
                best="O(1)",
                avg="O(√n)",
                worst="O(√n)",
                space="O(1)",
                speech="Jump Search offers a middle ground between linear and binary search. By jumping ahead in fixed blocks and then performing a short linear scan backward, it reduces the number of comparisons significantly. It is particularly effective on systems where jumping forward is cheaper than searching backward."
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
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (tts != null)
        {
            tts.Call<int>("stop"); // Added <int> to match signature returns
        }
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError("TTS Stop failed: " + e.Message);
        }

        infoPanel.SetActive(false);
    }

    void Speak(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        tts.Call<int>("speak", message, 0, null, null);
#endif
    }
}