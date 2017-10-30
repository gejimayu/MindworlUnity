using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;

public class ListController : MonoBehaviour {

	public GameObject ContentPanel;
	public GameObject ListItemPrefab;
	public string textsUrl = "https://mindworld-4964e.firebaseio.com/text.json";
	public JSONObject textsNode;
	public JSONObject imagesNode;
	ArrayList ItemsText;

	// Use this for initialization
	IEnumerator Start () {
		//Items = new ArrayList () {
			//new Barang ("asd.txt", "asem"),
			//new Barang ("Abc.txt", "aso"),
			//new Barang ("adem.txt", "abc")
		//};

		ItemsText = new ArrayList ();
		WWW www = new WWW(textsUrl);
		yield return www;

		textsNode = (JSONObject) JSON.Parse (www.text);

		foreach (string key in textsNode.Keys) {
			Debug.Log (textsNode[key]["title"]);
			WWW temp = new WWW (textsNode [key] ["url"]);
			yield return temp;
			ItemsText.Add(new BarangTxt(textsNode[key]["title"], temp.text));
		}

		foreach(BarangTxt temp in ItemsText) {
			//Debug.Log (temp.txt);
			GameObject newItem = Instantiate (ListItemPrefab) as GameObject;
			ListItemController controller = newItem.GetComponent<ListItemController>();
			controller.content.text = temp.txt;
			controller.fname.text = temp.filename;
			newItem.transform.SetParent (ContentPanel.transform, false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
