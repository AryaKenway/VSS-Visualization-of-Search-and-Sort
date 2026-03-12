using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject sortingPanel;
    public GameObject searchingPanel;

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowSorting()
    {
        mainMenuPanel.SetActive(false);
        searchingPanel.SetActive(false);
        sortingPanel.SetActive(true);
    }

    public void ShowSearching()
    {
        mainMenuPanel.SetActive(false);
        sortingPanel.SetActive(false);
        searchingPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        sortingPanel.SetActive(false);
        searchingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}