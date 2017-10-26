using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	public Rigidbody rb;
	public GameObject deathParticles;
	private Vector3 spawn;
	private Vector3 input;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		spawn = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		input = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		rb.AddForce (input * moveSpeed);
	}

	void OnCollisionEnter(Collision other){
		if (other.transform.tag == "Enemy") {
			Instantiate (deathParticles, transform.position, Quaternion.identity);
			transform.position = spawn;
		} else if (other.transform.tag == "Goal") {
		}
	}
}
