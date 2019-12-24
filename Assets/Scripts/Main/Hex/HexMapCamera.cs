using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    static HexMapCamera instance;

    public HexGrid grid;

    Transform swivel, stick;

    float zoom = 1f;

    public float stickMinZoom, stickMaxZoom;

    public float swivelMinZoom, swivelMaxZoom;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    float rotationAngle;

    public float rotationSpeed;

    bool stillUpdating;
    Vector3 originalPosition, targetPosition;
    float update;

    bool stillZooming;
    float originalZoom, targetZoom;
    float zoomUpdate;

    void Awake()
    {
        instance = this;
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    void Update()
    {
        if (stillZooming)
        {
            zoomUpdate += Time.deltaTime * 2 / Mathf.Abs(targetZoom - originalZoom);
            if (zoomUpdate >= 1)
            {
                zoom = targetZoom;
                stillZooming = false;
            }
            else
            {
                zoom = Mathf.SmoothStep(originalZoom, targetZoom, zoomUpdate);
                ValidatePosition();
            }
        }
        else if (stillUpdating)
        {
            update += Time.deltaTime * 230f / (Vector3.Distance(originalPosition, targetPosition));
            if (update >= 1)
            {
                transform.localPosition = targetPosition;
                stillUpdating = false;

                stillZooming = true;
                zoomUpdate = 0f;
                originalZoom = zoom;
                targetZoom = 0.7f;
            }
            else
            {
                transform.localPosition = Vector3.Slerp(originalPosition, targetPosition, update);
            }
        }
        else
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f)
            {
                AdjustZoom(zoomDelta);
            }

            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f)
            {
                AdjustRotation(rotationDelta);
            }
        }
    }

    public void Move()
    {
        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.SmoothStep(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.SmoothStep(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMin = (grid.border - 0.5f) * (2f * HexMetrics.innerRadius);
        float xMax = (grid.cellCountX - grid.border - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, xMin, xMax);

        float zMin = (grid.border - 1) * (1.5f * HexMetrics.outerRadius);
        float zMax = (grid.cellCountZ - grid.border - 1) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, zMin, zMax);

        return position;
    }

    public void CenterPosition()
    {
        transform.localPosition = new Vector3((grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius) / 2, 0f, (grid.cellCountZ - 1f) * (1.5f * HexMetrics.outerRadius) / 2);
        zoom = 0f;

        ValidatePosition();
    }

    public static void ValidatePosition()
    {
        instance.AdjustZoom(0f);
        instance.AdjustRotation(0f);
        instance.AdjustPosition(0f, 0f);
    }

    public void SetPosition(HexCell cell)
    {
        SetPosition(cell.Position, false, true);
    }

    public void SetPosition(Vector3 position, bool raw = false, bool zoomUp = true)
    {
        if (raw)
        {
            transform.localPosition = position;
        }
        else
        {
            stillUpdating = true;
            update = 0f;
            originalPosition = transform.localPosition;
            targetPosition = position;

            if (zoomUp)
            {
                stillZooming = true;
                zoomUpdate = 0f;
                originalZoom = zoom;
                targetZoom = 0f;
            }
        }

    }

    public float GetRotationAngle()
    {
        return instance.rotationAngle;
    }
}
