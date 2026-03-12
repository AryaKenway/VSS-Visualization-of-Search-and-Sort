using UnityEngine;
using UnityEngine.UI;

public class SortingMenuBinder : MonoBehaviour
{
    public Button mergeSortButton;
    public Button treeSortButton;
    public Button radixSortButton;
    public Button backButton;

    void Start()
    {
        mergeSortButton.onClick.RemoveAllListeners();
        mergeSortButton.onClick.AddListener(() =>
        {
            AppNavigation.Instance.LoadMergeSort();
        });

        treeSortButton.onClick.RemoveAllListeners();
        treeSortButton.onClick.AddListener(() =>
        {
            AppNavigation.Instance.LoadTreeSort();
        });

        radixSortButton.onClick.RemoveAllListeners();
        radixSortButton.onClick.AddListener(() =>
        {
            AppNavigation.Instance.LoadRadixSort();
        });

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            FindFirstObjectByType<MainMenuUI>().ShowMainMenu();
        });
    }
}