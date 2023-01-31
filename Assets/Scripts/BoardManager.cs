using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BoardManager : MonoBehaviour
{
    public enum GameState
    {
        NewGame,
        MovingPiece,
        MatchingGems,
        MatchingGems2,
        MatchingGems3,
        Combat,
        GameOver,
        EnemyPickSkill
    }
    public GameState currentState;
    public enum CombatState
    {
        PhaseOne,
        PhaseTwo,
        PhaseThree,
        PhaseFour,
    }
    public CombatState currentCombatState;

    // Not assigned


    // Grid vars
    public int gridWidth = 6;
    public const int gridHeight = 15; // ATENTO NUEVOS CAMBIOS
    public GameObject[,] grid;

    // Match Finder + FallingGems vars    
    private bool foundMatches = true;
    //public List<GameObject> finalList;
    public List<GameObject> diagonalList;
    public int fallingGemsCount = 0;
    public int gemsToDestroy = 0;
    private List<GameObject> gemsFallingList;
    private List<GameObject> destroyGemsList;

    // Move Piece + Timer vars
    private GameObject nextPiece;
    private GameObject holdPiece;
    public GameObject currentPiece;
    private bool isSwapeable = true;
    private float timer;
    static float timeOut = 0.5f;

    // CollectedGems Canvas vars
    public Canvas collectedGemsCanvas;
    public Canvas playerCanvas;
    public Canvas enemyCanvas;

    public CollectedGems collectedGems;
    //Unit References
    PlayerManager playerRef;
    EnemyManager enemyRef;

    //Interaccion Con otros Scripts
    public event Action<bool> onReturnControl;
    public bool externalUse = false;

    void Start()
    {
        //this.gameObject.GetComponent<Unit>().onDestroy += DestroyElement;

        playerRef = GameObject.Find("Player").GetComponent<PlayerManager>();

        GameObject go = new GameObject();                                   //
                                                                            //    TEMP
        collectedGems = new CollectedGems(0, 0, 0, 0, 0, 0); //
        currentState = GameState.EnemyPickSkill;
        diagonalList = new List<GameObject>();
        gemsFallingList = new List<GameObject>();
        destroyGemsList = new List<GameObject>();
        //finalList = new List<GameObject>();
        List<Transform> gemsInGrid = new List<Transform>();
        grid = new GameObject[15, 6];
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                grid[i, j] = null;
            }
        }
        GameObject background = (GameObject)Instantiate(Resources.Load("Prefabs/BoardEdges"), new Vector3(0, 0, 0), Quaternion.identity);
        GenerateNextPiece();
        SpawnCurrentPiece();
        playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<Canvas>();


        //Sprite sp = Resources.Load<Sprite>("Icons/CyanGem");
        //playerCanvas.transform.Find("Image").GetComponent<Image>().sprite = sp;

        //GameObject enemyObj;
        //enemyObj = Instantiate(Resources.Load("Prefabs/Enemy"),transform.position,Quaternion.identity) as GameObject;
        //enemyObj.name = "Enemy";
        enemyRef = GameObject.Find("Enemy").GetComponent<EnemyManager>();

        enemyCanvas = GameObject.Find("EnemyCanvas").GetComponent<Canvas>();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.EnemyPickSkill:
               enemyRef.EnemySkillPicker();
                if (enemyRef.currentSkill != null)
                    Debug.Log(enemyRef.currentSkill.GetComponent<Skill>().skillData.skillName);
                else
                    Debug.Log("Enemy action -> NOTHING");
                currentState = GameState.MovingPiece;
               break;
            case GameState.MovingPiece:
                {                    
                    
                    // SETEO TEMPORAL DE CANVAS
                    //playerCanvas.transform.GetChild(0).GetComponent<Text>().text = playerRef.armor.ToString();
                    //playerCanvas.transform.GetChild(1).GetComponent<Text>().text = playerRef.currentCastingTime.ToString();
                    //enemyCanvas.transform.GetChild(0).GetComponent<Text>().text = enemyRef.armor.ToString();
                    //enemyCanvas.transform.GetChild(1).GetComponent<Text>().text = enemyRef.currentCastingTime.ToString();
                    //enemyCanvas.transform.GetChild(2).GetComponent<Text>().text = enemyRef.currentHealth.ToString();
                    if (currentPiece == null)
                    {
                        SpawnCurrentPiece();
                        isSwapeable = true;
                    }
                    else
                    {
                        StartCoroutine(playerRef.CheckInput());
                        MovingCurrentPiece();
                        if (Input.GetKeyDown(KeyCode.S) && (isSwapeable == true))
                        {
                            isSwapeable = false;
                            SwapCurrentHold();
                        }
                    }
                }
                break;

            case GameState.MatchingGems:
                if (foundMatches)
                {
                    destroyGemsList.Clear();
                    ResolveMatches(MatchFinder());
                }
                break;

            case GameState.MatchingGems2:
                if (gemsToDestroy == 0)
                {
                    foreach (GameObject go in destroyGemsList)
                    {
                        DestroyImmediate(go);
                    }
                    //update
                    Debug.Log("GEMS TO DESTROY = " + gemsToDestroy);
                    CreateGemFallListAndUpdateGrid();
                    UpdateGrid();
                    currentState = GameState.MatchingGems3;
                }
                break;

            case GameState.MatchingGems3:
                //finalList.Clear();
                if (fallingGemsCount == 0)
                {
                    if (foundMatches == false)
                    {

                        if (IsGameOver())
                            currentState = GameState.GameOver;
                        else
                        {
                            if (externalUse == true)
                            {
                                onReturnControl?.Invoke(false);                      //firevent
                                currentState = GameState.Combat;                    //OTROS SCRIPTS
                                currentCombatState = CombatState.PhaseTwo;
                            }
                            else
                            {
                                currentState = GameState.Combat;
                                currentCombatState = CombatState.PhaseOne;          //NORMAL
                            }
                        }
                    }
                    else
                        currentState = GameState.MatchingGems;
                }
                break;

            case GameState.GameOver:
                GameOver();
                currentState = GameState.NewGame;
                break;

            case GameState.NewGame:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    List<GameObject> list = new List<GameObject>();
                    //list.AddRange(GameObject.Find("FreshGem"));
                    foreach (Transform child in this.transform)
                        Destroy(child.gameObject);
                    DestroyImmediate(currentPiece);
                    Start();
                }
                break;

            case GameState.Combat:
                switch (currentCombatState)
                {
                    case CombatState.PhaseOne:
                        {
                            StartCoroutine(playerRef.PhaseOne());       //PLAYER PHASE
                            currentCombatState = CombatState.PhaseTwo;
                        }
                        break;

                    case CombatState.PhaseTwo:
                        //INTERMEDIA                        
                        break;

                    case CombatState.PhaseThree:
                        {
                            // ENEMY PHASE
                            //StartCoroutine(GenerateDamage(Random.Range(1, 3)));
                            //currentCombatState = CombatState.PhaseFour;
                            StartCoroutine(enemyRef.PhaseThree());
                            currentCombatState = CombatState.PhaseTwo;
                        }
                        break;

                    case CombatState.PhaseFour:
                        if (IsIdle())
                            currentState = GameState.EnemyPickSkill;
                        break;//INTERMEDIA                  
                }
                break;
        }
    }
    private bool IsIdle()
    {
        while (fallingGemsCount > 0)
        {
            StartCoroutine(Waiter(0.2f));
            return false;
        }
        return true;
    }
    private IEnumerator Waiter(float cant)
    {
        yield return new WaitForSeconds(cant);
    }

    private bool[] SetRandoms(int cant)
    {
        bool[] arr = new bool[6];
        for (int i = 0; i < 6; i++)
        {
            arr[i] = false;
        }
        while (cant > 0)
        {
            int rnd = UnityEngine.Random.Range(0, 6);
            if (arr[rnd] == false)
            {
                arr[rnd] = true;
                cant--;
            }
        }
        return arr;
    }

    private void CreateDamageGems(bool[] arr)
    {
        List<GameObject> gemsFallingList = new List<GameObject>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == true)
            {
                GameObject go;
                go = (GameObject)Instantiate(Resources.Load(GenerateGems(true)), new Vector3(i, 14, 0), Quaternion.identity);
                go.AddComponent<Gem>();
                go.transform.parent = this.transform;
                grid[14, i] = go;
            }
        }
        CreateGemFallListAndUpdateGrid();
        UpdateGrid();
    }

    public IEnumerator GenerateDamage(int count)
    {
        while (count > 0)
        {
            if (count - 6 >= 0)
            {
                CreateDamageGems(SetRandoms(6));
                count -= 6;
                yield return new WaitForSeconds(0.12f);
            }
            else
            {
                CreateDamageGems(SetRandoms(count));
                count -= 6;
                yield return null;
            }
        }

        while (fallingGemsCount != 0)
        {
            yield return null;
        }
        //currentState = GameState.MovingPiece;
    }
    /*public IEnumerator GenerateDamage(int count)
    {
        while (count > 0)
        {
            if (count - 6 >= 0)
            {
                CreateDamageGems(SetRandoms(6));
                count -= 6;
                yield return new WaitForSeconds(0.12f);
            }
            else
            {
                CreateDamageGems(SetRandoms(count));
                count -= 6;
                yield return null;
            }
        }

        while (fallingGemsCount != 0)
        {
            yield return null;
        }
        //currentState = GameState.MovingPiece;
    }*/

    private void SwapCurrentHold()
    {
        GameObject temp;
        if (holdPiece != null)
        {
            temp = holdPiece;
            holdPiece = currentPiece;
            currentPiece = temp;
            currentPiece.transform.position = new Vector3(3, 13, 0);
            currentPiece.GetComponent<PieceManager>().isCurrent = true;
        }
        else
        {
            holdPiece = currentPiece;
            SpawnCurrentPiece();
        }
        holdPiece.GetComponent<PieceManager>().isCurrent = false;
        holdPiece.transform.position = new Vector3(-3, 10);
    }

    private void GameOver()
    {
        foreach (Transform child in this.transform)
        {
            child.GetComponent<SpriteRenderer>().color = Color.black;
        }
        Animator animator = GameObject.Find("Player").GetComponent<PlayerManager>().avatar.GetComponent<Animator>();
        animator.SetBool("Death", true);
        /*foreach (GameObject gem in currentPiece.GetComponent<PieceManager>().gems)
        {
            gem.GetComponent<SpriteRenderer>().color = Color.black;
        }*/
    }
    private bool IsGameOver()
    {
        for (int j = 0; j < gridWidth; j++)
        {
            if (grid[14, j] != null || grid[12, 3] != null)
                return true;
        }
        return false;
    }

    private void GenerateNextPiece()
    {
        nextPiece = new GameObject();
        nextPiece.name = "NextPiece";
        nextPiece.AddComponent<PieceManager>();
        GameObject go;
        for (int i = 0; i < 3; i++)
        {
            go = Instantiate(Resources.Load(GenerateGems(false)), new Vector3(0, i, 0), Quaternion.identity) as GameObject;
            go.transform.parent = nextPiece.transform;
            go.name = "FreshGem";
            nextPiece.GetComponent<PieceManager>().gems[i] = go;
            nextPiece.GetComponent<PieceManager>().gems[i].AddComponent<Gem>();
        }
        nextPiece.transform.position = new Vector3(8, 10, 0);
        nextPiece.GetComponent<PieceManager>().isCurrent = false;
    }
    private void SpawnCurrentPiece()
    {
        //currentPiece = new GameObject();
        currentPiece = nextPiece;
        currentPiece.name = "CurrentPiece";
        currentPiece.transform.position = new Vector3(3, 13, 0);
        currentPiece.GetComponent<PieceManager>().isCurrent = true;
        GenerateNextPiece();
    }

    public void DropPiece()
    {
        Vector3 pos;
        for (int i = 0; i < 3; i++)
        {
            pos = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y, 0);
            grid[(int)pos.y + i, (int)pos.x] = (GameObject)Instantiate(currentPiece.GetComponent<PieceManager>().gems[i], new Vector3(pos.x, pos.y + i, 0), Quaternion.identity);
            grid[(int)pos.y + i, (int)pos.x].transform.parent = this.gameObject.transform;
            grid[(int)pos.y + i, (int)pos.x].transform.position = new Vector3(pos.x, pos.y + i, 0);
        }
        Destroy(currentPiece);
        currentState = GameState.MatchingGems;
        foundMatches = true;
    }

    public void ResetTimer()
    {
        timer = Time.time + timeOut;
    }

    private void MovingCurrentPiece()
    {
        // Si (toco fondo del board o en board[fila , (current.y - 1)] hay una gema)
        if ((currentPiece.transform.position.y == 0) || (grid[(int)currentPiece.transform.position.y - 1, (int)currentPiece.transform.position.x] != null))
        {
            if (Time.time > timer)
            {
                DropPiece();
            }
        }
    }

    private void CreateGemFallListAndUpdateGrid()
    {
        int fallCount;
        for (int j = 0; j < gridWidth; j++) {

            fallCount = 0;
            for (int i = 0; i < gridHeight; i++)
            {
                if (grid[i, j] != null)
                {
                    if (fallCount != 0)
                    {
                        grid[i, j].GetComponent<Gem>().fallCount = fallCount;
                        gemsFallingList.Add(grid[i, j]);
                    }
                }
                else
                {
                    fallCount += 1;
                }
            }
        }
        for (int j = 0; j < gridWidth; j++)
        {
            for (int i = 0; i < gridHeight; i++)
            {
                if (grid[i, j] != null)
                    if (grid[i, j].GetComponent<Gem>().fallCount != 0)
                    {
                        grid[(i - grid[i, j].GetComponent<Gem>().fallCount), j] = grid[i, j];
                        grid[i, j] = null;
                    }
            }
        }
    }

    private void UpdateGrid()
    {
        foreach (GameObject go in gemsFallingList)
        {
            Vector2 pos;
            pos = new Vector2(go.transform.position.x, go.transform.position.y);
            fallingGemsCount += 1;
            go.GetComponent<Gem>().isFalling = true;
        }
        gemsFallingList.Clear();
    }

    private void CleanGrid(List<GameObject> list)
    {
        foreach (GameObject go in list)
        {
            Vector3 pos = new Vector3(go.transform.position.x, go.transform.position.y, 0);
            if (go.tag == "DamageGem")
            {
                destroyGemsList.Add(grid[(int)pos.y, (int)pos.x]);
                //gemsToDestroy++;
                //grid[(int)pos.y, (int)pos.x].GetComponent<Animator>().SetBool("Destroyed", true);
            }
            else
            {
                if (IsInsideGrid((int)pos.y + 1, (int)pos.x) && (grid[(int)pos.y + 1, (int)pos.x] != null) && (grid[(int)pos.y + 1, (int)pos.x].tag == "DamageGem"))
                {
                    destroyGemsList.Add(grid[(int)pos.y + 1, (int)pos.x]);
                    //gemsToDestroy++;
                    //grid[(int)pos.y + 1, (int)pos.x].GetComponent<Animator>().SetBool("Destroyed", true);
                }
                if (IsInsideGrid((int)pos.y - 1, (int)pos.x) && (grid[(int)pos.y - 1, (int)pos.x] != null) && (grid[(int)pos.y - 1, (int)pos.x].tag == "DamageGem"))
                {
                    destroyGemsList.Add(grid[(int)pos.y - 1, (int)pos.x]);
                    //gemsToDestroy++;
                    //grid[(int)pos.y - 1, (int)pos.x].GetComponent<Animator>().SetBool("Destroyed", true);
                }
                if (IsInsideGrid((int)pos.y, (int)pos.x + 1) && (grid[(int)pos.y, (int)pos.x + 1] != null) && (grid[(int)pos.y, (int)pos.x + 1].tag == "DamageGem"))
                {
                    destroyGemsList.Add(grid[(int)pos.y, (int)pos.x + 1]);
                    //gemsToDestroy++;
                    //grid[(int)pos.y, (int)pos.x + 1].GetComponent<Animator>().SetBool("Destroyed", true);
                }
                if (IsInsideGrid((int)pos.y, (int)pos.x - 1) && (grid[(int)pos.y, (int)pos.x - 1] != null) && (grid[(int)pos.y, (int)pos.x - 1].tag == "DamageGem"))
                {
                    destroyGemsList.Add(grid[(int)pos.y, (int)pos.x - 1]);
                    //gemsToDestroy++;
                    //grid[(int)pos.y, (int)pos.x - 1].GetComponent<Animator>().SetBool("Destroyed", true);
                }
            }
        }
        destroyGemsList = RemoveDuplicates(true, destroyGemsList);
        gemsToDestroy = destroyGemsList.Count;
        foreach (GameObject go in list)
        {
            if (go.tag != "DamageGem")
            {
                destroyGemsList.Add(go);
                gemsToDestroy++;
            }
        }
        foreach (GameObject go in destroyGemsList)
        {
            go.GetComponent<Animator>().SetBool("Destroyed", true);
        }
    }

    private void AddToCollectedGemsAndUpdateCanvas(List<GameObject> list)    // AND UPDATE CANVAS
    {
        foreach (GameObject go in list)
        {
            if (go.tag != "DamageGem")
            {
                Text childInCanvasText;
                int childInCanvasTextToInt;
                collectedGems.Add(go);
                childInCanvasText = GetChildTextInCanvas(go);
                childInCanvasTextToInt = int.Parse(childInCanvasText.text) + 1;  //Convert To int y Sumo 1
                childInCanvasText.text = childInCanvasTextToInt.ToString();
            }
        }
    }

    public void RemoveFromCollectedGemsAndUpdateCanvas(CollectedGems gemCost)     // AND UPDATE CANVAS
    {
        collectedGems.Remove(gemCost);
        int count;
        count = int.Parse(collectedGemsCanvas.transform.GetChild(0).GetComponent<Text>().text) - gemCost.attackGem;
        collectedGemsCanvas.transform.GetChild(0).GetComponent<Text>().text = count.ToString();
        count = int.Parse(collectedGemsCanvas.transform.GetChild(1).GetComponent<Text>().text) - gemCost.defendGem;
        collectedGemsCanvas.transform.GetChild(1).GetComponent<Text>().text = count.ToString();
        count = int.Parse(collectedGemsCanvas.transform.GetChild(2).GetComponent<Text>().text) - gemCost.fireGem;
        collectedGemsCanvas.transform.GetChild(2).GetComponent<Text>().text = count.ToString();
        count = int.Parse(collectedGemsCanvas.transform.GetChild(3).GetComponent<Text>().text) - gemCost.airGem;
        collectedGemsCanvas.transform.GetChild(3).GetComponent<Text>().text = count.ToString();
        count = int.Parse(collectedGemsCanvas.transform.GetChild(4).GetComponent<Text>().text) - gemCost.earthGem;
        collectedGemsCanvas.transform.GetChild(4).GetComponent<Text>().text = count.ToString();
        count = int.Parse(collectedGemsCanvas.transform.GetChild(5).GetComponent<Text>().text) - gemCost.waterGem;
        collectedGemsCanvas.transform.GetChild(5).GetComponent<Text>().text = count.ToString();
    }

    private Text GetChildTextInCanvas(GameObject go)
    {
        switch (go.tag)
        {
            case "AttackGem":
                return collectedGemsCanvas.transform.GetChild(0).GetComponent<Text>();
            case "DefendGem":
                return collectedGemsCanvas.transform.GetChild(1).GetComponent<Text>();
            case "FireGem":
                return collectedGemsCanvas.transform.GetChild(2).GetComponent<Text>();
            case "AirGem":
                return collectedGemsCanvas.transform.GetChild(3).GetComponent<Text>();
            case "EarthGem":
                return collectedGemsCanvas.transform.GetChild(4).GetComponent<Text>();
            case "WaterGem":
                return collectedGemsCanvas.transform.GetChild(5).GetComponent<Text>();
            default:
                return null;
        }
    }

    public void ResolveMatches(List<GameObject> list)
    {
        if (list.Count != 0)
        {
            foundMatches = true;
            AddToCollectedGemsAndUpdateCanvas(list);
            CleanGrid(list);
            currentState = GameState.MatchingGems2;
        }
        else
        {
            foundMatches = false;
            currentState = GameState.MatchingGems3;
        }
    }

    private List<GameObject> MatchFinder()
    {
        //finalList.Clear();
        List<GameObject> partialMatch = new List<GameObject>();
        List<GameObject> finalList = new List<GameObject>();
        // POR FILAS
        for (int i = 0; i < gridHeight; i++)
        {
            partialMatch.Clear();
            for (int j = 0; j < gridWidth; j++)
            {
                if (partialMatch.Count == 0 && grid[i, j] != null)
                    partialMatch.Add(grid[i, j]);
                else
                {
                    if (grid[i, j] == null)
                    {
                        if (partialMatch.Count >= 3)
                        {
                            finalList.AddRange(partialMatch);
                            partialMatch.Clear();
                        }
                        else
                        {
                            partialMatch.Clear();
                        }
                    }
                    else
                    {
                        if ((partialMatch[0].tag == grid[i, j].tag) && (grid[i, j].tag != "DamageGem"))
                        {
                            partialMatch.Add(grid[i, j]);
                        }
                        else
                        {
                            if (partialMatch.Count >= 3)
                            {
                                finalList.AddRange(partialMatch);
                                partialMatch.Clear();
                            }
                            else
                            {
                                partialMatch.Clear();
                                partialMatch.Add(grid[i, j]);
                            }
                        }
                    }
                }
            }
            if (partialMatch.Count >= 3)
                finalList.AddRange(partialMatch);
        }
        //POR COLUMNAS
        for (int j = 0; j < gridWidth; j++)
        {
            partialMatch.Clear();
            for (int i = 0; i < gridHeight; i++)
            {
                if (partialMatch.Count == 0 && grid[i, j] != null)
                    partialMatch.Add(grid[i, j]);
                else
                {
                    if (grid[i, j] == null)
                    {
                        if (partialMatch.Count >= 3)
                        {
                            finalList.AddRange(partialMatch);
                            partialMatch.Clear();
                        }
                        else
                        {
                            partialMatch.Clear();
                        }
                    }
                    else
                    {
                        if ((partialMatch[0].tag == grid[i, j].tag) && (grid[i, j].tag != "DamageGem"))
                        {
                            partialMatch.Add(grid[i, j]);
                        }
                        else
                        {
                            if (partialMatch.Count >= 3)
                            {
                                finalList.AddRange(partialMatch);
                                partialMatch.Clear();
                            }
                            else
                            {
                                partialMatch.Clear();
                                partialMatch.Add(grid[i, j]);
                            }
                        }
                    }
                }
            }
            if (partialMatch.Count >= 3)
                finalList.AddRange(partialMatch);
        }
        //POR DIAGONALES
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                diagonalList.Clear();
                FindDiagonalMatches(i, j, 1, finalList);
                diagonalList.Clear();
                FindDiagonalMatches(i, j, -1, finalList);
            }
        }

        return RemoveDuplicates(true, finalList);
    }

    private void FindDiagonalMatches(int i, int j, int mod, List<GameObject> finalList)
    {
        if (IsInsideGrid(i, j))
        {
            if (diagonalList.Count == 0)
            {
                if (grid[i, j] != null)
                {
                    diagonalList.Add(grid[i, j]);
                    FindDiagonalMatches(i + 1 * mod, j + 1, mod, finalList);
                }
                else
                    FindDiagonalMatches(i + 1 * mod, j + 1, mod, finalList);

            }
            else
            {
                if (grid[i, j] != null)
                {
                    if ((grid[i, j].tag == diagonalList[0].tag) && (grid[i, j].tag != "DamageGem"))
                    {
                        diagonalList.Add(grid[i, j]);
                        FindDiagonalMatches(i + 1 * mod, j + 1, mod, finalList);
                    }
                    else
                        if (diagonalList.Count >= 3)
                    {
                        finalList.AddRange(diagonalList);
                        diagonalList.Clear();
                        FindDiagonalMatches(i + 1 * mod, j + 1, mod, finalList);
                    }
                    else
                    {
                        diagonalList.Clear();
                        FindDiagonalMatches(i, j, mod, finalList);
                    }

                }
                else
                    if (diagonalList.Count >= 3)
                {
                    finalList.AddRange(diagonalList);
                    diagonalList.Clear();
                    FindDiagonalMatches(i, j, mod, finalList);
                }
                else
                {
                    diagonalList.Clear();
                    FindDiagonalMatches(i, j, mod, finalList);
                }

            }
        }
        else
        {
            if (diagonalList.Count >= 3)
            {
                finalList.AddRange(diagonalList);
                diagonalList.Clear(); // TAL VEZ NO VA
            }
            else
                diagonalList.Clear();
        }
    }

    public bool IsInsideGrid(int i, int j)
    {
        if (i >= 0 && j >= 0 && i < gridHeight && j < gridWidth)
            return true;
        else
            return false;
    }

    private List<GameObject> RemoveDuplicates(bool normalMode, List<GameObject> list)
    {
        if (normalMode)
            list = list.Distinct().ToList();
        else
        {
            int index = 0;
            GameObject[] arr = new GameObject[destroyGemsList.Count];
            foreach (GameObject go in destroyGemsList)
            {
                arr[index] = go;
                index++;
            }
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[i] != null && arr[j] != null)
                        if (arr[i] == arr[j])
                        {
                            arr[j] = null;
                            gemsToDestroy--;
                        }
                }
            }
            destroyGemsList.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                    destroyGemsList.Add(arr[i]);
            }
        }
        return list;
    }



    private string GenerateGems(bool damage)
    {
        int rndNumber = UnityEngine.Random.Range(1, 7);
        string gemPath = "Prefabs/DefendGem";
        if (damage)
            gemPath = "Prefabs/DamageGem";
        else
            switch (rndNumber)
            {
                case 1:
                    gemPath = "Prefabs/FireGem";
                    break;
                case 2:
                    gemPath = "Prefabs/AirGem";
                    break;
                case 3:
                    gemPath = "Prefabs/EarthGem";
                    break;
                case 4:
                    gemPath = "Prefabs/WaterGem";
                    break;
                case 5:
                    gemPath = "Prefabs/AttackGem";
                    break;
                case 6:
                    gemPath = "Prefabs/DefendGem";
                    break;
            }
        return gemPath;
    }

    public bool checkIsInsideGrid(Vector3 pos)
    {
        return (((int)pos.x >= 0) && ((int)pos.x < gridWidth) && ((int)pos.y >= 0));
    }


    //HELPER METHODS
    public List<GameObject> GetRow(int row)
    {
        List<GameObject> list = new List<GameObject>();
        for (int j = 0; j < gridWidth; j++)
        {
            if (grid[row,j] != null)
            {
                list.Add(grid[row, j]);
            }            
        }
        return list;
    }

    public List<GameObject> GetColumn(int col)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < gridHeight; i++)
        {
            if (grid[i, col] != null)
                list.Add(grid[i, col]);
            else
                break;
        }
        return list;
    }

    public bool IsPopulatedColumn(int col)
    {
        if (grid[0, col] != null)
            return true;
        else
            return false;
    }
    public int GetTopRow()
    {
        for (int i = gridHeight-1; i >= 0; i--)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (grid[i, j] != null)
                    return i;
            }
        }
        return 0;
    }

    public List<GameObject> Get3by3(int i, int j)
    {
        List<GameObject> list = new List<GameObject>();

        if (IsInsideGrid(i - 1, j - 1))
        {
            if (grid[i - 1, j - 1] != null)
                list.Add(grid[i - 1, j - 1]);
        }
        if (IsInsideGrid(i, j - 1))
        {
            if (grid[i, j - 1] != null)
                list.Add(grid[i, j- 1]);
        }
        if (IsInsideGrid(i + 1, j - 1))
        {
            if (grid[i + 1, j - 1] != null)
                list.Add(grid[i + 1, j - 1]);
        }
        if (IsInsideGrid(i - 1, j))
        {
            if (grid[i - 1, j] != null)
                list.Add(grid[i - 1, j]);
        }
        if (IsInsideGrid(i + 1, j))
        {
            if (grid[i + 1, j] != null)
                list.Add(grid[i + 1, j]);
        }
        if (IsInsideGrid(i - 1, j + 1))
        {
            if (grid[i - 1, j + 1] != null)
                list.Add(grid[i - 1, j + 1]);
        }
        if (IsInsideGrid(i, j + 1))
        {
            if (grid[i, j + 1] != null)
                list.Add(grid[i, j + 1]);
        }
        if (IsInsideGrid(i + 1, j + 1))
        {
            if (grid[i + 1, j + 1] != null)
                list.Add(grid[i + 1, j + 1]);
        }
        if (grid[i, j] != null)
            list.Add(grid[i, j]);

        return list;
    }
}
