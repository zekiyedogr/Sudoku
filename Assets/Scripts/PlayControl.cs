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

    public List<Vector2Int> blockStep;

    [SerializeField] GameObject startPanel, finishPanel, pausePanel;
    LevelType levelType;

    public TMP_Text timeText, finishPLevelTypeText, finishPTimeText, finishPBestTime;
    private float timeValue;

    bool GameIsPaused;

    private void Awake()
    {
        tableController = FindObjectOfType<TableController>();
        startPanel.SetActive(true);
        finishPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    private void Start()
    {
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (finish || GameIsPaused)
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
        if (GameIsPaused)
            return;

        if (numberObject != null)
        {
            ChangeNumbersAlpha(numberObject, 1f);
        }

        chooseNumber = number;
        numberObject = EventSystem.current.currentSelectedGameObject;

        ChangeNumbersAlpha(numberObject, 0.5f);
    }

    public void FillBlock(int blockValue, Vector2Int blockLoc)
    {
        if (wrongChoose || chooseNumber == 0 || GameIsPaused)
            return;

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

            ChangeImageAlpha(currentBlock, 0.5f);
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
        

        if (tableController.openedBlock.Count == 81)
        {
            if(PlayerPrefs.GetFloat("time") > timeValue || PlayerPrefs.GetFloat("time") == 0)
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

        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block" + chooseNumber.ToString());

        if (blocks.Length == 9)
        {
            numberObject.GetComponent<Button>().interactable = false;

            ChangeImageAlpha(numberObject, 1f);
        }
    }

    public void TakeItBack()
    {
        if (blockStep.Count == 0 || GameIsPaused)
            return;

        GameObject currentBlock = tableController.allBlock[blockStep[blockStep.Count - 1].x, blockStep[blockStep.Count - 1].y];

        Image imageComponent = currentBlock.GetComponent<Image>();

        if (imageComponent.color == wrongValue)
        {
            wrongChoose = false;
            imageComponent.color = currentColorBlock;
        }

        if (imageComponent.color.a == 0.5f)
        {
            Color currentColor = imageComponent.color;
            currentColor.a = 1f;
            imageComponent.color = currentColor;
        }

        Transform blockText = currentBlock.transform.Find("Block Text");
        blockText.GetComponent<TextMeshProUGUI>().text = "";

        currentBlock.tag = "Untagged";

        int currentBlockValue = currentBlock.GetComponent<Block>().blockValue;

        GameObject chooseNumber = GameObject.FindGameObjectWithTag("Number" + currentBlockValue.ToString());
        chooseNumber.GetComponent<Button>().interactable = true;
        currentBlock.GetComponent<Button>().interactable = true;

        blockStep.RemoveAt(blockStep.Count - 1);

        tableController.openedBlock.Remove(currentBlock);
    }

    public void Restart()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void Pause()
    {
        if (GameIsPaused)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }

    private void ChangeImageAlpha(GameObject validObject, float imageValue)
    {
        Image imageComponent = validObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            Color currentColor = imageComponent.color;
            currentColor.a = imageValue;
            imageComponent.color = currentColor;
        }
    }

    private void ChangeNumbersAlpha(GameObject validObject, float imageValue)
    {
        ChangeImageAlpha(validObject, imageValue);

        GameObject[] chooseNumbers = GameObject.FindGameObjectsWithTag("Block" + chooseNumber.ToString());
        foreach (var item in chooseNumbers)
        {
            ChangeImageAlpha(item, imageValue);
        }
    }
}
 