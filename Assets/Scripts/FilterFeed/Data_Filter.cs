using UnityEngine;
using System.Collections;

public class Data_Filter : Data_General {

	public Feed_Filter theFeed; 
	
	//prefab labels
	public string name;
	public string slug;

	public GameObject nameObj;

	//is the filter active
	private bool isOn = false;

	public override void DestroyTextures(){
	}
	
	public override void Create(){

		name = data[0];
		slug = data[1];

		SetLabel(nameObj, name.ToUpper());
	}

	public void ToggleFilter(){
		isOn = !isOn;

		theFeed.ToggleFilter(this, isOn);
	}
}
