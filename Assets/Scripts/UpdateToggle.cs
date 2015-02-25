using UnityEngine;
using System.Collections;

public class UpdateToggle : MonoBehaviour {

	public void Toggle(){
		EnableAutoDownloads.isEnabled = !EnableAutoDownloads.isEnabled;
	}
}
