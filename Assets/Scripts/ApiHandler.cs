using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ApiHandler : MonoBehaviour
{
    public static Uri ApiUri { get; } = new Uri(PlayerPrefs.GetString("apiUri"));
    static readonly HttpClient client = new HttpClient() { BaseAddress = ApiUri };
    public static ApiHandler _instance;
    public static ApiHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ApiHandler>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("ApiHandler");
                    _instance = container.AddComponent<ApiHandler>();
                    
                }
            }
            return _instance;
        }
    }

    public static string GetAnimationsSummaryOld()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(ApiUri, "animation"));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        var responseCode = response.StatusCode;
        var headers = response.Headers;
        var streamReader = new StreamReader(response.GetResponseStream());
        var jsonResponse = streamReader.ReadToEnd();
        return "LOL";
    }

    public static async Task<string> GetAnimationsSummary()
    {
        var answer = client.GetAsync("animation");
        var result = answer.Result;
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadAsStringAsync();
    }

    public static async Task<string> StopAnimation()
    {
        var answer = client.GetAsync("animation?key=stop");
        var result = answer.Result;
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadAsStringAsync();
    }

    public static async Task<string> ContinueAnimation()
    {
        var answer = client.GetAsync("animation?key=continue");
        var result = answer.Result;
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadAsStringAsync();
    }

    public static void CheckAnimationDataAvailability()
    {
        using (var client = new HttpClient() { BaseAddress = ApiUri })
        {
            var answer = client.GetAsync("/animation").Result;
        }
    }
}
