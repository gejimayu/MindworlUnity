using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	void Update () {
		transform.Rotate (new Vector3 (10, 30, 45) * Time.deltaTime);
	}

	void FixedUpdate () {
		input = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		rb.AddForce (input * moveSpeed);
	}

	void OnCollisionEnter(Collision other){
		Debug.Log ("oeoeoeoe");
		if(PlayerPrefs.HasKey("InMinigame")){
			Debug.Log("ada");
			Debug.Log(PlayerPrefs.GetInt("InMinigame"));
		}
		if (other.transform.tag == "Enemy") {
			Instantiate (deathParticles, transform.position, Quaternion.identity);
			lives = lives - 1;

			if (lives == 0) {
				notification.text = "You Lose !";

				SceneManager.LoadScene ("MainScene", LoadSceneMode.Single);
			}
			
			transform.position = spawn;
			lifebar.text = "Lives : " + lives.ToString ();
		} else if (other.transform.tag == "Goal") {
			notification.text = "You Win !";

			if (PlayerPrefs.GetInt ("InMinigame") == 1) {
				PlayerPrefs.SetInt ("IsWin1", 1);
				Debug.Log ("IsWin1");
				Debug.Log (PlayerPrefs.GetInt ("IsWin1"));
			} else if (PlayerPrefs.GetInt("InMinigame") == 2) {
				PlayerPrefs.SetInt ("IsWin2", 1);
				Debug.Log ("IsWin2");
				Debug.Log (PlayerPrefs.GetInt ("IsWin2"));
			}

			SceneManager.LoadScene ("MainScene", LoadSceneMode.Single);
		}
	}
}
