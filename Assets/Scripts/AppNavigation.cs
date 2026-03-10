using UnityEngine;
using UnityEngine.SceneManagement;

public class AppNavigation : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject sortingPanel;
    public GameObject searchingPanel;


    public void ShowSortingAlgorithms()
    {
        mainMenuPanel.SetActive(false);
        searchingPanel.SetActive(false);
        sortingPanel.SetActive(true);
    }

    public void ShowSearchingAlgorithms()
    {
        mainMenuPanel.SetActive(false);
        sortingPanel.SetActive(false);
        searchingPanel.SetActive(true);
    }

    public void BackToTitle()
    {
        sortingPanel.SetActive(false);
        searchingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
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
}