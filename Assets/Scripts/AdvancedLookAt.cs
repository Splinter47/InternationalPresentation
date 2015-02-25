using UnityEngine;
using System.Collections;

public class AdvancedLookAt : MonoBehaviour {

	public Transform target = null;
	public float damping = 20.0f;
	bool smooth = true;

	//logic
	private bool onLeft = true;
	private bool onTop = true;
	private bool isSliding = false;

	void LateUpdate () {
		if (target) {
			if (smooth)
			{
				// Look at and dampen the rotation
				var rotation = Quaternion.LookRotation(-target.position + transform.parent.position);
				transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, rotation, Time.deltaTime * damping);
			}
			else
			{
				// Just lookat
				transform.parent.LookAt(target);
			}
		}
	}

	void Start () {
		// Make the rigid body not change rotation
	   	if (rigidbody)
			rigidbody.freezeRotation = true;
			
		if(target == null){
			target = transform.parent.parent.parent.parent.parent.parent.Find("lookAt").transform;
		}
	}

	void Update(){
		ApplySliding();
	}

	private void ApplySliding(){
		float slideX = 0f;

		//if in right of screen
		if(transform.position.x > target.transform.position.x){
			if(onLeft){
				//move right
				slideX = 1f;
			}

		}
		//if in left of screen
		else{
			if(!onLeft){
				//move left
				slideX = -1f;
			}
		}

		//send info to Y slide
		ApplyYSliding(slideX);
	}

	private void ApplyYSliding(float slideX){
		//if in top of screen
		if(transform.position.y > target.transform.position.y){
			if(!onTop){
				//move up
				Slide(slideX, 1f);
			}else{
				Slide(slideX, 0f);
			}
		}
		//if in bottom of screen
		else{
			if(onTop){
				//move down
				Slide(slideX, -1f);
			}else{
				Slide(slideX, 0f);
			}
		}
	}

	private void Slide(float slideLeft, float slideUp){
		bool didSlide = false;
		//int pivotSide;

		//store the old pivot
		//UIWidget.Pivot oldPivot = transform.GetComponent<UIWidget>().pivot;
		//centre the pivot
		//SetPivot(UIWidget.Pivot.Center);

		if(slideLeft != 0){
			if(slideUp != 0){
				//try and move the widget
				float newX = slideLeft * (transform.GetComponent<UIWidget>().width/2);
				float newY = slideUp * (transform.GetComponent<UIWidget>().height/2);
				didSlide = TweenFromHere(new Vector3(newX, newY, transform.localPosition.z));
			}else{
				float newX = slideLeft * (transform.GetComponent<UIWidget>().width/2);
				didSlide = TweenFromHere(new Vector3(newX, transform.localPosition.y, transform.localPosition.z));
			}
		}else{
			if(slideUp != 0){
				//try and move the widget
				float newY = slideUp * (transform.GetComponent<UIWidget>().height/2);
				didSlide = TweenFromHere(new Vector3(transform.localPosition.x, newY, transform.localPosition.z));
			}
		}

		//if it moved 
		if(didSlide){
			if(slideLeft == 1f){
				//moved right
				onLeft = false;
			}else if(slideLeft == -1f){
				//moved left
				onLeft = true;
			}

			if(slideUp == 1f){
				//moved up
				onTop = true;
			}else if(slideUp == -1f){
				//moved down
				onTop = false;
			}
			//change the pivot to opposite side
			//SetPivot(FindXPivot(pivotSide, oldPivot));
		}else{
			//revert the pivot
			//SetPivot(oldPivot);
		}
	}

	private bool SlideY(float amount){
		float newY = amount * (transform.GetComponent<UIWidget>().height/2);
		return TweenFromHere(new Vector3(transform.localPosition.x, newY, transform.localPosition.z));
	}

	private bool TweenFromHere(Vector3 to){
		if(!isSliding){
			isSliding = true;
			//get the Y rotator tween
			TweenPosition tween = transform.GetComponent<TweenPosition>();

			if(tween == null){
				tween = gameObject.AddComponent<TweenPosition>();
				tween.AddOnFinished(new EventDelegate.Callback(SetSlidingFalse));
			}
			
			//set start and end
			tween.from = transform.localPosition;
			tween.to = to;
			
			//set to loop and linear
			tween.style = UITweener.Style.Once;
			tween.animationCurve = AnimationCurve.Linear(0,0,1,1);
			
			//set speed
			tween.duration = 0.2f;
			
			//play tween
			tween.ResetToBeginning();
			tween.PlayForward();
			return true;
		}else{
			return false;
		}
	}

	public void SetSlidingFalse(){
		isSliding = false;
	}

	public void SetPivot(UIWidget.Pivot to){
		transform.GetComponent<UIWidget>().pivot = to;
	}

	public UIWidget.Pivot FindXPivot(int newXPivot, UIWidget.Pivot oldPivot){

		//if was on bottom 
		if(oldPivot == UIWidget.Pivot.Bottom || oldPivot == UIWidget.Pivot.BottomLeft || oldPivot == UIWidget.Pivot.BottomRight){

			if(newXPivot == 1){
				return UIWidget.Pivot.BottomRight;
			}else{
				return UIWidget.Pivot.BottomLeft;
			}

		//if was on top
		}else{
			if(newXPivot == 1){
				return UIWidget.Pivot.TopRight;
			}else{
				return UIWidget.Pivot.TopLeft;
			}
		}

	}

}