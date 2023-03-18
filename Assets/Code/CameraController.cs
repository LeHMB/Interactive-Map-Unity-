using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton
    private static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            return instance ? instance : instance = FindObjectOfType<CameraController>();
        }
    }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion



    private Camera cam;
    private float imageReferenceScaling;
    private float targetZoomLevel;    
    private float lerpValue;

    private Vector2 dragOrigin;

    public float baseZoom;
    public float zoomStrengh;
    public Vector2 zoomLimits;
    public Vector2 boundaries;

    private Camera Cam
    {
        get
        {
            return cam ? cam : cam = Camera.main;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        imageReferenceScaling = Mathf.Max(MarkerController.Instance.image.width, MarkerController.Instance.image.height) / 2;
        Cam.orthographicSize = imageReferenceScaling / baseZoom;

        targetZoomLevel = Cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        bool mouseOnScreen = MouseOnScreen();

        // Zoom
        float deltaZoom = Input.GetAxis("Mouse ScrollWheel");

        if (deltaZoom != 0 && mouseOnScreen)
        {
            targetZoomLevel -= deltaZoom * (zoomStrengh * imageReferenceScaling);
            Vector2 clampMinMax = new Vector2(imageReferenceScaling / zoomLimits.x, imageReferenceScaling / zoomLimits.y);
            targetZoomLevel = Mathf.Clamp(targetZoomLevel, clampMinMax.x, clampMinMax.y);

            lerpValue = 0;
        }

        lerpValue += Time.deltaTime;

        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, targetZoomLevel, lerpValue);


        // Drag
        if (mouseOnScreen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Cam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 delta = dragOrigin - (Vector2)Cam.ScreenToWorldPoint(Input.mousePosition);

                Vector3 newPosition = transform.position;
                Vector2 clampMinMax = boundaries * imageReferenceScaling;
                newPosition.x = Mathf.Clamp(newPosition.x + delta.x, -clampMinMax.x, clampMinMax.x);
                newPosition.y = Mathf.Clamp(newPosition.y + delta.y, -clampMinMax.y, clampMinMax.y);

                cam.transform.position = newPosition;
            }
        }
    }

    private bool MouseOnScreen()
    {
        var view = Cam.ScreenToViewportPoint(Input.mousePosition);
        return (view.x >= 0 && view.x <= 1) && (view.y >= 0 && view.x <= 1);
    }
}
