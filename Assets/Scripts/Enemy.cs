using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public GameManager gm;
	public Transform target; // the object this object moving toward
	private float initDist = 50f;
	private float speed;

	// Use this for initialization
	void Start () {
		speed = 0f;
		if (target == null) {
			target = this.gameObject.transform;
			Debug.Log ("Enemy's target not specified. Defaulting to parent GameObject");
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
		// check if player get hit
		if (Vector3.Distance(transform.position, target.position) < 0.001f) {
			gm.playerGetShot ();
		}
	}

	public void setActive(bool isActive) {
		gameObject.GetComponent<Renderer>().enabled = isActive;
	}

	public void setDistance(float dist) {
		initDist = dist;
	}

	public void setSpeed(float _speed) {
		speed = _speed;
	}

	public void resetPosition() {
		transform.position = target.position + new Vector3 (0, 0, initDist);
	}

	public void teleport() {
		Vector3 rndOnHalfSphere = Random.onUnitSphere;
		rndOnHalfSphere.y = Mathf.Abs (rndOnHalfSphere.y);
		transform.position = target.position + rndOnHalfSphere * initDist;
	}
}
