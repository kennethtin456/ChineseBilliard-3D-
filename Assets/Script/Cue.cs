using UnityEngine;
using System.Collections;

public class Cue : MonoBehaviour 
{
	public Manager manager;
	public GameObject center;
	public GameObject powerLine;
	public int region;
	
	private float mousePos;
	float powerLineMinPos = -130;
	float maxPower = 100;
	int lineDir = 1;
	
	public void Init(Vector3 pos){
		transform.position = pos;
		center.transform.position = pos;
		mousePos = Input.mousePosition.x;
	}
	
	void Update(){
		if(manager.status == 2){
			Vector3 temp = center.transform.localEulerAngles;
			temp.y += (mousePos - Input.mousePosition.x);
			mousePos = Input.mousePosition.x;
			center.transform.localEulerAngles = temp;

			Vector3 temp2 = transform.localEulerAngles;
			if(manager.current_player == 0)
				temp2.y = temp.y + 180;
			else
				temp2.y = temp.y;
			transform.localEulerAngles = temp2;
			
			if(Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")){
				if(manager.current_player == 0 && region == 1)
					manager.SetStatus(3);
				else if(manager.current_player == 1 && region == 2)
					manager.SetStatus(3);
				else 
					manager.SetWarning(2);
			}
			
		}
		
		if(manager.status == 3){
			if(Input.GetKey("space")){
				Vector3 temp = powerLine.transform.position;
				temp.y += lineDir * 4;
				powerLine.transform.position = temp;
				if(powerLine.transform.localPosition.y > powerLineMinPos + maxPower)
					lineDir = -1;
				else if(powerLine.transform.localPosition.y < powerLineMinPos)
					lineDir = 1;
			}
			if(Input.GetKeyUp("space")){
				Shoot();
			}
			
			if(Input.GetMouseButton(0)){
				Vector3 temp = powerLine.transform.position;
				temp.y += lineDir * 4;
				powerLine.transform.position = temp;
				if(powerLine.transform.localPosition.y > powerLineMinPos + maxPower){
					lineDir = -1;
				}
				else if(powerLine.transform.localPosition.y < powerLineMinPos)
					lineDir = 1;
			}
			if(Input.GetMouseButtonUp(0)){
				Shoot();
			}
		}

		
	}
	
	void Shoot(){
		float force = (powerLine.transform.localPosition.y - powerLineMinPos) / maxPower * 100000000;
		Quaternion rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y, 0));
		Vector3 direction = rotation * Vector3.back;
		Rigidbody rb = manager.shooterSelected.GetComponent<Rigidbody>();
		rb.AddForce(direction * force);
		manager.SetStatus(4);
	}
	
	
}