using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HexUnit : MonoBehaviour
{
    public static HexUnit serverPrefab;
    public static HexUnit clientPrefab;
    public Animator anim;
    public AudioSource Walk;

    public bool Owned { get; set; }

    public HexGrid Grid { get; set; }

    public int VisionRange
    {
        get
        {
            return 3;
        }
    }

    List<HexCell> pathToTravel;
    const float travelSpeed = 4f;
    const float rotationSpeed = 180f;
    public bool isTraveling;

    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                Grid.DecreaseVisibility(location, VisionRange);
                location.Unit = null;
            }
            location = value;
            value.Unit = this;
            if (Owned)
            {
                Grid.IncreaseVisibility(value, VisionRange);
            }
            transform.localPosition = value.Position;
        }
    }
    HexCell location, currentTravelLocation;

    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }
    float orientation;

    public int Speed
    {
        get
        {
            return speed + defaultSpeed;
        }
    }
    int speed;
    int defaultSpeed = 30;

    public Renderer unitRenderer;

    public int Score { get; set; }
    public bool HasKey { get { return hasKey; } }
    bool hasKey;
    public bool HasTreasure { get { return hasTreasure; } }
    bool hasTreasure;
    public List<int> speedPlus = new List<int>();
    public List<int> speedMinus = new List<int>();

    public UIController ui;

    void Awake()
    {
        unitRenderer = GetComponentInChildren<Renderer>();
    }

    void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;
            if (currentTravelLocation && Owned)
            {
                Grid.IncreaseVisibility(location, VisionRange);
                Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
                currentTravelLocation = null;
            }
        }
    }

    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    public void Die()
    {
        if (location && Owned)
        {
            Grid.DecreaseVisibility(location, VisionRange);
        }
        location.Unit = null;
        Destroy(gameObject);
    }

    public void Travel(List<HexCell> path, bool costSpeed = false)
    {
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        if (costSpeed)
        {
            speed -= location.Distance;
        }
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }

    IEnumerator TravelPath()
    {
        isTraveling = true;
        anim.SetBool("Run", true);
        Walk.Play(0);

        Vector3 a, b, c = pathToTravel[0].Position;
        yield return LookAt(pathToTravel[1].Position);
        if (Owned)
        {
            Grid.DecreaseVisibility(currentTravelLocation ? currentTravelLocation : pathToTravel[0], VisionRange);
        }

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            currentTravelLocation = pathToTravel[i];
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + currentTravelLocation.Position) * 0.5f;

            if (Owned)
            {
                Grid.IncreaseVisibility(currentTravelLocation, VisionRange);
                if (currentTravelLocation.Item)
                {
                    currentTravelLocation.Item.Effect(this);
                    Grid.SendRemoveItem(currentTravelLocation.Item);
                }
            }
            else
            {
                unitRenderer.enabled = currentTravelLocation.IsVisible;
            }

            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            if (Owned)
            {
                Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
            }

            t -= 1f;
        }
        currentTravelLocation = null;

        a = c;
        b = location.Position;
        c = b;

        if (Owned)
        {
            Grid.IncreaseVisibility(location, VisionRange);
        }

        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = location.Position;
        orientation = transform.localRotation.eulerAngles.y;

        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;

        isTraveling = false;
        anim.SetBool("Run", false);
        Walk.Stop();
    }

    IEnumerator LookAt(Vector3 point)
    {
        anim.Play("Rotate", -1, 0f);
        anim.SetBool("Rotate", true);

        point.y = transform.localPosition.y;

        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation = Quaternion.LookRotation(point - transform.localPosition);

        float angle = Quaternion.Angle(fromRotation, toRotation);
        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;
            for (float t = Time.deltaTime * speed; t < 1f; t += Time.deltaTime * speed)
            {
                transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }

        transform.LookAt(point);
        orientation = transform.localRotation.eulerAngles.y;

        anim.SetBool("Rotate", false);
    }

    public bool IsValidDestination(HexCell cell, bool limitSearch = true)
    {
        if (limitSearch)
        {
            if (cell.Item)
            {
                if (!cell.Item.isWalkable(this))
                {
                    return false;
                }
            }
            return cell.IsExplored && !cell.IsUnderwater && !cell.Unit && cell.IsWalkable;
        }
        else
        {
            return !cell.IsUnderwater && !cell.Unit && cell.IsWalkable;
        }
    }

    public int GetMoveCost(HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        if (edgeType == HexEdgeType.Cliff)
        {
            return -1;
        }

        int moveCost;
        moveCost = edgeType == HexEdgeType.Flat ? 1 : 2;

        moveCost += toCell.terrainType.GetMoveCost();

        return moveCost;
    }

    public void Jump()
    {
        anim.Play("Jump", -1, 0f);
    }

    public void SetSpeed()
    {
        int delta = 0;
        if (speedPlus.Count > 0)
        {
            delta += speedPlus[0];
            speedPlus.RemoveAt(0);
        }
        if (speedMinus.Count > 0)
        {
            delta -= speedMinus[0];
            speedMinus.RemoveAt(0);
        }
        speed = delta;
    }

    public void SetZeroSpeed()
    {
        speed = -defaultSpeed;
    }

    public void setKey(bool key)
    {
        hasKey = key;
        if (key)
        {
            ui.GetKey();
        }
        else
        {
            ui.LoseKey();
        }
    }

    public void getTreasure()
    {
        hasTreasure = true;
    }

    public void speedEffect(int speed, int turns)
    {
        if (speed > 0)
        {
            for (int i = 0; i < turns; i++)
            {
                speedPlus.Add(speed);
            }
        }
        else if (speed < 0)
        {
            for (int i = 0; i < turns; i++)
            {
                speedMinus.Add(-speed);
            }
        }
    }

    public void getItem(HexItemType type)
    {
        ui.GetItem(type);
    }
}
