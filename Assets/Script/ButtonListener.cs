using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonListener : MonoBehaviour 
{
	public Manager manager;
	
	public void PlaceShooter(){
		manager.SetStatus(1);
	}
	
	public void Shoot(){
		manager.SetStatus(2);
	}
	
	public void DiscardChess(){
		manager.SetStatus(5);
	}
	
	public void OK(){
		manager.button4.SetActive(false);
		manager.Discard();
	}
	
	public void AiMode(){
		Mode.AImode = true;
		SceneManager.LoadScene("Game");
	}
	
	public void TwoPlayerMode(){
		Mode.AImode = false;
		SceneManager.LoadScene("Game");
	}
	
	public void Back(){
		SceneManager.LoadScene("Menu");
	}
	
}