using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayControl : MonoBehaviour
{
    public int chooseNumber;
    private TableController tableController;

    public Color wrongValue;
    Color currentColorBlock;

    bool wrongChoose, finish;

    GameObject numberObject;
    Image imageComponent;

    public List<Vector2Int> blockStep;

    [SerializeField] GameObject startPanel, finishPanel;
    LevelType levelType;

    public TMP_Text timeText, finishPLevelTypeText, finishPTimeText, finishPBestTime;
    private float timeValue;

    private void Awake()
    {
        tableController = FindObjectOfType<TableController>();
        startPanel.SetActive(true);
        finishPanel.SetActive(false);
    }

    private void Start()
    {
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (finish)
            return;

        timeValue += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeValue / 60);
        int seconds = Mathf.FloorToInt(timeValue % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SelectLevelType(int i)
    {
        Time.timeScale = 1f;

        if (i == 0)
            levelType = LevelType.easy;
        else if (i == 1)
            levelType = LevelType.middle;
        else if (i == 2)
            levelType = LevelType.difficult;
        else if (i == 3)
            levelType = LevelType.expert;

        startPanel.SetActive(false);

        tableController.levelType = levelType;
        tableController.FillBlocks();
    }

    public void ChooseNumber(int number)
    {
        if(numberObject != null)
        {
            imageComponent = numberObject.GetComponent<Image>();

            if (imageComponent != null)
            {
                Color currentColor = imageComponent.color;
                currentColor.a = 1f;
                imageComponent.color = currentColor;
            }

            GameObject[] chooseNumbers_ = GameObject.FindGameObjectsWithTag("Block" + chooseNumber.ToString());
            foreach (var item in chooseNumbers_)
            {
                Image image = item.GetComponent<Image>();

                if (image != null)
                {
                    Color currentColor = image.color;
                    currentColor.a = 1f;
                    image.color = currentColor;
                }
            }
        }

        chooseNumber = number;
        numberObject = EventSystem.current.currentSelectedGameObject;

        imageComponent = numberObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            Color currentColor = imageComponent.color;
            currentColor.a = 0.5f;
            imageComponent.color = currentColor;
        }

        GameObject[] chooseNumbers = GameObject.FindGameObjectsWithTag("Block" + chooseNumber.ToString());

        foreach (var item in chooseNumbers)
        {
            Image image = item.GetComponent<Image>();

            if (image != null)
            {
                Color currentColor = image.color;
                currentColor.a = 0.5f;
                image.color = currentColor;
            }
        }
    }

    public void FillBlock(int blockValue, Vector2Int blockLoc)
    {
        if (wrongChoose)
            return;

        if (chooseNumber != 0)
        {
            GameObject currentBlock = tableController.allBlock[blockLoc.x, blockLoc.y];

            if (tableController.openedBlock.Contains(currentBlock))
                return;

            blockStep.Add(blockLoc);

            if (blockValue == chooseNumber)
            {
                currentBlock.tag = "Block" + currentBlock.GetComponent<Block>().blockValue.ToString();
                currentBlock.GetComponent<Button>().interactable = false;

                Transform blockText = currentBlock.transform.Find("Block Text");
                blockText.GetComponent<TextMeshProUGUI>().text = blockValue.ToString();

                tableController.openedBlock.Add(currentBlock);
            }
            else
            {
                Image imageComponent = currentBlock.GetComponent<Image>();
                currentColorBlock = imageComponent.color;
                imageComponent.color = wrongValue;

                Transform blockText = currentBlock.transform.Find("Block Text");
                blockText.GetComponent<TextMeshProUGUI>().text = chooseNumber.ToString();
                wrongChoose = true;
            }
        }

        if (tableController.openedBlock.Count == 81)
        {
            if(PlayerPrefs.GetFloat("time") < timeValue)
            {
                PlayerPrefs.SetFloat("time", timeValue);
                PlayerPrefs.SetString("timeText", timeText.text);
            }

            finish = true;
            finishPanel.SetActive(true);
            finishPLevelTypeText.text = levelType.ToString();
            finishPTimeText.text = timeText.text;
            finishPBestTime.text = PlayerPrefs.GetString("timeText");
        }

        GameObject[] chooseNumbers = GameObject.FindGameObjectsWithTag("Block" + chooseNumber.ToString());
        if(chooseNumbers.Length == 9)
        {
            numberObject.GetComponent<Button>().interactable = false;
        }
    }

    public void TakeItBack()
    {
        if (blockStep.Count == 0)
            return;

        GameObject currentBlock = tableController.allBlock[blockStep[blockStep.Count - 1].x, blockStep[blockStep.Count - 1].y];

        Image imageComponent = currentBlock.GetComponent<Image>();

        if (imageComponent.color == wrongValue)
            wrongChoose = false;

        imageComponent.color = currentColorBlock;

        Transform blockText = currentBlock.transform.Find("Block Text");
        blockText.GetComponent<TextMeshProUGUI>().text = "";

        blockStep.RemoveAt(blockStep.Count - 1);

        tableController.openedBlock.Remove(currentBlock);
    }

    public void Restart()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
 