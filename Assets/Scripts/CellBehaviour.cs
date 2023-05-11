using System.Collections;
using System.Collections.Generic;
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
    }

    public void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!isFlagged)
            {
                if (this.isMine)
                {
                    Debug.Log("GAME OVER");
                    MainManager.ShowAllMines();
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
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
        if (value != 0)
        {
            this.transform.GetChild(1).gameObject.SetActive(true);
            this.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/num" + value.ToString());
        }
        
        if (!isRevealed)
        {
            isRevealed = true;
            MainManager.score += 1;
            if (MainManager.score >= MainManager.length * MainManager.height - MainManager.nbMine)
            {
                Debug.Log("You Win ! Congratulation !");
            }
        }
    }

    public void CheckCellsForReveal()
    {
        List<int> connexCells = new List<int>();
        List<int> stackCells = new List<int>();

        stackCells.Add(column * MainManager.height + line);

        while (stackCells.Count > 0)
        {
            int cl = stackCells[0] / MainManager.height;
            int ln = stackCells[0] % MainManager.height;
            stackCells.RemoveAt(0);
            
            int newIndex;
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
            }
        }

        for(int i = 0; i< connexCells.Count; i++)
        {
            int cl = connexCells[i] / MainManager.height;
            int ln = connexCells[i] % MainManager.height;
            MainManager.cells[cl][ln].GetComponent<CellBehaviour>().RevealCell();
        }
    }

    public bool IsFlagged()
    {
        return (isFlagged);
    }
}
