using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class CellBehaviour : MonoBehaviour
{
    public bool isMine;
    public int value;

    public int line;
    public int column;

    private bool isRevealed;
    private bool isFlagged;

    private void Start()
    {
        isRevealed = false; 
        isFlagged = false;
        timer = 0.0f;
    }

    public static float timer;

    public void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!isFlagged)
            {
                if(Time.time - timer < 0.2f)
                {
                    RevealNeighboursAndCheck();
                }
                timer = Time.time;

                if (this.isMine)
                {
                    Debug.Log("GAME OVER");
                    MainManager.Instance.ShowAllMines();
                }
                else
                {
                    RevealCell();
                    CheckCellsForReveal();
                }
            }
            else
            {
                isFlagged = false;
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
                this.transform.GetChild(1).gameObject.SetActive(false);
                this.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        else if(Input.GetMouseButtonDown(1) && !isRevealed)
        {
            if(!isFlagged)
            {
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/flag");
            }
            else
            {
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
                this.transform.GetChild(1).gameObject.SetActive(false);
                this.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
            }
            isFlagged = !isFlagged;
        }
        
    }

    public void RevealCell()
    {
        if(!isFlagged)
        {
            this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
            if (value != 0)
            {
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/num" + value.ToString());
            }

            if (!isRevealed)
            {
                isRevealed = true;
                MainManager.Instance.score += 1;
                if (MainManager.Instance.score >= MainManager.Instance.length * MainManager.Instance.height - MainManager.Instance.nbMine)
                {
                    Debug.Log("You Win ! Congratulation !");
                }
            }
        }
    }

    public void RevealNeighboursAndCheck()
    {
        int neighFlag = 0;
        bool mineNotFlagged = false;
        List<CellBehaviour> neighbours = GetNeighbours();
        foreach(CellBehaviour neighbour in neighbours)
        {
            neighFlag += (neighbour.IsFlagged() ? 1 : 0);
            if(!neighbour.IsFlagged() && neighbour.isMine)
            {
                mineNotFlagged = true;
            }
        }

        if (neighFlag >= value)
        {
            if(mineNotFlagged)
            {
                MainManager.Instance.ShowAllMines();
            }
            else
            {
                foreach (CellBehaviour neighbour in neighbours)
                {
                    if (!neighbour.IsFlagged())
                    {
                        neighbour.RevealCell();
                        neighbour.CheckCellsForReveal();
                    }
                }
            }
        }
    }


    public void CheckCellsForReveal()
    {
        List<int> connexCells = new List<int>();
        List<int> stackCells = new List<int>();

        stackCells.Add(column * MainManager.Instance.height + line);

        while (stackCells.Count > 0)
        {
            int cl = stackCells[0] / MainManager.Instance.height;
            int ln = stackCells[0] % MainManager.Instance.height;
            stackCells.RemoveAt(0);

            if (MainManager.Instance.cells[cl][ln].GetComponent<CellBehaviour>().value == 0)
            {
                List<CellBehaviour> neighbours = MainManager.Instance.cells[cl][ln].GetComponent<CellBehaviour>().GetNeighbours();

                foreach (CellBehaviour neighbour in neighbours)
                {
                    if (!connexCells.Contains(neighbour.column * MainManager.Instance.height + neighbour.line))
                    {
                        stackCells.Add(neighbour.column * MainManager.Instance.height + neighbour.line);
                        connexCells.Add(neighbour.column * MainManager.Instance.height + neighbour.line);
                    }
                }
            }

            /*int newIndex;
            if (MainManager.cells[cl][ln].GetComponent<CellBehaviour>().value == 0)
            {
                if (cl < MainManager.length - 1)
                {
                    newIndex = (cl + 1) * MainManager.height + ln;
                    if (!connexCells.Contains(newIndex))
                    {
                        stackCells.Add(newIndex);
                        connexCells.Add(newIndex);
                    }
                    if (ln < MainManager.height - 1)
                    {
                        newIndex = (cl + 1) * MainManager.height + ln + 1;
                        if (!connexCells.Contains(newIndex))
                        {
                            stackCells.Add(newIndex);
                            connexCells.Add(newIndex);
                        }
                    }
                    if (ln > 0)
                    {
                        newIndex = (cl + 1) * MainManager.height + ln - 1;
                        if (!connexCells.Contains(newIndex))
                        {
                            stackCells.Add(newIndex);
                            connexCells.Add(newIndex);
                        }
                    }
                }

                if (cl > 0)
                {
                    newIndex = (cl - 1) * MainManager.height + ln;
                    if (!connexCells.Contains(newIndex))
                    {
                        stackCells.Add(newIndex);
                        connexCells.Add(newIndex);
                    }
                    if (ln < MainManager.height - 1)
                    {
                        newIndex = (cl - 1) * MainManager.height + ln + 1;
                        if (!connexCells.Contains(newIndex))
                        {
                            stackCells.Add(newIndex);
                            connexCells.Add(newIndex);
                        }
                    }
                    if (ln > 0)
                    {
                        newIndex = (cl - 1) * MainManager.height + ln - 1;
                        if (!connexCells.Contains(newIndex))
                        {
                            stackCells.Add(newIndex);
                            connexCells.Add(newIndex);
                        }
                    }
                }

                if (ln < MainManager.height - 1)
                {
                    newIndex = cl * MainManager.height + ln + 1;
                    if (!connexCells.Contains(newIndex))
                    {
                        stackCells.Add(newIndex);
                        connexCells.Add(newIndex);
                    }
                }

                if (ln > 0)
                {
                    newIndex = cl * MainManager.height + ln - 1;
                    if (!connexCells.Contains(newIndex))
                    {
                        stackCells.Add(newIndex);
                        connexCells.Add(newIndex);
                    }
                }
            }*/
        }

        for(int i = 0; i< connexCells.Count; i++)
        {
            int cl = connexCells[i] / MainManager.Instance.height;
            int ln = connexCells[i] % MainManager.Instance.height;
            MainManager.Instance.cells[cl][ln].GetComponent<CellBehaviour>().RevealCell();
        }
    }

    public List<CellBehaviour> GetNeighbours()
    {
        List<CellBehaviour> list = new List<CellBehaviour>();
        if (column < MainManager.Instance.length - 1)
        {
            list.Add(MainManager.Instance.cells[column + 1][line].GetComponent<CellBehaviour>());
            if (line < MainManager.Instance.height - 1)
            {
                list.Add(MainManager.Instance.cells[column + 1][line + 1].GetComponent<CellBehaviour>());
            }
            if (line > 0)
            {
                list.Add(MainManager.Instance.cells[column + 1][line - 1].GetComponent<CellBehaviour>());
            }
        }
        if (column > 0)
        {
            list.Add(MainManager.Instance.cells[column - 1][line].GetComponent<CellBehaviour>());
            if (line < MainManager.Instance.height - 1)
            {
                list.Add(MainManager.Instance.cells[column - 1][line + 1].GetComponent<CellBehaviour>());
            }
            if (line > 0)
            {
                list.Add(MainManager.Instance.cells[column - 1][line - 1].GetComponent<CellBehaviour>());
            }
        }
        if (line < MainManager.Instance.height - 1)
        {
            list.Add(MainManager.Instance.cells[column][line + 1].GetComponent<CellBehaviour>());
        }
        if (line > 0)
        {
            list.Add(MainManager.Instance.cells[column][line - 1].GetComponent<CellBehaviour>());
        }
        return list;
    }

    public bool IsFlagged()
    {
        return (isFlagged);
    }
}
