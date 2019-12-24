using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Photon.Pun;

public class HexGameUI : MonoBehaviour
{
    PhotonView photonView;

    public HexGrid grid;

    HexCell currentCell;
    public HexUnit myUnit;
    public HexUnit otherUnit;
    bool selected;

    public HexMapCamera mapCamera;
    bool following, switchToFollowing;
    HexCell followedLastCell;

    public static Color selectedColor = new Color(0, 40, 70);
    public static Color pathColor = Color.white;
    public static Color toColor = new Color(255, 180, 0);
    public static Color unableColor = Color.red;

    void Awake()
    {
        photonView = PhotonView.Get(this);
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (selected && !myUnit.isTraveling)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                else
                {
                    DoPathfinding();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            DoSelection();
        }

        if (Input.GetKeyDown("space"))
        {
            if (selected)
            {
                myUnit.Jump();

                photonView.RPC("SendJump", RpcTarget.Others, true);
            }
        }

        // center camera to myUnit position
        if (Input.GetKeyDown(KeyCode.C))
        {
            mapCamera.SetPosition(myUnit.Location);
        }

        // toggle camera following
        if (Input.GetKeyDown(KeyCode.V))
        {
            following = !following;
            if (following)
            {
                mapCamera.SetPosition(myUnit.Location);
                switchToFollowing = true;
            }
        }
        if (switchToFollowing)
        {
            if (mapCamera.transform.localPosition == myUnit.transform.localPosition)
            {
                switchToFollowing = false;
            }
        }
        else if (following)
        {
            mapCamera.SetPosition(myUnit.transform.localPosition, true);
        }
        else
        {
            mapCamera.Move();
        }

        // networking
        if (UnitInfo.newPath)
        {
            DoMove(UnitInfo.Path);
            UnitInfo.newPath = false;
        }

        if (UnitInfo.Jump)
        {
            otherUnit.Jump();
            UnitInfo.Jump = false;
        }
    }

    bool UpdateCurrentCell()
    {
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    void DoSelection()
    {
        grid.ClearPath();
        // UpdateCurrentCell();
        selected = !selected;
        // if (currentCell)
        // {
        //     selected = currentCell.Unit && currentCell.Unit.Owned;
        //     if (selected)
        //     {
        //         currentCell.EnableHighlight(selectedColor);
        //     }
        //     else
        //     {
        //         currentCell.DisableHighlight();
        //     }
        // }
    }

    void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && myUnit.IsValidDestination(currentCell))
            {
                grid.FindPath(myUnit.Location, currentCell, myUnit);
            }
            else
            {
                grid.ClearPath();
            }
        }
    }

    void DoMove()
    {
        if (grid.HasPath)
        {
            List<HexCell> path = grid.GetPath();
            SendPath(path);
            myUnit.Travel(path);
            grid.ClearPath();
        }
    }

    void DoMove(int[] p)
    {
        List<HexCell> path = new List<HexCell>();
        for (int i = 0; i < p.Length; i++)
        {
            path.Add(grid.GetCell(p[i]));
        }

        otherUnit.Travel(path);
    }


    void SendPath(List<HexCell> p)
    {
        int[] path = new int[p.Count];

        for (int i = 0; i < p.Count; i++)
        {
            path[i] = p[i].Index;
        }

        photonView.RPC("SendPath", RpcTarget.Others, true, path);
    }


    [PunRPC]
    void SendPath(bool synced, int[] path)
    {
        UnitInfo.newPath = synced;
        UnitInfo.Path = path;
    }

    [PunRPC]
    void SendJump(bool jump)
    {
        UnitInfo.Jump = jump;
    }
}
