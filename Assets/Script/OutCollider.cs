using UnityEngine;
using System.Collections;

public class OutCollider : MonoBehaviour 
{
	public Manager manager;
	void OnTriggerEnter(Collider other){
		manager.Out(other);
	}
}