using UnityEngine;
using System.Collections;

public class CueRegion : MonoBehaviour 
{
	public Cue cue;
	void OnTriggerEnter(Collider other){
		if(other.gameObject.name == "Mesh"){
			if(this.gameObject.name == "CueRegion1")
				cue.region = 1;
			else if(this.gameObject.name == "CueRegion2")
				cue.region = 2;
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.gameObject.name == "Mesh"){
			cue.region = 0;
		}
	}
}