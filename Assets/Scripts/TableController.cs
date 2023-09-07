using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum LevelType { easy, medium, hard, expert }

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
    public List<int> value_;

    public List<GameObject> openedBlock;

    int visibleBlock;

    public LevelType levelType;

    public GameObject[] allObjects;

    private void Start()
    {
        allBlock = new GameObject[9, 9];

        for (int i = 0; i < 9; i++)
        {
            value.Add(i + 1);
        }

        SetupCluster();

        ValueBlock(0);

        LocalScaleUpdate();
    }

    private void SetupCluster()
    {
        for (int x = 0; x < 9; x++)
        {
            GameObject cluster = Instantiate(clusterPrefab, pos, Quaternion.identity);

            Transform clusterContent = transform.Find("Cluster Content");

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

            if (x % 2 == 0)
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
                for (int y = 0; y < 9; y++)
                {
                    GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity);

                    block.transform.parent = blockContent;
                    block.name = "Block(" + x + ", " + y + ")";

                    if (!changeCluster)
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
                    else
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

    private void ValueBlock(int witdh)
    {
        for (int x = witdh; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int i = y - 1; i >= 0; i--)
                {
                    int removeValue = allBlock[x, i].GetComponent<Block>().blockValue;
                    value.Remove(removeValue);
                }

                if (x > 2)
                {
                    int lenghtX = x / 3;
                    int controlX = x % 3;

                    for (int i = lenghtX; i >= 1; i--)
                    {
                        int controlY = y % 3;

                        for (int j = 0; j < 3; j++)
                        {
                            int removeValue = allBlock[controlX, controlY].GetComponent<Block>().blockValue;

                            if (value.Contains(removeValue))
                            {
                                value.Remove(removeValue);
                                value_.Add(removeValue);
                            }
                            controlY += 3;
                        }
                        controlX += 3;
                    }
                }

                if (x % 3 != 0)
                {
                    //  Debug.Log("Yatayda kontrol " + x + " ---- " + x % 3);

                    int lenght = x % 3;

                    int controlX = x - 1;

                    for (int i = lenght; i >= 1; i--)
                    {
                        int posY = y / 3;
                        int controlY = posY * 3;
                        for (int j = 0; j < 3; j++)
                        {
                            int removeValue = allBlock[controlX, controlY].GetComponent<Block>().blockValue;

                            if (value.Contains(removeValue))
                            {
                                value.Remove(removeValue);
                                value_.Add(removeValue);
                            }

                            controlY++;
                        }
                        controlX--;
                    }
                }

                if (value.Count == 0)
                {
                    ClearCluster(x, y);
                    return;
                }

                GameObject currentBlock = allBlock[x, y];

                int blockValue = Random.Range(0, value.Count);
                currentBlock.GetComponent<Block>().blockValue = value[blockValue];

                if (value_.Count != 0)
                {
                    foreach (var item in value_)
                    {
                        value.Add(item);
                    }
                    value_.Clear();
                }
            }

            value.Clear();
            for (int i = 1; i <= 9; i++)
            {
                value.Add(i);
            }
        }

       // FillBlocks();
    }

    private void ClearCluster(int cluster, int block)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < block; j++)
            {
                GameObject currentBlock = allBlock[cluster, j];
                currentBlock.GetComponent<Block>().blockValue = 0;
            }
            cluster--;
        }

            

        if (value_.Count != 0)
        {
            value_.Clear();
        }

        value.Clear();
        for (int i = 1; i <= 9; i++)
        {
            value.Add(i);
        }

        ValueBlock(cluster + 1);
    }

    public void FillBlocks()
    {
        if (levelType == LevelType.easy)
            visibleBlock = Random.Range(50, 60);
        else if (levelType == LevelType.medium)
            visibleBlock = Random.Range(45, 50);
        else if (levelType == LevelType.hard)
            visibleBlock = Random.Range(40, 45);
        else if (levelType == LevelType.expert)
            visibleBlock = Random.Range(35, 40);

        for (int i = 0; i < visibleBlock; i++)
        {
            GameObject currentBlock = allBlock[Random.Range(0, 9), Random.Range(0, 9)];

            int blockValue = currentBlock.GetComponent<Block>().blockValue;
            Transform blockText = currentBlock.transform.Find("Block Text");
            blockText.GetComponent<TextMeshProUGUI>().text = blockValue.ToString();

            currentBlock.tag = "Block" + currentBlock.GetComponent<Block>().blockValue.ToString();

            if (!openedBlock.Contains(currentBlock))
            {
                openedBlock.Add(currentBlock);
            }
        }
    }

    private void LocalScaleUpdate()
    {
        allObjects = GameObject.FindGameObjectsWithTag("Cluster");

        // Liste içindeki tüm objelerin RectTransform bileþenlerini al ve Scale deðerini Vector3.one yap
        foreach (GameObject cluster in allObjects)
        {
            RectTransform rectTransform = cluster.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
            }
        }
    }
}