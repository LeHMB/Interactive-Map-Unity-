using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Timeline;
using UnityEngine.UI;



public class MarkerController : ImageImporter
{
    #region Singleton
    private static MarkerController instance;
    public static MarkerController Instance
    {
        get
        {
            return instance ? instance : instance = FindObjectOfType<MarkerController>();
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

        //Serialize<Config>(test, Application.dataPath + @"\Config\config.xml");

#if UNITY_EDITOR
        LoadConfig(Application.dataPath + @"\Config\config.xml");
        LoadMarkers(Application.dataPath + @"\Config\markers.xml");
#else
        StartCoroutine(ImportConfig(@"Config\config.xml"));
        StartCoroutine(ImportMarkers(@"Config\markers.xml"));        
#endif
    }
    #endregion


    public Texture2D image;
    public SpriteRenderer mapRenderer;

    public GameObject content;

    [HideInInspector] public Dictionary<string, Sprite> markerTypes;



    // Experimental reasons
    public static void Serialize<T>(T data, string path)
    {
        XmlSerializer serializer = new XmlSerializer(data.GetType());
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer.BaseStream, data);
        writer.Close();
    }

    public static T DeserializeReader<T>(StreamReader reader)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));        
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }

    public static T DeserializeStream<T>(MemoryStream stream)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        T deserialized = (T)serializer.Deserialize(stream);
        stream.Close();
        return deserialized;
    }

    void LoadMarkers(string path)
    {
        List<Marker> markers = DeserializeReader<List<Marker>>(new StreamReader(path));

        CreateMarkers(markers);
    }

    IEnumerator ImportMarkers(string path)
    {
        //string path = "Config/markers.xml"; //This works because index.html is in the same folder as StreamingAssets ?
        UnityWebRequest uwr = UnityWebRequest.Get(path);
        yield return uwr.SendWebRequest();

        List<Marker> markers = DeserializeStream<List<Marker>>(new MemoryStream(uwr.downloadHandler.data));

        CreateMarkers(markers);
    }

    private void CreateMarkers(in List<Marker> markers)
    {
        foreach (Marker marker in markers)
        {
            GameObject gameobj = Instantiate(Resources.Load("Marker") as GameObject);
            gameobj.transform.SetParent(this.content.transform, false);

            MarkerObject markerObj = gameobj.GetComponent<MarkerObject>();
            markerObj.pixelWordPos = marker.Position;
            markerObj.type = marker.Type;
            markerObj.markerColor = marker.Color;
            markerObj.headline.text = marker.Headline;
            markerObj.text.text = marker.Text;
            markerObj.text.ForceMeshUpdate(false, true);
            markerObj.descriptionImagePath = marker.ImagePath;
        }
    }


    void LoadConfig(string path)
    {
        Config config = DeserializeReader<Config>(new StreamReader(path));

        ApplyConfig(config);
    }

    IEnumerator ImportConfig(string path)
    {
        //string path = "Config/markers.xml"; //This works because index.html is in the same folder as StreamingAssets ?
        UnityWebRequest uwr = UnityWebRequest.Get(path);
        yield return uwr.SendWebRequest();

        Config config = DeserializeStream<Config>(new MemoryStream(uwr.downloadHandler.data));

        ApplyConfig(config);
    }

    void ApplyConfig(Config config)
    {
        markerTypes = new Dictionary<string, Sprite>();
        if (config.Icons != null)
        {
            foreach (MarkerIcon icon in config.Icons)
            {
                // TODO: I had no time to implement a better solution, please fix this!!
                ImportImage(@"Images\" + icon.Filename, delegate
                {
                    if (isLoaded)
                    {
                        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), icon.Anchor, 1);
                        markerTypes.Add(icon.Typename, sprite);
                    }
                });
            }
        }

        // TODO: I had no time to implement a better solution, please fix this!!
        ImportImage(@"Images\" + config.Mapname, delegate
        {
            if (isLoaded)
            {
                image = texture;
                Sprite map = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
                mapRenderer.sprite = map;
            }
        });


        CameraController.Instance.baseZoom = config.BaseZoom;
        CameraController.Instance.zoomStrengh = config.ZoomStrengh;
        CameraController.Instance.zoomLimits = config.ZoomLimits;
        CameraController.Instance.boundaries = config.Boundaries;
    }

}
