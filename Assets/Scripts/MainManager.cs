using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public int length;
    public int height;
    public int nbMine;
    public int score;
    public int difficulty;

    public GameObject cellPrefab;
    public GameObject gamePanel;
    public Image difficultyLabel;
    public GameObject widthInput;
    public GameObject heightInput;
    public GameObject minesInput;
    public Text timerText;
    public GameObject winMsg;
    public GameObject looseMsg;


    public List<List<GameObject>> cells;
    private List<int> mineIndex;
    public bool bombSpawned;

    private float timer;
    public bool gameOver;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Instance already exists !");
        }
    }

    private void Start()
    {
        length = 14;
        height= 10;
        nbMine = 25;
        difficulty = 0;
        widthInput.transform.GetChild(0).GetComponent<InputField>().text = length.ToString();
        heightInput.transform.GetChild(0).GetComponent<InputField>().text = height.ToString();
        minesInput.transform.GetChild(0).GetComponent<InputField>().text = nbMine.ToString();
        SetDifficulty(0);
        winMsg.SetActive(false);
        looseMsg.SetActive(false);
        gameOver = false;

        InitGame();
    }

    private void Update()
    {
        if (!bombSpawned)
        {
            timerText.text = "-";
        }
        else if(!gameOver)
        {
            timerText.text = ((int)(Time.time - timer)).ToString();
        }
    }

    public void InitGame()
    {
        winMsg.SetActive(false);
        looseMsg.SetActive(false);
        gameOver = false;
        score = 0;
        timer = Time.time;
        cells = new List<List<GameObject>>();
        mineIndex = new List<int>();
        bombSpawned = false;
        InitGamePanel();
        float cellSize = ComputeCellSize();

        GameObject oldParent = GameObject.Find("CellsParent");
        if(oldParent != null)
        {
            Destroy(oldParent);
        }

        GameObject cellsParent = new GameObject("CellsParent");
        for (int i = 0; i < length; i++)
        {
            cells.Add(new List<GameObject>());
            for (int j = 0; j < height; j++)
            {
                GameObject newCell = Instantiate(cellPrefab);
                newCell.transform.position = gamePanel.transform.position + new Vector3((i + 0.5f - (length / 2.0f)) * cellSize, (j + 0.5f - (height / 2.0f)) * cellSize, 0);
                newCell.transform.localScale = cellSize * Vector3.one;
                newCell.transform.parent = cellsParent.transform;
                CellBehaviour cellBehaviour = newCell.GetComponent<CellBehaviour>();
                cellBehaviour.isMine = false;
                cellBehaviour.value = 0;
                cellBehaviour.column = i;
                cellBehaviour.line = j;
                cells[i].Add(newCell);
            }
        }
    }

    public void SpawnBombs(int i, int j)
    {
        bombSpawned = true;
        for (int m = 0; m < nbMine; m++)
        {
            int newIndex = Random.Range(0, length * height);
            while (mineIndex.Contains(newIndex) || (Mathf.Abs((newIndex / height) - i) <= 1 && Mathf.Abs((newIndex % height) - j) <= 1))
            {
                if((((newIndex / height) - i) <= 1 && ((newIndex % height) - j) <= 1))
                {
                    newIndex = Random.Range(0, length * height);
                }
                else
                {
                    newIndex = (newIndex + 1) % (length * height);
                }
            }
            mineIndex.Add(newIndex);
            int newCol = newIndex / height;
            int newLine = newIndex % height;

            cells[newCol][newLine].GetComponent<CellBehaviour>().isMine = true;

            if (newCol < length - 1)
            {
                cells[newCol + 1][newLine].GetComponent<CellBehaviour>().value += 1;
                if (newLine < height - 1)
                {
                    cells[newCol + 1][newLine + 1].GetComponent<CellBehaviour>().value += 1;
                }
                if (newLine > 0)
                {
                    cells[newCol + 1][newLine - 1].GetComponent<CellBehaviour>().value += 1;
                }
            }

            if (newCol > 0)
            {
                cells[newCol - 1][newLine].GetComponent<CellBehaviour>().value += 1;
                if (newLine < height - 1)
                {
                    cells[newCol - 1][newLine + 1].GetComponent<CellBehaviour>().value += 1;
                }
                if (newLine > 0)
                {
                    cells[newCol - 1][newLine - 1].GetComponent<CellBehaviour>().value += 1;
                }
            }

            if (newLine < height - 1)
            {
                cells[newCol][newLine + 1].GetComponent<CellBehaviour>().value += 1;
            }

            if (newLine > 0)
            {
                cells[newCol][newLine - 1].GetComponent<CellBehaviour>().value += 1;
            }
        }
    }

    public void InitGamePanel()
    {
        float frustumHeight = 2.0f * 10 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * Camera.main.aspect;
        gamePanel.transform.localScale = new Vector3(frustumWidth * 0.8f * 0.95f, frustumHeight * 0.95f, 1.0f);
        gamePanel.transform.position = new Vector3(frustumWidth * 0.1f, 0.0f, 0.0f);
    }

    public float ComputeCellSize()
    {
        float maxLength = gamePanel.transform.lossyScale.x / length;
        float maxHeight = gamePanel.transform.lossyScale.y / height;
        return Mathf.Min(maxLength, maxHeight);
    }

    public void ChangeDifficulty(int dir)
    {
        difficulty = (difficulty + dir + 4) % 4;
        SetDifficulty(difficulty);
    }

    public void SetDifficulty(int diff) 
    {
        difficulty = diff;
        switch (difficulty)
        {
            case 0:
                length = 14;
                height = 10;
                nbMine = 25;
                difficultyLabel.sprite = Resources.Load<Sprite>("Sprites/EasyLabel");
                break;
            case 1:
                length = 28;
                height = 20;
                nbMine = 90;
                difficultyLabel.sprite = Resources.Load<Sprite>("Sprites/MediumLabel");
                break;
            case 2:
                length = 56;
                height = 40;
                nbMine = 390;
                difficultyLabel.sprite = Resources.Load<Sprite>("Sprites/HardLabel");
                break;
            case 3:
                SetValuesFromFields();
                difficultyLabel.sprite = Resources.Load<Sprite>("Sprites/CustomLabel");
                break;
        }

        widthInput.SetActive((difficulty == 3));
        heightInput.SetActive((difficulty == 3));
        minesInput.SetActive((difficulty == 3));

        InitGame();
    }

    public void OnTextFieldChange()
    {
        if(difficulty== 3)
        {
            SetValuesFromFields();
            InitGame();
        }
    }

    public void SetValuesFromFields()
    {
        if (!string.IsNullOrEmpty(widthInput.transform.GetChild(0).GetComponent<InputField>().text) && !string.IsNullOrEmpty(heightInput.transform.GetChild(0).GetComponent<InputField>().text) && !string.IsNullOrEmpty(minesInput.transform.GetChild(0).GetComponent<InputField>().text))
        {
            length = int.Parse(widthInput.transform.GetChild(0).GetComponent<InputField>().text);
            height = int.Parse(heightInput.transform.GetChild(0).GetComponent<InputField>().text);
            nbMine = int.Parse(minesInput.transform.GetChild(0).GetComponent<InputField>().text);
            nbMine = Mathf.Min(nbMine, height * length - Mathf.Min(3, height) * Mathf.Min(3, length));
            minesInput.transform.GetChild(0).GetComponent<InputField>().text = nbMine.ToString();
        }
    }

    public void ShowAllMines()
    {
        Sprite bombSprite = Resources.Load<Sprite>("Sprites/bomb2");
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cell = cells[i][j];
                if(cell.GetComponent<CellBehaviour>().isMine)
                {
                    cell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
                    cell.transform.GetChild(1).gameObject.SetActive(true);
                    cell.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = bombSprite;
                }
                else if (cell.GetComponent<CellBehaviour>().IsFlagged())
                {
                    cell.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/error");
                } 
            }
        }
    }

    public void Win()
    {
        gameOver = true;
        winMsg.SetActive(true);
    }

    public void Loose()
    {
        gameOver = true;
        looseMsg.SetActive(true);
    }

}
