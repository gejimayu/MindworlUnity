using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SimpleJSON;

public class ListController2 : MonoBehaviour {

	public GameObject ContentPanel;
	public GameObject ListItemPrefab;
	public string imagesUrl = "https://mindworld-4964e.firebaseio.com/image.json";
	public JSONObject imagesNode;
	ArrayList ItemsImage;

	// Use this for initialization
	IEnumerator Start () {
		//Items = new ArrayList () {
		//new Barang ("asd.txt", "asem"),
		//new Barang ("Abc.txt", "aso"),
		//new Barang ("adem.txt", "abc")
		//};

		ItemsImage = new ArrayList ();
		WWW www = new WWW(imagesUrl);
		yield return www;

		imagesNode = (JSONObject) JSON.Parse (www.text);

		foreach (string key in imagesNode.Keys) {
			Debug.Log (imagesNode[key]["uploaderID"]);
			Debug.Log (PlayerPrefs.GetString("Email User"));
			if(imagesNode[key]["uploaderID"].Equals(PlayerPrefs.GetString("Email User"))){
				WWW temp = new WWW (imagesNode [key] ["url"]);
				yield return temp;
				//convert image to sprite
				Texture2D texture = new Texture2D (temp.texture.width, temp.texture.height, TextureFormat.DXT1, false);
				temp.LoadImageIntoTexture (texture);
				Rect rec = new Rect (0, 0, texture.width, texture.height);
				Sprite spriteToUse = Sprite.Create (texture, rec, new Vector2 (0.5f, 0.5f), 100);

				ItemsImage.Add(new BarangImg(imagesNode[key]["name"], spriteToUse));
			}
		}

		foreach(BarangImg temp in ItemsImage) {
			//Debug.Log (temp.txt);
			GameObject newItem = Instantiate (ListItemPrefab) as GameObject;
			ListItemController2 controller = newItem.GetComponent<ListItemController2>();
			controller.content.sprite = temp.img;
			controller.fname.text = temp.filename;
			newItem.transform.SetParent (ContentPanel.transform, false);
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
