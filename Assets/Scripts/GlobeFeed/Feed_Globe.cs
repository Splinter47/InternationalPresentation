using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Feed_Globe : Feed {

	public GameObject detailWindow;
	public GameObject details1;
	public GameObject details2;
	public GameObject globePanel;
	public GameObject textFade1;
	public GameObject filterPanel;
	public GameObject filterButton;
	public GameObject listButton;

	public GameObject nameObj;
	public GameObject valueObj;
	public GameObject regionObj;
	public GameObject countryObj;
	public GameObject sectorObj;
	public GameObject contractObj;

	public GameObject descriptionObj;
	public UIScrollView descScroll;

	public GameObject imageObj;

	//loading label
	public GameObject loadingObj;

	public List<string> currentFilters = new List<string>();

	//globe's variables
	private Data_Globe data;
	public GameObject pointPrefab;
	public GameObject pointOrigin;
	private bool isAutoRotOn = true;

	//auto focus variables
	private int currentClosestIndex;
	public bool autoFucus;
	private int currentCount = 0;

	void Start(){
		grid = gridObject.GetComponent<UIGrid>();
		grid.Reposition();
		LoadLocalData();
		if(EnableAutoDownloads.isEnabled == true){
			Download();
		}

		//begin the auto rotation
		StartRotation();
	}

	void Update(){
		//AutoFocusClosestProject();

		//
		if(isDownloading){
			loadingObj.SetActive(true);
		}else{
			loadingObj.SetActive(false);
		}
	}

	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_Globe>().theFeed = this;
	}

	protected override void LocalFilter(){
		if(currentFilters.Count>0){
			scrollView.ResetPosition();

			foreach (GameObject block in displayedBlocks){
				Data_Globe project = block.GetComponent<Data_Globe>();
				if(!ContainsAllFilters(project.taxonomies, currentFilters)){
					//hide it from the scene
					block.SetActive(false);
				}
			}

			//empty the list
			downloadedStrings.Clear();
			scrollView.ResetPosition();
		}
	}
		
	public void ShowDetails(Data_Globe dataRef){
		//get the data
		data = dataRef;

		//open the flag
		data.thePointScript.Maximize();

		//the fade out loads the details
		FadeOutDetails();

		//change the grid to 1 column
		//SetGridColumns(1);
		//Invoke("CentreOnCurrentData", 0.25f);
		CentreOnCurrentData();

		//move the window
		PlayTween(detailWindow, true);

		//move the filters
		if(filterPanel.GetComponent<TweenPosition>().to == filterPanel.transform.localPosition){
			PlayTween(filterPanel, false);
			PlayTween(listButton, false);
			//PlayTween(filterButton, false);
			filterButton.GetComponent<UIPlayTween>().Play(false);
		}
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
		PlayTween(details1, false);
		PlayTween(details2, false);
	}

	private void FadeOutDetails(){
		UITweener tween = textFade1.GetComponent<UITweener>();

		tween.ResetToBeginning();
		//load the details in
		tween.AddOnFinished(LoadDetails);
		//fade in on finish
		tween.AddOnFinished(FadeInDetails);
		//fade in
		PlayTween(textFade1, false);

	}

	private void FadeInDetails(){
		UITweener tween = textFade1.GetComponent<UITweener>();

		//remove load
		tween.RemoveOnFinished(new EventDelegate (LoadDetails));
		//remover fade in
		tween.RemoveOnFinished(new EventDelegate (FadeInDetails));
		//fade in
		PlayTween(textFade1, true);
	}

	private void LoadDetails(){
		//set labels
		SetLabel(nameObj, data.name);
		SetLabel(valueObj, data.value);
		SetLabel(regionObj, data.region);
		SetLabel(countryObj, data.country);
		SetLabel(sectorObj, data.sector);
		SetLabel(contractObj, data.contract);

		descriptionObj.GetComponent<UILabel>().fontSize = 25;
		descScroll.ResetPosition();
		SetLabel(descriptionObj, data.description);
		descScroll.ResetPosition();

		imageObj.GetComponent<UITexture>().mainTexture = data.imageObj.GetComponent<UITexture>().mainTexture;

	}

	public void SetInputActive(GameObject inputObj, bool isActive){
		UIInput input = inputObj.GetComponent<UIInput>();
		input.enabled = isActive;
	}

	//-------------- Auto rotate ----------------------
	public void StartRotation(){
		//enable auto focus
		autoFucus = true;

		//get the Y rotator tween
		TweenRotation tween = pointOrigin.transform.GetComponent<TweenRotation>();

		//set start and end
		tween.from = new Vector3(0,0,0);
		tween.to = new Vector3(0,360,0);

		//set to loop and linear
		tween.style = UITweener.Style.Loop;
		tween.animationCurve = AnimationCurve.Linear(0,0,1,1);

		//set speed
		tween.duration = 24f;

		//play tween
		tween.PlayForward();
	}

	public void StopRotation(){
		if(isAutoRotOn){
			isAutoRotOn = false;
			autoFucus = false;

			//stop tween
			TweenRotation tween = pointOrigin.transform.GetComponent<TweenRotation>();
			tween.enabled = false;
			
			//set to not loop and linear
			tween.style = UITweener.Style.Once;
			tween.animationCurve = AnimationCurve.Linear(0,0,1,1);
			
			//set speed
			tween.duration = 0.4f;
		}
	}

	private void AutoFocusClosestProject(){
		//if count changes, reset closest
		if(currentCount == displayedBlocks.Count){
		
			if(displayedBlocks.Count > 0 && autoFucus){
				//set intial closest to first 
				float minZ = displayedBlocks[0].GetComponent<Data_Globe>().thePointScript.transform.position.z;
				int indexOfClosest = 0;

				//find the closest
				for(int i = 1; i<displayedBlocks.Count; i++){
					GlobeTouchPoint point = displayedBlocks[i].GetComponent<Data_Globe>().thePointScript;

					float currentZ = point.transform.position.z;
					if(currentZ < minZ){
						minZ = currentZ;
						indexOfClosest = i;
					}
				}

				MinimizeAllButOne(displayedBlocks[indexOfClosest]);

				if(currentClosestIndex != indexOfClosest){
					//update current and open it
					currentClosestIndex = indexOfClosest;
					displayedBlocks[currentClosestIndex].GetComponent<Data_Globe>().thePointScript.Maximize();
				}
			}
		}else{
			currentClosestIndex = 0;
			currentCount = displayedBlocks.Count;
		}
	}

	public void MinimizeAllButOne(GameObject dontClose){
		foreach(GameObject Block in displayedBlocks){
			//close the rest
			if(Block != dontClose){
				GlobeTouchPoint point = Block.GetComponent<Data_Globe>().thePointScript;
				point.Minimize();
			}
		}
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


	//-------------- SORTING ----------------------
	/*public void SortByName(){
		SortByFunction(GetPersonName);
	}

	public void SortByJob(){
		SortByFunction(GetPersonJob);
	}

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

	private string GetPersonName(Data_People person){
		return person.nameObj.GetComponent<UILabel>().text;
	}

	private string GetPersonJob(Data_People person){
		return person.jobObj.GetComponent<UILabel>().text;
	}*/
	//-----------------------------------------

}
