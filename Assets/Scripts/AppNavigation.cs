using UnityEngine;

public class AppNavigation : MonoBehaviour
{
    public static AppNavigation Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadMergeSort()
    {
        PixelSceneTransition.Instance.TransitionToScene("MergeSortScene");
    }

    public void LoadTreeSort()
    {
        PixelSceneTransition.Instance.TransitionToScene("TreeSort");
    }

    public void LoadRadixSort()
    {
        PixelSceneTransition.Instance.TransitionToScene("RadixSort");
    }

    public void LoadLinearSearch()
    {
        PixelSceneTransition.Instance.TransitionToScene("LinearSearchScene");
    }

    public void LoadTreeSearch()
    {
        PixelSceneTransition.Instance.TransitionToScene("TreeSearch");
    }

    public void LoadJumpSearch()
    {
        PixelSceneTransition.Instance.TransitionToScene("JumpSearch");
    }

    public void LoadMainMenu()
    {
        PixelSceneTransition.Instance.TransitionToScene("MainMenu");
    }

    public static void GoToMainMenu()
    {
        if (Instance != null)
            Instance.LoadMainMenu();
    }
}