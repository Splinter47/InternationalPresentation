using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data_Globe : Data {
	
	public Feed_Globe theFeed; 
	//private string imageUrl = "http://www.samdavies.info/InternationalPresentaion/ProjectImages/";
	
	public int id;

	//strings
	public string name;
	public string value;
	public string region;
	public string country;
	public string sector;
	public string contract;

	public string description;
	public string image;

	public float latitude;
	public float longatude;

	//objects
	public GameObject nameObj;
	public GameObject imageObj;

	//image
	public Texture2D imageTex;

	//touch point on the sphere
	public GameObject thePoint;
	public GlobeTouchPoint thePointScript;
	
	public override void DestroyTextures(){
		Destroy(imageTex);
	}
	
	public override void Create(){
		//save images if saving data
		saveImagesLocally = theFeed.saveDataLocally;

		//set labels
		id = int.Parse(data["id"]);
		name = data["title"];
		description = data["description"];
		region = data["region"];
		sector = data["sector"];
		//service = data["service"];
		country = data["country"];
		contract = data["contract_type"];
		image = data["image"];
		value = data["value"];

		//create taxonomy list
		foreach(string i in data["terms"].Split(',')){
			taxonomies.Add(i);
		}

		//set title
		SetLabel(nameObj, name);

		//split image string into path and fileName
		if(image.Length > 1){
			int splitIndex = image.LastIndexOf("/");
			string imagePath = image.Substring(0, splitIndex);
			string imageName = image.Substring(splitIndex);
			//download image
			StartCoroutine(LoadLocalTexture(imagePath, imageName, AfterDownload));
		}

		//create point on globe
		latitude = float.Parse(data["lat"]);
		longatude = -float.Parse(data["lng"]);
		addThePoint();
	}

	public void AfterDownload(Texture2D newTex){
		imageTex = newTex;

		//set thumb image
		UITexture thumb = imageObj.GetComponent<UITexture>();
		//thumb.mainTexture = GetCentredSubTexture(imageTex, thumb.width*8, thumb.height*8);
		thumb.mainTexture = newTex;

		//set point flag image
		//UITexture flag = thePointScript.image.GetComponent<UITexture>();
		//flag.mainTexture = GetCentredSubTexture(imageTex, flag.width*8, flag.height*8);
		//flag.mainTexture = imageTex;
	}

	public void OpenDetails(){
		theFeed.ShowDetails(this);

		thePointScript.RotateGlobeToThis();
	}

	private void addThePoint(){
		//create new point
		thePoint = NGUITools.AddChild (theFeed.pointOrigin, theFeed.pointPrefab);

		//rotate the point
		thePoint.transform.localRotation = Quaternion.Euler(latitude, longatude, 0);
		//thePoint.transform.Rotate(latitude, longatude, 0);

		//add refernece to this
		thePointScript = thePoint.GetComponentInChildren<GlobeTouchPoint>();
		thePointScript.dataLink = this;
		thePointScript.Create();

	}

}
