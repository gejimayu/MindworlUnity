using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item{
	public string itemName;

	public Item(string name){
		itemName = name;
	}
}

public class ListOfFile : MonoBehaviour {
	public List<Item> itemList = new List<Item>();

	public Transform contentPanel;
	public SimpleObjectPool buttonObjectPool;


	// Use this for initialization
	void Start () {
		itemList.Add(new Item ("cacad"));

		RefreshDisplay ();
	}

	public void RefreshDisplay(){
		AddButtons ();
	}

	private void AddButtons(){
		for (int i = 0; i < itemList.Count; i++) {
			Debug.Log (itemList.Count);
			Item item = itemList [i];
			GameObject newButton = buttonObjectPool.GetObject ();
			newButton.transform.SetParent (contentPanel, false);

			ButtonController sampleButton = newButton.GetComponent<ButtonController> ();
			sampleButton.Setup (item, this);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
