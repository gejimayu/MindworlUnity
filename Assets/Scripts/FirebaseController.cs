using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;

public class FirebaseController : MonoBehaviour {
	public string textsUrl = "https://mindworld-4964e.firebaseio.com/text.json";
	public string imagesUrl = "https://mindworld-4964e.firebaseio.com/image.json";

	public JSONObject textsNode;
	public JSONObject imagesNode;

	// Use this for initialization
	IEnumerator Start () {
		WWW www = new WWW(imagesUrl);
		yield return www;

		imagesNode = (JSONObject) JSON.Parse (www.text);

		Debug.Log (imagesNode["-KxYBoreIhvzjez5B2lG"]["location"]);

		foreach (string key in imagesNode.Keys) {
			Debug.Log (imagesNode[key]);
		}
	}
}
