using UnityEngine;
using System.Collections;

public class PlaceShooter : MonoBehaviour 
{
	public GameObject[] circles;
	public GameObject shooter;
	public Camera camera;
	float circle_r = 4.5f/4;
	float shooter_r = 0.5f;
	public Manager manager;
	public Sprite shooterSprite;
	public Sprite cross;
	GameObject mouseIntersect;
	
    void Start() {
		
    }
	
	void Update(){
		if(manager.status == 1){
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = 18.9f;
			Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);
			if(!Mode.AImode || (Mode.AImode && manager.current_player == 1))
				transform.position = mouseWorldPosition;

			CheckFreeChess(mouseWorldPosition);
			
			if(mouseIntersect == null){
				gameObject.GetComponent<SpriteRenderer>().enabled = true;
				gameObject.GetComponent<SpriteRenderer>().sprite = cross;
			}
			else if(mouseIntersect.tag == "Red" || mouseIntersect.tag == "Black"){
				gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
			else if(mouseIntersect.tag == "Shooter"){
				gameObject.GetComponent<SpriteRenderer>().enabled = true;
				gameObject.GetComponent<SpriteRenderer>().sprite = shooterSprite;
			}
			
			
			
			if(Input.GetMouseButtonUp(0)){
				if(mouseIntersect == null){
						manager.SetWarning(1);
				}
				else if(CheckIntersect(gameObject)){
					if(mouseIntersect.tag == "Shooter"){
						Vector3 temp = transform.position;
						temp.y += 0.2f;
						transform.position = temp;
						shooter.transform.position = transform.position;
						shooter.SetActive(true);
						manager.shooterSelected = mouseIntersect;
						manager.SetStatus(0);
					}
					else if(mouseIntersect.tag == "Red" || mouseIntersect.tag == "Black"){
						Vector3 temp = mouseIntersect.transform.position;
						temp.y += 0;
						transform.position = temp;
						manager.shooterSelected = mouseIntersect;
						manager.SetStatus(0);
					}
				}
			}
		}
		
		else if(manager.status == 5){
			if(Input.GetMouseButtonDown(0)){
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.z = 18.9f;
				Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);
				CheckMouseIntersect(mouseWorldPosition);
				if(mouseIntersect != null){
					if(mouseIntersect.tag == "Red" && manager.current_player == 1){
						Chess c = mouseIntersect.GetComponent<Chess>();
						SpriteRenderer s = mouseIntersect.transform.GetChild(0).GetComponent<SpriteRenderer>();
						if(!c.discard){
							c.discard = true;
							s.color = new Color(0.2f, 0.2f, 0.2f, 1f);
						}
						else{
							c.discard = false;
							s.color = new Color(1f, 1f, 1f, 1f);
						}
					}
					else if(mouseIntersect.tag == "Black" && manager.current_player == 0){
						Chess c = mouseIntersect.GetComponent<Chess>();
						SpriteRenderer s = mouseIntersect.transform.GetChild(0).GetComponent<SpriteRenderer>();
						if(!c.discard){
							c.discard = true;
							s.color = new Color(0.2f, 0.2f, 0.2f, 1f);
						}
						else{
							c.discard = false;
							s.color = new Color(1f, 1f, 1f, 1f);
						}
					}
				}
				
			}
		}
	}
	
	void CheckMouseIntersect(Vector3 mouseWorldPosition){
		if(manager.current_player == 0){
			foreach(Transform child in manager.blackChess.transform){
				float dist = Mathf.Pow(Mathf.Pow(child.transform.position.x - mouseWorldPosition.x, 2f) + 
					Mathf.Pow(child.transform.position.z - mouseWorldPosition.z, 2f), 0.5f);
				if(dist <= shooter_r){
					mouseIntersect = child.gameObject;
					return;
				}
			}
			
		}
		else if(manager.current_player == 1){
			foreach(Transform child in manager.redChess.transform){
				float dist = Mathf.Pow(Mathf.Pow(child.transform.position.x - mouseWorldPosition.x, 2f) + 
					Mathf.Pow(child.transform.position.z - mouseWorldPosition.z, 2f), 0.5f);
				if(dist <= shooter_r){
					mouseIntersect = child.gameObject;
					return;
				}
			}
		}
		mouseIntersect = null;
	}
	
	void CheckFreeChess(Vector3 mouseWorldPosition){
		foreach(Transform child in manager.redChess.transform){
			if(CheckIntersect(child.gameObject)){
				float dist = Mathf.Pow(Mathf.Pow(child.transform.position.x - mouseWorldPosition.x, 2f) + 
					Mathf.Pow(child.transform.position.z - mouseWorldPosition.z, 2f), 0.5f);
				if(dist <= shooter_r){
					mouseIntersect = child.gameObject;
					return;
				}
			}
		}
		foreach(Transform child in manager.blackChess.transform){
			if(CheckIntersect(child.gameObject)){
				float dist = Mathf.Pow(Mathf.Pow(child.transform.position.x - mouseWorldPosition.x, 2f) + 
					Mathf.Pow(child.transform.position.z - mouseWorldPosition.z, 2f), 0.5f);
				if(dist <= shooter_r){
					mouseIntersect = child.gameObject;
					return;
				}
			}
		}
		
		foreach(Transform child in manager.redChess.transform){
			if(CheckIntersect(child.gameObject)){
				float dist = Mathf.Pow(Mathf.Pow(child.transform.position.x - mouseWorldPosition.x, 2f) + 
					Mathf.Pow(child.transform.position.z - mouseWorldPosition.z, 2f), 0.5f);
				if(dist <= shooter_r * 2){
					mouseIntersect = null;
					return;
				}
			}
		}
		foreach(Transform child in manager.blackChess.transform){
			if(CheckIntersect(child.gameObject)){
				float dist = Mathf.Pow(Mathf.Pow(child.transform.position.x - mouseWorldPosition.x, 2f) + 
					Mathf.Pow(child.transform.position.z - mouseWorldPosition.z, 2f), 0.5f);
				if(dist <= shooter_r * 2){
					mouseIntersect = null;
					return;
				}
			}
		}
		
		if(CheckIntersect(gameObject)){
			mouseIntersect = shooter;
		}
		else{
			mouseIntersect = null;
		}
	}
	
	bool CheckIntersect(GameObject chess){
		if(manager.current_player == 0){
			for(int i = 0; i < 2; i++){
				float dist = Mathf.Pow(Mathf.Pow(circles[i].transform.position.x - chess.transform.position.x, 2f) + 
					Mathf.Pow(circles[i].transform.position.z - chess.transform.position.z, 2f), 0.5f);
				if(dist <= circle_r + shooter_r)
					return true;
			}
		}
		else{
			for(int i = 2; i < 4; i++){
				float dist = Mathf.Pow(Mathf.Pow(circles[i].transform.position.x - chess.transform.position.x, 2f) + 
					Mathf.Pow(circles[i].transform.position.z - chess.transform.position.z, 2f), 0.5f);
				if(dist <= circle_r + shooter_r)
					return true;
			}
		}
		return false;
	}
}