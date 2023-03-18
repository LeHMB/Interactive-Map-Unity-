using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using Mono.Cecil;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MarkerObject : ImageImporter
{
    public Vector2 pixelWordPos;
    public Image marker;
    public TMP_Text headline;
    public TMP_Text text;
    public RawImage image;



    private static Camera cam;
    private RectTransform rectTrans;

    private RectTransform RectTrans
    {
        get
        {
            return rectTrans ? rectTrans : rectTrans = this.GetComponent<RectTransform>();
        }
    }

    private static Camera Cam
    {
        get
        {
            return cam ? cam : cam = Camera.main;
        }
    }

    private Vector2 PixelWordPos
    {
        get 
        { 
            return pixelWordPos - new Vector2(MarkerController.Instance.image.width/2, MarkerController.Instance.image.height / 2); 
        }
    }

    public string type
    {
        set
        {            

            if(MarkerController.Instance.markerTypes != null && MarkerController.Instance.markerTypes.ContainsKey(value))
            {
                this.marker.sprite = MarkerController.Instance.markerTypes[value];

                // set anchor
                Vector2 anchor = this.marker.sprite.pivot / this.marker.sprite.rect.size;
                RectTransform markerRectTrans = this.marker.GetComponent<RectTransform>();
                markerRectTrans.anchoredPosition = new Vector2(markerRectTrans.anchoredPosition.x, markerRectTrans.rect.height * anchor.y);
            }
        }
    }

    public Color32 markerColor
    {
        set
        {
            this.marker.color = value;
        }
    }

    public string descriptionImagePath
    {
        set
        {
            // TODO: I had no time to implement a better solution, please fix this!!
            ImportImage(@"Images\" + value, delegate 
            {
                if (isLoaded)
                {
                    this.image.texture = this.texture;
                }
                else this.image.gameObject.SetActive(false);
            });


        }
    }



    // Update is called once per frame
    void Update()
    {

        //RectTrans.anchoredPosition = Cam.WorldToScreenPoint(PixelWordPos);

        // The world center is the center of the map. "PixelWordPos" gives the offset to that center 
        // Screenpoint for 1920x1080 * ScaleFactor
        RectTrans.anchoredPosition = Cam.WorldToScreenPoint(PixelWordPos) * (1080f / Screen.height);
    }
}
