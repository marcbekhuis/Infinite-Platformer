using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkManager
{
    private static string apiUrl = "https://marcbekhuis.nl/platformer/api/";

    public delegate void Response(string date);

    public static IEnumerator Post(string api, WWWForm form, Response response)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(apiUrl + api, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                response(webRequest.downloadHandler.text);
            }
        }
    }

    public static IEnumerator Get(string api, Response response)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl + api))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                response(webRequest.downloadHandler.text);
            }
        }
    }
}
