using UnityEngine;
using UnityEngine.UI;

public class SearchMenuBinder : MonoBehaviour
{
    public Button treeSearchButton;
    public Button linearSearchButton;
    public Button jumpSearchButton;
    public Button backButton;

    void Start()
    {
        treeSearchButton.onClick.RemoveAllListeners();
        treeSearchButton.onClick.AddListener(() =>
        {
            AppNavigation.Instance.LoadTreeSearch();
        });

        linearSearchButton.onClick.RemoveAllListeners();
        linearSearchButton.onClick.AddListener(() =>
        {
            AppNavigation.Instance.LoadLinearSearch();
        });

        jumpSearchButton.onClick.RemoveAllListeners();
        jumpSearchButton.onClick.AddListener(() =>
        {
            AppNavigation.Instance.LoadJumpSearch();
        });

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            FindFirstObjectByType<MainMenuUI>().ShowMainMenu();
        });
    }
}