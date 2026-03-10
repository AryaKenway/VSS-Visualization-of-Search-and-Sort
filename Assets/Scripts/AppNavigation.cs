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
        SceneManager.LoadScene("MergeSortScene");
    }

    public void LoadTreeSort()
    {
        SceneManager.LoadScene("TreeSort");
    }

    public void LoadRadixSort()
    {
        PixelSceneTransition.Instance.TransitionToScene("RadixSort");
    }


    public void LoadLinearSearch()
    {
        SceneManager.LoadScene("LinearSearchScene");
    }

    public void LoadTreeSearch()
    {
        SceneManager.LoadScene("TreeSearch");
    }

    public void LoadJumpSearch()
    {
        SceneManager.LoadScene("JumpSearch");
    }
    public void LoadMainMenu()
    {
        Debug.Log("Main Menu");

        SceneManager.LoadScene("MainMenu");
    }
}