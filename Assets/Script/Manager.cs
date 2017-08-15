using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Manager : MonoBehaviour 
{
	public int status = 0;
	public Camera camera;
	public GameObject shootSprite;
	public GameObject center;
	public Cue cue;
	public GameObject shooter;
	public GameObject redChess;
	public GameObject blackChess;
	public GameObject powerLine;
	public GameObject button1;
	public GameObject button2;
	public GameObject button3;
	public GameObject button4;
	public GameObject button5;
	public Text cueText;
	public Text currentPlayerText;
	public Text endGameMessage;
	public Text message;
	public Text warning;
	public AI ai;
	public ButtonListener buttonListener;
	
	public int current_player;
	public int[] num_cue = new int[2];
	
	public GameObject shooterSelected;
	
	Vector3 shooterBackup;
	bool thisHitIn;


	
	static string[] warnings = {
		"",
		"Shooter cannot be placed here",
		"Cue out of bound",
		"Penalty. First touch Small Return or Big Return",
		"Chess discarded. First touch is opponent's chess"
	};
	

	
	void Start(){
		Random.seed = (int)System.DateTime.Now.Ticks;
		current_player = Random.Range(0, 2);
		Debug.Log("Player "+current_player);
		num_cue[0] = 1;
		num_cue[1] = 1;
		SetStatus(0);
		CheckStopMotion();
	}
	
	
	
	
	public void SetStatus(int sta){
		switch(sta){
		case 0:
			message.text = "Choose your action";
		
			center.transform.position = new Vector3(0, 10, 0);
			center.transform.localEulerAngles = new Vector3(0, 0, 0);
			if(current_player == 0){
				camera.transform.localPosition = new Vector3(-10f, 10f, -10f);
				camera.transform.localEulerAngles = new Vector3(45, 45, 0);
			}
			else{
				camera.transform.localPosition = new Vector3(10f, 10f, 10f);
				camera.transform.localEulerAngles = new Vector3(45, 225, 0);
			}
			cue.gameObject.SetActive(false);
			shootSprite.SetActive(false);
			powerLine.transform.localPosition = new Vector3(300, -130, 0);
			
			button1.SetActive(true);
			button3.SetActive(true);
			button4.SetActive(false);
			if(shooterSelected != null)
				button2.SetActive(true);
			else
				button2.SetActive(false);
			
			if(Mode.AImode && current_player == 0){
				button1.SetActive(false);
				button2.SetActive(false);
				button3.SetActive(false);
				button4.SetActive(false);
				ai.Hit();
			}
			status = 0;
			
			
			break;
		case 1:
			message.text = "Place your shooter";
			center.transform.position = new Vector3(0, 10, 0);
			center.transform.localEulerAngles = new Vector3(0, 0, 0);
			camera.transform.localPosition = new Vector3(0f, 20f, 0f);
			if(current_player == 0){
				camera.transform.localEulerAngles = new Vector3(90, 0, 0);
			}
			else{
				camera.transform.localEulerAngles = new Vector3(90, 180, 0);
			}
			
			if(Mode.AImode && current_player == 0){
				button1.SetActive(false);
				button2.SetActive(false);
				button3.SetActive(false);
				button4.SetActive(false);
				shootSprite.SetActive(false);
			}
			else{
				shootSprite.SetActive(true);
			}
			
			cue.gameObject.SetActive(false);
			
			
			status = 1;
			break;
		case 2:
			message.text = "Mouse mouse to set cue angle, hold left click to set power";
			center.transform.position = new Vector3(0, 10, 0);
			center.transform.localEulerAngles = new Vector3(0, 0, 0);
			if(current_player == 0){
				camera.transform.localPosition = new Vector3(0, 5f, -12f);
				camera.transform.localEulerAngles = new Vector3(30, 0, 0);
			}
			else{
				camera.transform.localPosition = new Vector3(0, 5f, 12f);
				camera.transform.localEulerAngles = new Vector3(30, 180, 0);
			}
			
			shooterBackup = shooterSelected.transform.position;
			thisHitIn = false;
			
			cue.gameObject.SetActive(true);
			cue.Init(shooterSelected.transform.position);
			status = 2;
			break;
		case 3:
			message.text = "Mouse mouse to set cue angle, hold left click to set power";
			button1.SetActive(false);
			button2.SetActive(false);
			button3.SetActive(false);
			status = 3;
			
			break;
		case 4:
			message.text = "";
			shooter.GetComponent<Shooter>().firstTouch = null;
			foreach(Transform child in redChess.transform){
				Chess c = child.gameObject.GetComponent<Chess>();
				c.backup = child.position;
			}
			foreach(Transform child in blackChess.transform){
				Chess c = child.gameObject.GetComponent<Chess>();
				c.backup = child.position;
			}
			button1.SetActive(false);
			button2.SetActive(false);
			button3.SetActive(false);
			status = 4;
			
			break;
		case 5:
			message.text = "Select chess you want to discard and click OK";
			center.transform.position = new Vector3(0, 10, 0);
			center.transform.localEulerAngles = new Vector3(0, 0, 0);
			camera.transform.localPosition = new Vector3(0f, 20f, 0f);
			if(current_player == 0){
				camera.transform.localEulerAngles = new Vector3(90, 0, 0);
			}
			else{
				camera.transform.localEulerAngles = new Vector3(90, 180, 0);
			}
			button4.SetActive(true);
			shootSprite.SetActive(true);
			status = 5;
			break;
		case 6:
			message.text = "Game ends";
			button1.SetActive(false);
			button2.SetActive(false);
			button3.SetActive(false);
			button4.SetActive(false);
			if(Mode.AImode){
				if(redChess.transform.childCount == 0){
					endGameMessage.text = "You Lose";
				}
				else
					endGameMessage.text = "You Win";
			}
			else{
				if(redChess.transform.childCount == 0){
					endGameMessage.text = "Player1 Win";
				}
				else
					endGameMessage.text = "Player2 Win";
			}
			status = 6;
			button5.SetActive(true);
			break;
		}
	}

	public void In(Collider other){
		Chess c;
		switch(other.gameObject.tag){
		case "Shooter":
			num_cue[current_player]--;
			Stop(other.gameObject);
			other.gameObject.SetActive(false);
			break;
		case "Red":
			if(current_player == 0){
				thisHitIn = true;
			}
			c = other.gameObject.GetComponent<Chess>();
			c.inside = true;
			break;
		case "Black":
			if(current_player == 1){
				thisHitIn = true;
			}
			c = other.gameObject.GetComponent<Chess>();
			c.inside = true;
			break;
		}
	}
	
	public void Out(Collider other){
		Chess c;
		switch(other.gameObject.tag){
		case "Shooter":
			num_cue[current_player]--;
			Stop(other.gameObject);
			other.gameObject.SetActive(false);
			break;
		case "Red":
			Stop(other.gameObject);
			other.gameObject.SetActive(false);
			c = other.gameObject.GetComponent<Chess>();
			c.outside = true;
			break;
		case "Black":
			Stop(other.gameObject);
			other.gameObject.SetActive(false);
			c = other.gameObject.GetComponent<Chess>();
			c.outside = true;
			break;
		}
	}

	void Update(){
		cueText.text = "Cues : " + num_cue[current_player];
		if(Mode.AImode){
			if(current_player == 0)
				currentPlayerText.text = "Opponent's Turn";
			else
				currentPlayerText.text = "Your Turn";
		}
		else{
			if(current_player == 0)
				currentPlayerText.text = "Player1's Turn";
			else
				currentPlayerText.text = "Player2's Turn";
		}
		if(redChess.transform.childCount == 0 || blackChess.transform.childCount == 0)
			SetStatus(6);
		
		if(Input.GetKeyUp("z")){
			if(button1.activeSelf){
				buttonListener.PlaceShooter();
			}
		}
		if(Input.GetKeyUp("x")){
			if(button2.activeSelf){
				buttonListener.Shoot();
			}
		}
		if(Input.GetKeyUp("c")){
			if(button3.activeSelf){
				buttonListener.DiscardChess();
			}
		}
		switch(status){
		case 0:

			break;
		case 1:
			break;
		case 2:
			
			break;
		case 3:
			break;
		case 4:
			



			if(CheckStopMotion()){
				
				TurnChess();
				HandleFirstTouch();
				HandleInsideChess();
				HandleReturn();
				HandleOutside();
				if(!thisHitIn && shooterSelected != null){
					shooterSelected.transform.position = shooterBackup;
				}
				num_cue[current_player]--;
				
				shooterSelected = null;
				Stop(shooter);
				shooter.SetActive(false);
				
				
				if(num_cue[current_player] <= 0){
					NextPlayer();
						
				}
				SetStatus(0);
				
				
				
				
			}
			break;
		}
	}
	
	
	
	
	void NextPlayer(){
		if(current_player == 0){
			current_player = 1;
			num_cue[1] += 0 - num_cue[0];
			num_cue[0] = 1;
		}
		else if(current_player == 1){
			current_player = 0;
			num_cue[0] += 0 - num_cue[1];
			num_cue[1] = 1;
		}
	}
	
	bool CheckStopMotion(){
		
		Rigidbody rb = shooter.GetComponent<Rigidbody>();

		if(rb.velocity.magnitude > 0.001f){
			return false;
		}
		foreach(Transform child in redChess.transform){
			rb = child.gameObject.GetComponent<Rigidbody>();
			if(rb.velocity.magnitude > 0.001f){
				return false;
			}
		}
		foreach(Transform child in blackChess.transform){
			rb = child.gameObject.GetComponent<Rigidbody>();
			if(rb.velocity.magnitude > 0.001f){
				return false;
			}
		}
		return true;
	}
	
	void Stop(GameObject chess){
		chess.transform.position = new Vector3(0, 11.25f, 0);
		chess.transform.localEulerAngles = new Vector3(0, 0, 0);
		Rigidbody rb = chess.GetComponent<Rigidbody>();
		rb.velocity = new Vector3(0, 0, 0);
		rb.angularVelocity = new Vector3(0, 0, 0);
		
	}
	
	void TurnChess(){
		foreach(Transform child in redChess.transform){
			Vector3 temp = child.localEulerAngles;
			temp.z = 0;
			child.localEulerAngles = temp;
		}
		foreach(Transform child in blackChess.transform){
			Vector3 temp = child.localEulerAngles;
			temp.z = 0;
			child.localEulerAngles = temp;
		}
	}
	void HandleFirstTouch(){
		GameObject ft = shooter.GetComponent<Shooter>().firstTouch;
		if(ft != null){
			// check if firstTouch is Small Return or Big Return
			if((current_player == 0 && ft.tag == "Red") || 
				(current_player == 1 && ft.tag == "Black")){
				Chess c = ft.GetComponent<Chess>();
				if(c.type != 0){
					num_cue[current_player] -= 1;
					foreach(Transform child in redChess.transform){
						c = child.gameObject.GetComponent<Chess>();
						child.position = c.backup;
						c.inside = false;
					}
					foreach(Transform child in blackChess.transform){
						c = child.gameObject.GetComponent<Chess>();
						child.position = c.backup;
						c.inside = false;
					}
					SetWarning(3);
				}
			}
			// check if firstTouch is opponent's chess
			else if((current_player == 0 && ft.tag == "Black") || 
				(current_player == 1 && ft.tag == "Red")){
					Destroy(ft);
					SetWarning(4);
				}
		}
	}
	
	void HandleInsideChess(){
		foreach(Transform child in redChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(c.inside){
				if(current_player == 0)
					num_cue[current_player]++;
				Destroy(child.gameObject);
			}
		}
		foreach(Transform child in blackChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(c.inside){
				if(current_player == 1)
					num_cue[current_player]++;
				Destroy(child.gameObject);
			}
		}
	}
	
	void HandleReturn(){
		foreach(Transform child in redChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(child.position.z <= -6.3){
				c.type = 2;
			}
			else if(c.type == 1){
				float dist = Mathf.Pow(Mathf.Pow(child.position.x, 2f) + 
					Mathf.Pow(child.position.z, 2f), 0.5f);
				if(dist < 1.2)
					child.position = c.backup;
				else
					c.type = 0;
			}
			else{
				c.type = 0;
			}
		}
		foreach(Transform child in blackChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(child.position.z >= 6.3){
				c.type = 2;
			}
			else if(c.type == 1){
				float dist = Mathf.Pow(Mathf.Pow(child.position.x, 2f) + 
					Mathf.Pow(child.position.z, 2f), 0.5f);
				if(dist < 1.2)
					child.position = c.backup;
				else
					c.type = 0;
			}
			else{
				c.type = 0;
			}
		}
	}
	
	void HandleOutside(){
		foreach(Transform child in redChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(c.outside){
				child.position = new Vector3(0f, 11.25f, 0f);
				c.outside = false;
				child.gameObject.SetActive(true);
				c.type = 1;
			}
		}
		foreach(Transform child in blackChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(c.outside){
				child.position = new Vector3(0f, 11.25f, 0f);
				c.outside = false;
				child.gameObject.SetActive(true);
				c.type = 1;
			}
		}
	}
	
	public void Discard(){
		foreach(Transform child in redChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(c.discard){
				Destroy(child.gameObject);
			}
		}
		foreach(Transform child in blackChess.transform){
			Chess c = child.gameObject.GetComponent<Chess>();
			if(c.discard){
				Destroy(child.gameObject);
			}
		}
		SetStatus(0);
	}
	
	
	
	void ResetWarning(){
		warning.text = warnings[0];
	}
	
	public void SetWarning(int i){
		warning.text = "Warning: " + warnings[i];
		Invoke("ResetWarning", 3);
	}
}