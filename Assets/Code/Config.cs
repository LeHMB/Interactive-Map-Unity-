using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct Config
{
    public string Mapname;

    public List<MarkerIcon> Icons;
    public float BaseZoom;
    public float ZoomStrengh;
    public Vector2 ZoomLimits;
    public Vector2 Boundaries;
}

[System.Serializable]
public struct MarkerIcon
{
    public string Typename;
    public string Filename;
    public Vector2 Anchor;
}

//public struct MarkerSprite
//{
//    MarkerSprite(Sprite sprite, Vector2 anchor)
//    {
//        this.sprite = sprite;
//        this.anchor = anchor;
//    }

//    public Sprite sprite;
//    public Vector2 anchor;
//}