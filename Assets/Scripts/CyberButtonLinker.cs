//using UnityEngine;
//using UnityEngine.UI;

//[RequireComponent(typeof(Button))]
//public class CyberButtonLinker : MonoBehaviour
//{
//    public enum NavigationAction
//    {
//        ShowSorting,
//        ShowSearching,
//        BackToTitle,
//        LoadMergeSort,
//        LoadTreeSort,
//        LoadRadixSort,
//        LoadLinearSearch,
//        LoadTreeSearch,
//        LoadJumpSearch,
//        LoadMainMenu
//    }

//    [Header("Select Button Function")]
//    public NavigationAction buttonAction;

//    void Start()
//    {
//        Button btn = GetComponent<Button>();

//        btn.onClick.RemoveAllListeners();

//        btn.onClick.AddListener(ExecuteAction);
//    }

//    void ExecuteAction()
//    {
//        if (AppNavigation.Instance == null)
//        {
//            Debug.LogError("AppNavigation Instance not found! Ensure it is in your first scene.");
//            return;
//        }

//        switch (buttonAction)
//        {
//            case NavigationAction.ShowSorting: AppNavigation.Instance.ShowSortingAlgorithms(); break;
//            case NavigationAction.ShowSearching: AppNavigation.Instance.ShowSearchingAlgorithms(); break;
//            case NavigationAction.BackToTitle: AppNavigation.Instance.BackToTitle(); break;
//            case NavigationAction.LoadMergeSort: AppNavigation.Instance.LoadMergeSort(); break;
//            case NavigationAction.LoadTreeSort: AppNavigation.Instance.LoadTreeSort(); break;
//            case NavigationAction.LoadRadixSort: AppNavigation.Instance.LoadRadixSort(); break;
//            case NavigationAction.LoadLinearSearch: AppNavigation.Instance.LoadLinearSearch(); break;
//            case NavigationAction.LoadTreeSearch: AppNavigation.Instance.LoadTreeSearch(); break;
//            case NavigationAction.LoadJumpSearch: AppNavigation.Instance.LoadJumpSearch(); break;
//            case NavigationAction.LoadMainMenu: AppNavigation.Instance.LoadMainMenu(); break;
//        }
//    }
//}