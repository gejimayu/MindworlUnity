using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	public Rigidbody rb;
	public GameObject deathParticles;
	public Text lifebar;
	public Text notification;
	private Vector3 spawn;
	private Vector3 input;
	private int lives;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		spawn = transform.position;
		lives = 5;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		input = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		rb.AddForce (input * moveSpeed);
	}

	void OnCollisionEnter(Collision other){
		if (other.transform.tag == "Enemy") {
			Instantiate (deathParticles, transform.position, Quaternion.identity);
			lives = lives - 1;

			if (lives == 0) 
				notification.text = "You Lose !";
			
			transform.position = spawn;
			lifebar.text = "Lives : " + lives.ToString ();
		} else if (other.transform.tag == "Goal") {
			notification.text = "You Win !";
		}
	}
}
