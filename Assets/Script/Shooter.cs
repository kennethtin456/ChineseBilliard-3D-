using UnityEngine;

public class Shooter : MonoBehaviour 
{
    public float thrust;
    public Rigidbody rb;
	public GameObject firstTouch;


    void Start() 
    {
        rb = GetComponent<Rigidbody>();
		
		
    }

    void FixedUpdate() 
    {
        //Debug.Log(transform.position);
    }
	
	void OnCollisionEnter(Collision collision){
		if(collision.collider.gameObject.tag == "Red" || 
			collision.collider.gameObject.tag == "Black" || 
			collision.collider.gameObject.tag == "Wall"){
			if(firstTouch == null){
				firstTouch = collision.collider.gameObject;
			}
		}
	}
}