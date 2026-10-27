using System;
using System.Collections;
using UnityEngine.Networking;

// Plain result of an HTTP call, so nothing else needs to know about UnityWebRequest.
public class WebResponse
{
    public bool IsSuccess;
    public long StatusCode;
    public string Body;
    public string Error;
}

// How HTTP GET requests are sent. Swap for a fake in tests or a different HTTP library later.
public interface IWebRequestSender
{
    IEnumerator Get(string url, Action<WebResponse> onComplete);
}

public class UnityWebRequestSender : IWebRequestSender
{
    private readonly int _timeoutSeconds;

    public UnityWebRequestSender(int timeoutSeconds = 15)
    {
        _timeoutSeconds = timeoutSeconds;
    }

    public IEnumerator Get(string url, Action<WebResponse> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = _timeoutSeconds;

            yield return request.SendWebRequest();

            onComplete?.Invoke(new WebResponse
            {
                IsSuccess = request.result == UnityWebRequest.Result.Success,
                StatusCode = request.responseCode,
                Body = request.downloadHandler != null ? request.downloadHandler.text : null,
                Error = request.error
            });
        }
    }
}
