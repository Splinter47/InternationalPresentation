using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	public void LoadPeople(){
		Application.LoadLevel("People2");
	}

	public void LoadProjects(){
		Application.LoadLevel("Projects2");
	}

	public void LoadSectors(){
		Application.LoadLevel("Sectors");
	}

	public void LoadServices(){
		Application.LoadLevel("Services");
	}

	public void LoadIntro(){
		Application.LoadLevel("Intro");
	}
}
