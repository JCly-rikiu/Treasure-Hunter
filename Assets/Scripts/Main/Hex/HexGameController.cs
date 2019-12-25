using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Photon.Pun;

public class HexGameController : MonoBehaviour
{
    public static float turnTime = 20f;

    PhotonView photonView;

    bool isServer;

    public HexGrid grid;
    public HexGameUI gameUI;
    public HexMapCamera mapCamera;
    public HexMapGenerator mapGenerator;
    int seed;

    List<HexCell> availableCells;
    int cellCount;

    HexCell treasureCell;

    HexUnit serverUnit, clientUnit, myUnit;
    HexCell serverStart, clientStart;

    List<int> itemTypes = new List<int>();
    List<int> itemIndex = new List<int>();

    bool serverTurn;
    public static bool myTurn = false;
    float second;

    public BarControl timeBar;
    public BarControl energyBar;

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

        gameUI.mapCamera = mapCamera;

        photonView = PhotonView.Get(this);
        isServer = PhotonNetwork.IsMasterClient;

        serverUnit = Instantiate<HexUnit>(HexUnit.serverPrefab);
        serverUnit.Owned = isServer;
        clientUnit = Instantiate<HexUnit>(HexUnit.clientPrefab);
        clientUnit.Owned = !isServer;
    }

    void Start()
    {
        mapGenerator.GenerateMap(70, 60, seed);
        cellCount = 70 * 60;

        mapCamera.CenterPosition();

        if (isServer)
        {
            GetAvailableCells();

            do
            {
                FindTreasureCell();
            } while (!FindStartCell(40, 20));

            itemTypes.Add((int)HexItemType.Treasure);
            itemIndex.Add(treasureCell.Index);

            RemoveAvailableCell(serverStart);
            RemoveAvailableCell(clientStart);
            RemoveAvailableCell(treasureCell);

            SpreadItems(50);

            SendStart();
        }
    }

    void Update()
    {
        if (StartInfo.Synced)
        {
            Log.Status(GetType(), "StartInfo synced.");

            AddUnits();
            AddItems();

            grid.ResetVisibility();

            if (!isServer)
            {
                SendTurn();
            }
            StartInfo.Synced = false;
        }

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
                myUnit.SetSpeed();
            }

            TurnInfo.Synced = false;
        }

        if (myTurn)
        {
            second += Time.deltaTime;

            if (second > turnTime)
            {
                myTurn = false;
                SendTurn();
            }
            timeBar.StartCounting(second / turnTime);
            energyBar.StartCounting(1f - myUnit.Speed / 30f);
        }
        else
        {
            timeBar.StartCounting(0f);
            energyBar.StartCounting(0f);
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

        Log.Status(GetType(), "available cells: " + availableCells.Count.ToString());
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

    void SpreadItems(int count)
    {
        HexItemType[] types = HexItemTypeCollection.GetMapRandom();

        while (count-- > 0 && availableCells.Count >= 0)
        {
            int index = Random.Range(0, availableCells.Count);

            itemTypes.Add((int)types[Random.Range(0, types.Length)]);
            itemIndex.Add(availableCells[index].Index);

            RemoveAvailableCell(index);
        }
    }

    void AddUnits()
    {
        if (isServer)
        {
            gameUI.myUnit = myUnit = serverUnit;
            gameUI.otherUnit = clientUnit;
        }
        else
        {
            serverStart = grid.GetCell(StartInfo.ServerIndex);
            clientStart = grid.GetCell(StartInfo.ClientIndex);

            gameUI.myUnit = myUnit = clientUnit;
            gameUI.otherUnit = serverUnit;
        }

        Log.Status(GetType(), "server starts at " + serverStart.coordinates.ToString() + " client starts at " + clientStart.coordinates.ToString());

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
                itemIndex.Add(StartInfo.ItemIndex[i]);
            }
        }

        for (int i = 0; i < itemTypes.Count; i++)
        {
            HexItem item = Instantiate<HexItem>(HexItem.itemPrefab);
            item.itemType = (HexItemType)itemTypes[i];
            item.Owned = false; //item.itemType == HexItemType.Treasure;
            grid.AddItem(item, grid.GetCell(itemIndex[i]));
        }
    }

    void SendStart()
    {
        int[] tempItemTypes = new int[itemTypes.Count];
        int[] tempItemIndex = new int[itemIndex.Count];
        for (int i = 0; i < itemTypes.Count; i++)
        {
            tempItemTypes[i] = itemTypes[i];
            tempItemIndex[i] = itemIndex[i];
        }

        photonView.RPC("SendStart", RpcTarget.All, true, serverStart.Index, clientStart.Index, tempItemTypes, tempItemIndex);
    }

    void SendTurn()
    {
        grid.ClearPath();
        photonView.RPC("SendTurn", RpcTarget.All, true, TurnInfo.Turn + 1);
    }

    [PunRPC]
    void SendStart(bool synced, int serverIndex, int clientIndex, int[] itemTypes, int[] itemIndex)
    {
        StartInfo.Synced = synced;
        StartInfo.ServerIndex = serverIndex;
        StartInfo.ClientIndex = clientIndex;
        StartInfo.ItemTypes = itemTypes;
        StartInfo.ItemIndex = itemIndex;
    }

    [PunRPC]
    void SendTurn(bool synced, int turn)
    {
        TurnInfo.Synced = synced;
        TurnInfo.Turn = turn;
    }
}
