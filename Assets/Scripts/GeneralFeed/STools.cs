using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public static class STools{

	public static string BoolToString(bool isTrue){
		if(isTrue) { return "1"; }
		else { return "0"; }
	}

	static public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;
		
		if (go != null && parent != null)
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}

	static public string NGUIGetLabel(GameObject obj){
		return obj.GetComponent<UILabel>().text;
	}
	
	static public void NGUISetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}

	static public string UIGetLabel(GameObject obj){
		return obj.GetComponent<UILabel>().text;
	}
	
	static public void UISetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}

	static public void NGUIPlayTween(GameObject obj, bool direction){
		obj.GetComponent<UITweener>().Play(direction);
	}
}
