using UnityEngine;
using System.Collections;
using System;

public class Feed_Filter : Feed_General {

	public int FilterParent = 1;
	private int numCurrentBlocks = 0;
	public int numRows;
	public Vector3 baseYPos;
	public bool isOpen =false;

	public Feed_People peopleFeed;
	public Feed_Globe projectFeed;

	void Start(){
		numRows = 1;
		baseYPos = scrollView.transform.parent.localPosition;

		LoadLocalData();
		FilterBy(FilterParent, "testing");

		grid = gridObject.GetComponent<UIGrid>();
		grid.Reposition();
		//Download();
	}
	
	protected override void SetFeedReference(GameObject feedObject){
		feedObject.GetComponent<Data_Filter>().theFeed = this;
	}

	protected override void LocalFilter(){
	}

	public void ToggleFilter(Data_Filter data, bool isOn){
		string filterString = data.slug;
		if(isOn){
			if(peopleFeed != null){
				peopleFeed.FilterByReSeSe("filters", filterString);
			}
			if(projectFeed != null){
				projectFeed.FilterByReSeSe("filters", filterString);
			}
		}else{
			if(peopleFeed != null){
				peopleFeed.FilterRemoveReSeSe("filters", filterString);
			}
			if(projectFeed != null){
				projectFeed.FilterRemoveReSeSe("filters", filterString);
			}
		}
	}

	void Update(){

		//if size of data changes
		/*
		int numBlocks = displayedBlocks.Count;
		if(numBlocks != numCurrentBlocks){
			//calc number of rows
			numRows = (int)Math.Ceiling(numBlocks/4.0);
			//limit rows to 1
			if(numRows<1){
				numRows=1;
			}
			//calc y transform increase
			int yOffset = (numRows-1)*45;
			//set clipping
			scrollView.transform.GetComponent<UIPanel>().baseClipRegion = new Vector4(0,0,600,numRows*90);
			Vector3 pos = scrollView.transform.localPosition;
			scrollView.transform.localPosition = new Vector3(pos.x, -yOffset, pos.y);
			Vector3 posGrid = grid.transform.localPosition;
			grid.transform.localPosition = new Vector3(posGrid.x, yOffset, posGrid.y);

		}
		numCurrentBlocks = numBlocks;*/
	}

	public void tweenAllBlocksForward(){
		float i = 0;
		foreach(GameObject block in displayedBlocks){
			i +=0.02f;
			DelayedTween(block, true, i);
		}
	}
	public void tweenAllBlocksBackward(){
		float i = 0;
		foreach(GameObject block in displayedBlocks){
			DelayedTween(block, false, i);
		}
	}

	private void DelayedTween(GameObject obj, bool direction, float wait){
		//obj.SetActive(false);
		//obj.SetActive(true);
		TweenAlpha tween = obj.GetComponent<TweenAlpha>();
		tween.delay = wait;
		tween.Play(direction);
	}


}
