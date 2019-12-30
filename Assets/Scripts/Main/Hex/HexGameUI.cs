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
            if (HexGameController.myTurn && !myUnit.isTraveling)
            {
                DoPathfinding();
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
            }
            else
            {
                currentCell = null;
            }
        }

        if (Input.GetKeyDown("space"))
        {
            myUnit.Jump();
            photonView.RPC("GetJump", RpcTarget.Others, true);
        }

        // center camera to myUnit position
        if (Input.GetKeyDown(KeyCode.C))
        {
            HexMapCamera.SetPosition(myUnit.Location);
        }

        // toggle camera following
        if (Input.GetKeyDown(KeyCode.V))
        {
            following = !following;
            if (following)
            {
                HexMapCamera.SetPosition(myUnit.Location);
                switchToFollowing = true;
            }
        }

        // move camera
        if (switchToFollowing)
        {
            if (HexMapCamera.GetLocalPosition() == myUnit.transform.localPosition)
            {
                switchToFollowing = false;
            }
        }
        else if (following)
        {
            HexMapCamera.SetPosition(myUnit.transform.localPosition, true);
        }
        else
        {
            HexMapCamera.Move();
        }

        // networking
        if (otherUnit)
        {
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
    }

    bool UpdateCurrentCell()
    {
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell && cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell)
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
            myUnit.Travel(path, true);
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

        photonView.RPC("GetPath", RpcTarget.Others, path);
    }


    [PunRPC]
    void GetPath(int[] path)
    {
        UnitInfo.newPath = true;
        UnitInfo.Path = path;
    }

    [PunRPC]
    void GetJump(bool jump)
    {
        UnitInfo.Jump = jump;
    }
}
