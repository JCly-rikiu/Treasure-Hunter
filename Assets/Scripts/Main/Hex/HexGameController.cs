using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Photon.Pun;

public class HexGameController : MonoBehaviour
{
    PhotonView photonView;

    bool isServer;

    public HexGrid grid;
    public HexGameUI gameUI;
    public HexMapGenerator mapGenerator;
    int seed;

    List<HexCell> availableCells;
    int cellCount;

    HexUnit serverUnit, clientUnit;
    HexCell serverStart, clientStart;

    void Awake()
    {
        SHA256 mySHA256 = SHA256.Create();
        byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(MenuInfo.Seed));

        HexMetrics.InitializeHashGrid(System.BitConverter.ToInt32(hashValue, 0));
        seed = System.BitConverter.ToInt32(hashValue, 4);

        isServer = PhotonNetwork.IsMasterClient;

        photonView = PhotonView.Get(this);
    }

    void Start()
    {
        mapGenerator.GenerateMap(70, 60, seed);
        cellCount = 70 * 60;

        if (isServer)
        {
            GetAvailableCells();
            FindStartCell();

            AddUnits();
        }
    }

    void Update()
    {
        if (StartPointsInfo.Synced)
        {
            if (!isServer)
            {
                AddUnits();
                StartPointsInfo.Synced = false;
            }
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
    }

    void FindStartCell()
    {
        int index = Random.Range(0, availableCells.Count);
        serverStart = availableCells[index];
        availableCells[index] = availableCells[availableCells.Count - 1];
        availableCells.RemoveAt(availableCells.Count - 1);

        index = Random.Range(0, availableCells.Count);
        clientStart = availableCells[index];
        availableCells[index] = availableCells[availableCells.Count - 1];
        availableCells.RemoveAt(availableCells.Count - 1);

        photonView.RPC("SendStart", RpcTarget.Others, true, serverStart.Index, clientStart.Index);
    }

    void AddUnits()
    {
        serverUnit = Instantiate<HexUnit>(HexUnit.serverPrefab);
        serverUnit.Owned = isServer;
        clientUnit = Instantiate<HexUnit>(HexUnit.clientPrefab);
        clientUnit.Owned = !isServer;

        if (isServer)
        {
            gameUI.myUnit = serverUnit;
            gameUI.otherUnit = clientUnit;
        }
        else
        {
            serverStart = grid.GetCell(StartPointsInfo.ServerIndex);
            clientStart = grid.GetCell(StartPointsInfo.ClientIndex);

            gameUI.myUnit = clientUnit;
            gameUI.otherUnit = serverUnit;
        }

        grid.AddUnit(serverUnit, serverStart);
        grid.AddUnit(clientUnit, clientStart);
    }

    [PunRPC]
    void SendStart(bool synced, int serverIndex, int clientIndex)
    {
        StartPointsInfo.Synced = synced;
        StartPointsInfo.ServerIndex = serverIndex;
        StartPointsInfo.ClientIndex = clientIndex;
    }
}
