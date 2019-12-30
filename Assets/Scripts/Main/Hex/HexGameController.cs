using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Photon.Pun;

public class HexGameController : MonoBehaviour
{
    public float turnTime = 15f;

    PhotonView photonView;

    bool isServer;

    public HexGrid grid;
    public HexGameUI gameUI;
    public HexMapGenerator mapGenerator;
    int seed;

    List<HexCell> availableCells;
    int cellCount;

    HexCell treasureCell;

    public HexUnit serverUnit, clientUnit;
    HexUnit myUnit, otherUnit;
    HexCell serverStart, clientStart;

    List<int> itemTypes = new List<int>();
    List<int> itemCellIndex = new List<int>();

    bool serverTurn;
    public static bool myTurn = false;
    float second;
    public static bool endTurn = false;
    public bool inGame;

    public UIController ui;

    void Awake()
    {
        SHA256 mySHA256 = SHA256.Create();
        if (MenuInfo.Seed == null)
        {
            MenuInfo.Seed = "123";
        }
        byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(MenuInfo.Seed));

        HexMetrics.InitializeHashGrid(System.BitConverter.ToInt32(hashValue, 0));
        seed = System.BitConverter.ToInt32(hashValue, 4);

        photonView = PhotonView.Get(this);
        isServer = PhotonNetwork.IsMasterClient;

        serverUnit = Instantiate<HexUnit>(HexUnit.serverPrefab);
        serverUnit.Owned = isServer;
        serverUnit.ui = ui;
        clientUnit = Instantiate<HexUnit>(HexUnit.clientPrefab);
        clientUnit.Owned = !isServer;
        clientUnit.ui = ui;
    }

    void Start()
    {
        mapGenerator.GenerateMap(70, 60, seed);
        cellCount = 70 * 60;

        HexMapCamera.CenterPosition();

        if (isServer)
        {
            GetAvailableCells();

            do
            {
                FindTreasureCell();
            } while (!FindStartCell(40, 20));

            itemTypes.Add((int)HexItemType.Treasure);
            itemCellIndex.Add(treasureCell.Index);

            RemoveAvailableCell(serverStart);
            RemoveAvailableCell(clientStart);
            RemoveAvailableCell(treasureCell);

            SpreadKeys(10);
            SpreadItems(100);

            SendStart();
        }

        inGame = true;
    }

    void Update()
    {
        if (inGame)
        {
            StartGame();

            SetTurn();
            DoTurn();

            CheckWin();
        }
    }

    void GetAvailableCells()
    {
        availableCells = new List<HexCell>();

        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            if (cell.Explorable && !cell.IsUnderwater && cell.IsWalkable)
            {
                availableCells.Add(cell);
            }
        }

        Log.Dev(GetType(), "available cells: " + availableCells.Count.ToString());
    }

    void RemoveAvailableCell(HexCell cell)
    {
        availableCells.Remove(cell);
    }

    void RemoveAvailableCell(int index)
    {
        availableCells[index] = availableCells[availableCells.Count - 1];
        availableCells.RemoveAt(availableCells.Count - 1);
    }

    void FindTreasureCell()
    {
        int index = Random.Range(0, availableCells.Count);
        treasureCell = availableCells[index];
    }

    bool FindStartCell(int treasureGap, int playerGap)
    {
        int guard = 0;

        bool finding = true;
        while (finding)
        {
            if (guard++ == 10000)
            {
                return false;
            }

            int index = Random.Range(0, availableCells.Count);
            serverStart = availableCells[index];
            grid.FindPath(serverStart, treasureCell, serverUnit, false);
            if (!grid.HasPath || grid.GetPath().Count < treasureGap)
            {
                grid.ClearPath();
                continue;
            }
            grid.ClearPath();

            finding = false;
        }

        finding = true;
        while (finding)
        {
            if (guard++ == 10000)
            {
                return false;
            }

            int index = Random.Range(0, availableCells.Count);
            clientStart = availableCells[index];
            grid.FindPath(clientStart, treasureCell, clientUnit, false);
            if (!grid.HasPath || grid.GetPath().Count < treasureGap)
            {
                grid.ClearPath();
                continue;
            }
            grid.ClearPath();

            grid.FindPath(clientStart, serverStart, clientUnit, false);
            if (!grid.HasPath || grid.GetPath().Count < playerGap)
            {
                grid.ClearPath();
                continue;
            }
            grid.ClearPath();

            finding = false;
        }

        return true;
    }

    void SpreadKeys(int count)
    {
        while (count-- > 0 && availableCells.Count >= 0)
        {
            int index = Random.Range(0, availableCells.Count);

            itemTypes.Add((int)HexItemType.Key);
            itemCellIndex.Add(availableCells[index].Index);

            RemoveAvailableCell(index);
        }
    }

    void SpreadItems(int count)
    {
        HexItemType[] types = HexItemTypeCollection.GetMapRandomType();
        float[] probabilities = HexItemTypeCollection.GetMapRandomValue();

        while (count-- > 0 && availableCells.Count >= 0)
        {
            float p = Random.value;
            int i = 0;
            for (; p > probabilities[i]; i++) ;
            itemTypes.Add((int)types[i]);

            int index = Random.Range(0, availableCells.Count);
            itemCellIndex.Add(availableCells[index].Index);

            RemoveAvailableCell(index);
        }
    }

    void StartGame()
    {
        if (StartInfo.Synced)
        {
            Log.Status(GetType(), "Game start.");

            AddUnits();
            AddItems();

            grid.ResetVisibility();

            if (!isServer)
            {
                SendTurn();
            }

            StartInfo.Synced = false;
        }
    }

    void SetTurn()
    {
        if (TurnInfo.Synced)
        {
            Log.Status(GetType(), "turn " + TurnInfo.Turn);

            if (isServer)
            {
                myTurn = TurnInfo.Turn % 2 == 1;
            }
            else
            {
                myTurn = TurnInfo.Turn % 2 == 0;
            }

            if (myTurn)
            {
                second = 0;
                endTurn = false;
                myUnit.SetSpeed();

                ui.StartCounting(0f);
                ui.EnergyCounting(30);
            }
            else
            {
                ui.StartCounting(-1f);
                ui.EnergyCounting(-1);
            }

            TurnInfo.Synced = false;
        }
    }

    void DoTurn()
    {
        if (myTurn)
        {
            second += Time.deltaTime;

            if (endTurn || second > turnTime)
            {
                myTurn = false;
                SendTurn();
            }
            ui.StartCounting(second / turnTime);
            ui.EnergyCounting(myUnit.Speed);

            SendScore();
        }

        if (ItemInfo.Synced)
        {
            addItem((HexItemType)ItemInfo.ItemType, grid.GetCell(ItemInfo.CellIndex));

            ItemInfo.Synced = false;
        }

        if (RemoveItemInfo.Synced)
        {
            Log.Dev(GetType(), "remove item at cell index: " + RemoveItemInfo.CellIndex.ToString());

            grid.RemoveItem(RemoveItemInfo.CellIndex);
            RemoveItemInfo.Synced = false;
        }

        if (EffectInfo.Synced)
        {
            switch ((HexItemType)EffectInfo.ItemIype)
            {
                case HexItemType.Poison:
                    myUnit.speedEffect(-20, 3);
                    break;
                case HexItemType.Change:
                    grid.changeUnits();
                    HexMapCamera.SetPosition(myUnit.Location);
                    break;
            }

            EffectInfo.Synced = false;
        }

        if (ScoreInfo.Synced)
        {
            if (ScoreInfo.IsServer)
            {
                serverUnit.Score = ScoreInfo.Score;
            }
            else
            {
                clientUnit.Score = ScoreInfo.Score;
            }

            ScoreInfo.Synced = false;
        }

        ui.MyScore(myUnit.Score);
        ui.OtherScore(otherUnit.Score);
    }

    void CheckWin()
    {
        if (myUnit.HasTreasure)
        {
            SendWin();
        }

        if (WinInfo.Synced)
        {
            inGame = false;

            Log.Status(GetType(), "server/client has treasure: " + serverUnit.HasTreasure + " / " + clientUnit.HasTreasure);
            Log.Status(GetType(), "server/client score: " + serverUnit.Score + " / " + clientUnit.Score);

            if (WinInfo.IsServerWin == isServer)
            {
                serverUnit.anim.SetBool("Victory", true);
                clientUnit.anim.SetBool("Lost", true);
                ui.isWin(true);
                Log.Status(GetType(), "you win");
            }
            else
            {
                clientUnit.anim.SetBool("Victory", true);
                serverUnit.anim.SetBool("Lost", true);
                ui.isWin(false);
                Log.Status(GetType(), "you lose");
            }

            WinInfo.Synced = false;
        }
    }

    void AddUnits()
    {
        if (isServer)
        {
            gameUI.myUnit = myUnit = serverUnit;
            gameUI.otherUnit = otherUnit = clientUnit;
        }
        else
        {
            serverStart = grid.GetCell(StartInfo.ServerIndex);
            clientStart = grid.GetCell(StartInfo.ClientIndex);

            gameUI.myUnit = myUnit = clientUnit;
            gameUI.otherUnit = otherUnit = serverUnit;
        }

        Log.Dev(GetType(), "server starts at " + serverStart.coordinates.ToString() + " client starts at " + clientStart.coordinates.ToString());

        grid.AddUnit(serverUnit, serverStart);
        grid.AddUnit(clientUnit, clientStart);
    }

    void AddItems()
    {
        if (!isServer)
        {
            for (int i = 0; i < StartInfo.ItemTypes.Length; i++)
            {
                itemTypes.Add(StartInfo.ItemTypes[i]);
                itemCellIndex.Add(StartInfo.ItemCellIndex[i]);
            }
        }

        for (int i = 0; i < itemTypes.Count; i++)
        {
            HexItem item = Instantiate<HexItem>(HexItem.itemPrefab);
            item.itemType = (HexItemType)itemTypes[i];
            grid.AddItem(item, grid.GetCell(itemCellIndex[i]));
        }
    }

    void addItem(HexItemType type, HexCell cell)
    {
        HexItem item = Instantiate<HexItem>(HexItem.itemPrefab);
        item.itemType = type;
        grid.AddItem(item, cell);
    }

    public void useItem(HexItemType type)
    {
        switch (type)
        {
            case HexItemType.Poison:
                otherUnit.speedEffect(-20, 3);
                sendEffect(HexItemType.Poison);
                break;
            case HexItemType.FakeTreasureItem:
                addItem(HexItemType.FakeTreasure, myUnit.Location);
                sendItem(HexItemType.FakeTreasure, myUnit);
                break;
            case HexItemType.Change:
                grid.changeUnits();
                HexMapCamera.SetPosition(myUnit.Location);
                sendEffect(HexItemType.Change);
                break;
        }
    }

    void SendStart()
    {
        int[] tempItemTypes = new int[itemTypes.Count];
        int[] tempItemCellIndex = new int[itemCellIndex.Count];
        for (int i = 0; i < itemTypes.Count; i++)
        {
            tempItemTypes[i] = itemTypes[i];
            tempItemCellIndex[i] = itemCellIndex[i];
        }

        photonView.RPC("GetStart", RpcTarget.All, serverStart.Index, clientStart.Index, tempItemTypes, tempItemCellIndex);
    }

    void SendTurn()
    {
        grid.ClearPath();
        photonView.RPC("GetTurn", RpcTarget.All, TurnInfo.Turn + 1);
    }

    void SendScore()
    {
        photonView.RPC("GetScore", RpcTarget.Others, isServer, myUnit.Score);
    }

    void sendItem(HexItemType type, HexUnit unit)
    {
        photonView.RPC("GetItem", RpcTarget.Others, (int)type, (int)unit.Location.Index);
    }

    void sendEffect(HexItemType type)
    {
        photonView.RPC("GetEffect", RpcTarget.Others, (int)type);
    }

    void SendWin()
    {
        grid.ClearPath();
        photonView.RPC("GetWin", RpcTarget.All, serverUnit.Score > clientUnit.Score || serverUnit.HasTreasure);
    }

    [PunRPC]
    void GetStart(int serverIndex, int clientIndex, int[] itemTypes, int[] itemCellIndex)
    {
        StartInfo.Synced = true;
        StartInfo.ServerIndex = serverIndex;
        StartInfo.ClientIndex = clientIndex;
        StartInfo.ItemTypes = itemTypes;
        StartInfo.ItemCellIndex = itemCellIndex;
    }

    [PunRPC]
    void GetTurn(int turn)
    {
        TurnInfo.Synced = true;
        TurnInfo.Turn = turn;
    }

    [PunRPC]
    void GetScore(bool isServer, int score)
    {
        ScoreInfo.Synced = true;
        ScoreInfo.IsServer = isServer;
        ScoreInfo.Score = score;
    }

    [PunRPC]
    void GetItem(int itemType, int itemCellIndex)
    {
        ItemInfo.Synced = true;
        ItemInfo.ItemType = itemType;
        ItemInfo.CellIndex = itemCellIndex;
    }

    [PunRPC]
    void GetEffect(int itemType)
    {
        EffectInfo.Synced = true;
        EffectInfo.ItemIype = itemType;
    }

    [PunRPC]
    void GetWin(bool isServerWin)
    {
        WinInfo.Synced = true;
        WinInfo.IsServerWin = isServerWin;
    }
}
