using System;
using UnityEngine;
using DG.Tweening;

public class CameraControllerTopDown : MonoBehaviour
{
    public static CameraControllerTopDown Instance;
    Vector3 deltaPos;
    Vector3 mStartPos;
    Vector3 mDeltaPos;
    Camera camera;
    float minZoomLevel = 10;
    float cameraMaxZoomLevel = 30;

    bool camInitialised = false;

    public bool CameraBoundByCenter = false;


    private void Awake() => Instance = this;

    public void SetCameraPos(Vector3 pos)
    {
        pos.z = camera.transform.position.z;
        camera.transform.DOMove(pos, 0.5f).SetEase(Ease.OutCubic);

    }
    void UpdateCameraMouse()
    {

        if (Input.GetMouseButtonDown(2))
        {
            camera.transform.DOKill();

            mStartPos = camera.ScreenToWorldPoint(Input.mousePosition);
            return;
        }
        if (Input.GetMouseButton(2))
        {
            mDeltaPos = camera.ScreenToWorldPoint(Input.mousePosition) - mStartPos;
            camera.transform.position -= mDeltaPos;
        }
        if (Input.GetMouseButtonUp(2))
        {
            mDeltaPos = (camera.ScreenToWorldPoint(Input.mousePosition) - mStartPos).normalized;
            camera.transform.DOMove((camera.transform.position - mDeltaPos), 0.5f).SetEase(Ease.OutExpo);
        }




        if (Input.mouseScrollDelta.y != 0 && !Input.GetKey(KeyCode.LeftControl))
        {

            // if (camera.orthographicSize + Input.mouseScrollDelta.y > minZoomLevel && camera.orthographicSize + Input.mouseScrollDelta.y < cameraMaxZoomLevel)
            {
                // Debug.Log(Input.mouseScrollDelta.y * camera.orthographicSize * 0.1f);
                // camera.DOOrthoSize(Mathf.Clamp(minZoomLevel, cameraMaxZoomLevel, camera.orthographicSize + Input.mouseScrollDelta.y * camera.orthographicSize * 0.2f), 0.5f);
                camera.DOOrthoSize(camera.orthographicSize + Input.mouseScrollDelta.y * camera.orthographicSize * 0.2f, 0.5f);
                // camera.orthographicSize += Input.mouseScrollDelta.y;
            }
        }
        // if (camera.orthographicSize >= cameraMaxZoomLevel) camera.orthographicSize = cameraMaxZoomLevel;
        // if (camera.orthographicSize <= minZoomLevel) camera.orthographicSize = minZoomLevel;

    }

    float cameraPanSpeed;
    void UpdateCameraWASD()
    {

        if (Input.GetKey(KeyCode.W))
        {
            camera.transform.DOKill();
            mDeltaPos = Vector3.up;
            camera.transform.position += mDeltaPos * cameraPanSpeed * camera.orthographicSize;

        }
        if (Input.GetKey(KeyCode.S))
        {
            camera.transform.DOKill();

            mDeltaPos = Vector3.down;
            camera.transform.position += mDeltaPos * cameraPanSpeed * camera.orthographicSize;

        }
        if (Input.GetKey(KeyCode.A))
        {
            camera.transform.DOKill();

            mDeltaPos = Vector3.left;
            camera.transform.position += mDeltaPos * cameraPanSpeed * camera.orthographicSize;

        }
        if (Input.GetKey(KeyCode.D))
        {
            camera.transform.DOKill();

            mDeltaPos = Vector3.right;
            camera.transform.position += mDeltaPos * cameraPanSpeed * camera.orthographicSize;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            mDeltaPos = Vector3.up;
            camera.transform.DOMove((camera.transform.position + mDeltaPos), 0.5f).SetEase(Ease.OutExpo);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            mDeltaPos = Vector3.down;
            camera.transform.DOMove((camera.transform.position + mDeltaPos), 0.5f).SetEase(Ease.OutExpo);

        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            mDeltaPos = Vector3.left;
            camera.transform.DOMove((camera.transform.position + mDeltaPos), 0.5f).SetEase(Ease.OutExpo);

        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            mDeltaPos = Vector3.right;
            camera.transform.DOMove((camera.transform.position + mDeltaPos), 0.5f).SetEase(Ease.OutExpo);

        }
    }


    float minX, minY, maxX, maxY;
    float vertExtent, horzExtent;
    void UpdateCameraBounds()
    {
        // //Camera bounds
        if (!camInitialised) return;

        vertExtent = camera.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;

        float mapX = MapController.Instance.MapW;
        float mapY = MapController.Instance.MapH;
        //float fraction = mapX / mapY;

        if (!CameraBoundByCenter)
        {

            minX = Mathf.Min(horzExtent, mapX * 0.5f); //horzExtent;//mapX * 0.5f - horzExtent;
            maxX = Mathf.Max(mapX - horzExtent, mapX * 0.5f); // - horzExtent;

            minY = vertExtent;
            maxY = mapY - vertExtent;
        }
        else
        {
            minX = 0;//mapX * 0.5f - horzExtent;
            maxX = mapX;
            minY = 0;
            maxY = mapY;
        }



        var v3 = camera.transform.position;
        if (v3.x < minX || v3.x > maxX || v3.y < minY || v3.y > maxY) camera.transform.DOKill();

        v3.x = Mathf.Clamp(v3.x, minX, maxX);
        v3.y = Mathf.Clamp(v3.y, minY, maxY);
        camera.transform.position = v3;
    }

    public void InitCamera()
    {
        camera = GetComponent<Camera>();

#if UNITY_EDITOR
        cameraPanSpeed = 0.01f;
#endif
#if !UNITY_EDITOR
                cameraPanSpeed = 0.1f;
#endif
        float fraction = MapController.Instance.MapW / MapController.Instance.MapH;
        if (fraction >= 1)
        {
            cameraMaxZoomLevel = MapController.Instance.MapH * 0.5f;
        }
        else
        {
            cameraMaxZoomLevel = MapController.Instance.MapW * 0.5f;
        }
        camInitialised = true;
    }

    private void Update()
    {
        if (!camInitialised) return;
        UpdateCameraMouse();
        UpdateCameraWASD();

        UpdateCameraBounds();
    }
}