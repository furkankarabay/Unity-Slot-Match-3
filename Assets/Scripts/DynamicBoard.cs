using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DynamicBoard : MonoBehaviour
{
    public static DynamicBoard Instance;

    [Header("References")]
    public GameObject tileNodePrefab;
    public GameObject spritePrefabSpin;
    public Transform board;
    public TileDatabase tileDatabase;

    [Header("Settings")]
    [Min(5)]
    public int boardSize = 5;
    public bool randomizeSize;
    public float spriteSpacingMin;

    private TileData[,] grid;
    private Tile[,] tiles;

    private float boardX, boardY;
    private float spriteScaleX;
    private float spriteScaleY;
    private float spriteScale;
    private float xStartPosition;
    private float yStartPosition;

    public List<SpinningRows_List> spinningRowLists = new List<SpinningRows_List>();

    void Awake()
    {
        Instance = this;

        AdjustBoard();

        FillGridsWithTileDatas();

        FillSpinningRowLists();

        tiles = new Tile[boardSize, boardSize];

        for (int i = 0; i < boardSize; i++)
        {
            GameObject parentObject = new GameObject("ParentObject_" + i);
            parentObject.transform.SetParent(transform);
            GameObject parentObject2 = new GameObject("ParentObject2_" + i);
            parentObject2.transform.SetParent(transform);
            GameObject parentObject3 = new GameObject("ParentObject3_" + i);
            parentObject3.transform.SetParent(transform);

            parentObject.transform.position = Vector3.zero;
            for (int j = 0; j < boardSize; j++)
            {
                GameObject tileObject = Instantiate(tileNodePrefab, transform);
                Tile tile = tileObject.GetComponent<Tile>();
                TileData tileData = grid[i, j];

                tile.Initialize(tileData, new Vector2Int(i, j));
                tiles[i, j] = tile;

                Vector3 tileLocalScale = new Vector3(spriteScale, spriteScale);
                Vector3 tileLocalPostion = new Vector3(xStartPosition + (spriteSpacingMin * i) + (spriteScale * i),
                    yStartPosition + (spriteSpacingMin * j) + (spriteScale * j));

                tile.SetTileScale(tileLocalScale, tileLocalPostion);

                StartCoroutine(CreateThreeSpinningRow(tileObject, tile.tileData.tileSprite, 
                    parentObject, parentObject2, parentObject3));
            }

            spinningRowLists[i].rows.Add(new SpinningRow(parentObject, false));
            spinningRowLists[i].rows.Add(new SpinningRow(parentObject2, true));
            spinningRowLists[i].rows.Add(new SpinningRow(parentObject3, false));
        }

    }
    private void Start()
    {
        StartCoroutine(SetDelayInitialize());
    }

    IEnumerator SetDelayInitialize()
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.ChangeState(GameState.Initialize);
    }
    public void AdjustBoard()
    {
        if (randomizeSize)
            boardSize = UnityEngine.Random.Range(5, 15);


        boardX = board.localScale.x;
        boardY = board.localScale.y;

        spriteScaleX = (boardX - spriteSpacingMin * boardSize) / boardSize;
        spriteScaleY = (boardY - spriteSpacingMin * boardSize) / boardSize;

        spriteScale = Mathf.Min(spriteScaleX, spriteScaleY);

        yStartPosition = (boardY / -2) + (spriteScale / 2);
        xStartPosition = (spriteScale * boardSize) + (spriteSpacingMin * (boardSize + 1));

        yStartPosition += spriteSpacingMin / 2;
        xStartPosition = (boardX - xStartPosition) / -2;
    }

    public void FillSpinningRowLists()
    {
        for (int i = 0; i < boardSize; i++)
        {
            SpinningRows_List rowFakeList = new();
            rowFakeList.hasStopped = true;
            rowFakeList.rows = new();
            spinningRowLists.Add(rowFakeList);
        }
    }
    public void ResetBoard()
    {
        FillGridsWithTileDatas();

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                tiles[i, j].Initialize(grid[i, j], new Vector2Int(i, j));
            }

            foreach (var row in spinningRowLists[i].rows)
            {
                int index = 0;

                if (row.onView)
                    continue;

                foreach (Transform rowTile in row.row.transform)
                {
                    rowTile.GetChild(0).GetComponent<SpriteRenderer>().sprite = tiles[i, index].tileData.tileSprite;
                    index++;
                }
            }
        }

    }

    IEnumerator CreateThreeSpinningRow(GameObject tileObject, Sprite tileSprite, GameObject parentObject, 
        GameObject parentObject2, GameObject parentObject3)
    {
        yield return new WaitForSeconds(0.5f);

        CreateSpinningRow_Tile(tileObject, tileSprite, parentObject); // Upper
        CreateSpinningRow_Tile(tileObject, tileSprite, parentObject2); // Middle (On view)
        CreateSpinningRow_Tile(tileObject, tileSprite, parentObject3); // Lower

        yield return new WaitForSeconds(0.25f);

        parentObject.transform.position = new Vector3(0, boardY, 0);
        parentObject2.transform.position = new Vector3(0, 0, 0);
        parentObject3.transform.position = new Vector3(0, -boardY, 0);
    }

    public void SetSpinningRowsActive()
    {
        for (int i = 0; i < spinningRowLists.Count; i++)
        {
            spinningRowLists[i].rows.ForEach(row => { row.row.SetActive(true); });
        }
    }
    public void StartSpinning()
    {
        ResetBoard();

        StopSpinning(false);
        
        for (int i = 0; i < boardSize; i++)
        {
            StartCoroutine(SetDelayBeforeSpinningRows(i));

        }

        SetSpinningRowsActive();
        DeactiveTilesSprites();
    }

    public void StopSpinning(bool isStopped)
    {
        for (int i = 0; i < spinningRowLists.Count; i++)
        {
            spinningRowLists[i].willStopInSeconds = isStopped;
        }
    }
    public IEnumerator SetDelayBeforeSpinningRows(int row)
    {
        yield return new WaitForSecondsRealtime(0.1f * row);
        StartCoroutine(SpinRows(row, GetRowLists()));

        spinningRowLists[row].isStartedMoving = true;
    }

    public void DeactiveTilesSprites()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                tiles[i, j].tileAnimation.spriteRenderer.sprite = null;
            }
        }
    }

    public List<SpinningRows_List> GetRowLists()
    {
        return spinningRowLists;
    }

    public IEnumerator SpinRows(int row, List<SpinningRows_List> rowLists)
    {
        if (rowLists[row].willStopInSeconds)
        {
            for (int i = 0; i < boardSize; i++)
            {
                tiles[row, i].tileAnimation.spriteRenderer.sprite = tiles[row, i].tileData.tileSprite;
            }

            rowLists[row].rows[0].row.SetActive(false);
            rowLists[row].rows[1].row.SetActive(false);
            rowLists[row].rows[2].row.SetActive(false);

            rowLists[row].hasStopped = true;
            yield break;
        }
        GameObject upper = rowLists[row].rows[0].row;
        GameObject middle = rowLists[row].rows[1].row;
        GameObject lower = rowLists[row].rows[2].row;

        rowLists[row].rows[0].row.transform.DOMoveY(0, 0.6f).SetEase(Ease.Linear);
        rowLists[row].rows[1].row.transform.DOMoveY(-boardY, 0.6f).SetEase(Ease.Linear);
        rowLists[row].rows[2].row.transform.position = new Vector3(0, boardY, 0);

        rowLists[row].rows[0].row = lower;
        rowLists[row].rows[0].onView = false;

        int index = 0;
        foreach (Transform item in rowLists[row].rows[0].row.transform)
        {
            item.GetChild(0).GetComponent<SpriteRenderer>().sprite = tiles[row, index].tileData.tileSprite;
            index++;
        }

        rowLists[row].rows[1].row = upper;
        rowLists[row].rows[1].onView = true;

        rowLists[row].rows[2].row = middle;
        rowLists[row].rows[2].onView = false;

        yield return new WaitForSecondsRealtime(0.6f);

        StartCoroutine(SpinRows(row, GetRowLists()));
    }

    public void CreateSpinningRow_Tile(GameObject tileObject, Sprite tileSprite, GameObject parentObject)
    {
        GameObject tileObjectSpin = Instantiate(spritePrefabSpin, transform);

        tileObjectSpin.transform.localScale = tileObject.transform.localScale;
        tileObjectSpin.transform.localPosition = tileObject.transform.localPosition;
        tileObjectSpin.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tileSprite;
        tileObjectSpin.transform.SetParent(parentObject.transform);
    }
    void FillGridsWithTileDatas()
    {
        grid = new TileData[boardSize, boardSize];

        // En az 3 kez her renk kullanýlacak þekilde, grid'e renk yerleþtir
        Dictionary<TileData, int> colorCount = new Dictionary<TileData, int>();
        foreach (TileData color in tileDatabase.tiles)
        {
            colorCount[color] = 0;
        }

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                TileData color = GetRandomColorWithConditions(colorCount, i, j);

                grid[i, j] = color;
                colorCount[color]++;
            }
        }
    }

    // Kurallara göre renk seçimi yap
    TileData GetRandomColorWithConditions(Dictionary<TileData, int> colorCount, int x, int y)
    {
        List<TileData> availableColors = new List<TileData>();

        // Her renkten en az 3 tane olacak þekilde, renklere filtre uygula
        foreach (var color in tileDatabase.tiles)
        {
            // Eðer renk 3'ten az kullanýldýysa ya da bu rengin sayýsý, board'un toplam hücresinin
            // renk sayýsýna oranla uygun sýnýrda ise, bu rengi kullanýlabilir olarak ekle
            if (colorCount[color] < 3)
            {
                availableColors.Add(color);
            }
        }

        // Eðer tüm renkler en az 3 defa kullanýlmýþsa, hepsini ekleyebiliriz
        if (availableColors.Count == 0)
        {
            // Tüm renkleri kullanabileceðimiz bir durumda
            foreach (var color in tileDatabase.tiles)
            {
                availableColors.Add(color);
            }
        }

        // Eðer availableColors boþsa, hata mesajý göster
        if (availableColors.Count == 0)
        {
            Debug.LogError("No available colors left to choose from!");
            return tileDatabase.tiles[0]; // Default renk döndür
        }

        TileData newColor;
        bool hasThreeInRow = false;
        int attempts = 0;

        do
        {
            // Uygun renkten bir tane seç
            newColor = availableColors[UnityEngine.Random.Range(0, availableColors.Count)];

            // Yeni seçilen rengin 3lü dizilim yapýp yapmadýðýný kontrol et
            hasThreeInRow = HasThreeInARow(newColor, x, y);

            if(hasThreeInRow)
            {
                availableColors.Remove(newColor);

                if (availableColors.Count == 0)
                {
                    // Tüm renkleri kullanabileceðimiz bir durumda
                    foreach (var color in tileDatabase.tiles)
                    {
                        availableColors.Add(color);
                    }
                }
            }

            attempts++;
            if (attempts > 100)
            {
                Debug.LogError("Too many attempts to find a valid color. Returning default color.");
                break;
            }

        } while (hasThreeInRow);

        return newColor;
    }

    bool HasThreeInARow(TileData color, int x, int y)
    {
        if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
        {
            return false;
        }

        if (x > 1 && grid[x - 1, y].tileType == color.tileType && grid[x - 2, y].tileType == color.tileType)
        {
            return true;
        }

        if (y > 1 && grid[x, y - 1].tileType == color.tileType && grid[x, y - 2].tileType == color.tileType)
        {
            return true;
        }

        return false;
    }

    public void SwapTiles(Vector2Int position, Vector2Int direction)
    {
        Vector2Int targetPosition = position + direction;

        if (targetPosition.x >= 0 && targetPosition.x < boardSize &&
            targetPosition.y >= 0 && targetPosition.y < boardSize)
        {
            Tile tileA = tiles[position.x, position.y];
            Tile tileB = tiles[targetPosition.x, targetPosition.y];

            if (tileA.isSwapping || tileB.isSwapping)
                return;

            if (!tileA.isSwappable || !tileB.isSwappable)
                return;

            Tile tempTileA = tiles[tileA.position.x, tileA.position.y];
            tiles[tileA.position.x, tileA.position.y] = tileB;

            tiles[tileB.position.x, tileB.position.y] = tempTileA;

            Vector2Int tempPosition = tileA.position;
            tileA.position = tileB.position;
            tileB.position = tempPosition;

            tileA.tileInput.position = tileA.position;
            tileB.tileInput.position = tileB.position;

            tileA.tileAnimation.OnSwapped(tileB);
            tileB.tileAnimation.OnSwapped(tileA);

            CheckMatch(tileA, tileB);
        }
    }
    private void CheckMatch(Tile tileA, Tile tileB)
    {
        bool isMatch = FindMatches(tileA) || FindMatches(tileB);

        if (isMatch)
        {
            GameManager.Instance.ChangeState(GameState.Result);
            StartCoroutine(HandleMatches());
        }
    }
    private IEnumerator HandleMatches()
    {
        yield return new WaitForSeconds(0.5f);
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (FindMatches(tiles[x, y]))
                {
                    tiles[x, y].tileAnimation.OnMatched();
                }
            }
        }
    }

    public void SetTilesSwappable(bool canSwap)
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                tiles[x, y].isSwappable = canSwap;
            }
        }
    }

    bool FindMatches(Tile tile)
    {
        int matchCount = 1;

        // Yatay kontrol
        for (int x = tile.position.x + 1; x < boardSize; x++)
        {
            if (tiles[x, tile.position.y] == null)
                continue;

            if (tiles[x, tile.position.y].tileData.tileType == tile.tileData.tileType)
                matchCount++;
            else
                break;
        }

        for (int x = tile.position.x - 1; x >= 0; x--)
        {
            if (tiles[x, tile.position.y] == null)
                continue;

            if (tiles[x, tile.position.y].tileData.tileType == tile.tileData.tileType)
                matchCount++;
            else
                break;
        }

        if (matchCount >= 3)
            return true;

        // Dikey kontrol
        matchCount = 1;

        for (int y = tile.position.y + 1; y < boardSize; y++)
        {
            if (tiles[tile.position.x, y] == null)
                continue;

            if (tiles[tile.position.x, y].tileData.tileType == tile.tileData.tileType)
                matchCount++;
            else
                break;
        }

        for (int y = tile.position.y - 1; y >= 0; y--)
        {
            if (tiles[tile.position.x, y] == null)
                continue;

            if (tiles[tile.position.x, y].tileData.tileType == tile.tileData.tileType)
                matchCount++;
            else
                break;
        }

        return matchCount >= 3;
    }
}

[Serializable]
public class SpinningRows_List
{
    public bool willStopInSeconds;
    public List<SpinningRow> rows;
    public bool isStartedMoving;
    public bool hasStopped;
}
[Serializable]
public class SpinningRow
{
    public GameObject row;
    public int index;
    public bool onView;

    public SpinningRow(GameObject row, bool onView)
    {
        this.row = row;
        this.onView = onView;
    }
}