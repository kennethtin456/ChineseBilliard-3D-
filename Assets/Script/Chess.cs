using UnityEngine;
using System.Collections;

public class Chess : MonoBehaviour 
{
	public int type;
	public Vector3 backup;
	public bool inside;
	public bool discard;
	public bool outside;
	public int region;
	
	void Start(){
		type = 0;
		inside = false;
		outside = false;
		discard = false;
	}
	
	

}