using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static int length;
    public static int height;
    public static int nbMine;
    public static int score;

    public static List<List<GameObject>> cells;

    public GameObject cellPrefab;
    public GameObject gamePanel;

    private static List<int> mineIndex = new List<int>();

    private void Start()
    {
        length = 28;
        height= 20;
        nbMine = 100;
        score = 0;
        
        cells = new List<List<GameObject>>();
        InitGamePanel();
        float cellSize = ComputeCellSize();
        for (int i = 0; i < length; i++)
        {
            cells.Add(new List<GameObject>());
            for (int j = 0; j < height; j++)
            {
                GameObject newCell = Instantiate(cellPrefab);
                newCell.transform.position = gamePanel.transform.position + new Vector3((i + 0.5f - (length/2.0f)) * cellSize, (j + 0.5f - (height/2.0f)) * cellSize, 0);
                newCell.transform.localScale = cellSize * Vector3.one;
                CellBehaviour cellBehaviour = newCell.GetComponent<CellBehaviour>();
                cellBehaviour.isMine = false;
                cellBehaviour.value = 0;
                cellBehaviour.column = i;
                cellBehaviour.line = j;
                cells[i].Add(newCell);
            }
        }

        for(int m=0; m<nbMine; m++)
        {
            int newIndex = Random.Range(0, length*height);
            while (mineIndex.Contains(newIndex))
            {
                newIndex = (newIndex + 1) % (length * height);
            }
            mineIndex.Add(newIndex);
            int newCol = newIndex / height;
            int newLine = newIndex % height;
            cells[newCol][newLine].GetComponent<CellBehaviour>().isMine = true;

            if (newCol < length - 1)
            {
                cells[newCol + 1][newLine].GetComponent<CellBehaviour>().value += 1;
                if(newLine < height - 1)
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

    public static void ShowAllMines()
    {
        Sprite bombSprite = Resources.Load<Sprite>("Sprites/bomb");
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

}