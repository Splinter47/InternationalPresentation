using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Feed_SectorPage : Feed_General {

	public GameObject detailWindow;
	public GameObject details1;

	public GameObject nameObj;
	public UIScrollView descScroll;
	public GameObject descObj;
	public GameObject imageObj;

	private Data_SectorPage data;
	private bool skipOnce = false;

	void Start(){
		grid = gridObject.GetComponent<UIGrid>();
		grid.Reposition();
		LoadLocalData();
		if(EnableAutoDownloads.isEnabled == true){
			Download();
		}
	}

	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_SectorPage>().theFeed = this;
	}

	protected override void LocalFilter(){
	}

	public void GetImage(Data_SectorPage data){
		//split image string into path and fileName
		if(data.image.Length > 1){
			int splitIndex = data.image.LastIndexOf("/");
			string imagePath = data.image.Substring(0, splitIndex);
			string imageName = data.image.Substring(splitIndex);
			//download image
			StartCoroutine(data.LoadLocalTexture(imagePath, imageName, AfterDownload, 0));
		}
	}

	public void AfterDownload(Texture2D newTex){
		if(skipOnce){
			Destroy(imageObj.GetComponent<UITexture>().mainTexture);
		}else{
			skipOnce = true;
		}

		imageObj.GetComponent<UITexture>().mainTexture = newTex;
	}
		
	public void ShowDetails(Data_SectorPage dataRef){
		//get the data
		data = dataRef;

		//the fade out loads the details
		FadeOutDetails();

		//move the window
		PlayTween(detailWindow, true);
	}

	public void HideDetails(){
		//move the window
		PlayTween(detailWindow, false);
	}

	private void FadeOutDetails(){
		//load the details in
		details1.GetComponent<UITweener>().AddOnFinished(LoadDetails);
		//fade in on finish
		details1.GetComponent<UITweener>().AddOnFinished(FadeInDetails);
		//fade in
		PlayTween(details1, false);
	}

	private void FadeInDetails(){
		//remove load
		details1.GetComponent<UITweener>().RemoveOnFinished(new EventDelegate (LoadDetails));
		//remover fade in
		details1.GetComponent<UITweener>().RemoveOnFinished(new EventDelegate (FadeInDetails));
		//fade in
		PlayTween(details1, true);
	}

	private void LoadDetails(){
		//set label (it has no input)
		SetLabel(nameObj, GetLabel(data.nameObj));
		descScroll.ResetPosition();
		SetLabel(descObj, data.desc);
		descScroll.ResetPosition();

		GetImage(data);
	}
}
