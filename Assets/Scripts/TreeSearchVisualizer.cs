using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening; // Requires DOTween Asset

public class TreeSearchVisualizer : MonoBehaviour
{
    [Header("UI References")]
    public GameObject nodePrefab;
    public RectTransform treeContainer;
    public TMP_InputField searchInput;
    public Button searchButton;
    public Button resetButton;

    [Header("Layout Settings")]
    public float verticalSpacing = 150f;
    public float startingHorizontalSpacing = 400f;
    public int nodeCount = 15;

    [Header("Manual Offset")]
    public Vector2 treeStartingPos = new Vector2(0, 400);

    [Header("Visual Colors")]
    public Color normalColor = Color.white;
    public Color checkingColor = Color.cyan;
    public Color foundColor = Color.green;
    public Color discardedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    private TreeNode root;
    private List<TreeNode> allNodes = new List<TreeNode>();
    private List<GameObject> allLines = new List<GameObject>();
    private float maxValue = 100f; // For audio scaling

    class TreeNode
    {
        public int value;
        public RectTransform rect;
        public Image nodeImage;
        public TreeNode left;
        public TreeNode right;

        public TreeNode(int v, RectTransform r)
        {
            value = v;
            rect = r;
            nodeImage = r.GetComponent<Image>();
        }
    }

    void Start()
    {
        searchButton.onClick.AddListener(OnSearchClicked);
        resetButton.onClick.AddListener(OnResetClicked);

        GenerateRandomTree();
    }

    // ===============================
    // 1. GENERATION & SPACING
    // ===============================

    public void GenerateRandomTree()
    {
        ClearTree();
        List<int> values = new List<int>();

        while (values.Count < nodeCount)
        {
            int val = Random.Range(10, 100);
            if (!values.Contains(val)) values.Add(val);
        }
        values.Sort();

        maxValue = values[values.Count - 1];

        // Build balanced BST
        root = BuildBalancedTree(values, 0, values.Count - 1);

        // Position nodes starting from your manual offset
        PositionNodesRecursive(root, treeStartingPos, startingHorizontalSpacing);

        StartCoroutine(AnimateTreeEntry());
    }

    TreeNode BuildBalancedTree(List<int> values, int start, int end)
    {
        if (start > end) return null;

        int mid = (start + end) / 2;
        GameObject obj = Instantiate(nodePrefab, treeContainer);
        RectTransform rt = obj.GetComponent<RectTransform>();
        obj.GetComponentInChildren<TMP_Text>().text = values[mid].ToString();

        TreeNode node = new TreeNode(values[mid], rt);
        allNodes.Add(node);

        node.left = BuildBalancedTree(values, start, mid - 1);
        node.right = BuildBalancedTree(values, mid + 1, end);

        return node;
    }

    void PositionNodesRecursive(TreeNode node, Vector2 pos, float xOffset)
    {
        if (node == null) return;

        node.rect.anchoredPosition = pos;
        float nextOffset = xOffset * 0.55f;

        if (node.left != null)
            PositionNodesRecursive(node.left, pos + new Vector2(-xOffset, -verticalSpacing), nextOffset);

        if (node.right != null)
            PositionNodesRecursive(node.right, pos + new Vector2(xOffset, -verticalSpacing), nextOffset);
    }

    // ===============================
    // 2. LINE DRAWING & ANIMATION
    // ===============================

    IEnumerator AnimateTreeEntry()
    {
        foreach (var node in allNodes)
        {
            node.rect.localScale = Vector3.zero;
        }

        CreateAllLines();

        foreach (var node in allNodes)
        {
            node.rect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void CreateAllLines()
    {
        foreach (var line in allLines) Destroy(line);
        allLines.Clear();
        CreateLinesRecursive(root);
    }

    void CreateLinesRecursive(TreeNode node)
    {
        if (node == null) return;
        if (node.left != null) DrawLine(node, node.left);
        if (node.right != null) DrawLine(node, node.right);
        CreateLinesRecursive(node.left);
        CreateLinesRecursive(node.right);
    }

    void DrawLine(TreeNode parent, TreeNode child)
    {
        GameObject lineObj = new GameObject("Line", typeof(Image));
        lineObj.transform.SetParent(treeContainer, false);
        lineObj.transform.SetAsFirstSibling();
        allLines.Add(lineObj);

        Image img = lineObj.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0.4f);

        RectTransform rt = lineObj.GetComponent<RectTransform>();
        Vector2 dir = child.rect.anchoredPosition - parent.rect.anchoredPosition;
        float distance = dir.magnitude;

        rt.sizeDelta = new Vector2(distance, 4f);
        rt.anchoredPosition = parent.rect.anchoredPosition + (dir / 2);
        rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    // ===============================
    // 3. THE SEARCH LOGIC (With Metrics & Audio)
    // ===============================

    public void OnSearchClicked()
    {
        if (int.TryParse(searchInput.text, out int target))
        {
            StopAllCoroutines();
            ResetVisuals();
            StartCoroutine(SearchCoroutine(target));
        }
    }

    IEnumerator SearchCoroutine(int target)
    {
        TreeNode current = root;
        AlgorithmMetrics.Instance.StartTracking(allNodes.Count);

        while (current != null)
        {
            AlgorithmMetrics.Instance.AddStep();

            // Highlight and Play Audio
            current.nodeImage.DOColor(checkingColor, 0.3f);
            current.rect.DOScale(Vector3.one * 1.2f, 0.3f);

            // Audio Ping based on node value
            AlgorithmAudioGenerator.Instance.PlayPing(current.value, maxValue);

            yield return new WaitForSeconds(0.8f);

            if (current.value == target)
            {
                current.nodeImage.DOColor(foundColor, 0.4f);
                current.rect.DOPunchScale(Vector3.one * 0.2f, 0.5f);

                AlgorithmAudioGenerator.Instance.PlaySuccessSound();
                AlgorithmMetrics.Instance.StopTracking();
                yield break;
            }

            // Discard and move on
            current.nodeImage.DOColor(discardedColor, 0.3f);
            current.rect.DOScale(Vector3.one, 0.3f);

            current = (target < current.value) ? current.left : current.right;

            if (current == null)
            {
                Debug.Log("Value not found.");
                AlgorithmMetrics.Instance.StopTracking();
            }
        }
    }

    // ===============================
    // 4. UTILS
    // ===============================

    void ResetVisuals()
    {
        foreach (var node in allNodes)
        {
            node.nodeImage.color = normalColor;
            node.rect.localScale = Vector3.one;
        }
    }

    public void OnResetClicked()
    {
        StopAllCoroutines();
        GenerateRandomTree();
    }

    void ClearTree()
    {
        foreach (var node in allNodes) if (node.rect != null) Destroy(node.rect.gameObject);
        foreach (var line in allLines) Destroy(line);
        allNodes.Clear();
        allLines.Clear();
        root = null;
    }
}