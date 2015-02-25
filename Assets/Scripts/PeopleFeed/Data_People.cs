using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data_People : Data {
	
	public Feed_People theFeed; 

	//prefab labels
	public int id;
	public GameObject nameObj;
	public GameObject job_titleObj;
	public string qualAcademic;
	public string qualProf;
	public string description;
	public string image;
	public string office;

	public Texture2D imageTex;
	public GameObject imageObj;

	public TweenAlpha twA;
	public TweenRotation twR;
	public TweenColor twC;

	public override void DestroyTextures(){
		Destroy(imageTex);
	}
	
	public override void Create(){
		//save images if saving data
		saveImagesLocally = theFeed.saveDataLocally;

		//set labels
		id = int.Parse(data["id"]);
		SetLabel(nameObj, data["salutation"] + " " + data["firstname"] + " " + data["surname"]);
		SetLabel(job_titleObj, data["job_title"]);

		//store strings
		qualAcademic = data["qualAcademic"];
		qualProf = data["qualProf"];
		description = data["jobLong"];
		office = data["office"];

		image = data["image"];

		//split image string into path and fileName
		if(image.Length > 1){
			int splitIndex = image.LastIndexOf("/");
			string imagePath = image.Substring(0, splitIndex);
			string imageName = image.Substring(splitIndex);
			//download image
			StartCoroutine(LoadLocalTexture(imagePath, imageName, AfterDownload));
			//StartCoroutine(DownloadTexture(imagePath, imageName, AfterDownload, 0));
		}

		//create taxonomy list
		foreach(string i in data["terms"].Split(',')){
			taxonomies.Add(i);
		}
	}

	public void AfterDownload(Texture2D newTex){
		imageTex = newTex;

		if(imageTex != null){
			UITexture thumb = imageObj.GetComponent<UITexture>();
			thumb.mainTexture = newTex;
		}
	}

	public void OpenDetails(){
		theFeed.ShowDetails(this);
	}

	public bool ContainsFilter(List<string> filterBy){
		
		bool doesNotContain = true;
		foreach(string filter in filterBy){
			if(taxonomies.Contains(filter)){
				doesNotContain &= false;
			}
		}
		return !doesNotContain;
	}
}
