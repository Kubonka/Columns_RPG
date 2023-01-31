using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public GameObject[] gems = new GameObject[3];
    BoardManager boardRef;
    private Vector3 pos;
    private float fall = 0;
    public float fallSpeed = 1.5f;
    public bool isCurrent = false;
    private float timer;
    private float threshold = 0.15f;

    void Start()
    {
        boardRef = GameObject.Find("Board").GetComponent<BoardManager>();
        timer = Time.time + threshold;
    }
        
    void Update()
    {
        if (isCurrent == true)
            CheckUserInput();
    }

    private void CheckUserInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (timer < Time.time)
            {
                timer = Time.time + threshold;
                if ((transform.position.x + 1 < boardRef.gridWidth))
                {
                    if (boardRef.grid[(int)transform.position.y, (int)transform.position.x + 1] == null)
                    {
                        transform.position += new Vector3(1, 0, 0);
                        boardRef.GetComponent<BoardManager>().ResetTimer();
                    }
                }
                if (!IsValid())
                {
                    transform.position += new Vector3(-1, 0, 0);
                }
            }            
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (timer < Time.time)
            {
                timer = Time.time + threshold;
                if ((transform.position.x - 1 >= 0))
                {
                    if (boardRef.grid[(int)transform.position.y, (int)transform.position.x - 1] == null)
                    {
                        transform.position += new Vector3(-1, 0, 0);
                        boardRef.GetComponent<BoardManager>().ResetTimer();
                    }
                }
                if (!IsValid())
                {
                    transform.position += new Vector3(+1, 0, 0);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotarGemas();
            boardRef.GetComponent<BoardManager>().ResetTimer();
        }
        else if (Input.GetKey(KeyCode.DownArrow) || (Time.time - fall >= fallSpeed))
        {
            if (boardRef.GetComponent<BoardManager>().IsInsideGrid((int)transform.position.y - 1, (int)transform.position.x))
            {
                if (timer < Time.time)
                {
                    timer = Time.time + threshold;
                    if (boardRef.GetComponent<BoardManager>().grid[(int)transform.position.y - 1, (int)transform.position.x] == null)
                    {
                        transform.position += new Vector3(0, -1, 0);
                        boardRef.GetComponent<BoardManager>().ResetTimer();
                    }
                    else
                        boardRef.GetComponent<BoardManager>().DropPiece();
                }
            }
            else
            {
                boardRef.GetComponent<BoardManager>().DropPiece();
            }            
            fall = Time.time;
        }        
    }

    private void RotarGemas() // MAL HECHA (modificar)
    {   


        // change.position
        for (int j = 0; j < 3; j++)
        {
            if (gems[j].transform.localPosition.y == 0)
                gems[j].transform.localPosition = new Vector3(0, 2, 0);
            else
                gems[j].transform.localPosition = new Vector3(0, gems[j].transform.localPosition.y - 1, 0);
        }
        // shift array elements
        GameObject temp;
        temp = gems[0];
        for (int i = 0; i < 2; i++)
        {
            gems[i] = gems[i + 1];
        }
        gems[2] = temp;        
    }

    private bool IsValid()
    {
        if (FindObjectOfType<BoardManager>().checkIsInsideGrid(transform.position) == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
