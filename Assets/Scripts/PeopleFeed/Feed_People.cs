using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Feed_People : Feed {

	public GameObject detailWindow;
	public GameObject details1;
	public GameObject details2;
	public GameObject mainPanel;
	public Window_Flip windowFlip;

	public GameObject nameObj;
	public GameObject job_titleObj;
	public GameObject qualAcademicObj;
	public GameObject qualProfObj;
	public GameObject descrObj;
	public GameObject imageObj;

	public GameObject filterFirstNameObj;
	public GameObject filterSurnameObj;

	//alphas to tween
	public GameObject[] editButtonAlphas;

	public List<string> currentFilters = new List<string>();

	private Data_People data;

	void Start(){
		grid = gridObject.GetComponent<UIGrid>();
		grid.Reposition();
		LoadLocalData();
		if(EnableAutoDownloads.isEnabled == true){
			Download();
		}
	}

	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_People>().theFeed = this;
	}

	protected override void LocalFilter(){
		if(currentFilters.Count>0){
			scrollView.ResetPosition();
			
			List<GameObject> toAdd = new List<GameObject>();
			
			foreach (GameObject block in displayedBlocks){
				Data_People project = block.GetComponent<Data_People>();
				if(!ContainsAllFilters(project.taxonomies, currentFilters)){
					//save index to remove later
					block.SetActive(false);
				}
			}
			
			//empty the list
			downloadedStrings.Clear();
			scrollView.ResetPosition();
		}
	}
		
	public void ShowDetails(Data_People dataRef){
		//get the data
		data = dataRef;
		LoadDetails();

		//move the window
		Invoke("GrowCard", 0.4f);
	}

	private void GrowCard(){
		windowFlip.TweenFromTarget(data.gameObject);
		Invoke("RevealDetails", 0.4f);
	}

	private void RevealDetails(){
		PlayTween(detailWindow, true);
	}

	public void CentreOnCurrentData(){

		UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild>(data.gameObject);
		UIPanel panel = NGUITools.FindInParents<UIPanel>(data.gameObject);
		
		if (center != null)
		{
			if (center.enabled)
				center.CenterOn(transform);
		}
		else if (panel != null && panel.clipping != UIDrawCall.Clipping.None)
		{
			UIScrollView sv = panel.GetComponent<UIScrollView>();
			Vector3 offset = -panel.cachedTransform.InverseTransformPoint(data.transform.position);
			if (!sv.canMoveHorizontally) offset.x = panel.cachedTransform.localPosition.x;
			if (!sv.canMoveVertically) offset.y = panel.cachedTransform.localPosition.y;

			if(offset.y < basePos.y){
				offset.y = basePos.y;
			}
			SpringPanel.Begin(panel.cachedGameObject, offset, 6f);
		}
	}

	public void HideDetails(){
		
		//move the window
		PlayTween(detailWindow, false);
		//move the window
		Invoke("DelayHide", 0.4f);
	}

	private void DelayHide(){
		windowFlip.TweenBack();
		Invoke("DelayTwist", 0.4f);
	}

	private void DelayTwist(){
		data.twA.Play( false);
		data.twR.Play( false);
		data.twC.Play( false);
	}

	private void LoadDetails(){
		//set label (it has no input)
		SetLabel(nameObj, GetLabel(data.nameObj));
		SetLabel(job_titleObj, GetLabel(data.job_titleObj));
		SetLabel(qualAcademicObj, data.qualAcademic);
		SetLabel(qualProfObj, data.qualProf);

		//set image
		if(data.imageTex != null){
			imageObj.GetComponent<UITexture>().mainTexture = data.imageTex;
		}

		//set text size
		descrObj.GetComponent<UILabel>().fontSize = 20;
		SetLabel(descrObj, data.description);
	}

	//-------------- FILTERS ----------------------

	public void FilterByReSeSe(string key, string filterString){
		currentFilters.Remove(filterString);
		currentFilters.Add(filterString);
		FilterByList(key, currentFilters);
	}

	public void FilterRemoveReSeSe(string key, string filterString){
		currentFilters.Remove(filterString);
		FilterByList(key, currentFilters);
	}

	public void FilterByFirstName(){
		//get filter box string
		string filterString = GetLabel(filterFirstNameObj);

		//number indicates a preset method in PHP
		FilterBy("firstName", "%" + filterString + "%");
	}

	public void FilterBySurname(){
		//get filter box string
		string filterString = GetLabel(filterSurnameObj);
		
		//number indicates a preset method in PHP
		FilterBy("surname", "%" + filterString + "%");
	}

	public void UploadData(){
		WWWForm form = new WWWForm();

		//get the strings from detail objects
		//data.salutation = GetLabel(salutationObj);

		//get the new data
		//form.AddField("id", data.id);

		StartCoroutine(UploadBlock(form, uploadURL));
	}


	//-------------- SORTING ----------------------
	/*public void SortByName(){
		SortByFunction(GetPersonName);
	}

	public void SortByJob(){
		SortByFunction(GetPersonJob);
	}*/

	private void SortByFunction(Func<Data_People, string> GetAttribute){
		foreach(GameObject person in displayedBlocks){
			//get the data for according to the function
			Data_People personData = person.GetComponent<Data_People>();
			string newName = GetAttribute(personData);
			//change the object name
			person.name = newName;
		}
		//sort grid by object name
		grid.Reposition();
	}
	/*
	private string GetPersonName(Data_People person){
		return person.nameObj.GetComponent<UILabel>().text;
	}

	private string GetPersonJob(Data_People person){
		return person.jobObj.GetComponent<UILabel>().text;
	}*/
	//-----------------------------------------

}
