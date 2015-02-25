using UnityEngine;

public class SpinGlobe : MonoBehaviour
{

	public GameObject targetItem;
	public Camera GUICamera;
	//var ambient : GameObject;
	public bool isZoomable = true;


	/********Rotation Variables*********/
	float rotationRate = 0.075f;
	public bool wasRotating;

	/************Scrolling inertia variables************/
	private float inertiaDuration = 5f;

	private float itemInertiaDuration = 1.0f;
	private float itemTimeTouchPhaseEnded;
	private float rotateVelocityX = 0;
	private float rotateVelocityY = 0;

	RaycastHit hit;

	public LayerMask layerMask;
	public Feed_Globe feedGlobe;


	void Start(){  
		//increase rotaion rate on android
		if (Application.platform == RuntimePlatform.Android) {
			rotationRate = 1f;
		}
	}

	void FixedUpdate(){

		//  If there are touches...
		if (Input.touchCount > 0) {  
			Touch theTouch = Input.GetTouch(0);

			// create the ray
			Ray ray = GUICamera.ScreenPointToRay(theTouch.position);

			// rotat the globe
			calculateSpin (theTouch, ray);
		}

		// slow the spin down
		if (Input.touchCount == 0) {
			calculateInertia ();
		}

		//find 2 finger pinchZoom and rotate
		if (isZoomable) {
			calculateRotationAndZoom ();
		}
	}

	void calculateSpin(Touch theTouch, Ray ray){
		if(Physics.Raycast(ray,out hit,1000,layerMask)){

			//ignor touches on the side list
			if(hit.collider.gameObject.tag == "globeCollider"){
				//activate auto focus on globe
				feedGlobe.StopRotation();

				if(Input.touchCount == 1){

					if (theTouch.phase == TouchPhase.Began){
						wasRotating = false; 
					}     
					
					if (theTouch.phase == TouchPhase.Moved){
						LimitedRotate(theTouch.deltaPosition.y * rotationRate, -theTouch.deltaPosition.x * rotationRate, 0);
						wasRotating = true;
					}
					
					if ((theTouch.phase == TouchPhase.Ended) || (theTouch.phase == TouchPhase.Canceled)){
						if(wasRotating){
							
							if(Mathf.Abs(theTouch.deltaPosition.x) >=5){
								rotateVelocityX = theTouch.deltaPosition.x / theTouch.deltaTime;
							}else{
								//stop the inertia in X
								rotateVelocityX = 0;
							}
							if(Mathf.Abs(theTouch.deltaPosition.y) >=5){
								rotateVelocityY = theTouch.deltaPosition.y / theTouch.deltaTime;
							}else{
								//stop the inertia in Y
								rotateVelocityY = 0;
							}
						}else{
							//stop the inertia in X & Y
							rotateVelocityX = 0;
							rotateVelocityY = 0;
						}
						itemTimeTouchPhaseEnded = Time.time;
					}
				}
			}
		}
	}

	void calculateInertia(){
		
		if(rotateVelocityX != 0.0f || rotateVelocityY != 0.0f){
			//slowing down
			float ty = (Time.time - itemTimeTouchPhaseEnded)/ itemInertiaDuration;
			float XVelocity = Mathf.Lerp(rotateVelocityX, 0, ty);
			//float YVelocity = Mathf.Lerp(rotateVelocityY, 0, ty);
			
			if(ty >= inertiaDuration){
				rotateVelocityX = 0.0f;
				rotateVelocityY = 0.0f;
			}
			//LimitedRotate(YVelocity*Time.deltaTime * rotationRate, -XVelocity*Time.deltaTime * rotationRate, 0);
			LimitedRotate(0, -XVelocity*Time.deltaTime * rotationRate, 0);
		}
	}

	void LimitedRotate(float x, float y, float z){
		//rotate the parent
		targetItem.transform.Rotate(0, y, 0, Space.Self);

		//find current rotations
		float xRot = targetItem.transform.parent.rotation.eulerAngles.x;
		//float zRot = targetItem.transform.parent.rotation.eulerAngles.z;
		float yRot = targetItem.transform.parent.rotation.eulerAngles.y;

		float newX = xRot + x;

		if(xRot > 320f || xRot < 40f){
			targetItem.transform.parent.rotation = Quaternion.Euler(newX, yRot, 0);
		}else if(xRot < 320f && xRot > 180f) {
			if(x > 0){
				targetItem.transform.parent.rotation = Quaternion.Euler(newX, yRot, 0);
			}
		}else if(xRot > 40f && xRot < 180f){
			if(x < 0){
				targetItem.transform.parent.rotation = Quaternion.Euler(newX, yRot, 0);
			}
		}
	}

	float InverseClamp (float angle, float min, float max) {
		//find the value its closest to
		float minDelta = Mathf.Abs (angle - min);
		float maxDelta = Mathf.Abs (angle - max);

		//closer to min
		if(minDelta<maxDelta){
			return Mathf.Clamp (angle, 0, min);
		}
		//closer to max
		else{
			return Mathf.Clamp (angle, max, 360);
		}
	}

	void calculateRotationAndZoom(){
		//rotate and zoom
		float pinchAmount = 0;
		
		DetectTouchMovement.Calculate();
		
		if (Mathf.Abs(DetectTouchMovement.pinchDistanceDelta) > 0) { // zoom
			pinchAmount = DetectTouchMovement.pinchDistanceDelta;

			float moveRate = -1f;
			float zPos = targetItem.transform.parent.parent.localPosition.z;
			float newZpos = zPos + (pinchAmount * moveRate);
			//limit the zoom
			if(newZpos < 0.0f && newZpos > -800f){
				targetItem.transform.parent.parent.localPosition += Vector3.forward * pinchAmount * moveRate;
			}
		}
		
		if (Mathf.Abs(DetectTouchMovement.turnAngleDelta) > 0) { // rotate
			float rotationDegZ = DetectTouchMovement.turnAngleDelta;
			//limit the twist

			//only rotate in Z if within the restricted amount
			//LimitedRotate(0, 0, rotationDegZ);
		}
	}
}