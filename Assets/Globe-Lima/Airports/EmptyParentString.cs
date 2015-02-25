using UnityEngine;
using System.Collections;

public class EmptyParentString : MonoBehaviour {

	string real = "";
	string empty = "";

	UILabel thisLabel;
	UILabel parentLabel;

	// Use this for initialization
	void Start () {
		thisLabel = transform.GetComponent<UILabel> ();
		real = thisLabel.text;
		parentLabel = transform.parent.GetComponent<UILabel> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(!ConsistsOfWhiteSpace(parentLabel.text)){
			thisLabel.text = real;
		}else{
			thisLabel.text = empty;
		}
	}

	public bool ConsistsOfWhiteSpace(string s){
		foreach(char c in s){
			if(c != ' ' && c != '\n' && c != '\t' && c != '\r') return false;
		}
		return true;
	}
}
