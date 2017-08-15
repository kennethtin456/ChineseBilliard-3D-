using UnityEngine;
using System.Collections;

public class AIRegion : MonoBehaviour 
{
	Manager manager;
	
	void Start(){
		manager = GameObject.Find("/GameManager").GetComponent<Manager>();
	}
	void OnTriggerStay(Collider other){
		if(other.gameObject.tag == "Red" || other.gameObject.tag == "Black"){
			Chess c = other.gameObject.GetComponent<Chess>();
			c.region = int.Parse(gameObject.name[gameObject.name.Length-1].ToString());
		}
	}
}