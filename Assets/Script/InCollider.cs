using UnityEngine;
using System.Collections;

public class InCollider : MonoBehaviour 
{
	public Manager manager;
	void OnTriggerEnter(Collider collider){
		manager.In(collider);
	}
}