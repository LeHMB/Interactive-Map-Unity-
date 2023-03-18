using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.Events;

public class ImageImporter : MonoBehaviour
{
    protected Texture2D texture;
    protected bool isLoaded = false;

    protected void ImportImage(string path, UnityAction finalizer)
    {
#if UNITY_EDITOR
        Load(path);
        finalizer.Invoke();
#else
        StartCoroutine(Import(path, finalizer));
#endif
    }

    private void Load(string path)
    {
        string fullPath = Application.dataPath + "\\" + path;
        if (File.Exists(fullPath))
        {
            byte[] byteArray = File.ReadAllBytes(fullPath);
            texture = new Texture2D(2, 2);
            isLoaded = texture.LoadImage(byteArray);
        }
        else isLoaded = false;
    }


    private IEnumerator Import(string path, UnityAction finalizer)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(path);
        yield return uwr.SendWebRequest();
        //while (!uwr.SendWebRequest().isDone) ;

        byte[] byteArray = uwr.downloadHandler.data;
        texture = new Texture2D(2, 2);
        isLoaded = texture.LoadImage(byteArray);

        finalizer.Invoke();
    }
}
