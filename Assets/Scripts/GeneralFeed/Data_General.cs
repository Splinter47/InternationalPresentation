using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public abstract class Data_General : MonoBehaviour {

	protected string[] data;
	public bool saveImagesLocally = false;

	//visual elements to display on seperate page
	public GameObject[] displayElementLinks;
	protected Dictionary<string, GameObject> displayElements = new Dictionary<string, GameObject>();

	public void AddVariables(string bigString){

		//divide into its data components
		string[] attStringSeparators = {"<!>"};
		data = bigString.Split(attStringSeparators, System.StringSplitOptions.None);

		Create();
	}

	abstract public void Create();

	abstract public void DestroyTextures();

	protected string GetLabel(GameObject obj){
		return obj.GetComponent<UILabel>().text;
	}

	protected void SetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}

	protected IEnumerator DownloadTexture(string url, string fileName, Action<Texture2D> onFinish, int attempt){

		WWW download = new WWW(url + fileName);
		yield return download;
		
		if(download.error != null){
			print(download.error);
			if(attempt < 10){
				StartCoroutine(DownloadTexture(url, fileName, onFinish, attempt+1));
			}else{
				print ("Download failed after " + attempt + " attempts");
			}
		}else{
			print ("found image");
			if(saveImagesLocally){
				SaveTexture(fileName, download.texture);
			}
			onFinish(download.texture);
		}
	}

	public IEnumerator LoadLocalTexture(string url, string fileName, Action<Texture2D> onFinish, int attempt){
		print ("loading locally");
		string path;
		if(Application.platform == RuntimePlatform.OSXEditor){
			path = "file://" + Application.dataPath + "/Images/";
		}else{
			path = "file://" + Application.persistentDataPath + "/";
		}
		
		WWW download = new WWW(path + fileName);
		yield return download;
		
		if(download.error != null){
			print(download.error);
			//find from web
			StartCoroutine(DownloadTexture(url, fileName, onFinish, attempt+1));
		}else{
			print ("found image");
			onFinish(download.texture);
		}
	}



	protected void SaveTexture(string fileName, Texture2D texture){

		string path;
		if(Application.platform == RuntimePlatform.OSXEditor){
			path = Application.dataPath + "/Images/";
		}else{
			path = Application.persistentDataPath + "/";
		}

		Byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(path+fileName, bytes);
		print ("Saved image");

	}
	
	/*public Texture2D GetCentredSubTexture(Texture2D inputTex, int newWidth, int newHeight){

		int targetWidth = newWidth;
		int targetHeight = newHeight;

		//differnce between current width/height and new width/height
		int widthDiff = inputTex.width - newWidth;
		int heighDiff = inputTex.height - newHeight;

		//ratio between current width/height and new width/height
		double widthRatio = (1.0*inputTex.width)/newWidth;
		double heighRatio = (1.0*inputTex.height)/newHeight;

		//check the new dimensions are smaller
		if(widthRatio < 1 || heighRatio < 1){
			if(widthRatio<1 && heighRatio < 1){
				//both too big here
				if(widthRatio<heighRatio){
					//calc from old width
					newHeight = ScaleAToB(widthRatio, newHeight);
					newWidth = inputTex.width;
				}else{
					//calc from old height
					newWidth = ScaleAToB(heighRatio, newWidth);
					newHeight = inputTex.height;
				}
			}else if(widthRatio<1){
				//calc from old width
				newHeight = ScaleAToB(widthRatio, newHeight);
				newWidth = inputTex.width;
			}else{
				//calc from old height
				newWidth = ScaleAToB(heighRatio, newWidth);
				newHeight = inputTex.height;
			}

			//recalculate
			widthDiff = inputTex.width - newWidth;
			heighDiff = inputTex.height - newHeight;
		}

		//find centre
		int x = (widthDiff)/2;
		int y = (heighDiff)/2;

		Color[] pix = inputTex.GetPixels(x, y, newWidth, newHeight);
		Texture2D destTex = new Texture2D(newWidth, newHeight);
		destTex.SetPixels(pix);
		destTex.Apply();

		print ("Target: (" + targetWidth + "," + targetHeight + ")" + 
		       " max: (" + inputTex.width + "," + inputTex.height + ")" +
		       " Actual: (" + newWidth + "," + newHeight + ")");

		return destTex;
	}*/

	private int ScaleAToB(double ratioA, int newB){
		return (int)Math.Floor(newB * ratioA);
	}
}


