
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TableController : MonoBehaviour
{
    public GameObject clusterPrefab;
    public GameObject blockPrefab;
    Vector3 pos = Vector3.zero;

    public Color colorFirst, colorSecond;
    Color currentColor, currentColor_;

    bool changeCluster = false;

    public GameObject[,] allBlock;

    public List<int> value;

    private void Start()
    {
        allBlock = new GameObject[10, 10];

        for (int i = 1; i <= 9; i++)
        {
            value.Add(i);
        }

        SetupCluster();

        WriteBlock();
    }

    private void SetupCluster()
    {
        for (int x = 1; x <= 9; x++)
        {
            GameObject cluster = Instantiate(clusterPrefab, pos, Quaternion.identity);

            Transform clusterContent = transform.Find("Cluster Content");
            RectTransform rectTransform = cluster.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;

            if (clusterContent != null)
            {
                cluster.transform.localScale = new Vector3(1, 1, 1);
                cluster.transform.parent = clusterContent;
                cluster.name = "Cluster - " + x;
            }
            else
            {
                Debug.LogError("Cluster Content object not found in the hierarchy.");
            }

            Transform blockContent = cluster.transform.Find("Block Content");

            if (x % 2 != 0)
            {
                currentColor = colorFirst;
            }
            else
            {
                currentColor = colorSecond;
            }
            currentColor_ = currentColor;

            if (blockContent != null)
            {
                for (int y = 1; y <= 9; y++)
                {
                    GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity);

                    block.transform.parent = blockContent;
                    block.name = "Block ( " + x + ", " + y + ")";

                    if (!changeCluster)
                    {
                        if (y % 2 == 0)
                        {
                            currentColor = Color.white;
                        }
                        else
                        {
                            currentColor = currentColor_;
                        }
                    }
                    else
                    {
                        if (y % 2 != 0)
                        {
                            currentColor = Color.white;
                        }
                        else
                        {
                            currentColor = currentColor_;
                        }
                    }

                    Image imageComponent = block.GetComponent<Image>();
                    imageComponent.color = currentColor;
                    allBlock[x, y] = block;
                    block.GetComponent<Block>().blockLoc = new Vector2Int(x, y);
                }

                if (changeCluster)
                    changeCluster = false;
                else
                    changeCluster = true;
            }
            else
            {
                Debug.LogError("Block Content object not found in the hierarchy.");
            }
        }
    }

    private void WriteBlock()
    {        
        for (int x = 1; x <= 9; x++)
        {
            for (int y = 1; y <= 9; y++)
            {
                GameObject currentBlock = allBlock[x, y];

                int blockValue = Random.Range(0, value.Count);
                currentBlock.GetComponent<Block>().blockValue = value[blockValue];

                Transform blockText = allBlock[x, y].transform.Find("Block Text");
                blockText.GetComponent<TextMeshProUGUI>().text = value[blockValue].ToString();
                value.RemoveAt(blockValue);
            }

            for (int i = 1; i <= 9; i++)
            {
                value.Add(i);
            }
        }
    }
}
