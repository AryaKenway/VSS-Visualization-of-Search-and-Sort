using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class TreeSortVisualizer : MonoBehaviour
{
    public GameObject nodePrefab;
    public RectTransform treeContainer;

    public float verticalSpacing = 230f;
    public float horizontalSpacing = 650f;
    public float sortedYPosition = -350f;

    private TreeNode root;
    private List<TreeNode> allNodes = new List<TreeNode>();
    private int sortedIndex = 0;
    private int maxValue;

    class TreeNode
    {
        public int value;
        public RectTransform rect;
        public TreeNode left;
        public TreeNode right;

        public TreeNode(int val, RectTransform rt)
        {
            value = val;
            rect = rt;
        }
    }

    void Start()
    {
        GenerateRandomTree();
    }

    public void GenerateRandomTree()
    {
        ClearTree();

        List<int> values = new List<int>();

        while (values.Count < 7)
        {
            int val = Random.Range(10, 100);
            if (!values.Contains(val))
                values.Add(val);
        }

        maxValue = 0;

        foreach (int value in values)
        {
            InsertNode(value);

            if (value > maxValue)
                maxValue = value;
        }
    }
    //void BuildBalancedTree(List<int> values, int start, int end)
    //{
    //    if (start > end) return;

    //    int mid = (start + end) / 2;
    //    InsertNode(values[mid]);

    //    BuildBalancedTree(values, start, mid - 1);
    //    BuildBalancedTree(values, mid + 1, end);
    //}

    void ClearTree()
    {
        foreach (Transform child in treeContainer)
            Destroy(child.gameObject);

        root = null;
        allNodes.Clear();
        sortedIndex = 0;
    }

    void InsertNode(int value)
    {
        root = InsertRecursive(root, value, 0, 0, horizontalSpacing, null);
    }

    TreeNode InsertRecursive(TreeNode node, int value, int depth, float x, float spread, TreeNode parent)
    {
        if (node == null)
        {
            GameObject obj = Instantiate(nodePrefab, treeContainer);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, -depth * verticalSpacing);
            rt.localScale = Vector3.zero;
            rt.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            obj.GetComponentInChildren<TMP_Text>().text = value.ToString();

            TreeNode newNode = new TreeNode(value, rt);
            allNodes.Add(newNode);

            if (parent != null)
                CreateLine(parent, newNode);

            return newNode;
        }

        if (value < node.value)
            node.left = InsertRecursive(node.left, value, depth + 1, x - spread / 2, spread / 2, node);
        else
            node.right = InsertRecursive(node.right, value, depth + 1, x + spread / 2, spread / 2, node);

        return node;
    }

    void CreateLine(TreeNode parent, TreeNode child)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(treeContainer);

        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = Color.white;

        RectTransform rt = lineObj.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);

        Vector2 start = parent.rect.anchoredPosition;
        Vector2 end = child.rect.anchoredPosition;

        Vector2 direction = end - start;
        float length = direction.magnitude;

        rt.sizeDelta = new Vector2(length, 4f); // 4px thickness
        rt.anchoredPosition = start + direction / 2f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);

    }
    void RemoveAllLines()
    {
        List<GameObject> linesToRemove = new List<GameObject>();

        foreach (Transform child in treeContainer)
        {
            if (child.name == "Line")
                linesToRemove.Add(child.gameObject);
        }

        foreach (GameObject line in linesToRemove)
            Destroy(line);
    }
    public void StartTreeSort()
    {
        AlgorithmMetrics.Instance.StartTracking(allNodes.Count);
        StartCoroutine(InOrderTraversal(root));
    }

    IEnumerator InOrderTraversal(TreeNode node)
    {
        if (node == null) yield break;

        yield return InOrderTraversal(node.left);

        AlgorithmMetrics.Instance.AddStep();

        // Highlight
        node.rect.GetComponent<Image>().color = Color.yellow;

        AlgorithmAudioGenerator.Instance.PlayPing(node.value, maxValue);
        yield return new WaitForSeconds(0.4f);

        Vector2 targetPos = new Vector2(
            -300 + sortedIndex * 100,
            sortedYPosition
        );

        node.rect.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.InOutQuad);

        sortedIndex++;

        yield return new WaitForSeconds(0.6f);

        node.rect.GetComponent<Image>().color = Color.green;

        yield return InOrderTraversal(node.right);

        if (node == root)
        {
            AlgorithmAudioGenerator.Instance.PlaySuccessSound();
            AlgorithmMetrics.Instance.StopTracking();
            RemoveAllLines();
        }
    }
}