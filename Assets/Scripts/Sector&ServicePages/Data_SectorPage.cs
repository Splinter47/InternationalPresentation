using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data_SectorPage : Data_General {
	
	public Feed_SectorPage theFeed; 

	//prefab labels
	public int id;
	public GameObject nameObj;
	public string desc;

	public string image;

	public override void DestroyTextures(){
	}
	
	public override void Create(){
		//save images if saving data
		saveImagesLocally = theFeed.saveDataLocally;

		//set labels
		id = int.Parse(data[0]);
		SetLabel(nameObj, data[1]);
		desc = data[2];

		image = data[3];

		//now done from feed
		/*
		if(image.Length > 1){
			int splitIndex = image.LastIndexOf("/");
			string imagePath = image.Substring(0, splitIndex);
			string imageName = image.Substring(splitIndex);
			//download image
			StartCoroutine(LoadLocalTexture(imagePath, imageName, AfterDownload, 0));
		}*/
	}

	public void OpenDetails(){
		theFeed.ShowDetails(this);
	}
}
