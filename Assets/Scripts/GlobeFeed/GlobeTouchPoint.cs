using UnityEngine;
using System.Collections;
using System;

public class GlobeTouchPoint : MonoBehaviour {

	public Data_Globe dataLink;

	//objects
	public GameObject name;
	public GameObject image;
	public GameObject imageParent;

	public bool isOpen = false;

	void Update(){
		//if link is destroyed, destroy this
		if(dataLink == null){
			NGUITools.Destroy(transform.parent.gameObject);
		}else if(dataLink.gameObject.activeSelf == false){
			NGUITools.Destroy(transform.parent.gameObject);
		}
	}

	public void OpenPoint(){
		print ("clicked");
		dataLink.OpenDetails();
		//now called from thumnail
	}

	public void Minimize(){
		if(isOpen){
			imageParent.GetComponent<TweenScale>().PlayReverse();
			print ("closing " + dataLink.name);
			isOpen = false;
		}
	}

	public void Maximize(){
		if(!isOpen){
			imageParent.GetComponent<TweenScale>().PlayForward();
			print ("opening " + dataLink.name);
			isOpen = true;
		}
	}

	public void RotateGlobeToThis(){
		//cancel auto rotation
		dataLink.theFeed.MinimizeAllButOne(dataLink.gameObject);
		dataLink.theFeed.StopRotation();

		//globe's Y roation
		Transform targetTransform = dataLink.theFeed.pointOrigin.transform;

		//current rot
		float yRot = targetTransform.localRotation.eulerAngles.y;
	
		//change the tween
		TweenRotation tween = targetTransform.GetComponent<TweenRotation>();
		tween.from = new Vector3(0, yRot, 0);

		tween.to = new Vector3(0, modulo(-dataLink.longatude, 360), 0);
		tween.ResetToBeginning();
		tween.PlayForward();
	}

	public void Create(){
		SetLabel(name, dataLink.name);
	}

	private float modulo(float a,float b){
		return a - b * (float)Math.Floor(a / b);
	}

	private void SetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}

}
