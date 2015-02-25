using UnityEngine;
using System.Collections;

public class PinTween : MonoBehaviour {

	public TweenPosition tween;

	// Use this for initialization
	void Start () {

		tween.duration = Random.Range(1f,2f);

		tween.PlayForward();
	
	}
}
