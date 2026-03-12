using UnityEngine;

public class MenuButtonActions : MonoBehaviour
{
    public void OpenSortPanel()
    {
        FindFirstObjectByType<MainMenuUI>().ShowSorting();
    }

    public void OpenSearchPanel()
    {
        FindFirstObjectByType<MainMenuUI>().ShowSearching();
    }

    public void BackToTitle()
    {
        FindFirstObjectByType<MainMenuUI>().ShowMainMenu();
    }

    public void GoToMainMenu()
    {
        AppNavigation.GoToMainMenu();
    }
}