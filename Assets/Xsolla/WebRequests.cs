using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
    static class WebRequests
    {
        public static IEnumerator PostRequest(string url, WWWForm form, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, form);

#if UNITY_2018_1_OR_NEWER
            yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
            if (request.isNetworkError && callback != null)
            {
                callback.Invoke(false, request.error);
            }
            else if (callback != null)
            {
                callback.Invoke(true, request.downloadHandler.text);
            }
        }
        
        public static IEnumerator PostRequest(string url, string jsonData, Action<bool, string> callback = null)
        {
	        var request = new UnityWebRequest(url, "POST");
	        
	        request.downloadHandler = new DownloadHandlerBuffer();
			
	        if (!string.IsNullOrEmpty(jsonData))
	        {
		        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
	        }
	        
	        request.SetRequestHeader("Content-Type", "application/json");

#if UNITY_2018_1_OR_NEWER
	        yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
	        if (request.isNetworkError && callback != null)
	        {
		        callback.Invoke(false, request.error);
	        }
	        else if (callback != null)
	        {
		        callback.Invoke(true, request.downloadHandler.text);
	        }
        }
        
        
        public static IEnumerator GetRequest(string uri, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(uri);

#if UNITY_2018_1_OR_NEWER
            yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
            if (request.isNetworkError && callback != null)
            {
                callback.Invoke(false, request.error);
            }
            else if (callback != null)
            {
                callback.Invoke(true, request.downloadHandler.text);
            }
        }
    }

}