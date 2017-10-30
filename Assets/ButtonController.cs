using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {
	public Button button;
	public Text nameLabel;

	private Item item;
	private ListOfFile listOfFile;

	// Use this for initialization
	void Start () {
		
	}

	public void Setup(Item currentItem, ListOfFile currentListOfFile){
		item = currentItem;
		nameLabel.text = item.itemName;
		listOfFile = currentListOfFile;
	}
}
