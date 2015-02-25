using UnityEngine;
using System.Collections;

public class Window_Flip : MonoBehaviour {

	public TweenPosition twPos;
	public TweenHeight twH;
	public TweenWidth twW;

	public void TweenFromTarget(GameObject target){
		gameObject.SetActive(true);

		transform.localPosition = new Vector3(0f,0f,0f);

		Vector3 pos = transform.InverseTransformPoint(target.transform.position);
		twPos.from = new Vector3(pos.x-88f, pos.y+109.5f, pos.z);

		twPos.Play(true);
		twH.Play(true);
		twW.Play(true);
	}

	public void TweenBack(){
		twPos.Play(false);
		twH.Play(false);
		twW.Play(false);

		Invoke("Disable", 0.4f);
	}

	public void Disable(){
		gameObject.SetActive(false);
	}
}
