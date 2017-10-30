using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : MonoBehaviour {

	public GameObject ContentPanel;
	public GameObject ListItemPrefab;

	ArrayList Items;

	// Use this for initialization
	void Start () {
		Items = new ArrayList () {
			new Barang ("asd.txt", "asem"),
			new Barang ("Abc.txt", "aso"),
			new Barang ("adem.txt", "abc")
		};

		foreach(Barang temp in Items) {
			Debug.Log (temp.txt);
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
