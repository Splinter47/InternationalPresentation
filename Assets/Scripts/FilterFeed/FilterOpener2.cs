using UnityEngine;
using System.Collections;

public class FilterOpener2 : MonoBehaviour {

	public GameObject region;
	public GameObject sector;
	public GameObject service;

	public GameObject regionTri;
	public GameObject sectorTri;
	public GameObject serviceTri;

	public Feed_Region regionFeed;
	public Feed_Sector sectorFeed;
	public Feed_Service serviceFeed;

	public void OpenRegion(){
		Toggle(region);
	}

	public void OpenSector(){
		Toggle(sector);
	}

	public void OpenService(){
		Toggle(service);
	}

	void Update(){
		//triagles alpha follows scrollview
		regionTri.GetComponent<UIWidget>().alpha = regionFeed.scrollView.GetComponent<UIPanel>().alpha;
		sectorTri.GetComponent<UIWidget>().alpha = sectorFeed.scrollView.GetComponent<UIPanel>().alpha;
		serviceTri.GetComponent<UIWidget>().alpha = serviceFeed.scrollView.GetComponent<UIPanel>().alpha;
	}

	private void Toggle(GameObject filter){
		
		//reset position of all

		//push fiters that are lower than this
		if(filter == region){
			if(!regionFeed.isOpen){
				CloseAll();
				regionFeed.isOpen = true;
				//tween filter alpha
				regionFeed.scrollView.GetComponent<TweenAlpha>().PlayForward();
				regionFeed.tweenAllBlocksForward();
			}else{
				CloseAll();
				regionFeed.isOpen = false;
			}
		}else if(filter == sector){
			if(!sectorFeed.isOpen){
				CloseAll();
				sectorFeed.isOpen = true;
				//tween filter alpha
				sectorFeed.scrollView.GetComponent<TweenAlpha>().PlayForward();
				sectorFeed.tweenAllBlocksForward();
			}else{
				CloseAll();
				sectorFeed.isOpen = false;
			}
		}else{
			if(!serviceFeed.isOpen){
				CloseAll();
				serviceFeed.isOpen = true;
				//tween filter alpha
				serviceFeed.scrollView.GetComponent<TweenAlpha>().PlayForward();
				serviceFeed.tweenAllBlocksForward();
			}else{
				CloseAll();
				serviceFeed.isOpen = false;
			}
		}
	}

	private void CloseAll(){
		Close(region, regionFeed);
		Close(sector, sectorFeed);
		Close(service, serviceFeed);
	}

	public void Close(GameObject toTween, Feed_Filter tweeningFilter){
		tweeningFilter.scrollView.GetComponent<TweenAlpha>().PlayReverse();
		tweeningFilter.tweenAllBlocksBackward();
		//tween to base
		
		tweeningFilter.isOpen = false;
	}

	private void TweenPos(GameObject toTween, Vector3 to, Vector3 from){
		TweenPosition tween = toTween.GetComponent<TweenPosition>();
		tween.from = from;
		
		tween.to = to;
		tween.ResetToBeginning();
		tween.PlayForward();
	}
}
