using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using System;

public abstract class Feed_General : MonoBehaviour {

	public UIScrollView scrollView;
	public GameObject gridObject;
	public GameObject displayPrefab;
	protected UIGrid grid;
	protected Vector3 basePos;
	
	protected List<string> downloadedStrings = new List<string>();
	protected List<GameObject> displayedBlocks = new List<GameObject>();
	public bool saveDataLocally;
	private bool saved = false;

	public string downloadURL = "http://www.samdavies.info/Systech/DownloadBlocks.php";
	public string uploadURL = "http://www.samdavies.info/Systech/UpdateBlocks.php";
	protected Dictionary<int, string> filterList = new Dictionary<int, string>();
	private int latestDownloadRequest;
	protected bool isDownloading = false;

	//upload queue
	protected LinkedList<WWWForm> uploadQueue = new LinkedList<WWWForm>();
	private bool uploading = false;

	void Awake(){
		// Forces a different code path in the BinaryFormatter that doesn't rely on run-time code generation (which would break on iOS).
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		grid = gridObject.GetComponent<UIGrid>();
	}
	
	void Start () {
		grid.Reposition();
		//Download();
	}

	void Update(){

		//push updates if they exist
		if(!uploading){
			if(uploadQueue.Count > 0){
				print ("Upload Queue size: " + uploadQueue.Count);
				uploading = true;
				WWWForm form = uploadQueue.First.Value;
				StartCoroutine(UploadBlock(form, uploadURL));
			}
		}
	}
	
	public void Download(){
		StartCoroutine(DownloadBlocks(filterList, downloadURL, 0));
	}

	protected IEnumerator DownloadBlocks(Dictionary<int, string> filters, string dataURL, int attempt){
		isDownloading = true;
		latestDownloadRequest = (int)Time.time;
		string URL = dataURL;
		WWWForm form = new WWWForm();

		//add all filters
		string methods = "";
		foreach(var filter in filters){
			methods += filter.Key + ":" + filter.Value + ";";
		}
		print ("downloading...with filters: " + methods);

		//send all methods in one string
		form.AddField(("methods"), methods);

		//add auth feilds
		form.AddField("userCookie", PlayerPrefs.GetString("userCookie"));
		form.AddField("passCookie", PlayerPrefs.GetString("passCookie"));
		form.AddField("time", latestDownloadRequest);
		
		WWW download = new WWW(URL, form);
		yield return download;
		
		if(download.error != null){
			print(download.error);
			if(attempt < 10){
				StartCoroutine(DownloadBlocks(filters, dataURL, attempt+1));
			}else{
				print ("Download failed after " + attempt + " attempts");
				//load from player prefs
				LoadLocalData();
			}
		}else{
			if(!download.text.Equals("failed")){
				print ("found data: " + download.text);
				//split into data and time
				CheckIsLastestDownload(download.text);
			}else{
				//you must login in again
				print ("Authorisation invalid");
			}
		}
	}

	private void CheckIsLastestDownload(string downloadText){
		string[] stringSeparators = {"<!time!>"};
		string[] dataAndTime = downloadText.Split(stringSeparators, System.StringSplitOptions.None);
		
		if(Int32.Parse(dataAndTime[1]) == latestDownloadRequest){
			isDownloading = false;
			//destroy old data
			RemoveDataObjects();
			//add new data
			AddBlocksData(dataAndTime[0]);
		}
	}

	protected IEnumerator UploadBlock(WWWForm form, string uploadURL){
		print ("uploading...");

		//add auth feilds
		form.AddField("userCookie", PlayerPrefs.GetString("userCookie"));
		form.AddField("passCookie", PlayerPrefs.GetString("passCookie"));
		
		WWW upload = new WWW(uploadURL, form);
		yield return upload;

		if(upload.error != null){
			print(upload.error);
			StartCoroutine(UploadBlock(form, uploadURL));
		}else{
			//removed the upload from the queue
			uploading = false;
			uploadQueue.RemoveFirst();

			if(upload.text.Equals("updated")){
				print ("upload sucessful");
			}else{
				print (upload.text);
				//you must login in again
				print ("Authorisation invalid");
			}
		}
	}

	void AddBlocksData(string toCut){
		// split the rawData string into blocks and add new blocks
		string[] stringSeparators = {"<!!>"};
		string[] people = toCut.Split(stringSeparators, System.StringSplitOptions.None);
		
		foreach(string block in people){
			//remove the last empty thing
			if(block.Length>1){
				downloadedStrings.Add(block);
			}
		}

		// important!
		DisplayBlocks(true);
	}

	void DisplayBlocks(bool toSave){
		//create real objects
		scrollView.ResetPosition();
		print ("creating " + downloadedStrings.Count + " blocks");
		foreach (string stringData in downloadedStrings){
			// create a new block Thumbnail
			GameObject blockObject = NGUITools.AddChild(gridObject, displayPrefab);
			displayedBlocks.Add(blockObject);
			SetFeedReference(blockObject);
			blockObject.GetComponent<Data_General>().AddVariables(stringData);
		}
		//save 
		if(toSave){
			SaveDataLocal();
		}
		grid.Reposition();
		scrollView.ResetPosition();
		basePos = scrollView.transform.localPosition;
	}

	abstract protected void SetFeedReference(GameObject feedObject);

	protected void RemoveDataObjects(){
		// destroy all objects
		scrollView.ResetPosition();
		foreach (GameObject block in displayedBlocks){
			block.GetComponent<Data_General>().DestroyTextures();
			NGUITools.Destroy(block);

		}
		//empty the list
		displayedBlocks.Clear();
		downloadedStrings.Clear();
		scrollView.ResetPosition();
	}

	//-------------- HELPER FUNCTIONS --------------------

	protected string GetLabel(GameObject obj){
		return obj.GetComponent<UILabel>().text;
	}
	
	protected void SetLabel(GameObject to, string newText){
		UILabel lbl = to.transform.GetComponent<UILabel>();
		lbl.text = newText;
		lbl.MarkAsChanged();
	}

	protected void SetLabelAndInput(GameObject to, string newText){
		UIInput input = to.transform.GetComponent<UIInput>();
		input.value = newText;
		SetLabel(to, newText);
	}

	/*public void SetImage(GameObject to, Texture2D newTex){
		UITexture image = to.GetComponent<UITexture>();
		image.mainTexture = newTex;
	}*/
	
	protected void PlayTween(GameObject obj, bool direction){
		obj.GetComponent<UITweener>().Play(direction);
	}
	
	protected void SetGridColumns(int cols){
		grid.maxPerLine = cols;
		grid.Reposition();
	}
	
	protected void FilterBy(int methodNum, string search){
		//remove old filter
		filterList.Remove(methodNum);
		//check search is not empty
		if(!search.Equals("")){
			//add the filter
			filterList.Add(methodNum, search);
		}
		//----------filter locally----------

		LoadLocalData();


		if(EnableAutoDownloads.isEnabled == true){
			//download with filter applied
			StartCoroutine(DownloadBlocks(filterList, downloadURL, 0));
		}
	}

	abstract protected void LocalFilter();

	public void FilterByList(int method, List<string> list){

		
		string bigString = "";

		if(list.Count > 0){
			bigString += list[0];

			if(list.Count > 1){
				for(int i = 1; i<list.Count; i++){
					bigString += ("," + list[i]);
				}
			}
		}

		print ("filtering");
		//currentFilters
		FilterBy(method, bigString);
	}

	public void AddListFilter(int method, string filterString, List<string> list){
		//remove this
		list.Remove(filterString);
		
		//add new filter
		list.Add(filterString);

		FilterByList(method, list);
	}
	
	public void RemoveListFilter(int method, string filterString, List<string> list){
		list.Remove(filterString);

		FilterByList(method, list);
	}

	//------------ SAVE AND LOAD FUNCTIONS -----------------

	public void SaveDataLocal(){
		if(saveDataLocally){
			if(!saved){
				print ("Saved.");
				//only store first occurence
				saved = true;
				//use downloadURL since we know all saves of this name will be the same
				convertTo<string>(downloadURL, downloadedStrings);
			}
		}
	}
	
	private void convertTo<T>(string key, List<T> list){
		BinaryFormatter b = new BinaryFormatter();
		MemoryStream m = new MemoryStream();
		b.Serialize(m, list);
		PlayerPrefs.SetString(key, Convert.ToBase64String(m.GetBuffer()));
	}
	
	protected void LoadLocalData(){
		if(saveDataLocally){
			isDownloading = false;
			print ("Loading locally saved data");
			RemoveDataObjects();
			//load string from prefabs
			string data = PlayerPrefs.GetString(downloadURL);
			//convert string to list of strings
			convertBackGameObject(data);
			DisplayBlocks(false);
			LocalFilter();
			grid.Reposition();
			scrollView.ResetPosition();
		}
	}

	private void convertBackGameObject(string data){
		if(!String.IsNullOrEmpty(data)){
			//load the data from binary back into the list
			var b = new BinaryFormatter();
			var m = new MemoryStream(Convert.FromBase64String(data));
	
			downloadedStrings = b.Deserialize(m) as List<string>;
			print(downloadedStrings.Count);
		}
	}
}
