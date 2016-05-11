using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using System.Math;

public class GameManager : MonoBehaviour {
	// global GameManager instance
	public static GameManager gameManager;

//	public GameObject startButton;
	public GameObject uiMenu;
	public GameObject uiPlaying;
	public TextMesh countdownText;
	public TextMesh scoreText;
	public TextMesh timerText;
	public TextMesh lifeText;
	public TextMesh gameOverText;
	public GameObject timerQuad;
	public GameObject timerBackQuad;
	public Enemy enemy;

	// game state
	private enum gameStates {Menu, Countdown, Playing, GameOver}; // , ToReplay
	private gameStates gameState;

	public int initLife = 3;
	public float initEnemySpeed = 5.0f;
	public float initEnemyDist = 50f;
	public float timerLength = 5.0f;

	private int score, highscore;
	private int lifeLeft;
	private float enemySpeed;
	private float countdown;
	private float roundTotalTime;
	private float roundElapsedTime;
	private float tempTimerLength;
	private float tempTimerBackLength;

	// Use this for initialization
	void Start () {
		if (gameManager == null)
			gameManager = this;
		//If gameManager already exists and it's not this:
		else if (gameManager != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);
		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);

		// initialize state
		setState(gameStates.Menu);

		highscore = PlayerPrefs.GetInt("High Score");
		resetGame ();
//		timeLeft = 10.0f;
//		scoreText = GetComponent<TextMesh>();
//		scoreText.text = score.ToString();
	}

	// Update is called once per frame
	void Update() {
		if (Cardboard.SDK.BackButtonPressed) {
			Application.Quit();
		}

		switch(gameState) {
		case gameStates.Menu:
			break;
		case gameStates.Countdown:
			countdown -= Time.deltaTime;
			countdownText.text = ((int)countdown + 1).ToString ();
			if (countdown <= 0) {
				startPlaying ();
			}
			break;
		case gameStates.Playing:
			roundElapsedTime += Time.deltaTime;
			tempTimerLength = timerLength * (roundTotalTime - roundElapsedTime) / roundTotalTime;
			tempTimerBackLength = timerLength * roundElapsedTime / roundTotalTime;
			timerQuad.transform.localScale = new Vector3(tempTimerLength, timerQuad.transform.localScale.y, timerQuad.transform.localScale.z);
			timerBackQuad.transform.localScale = new Vector3(tempTimerBackLength, timerBackQuad.transform.localScale.y, timerBackQuad.transform.localScale.z);
			timerQuad.transform.localPosition = new Vector3((-timerLength / 2) + (tempTimerLength / 2), timerQuad.transform.localPosition.y, timerQuad.transform.localPosition.z);
			timerBackQuad.transform.localPosition = new Vector3((timerLength / 2) - (tempTimerBackLength / 2), timerBackQuad.transform.localPosition.y, timerBackQuad.transform.localPosition.z);
			break;
		case gameStates.GameOver:
			break;
		}
	}

	// ==== state changer ====

	private void setState(gameStates newState) {
		gameState = newState;

		switch(newState) {
		case gameStates.Menu:
			uiMenu.SetActive (true);
			uiPlaying.SetActive (false);
			countdownText.GetComponent<Renderer>().enabled = false;
			gameOverText.GetComponent<Renderer>().enabled = false;
			timerQuad.SetActive (false);
			timerBackQuad.SetActive (false);
			enemy.setActive (false);
			break;
		case gameStates.Countdown:
			countdownText.GetComponent<Renderer>().enabled = true;
			gameOverText.GetComponent<Renderer>().enabled = false;
			uiMenu.SetActive (false);
			timerQuad.SetActive (false);
			timerBackQuad.SetActive (false);
			enemy.setActive (false);
			break;
		case gameStates.Playing:
			uiPlaying.SetActive (true);
			countdownText.GetComponent<Renderer>().enabled = false;
			timerQuad.SetActive (true);
			timerBackQuad.SetActive (true);
			enemy.setActive (true);
			break;
		case gameStates.GameOver:
			uiPlaying.SetActive (false);
			uiMenu.SetActive (true);
			timerQuad.SetActive (false);
			timerBackQuad.SetActive (false);
			enemy.setActive (false);
			gameOverText.GetComponent<Renderer>().enabled = true;
			break;
		}
	}

	// ==== public functions ====

	public void startGame() {
		countdown = 3.0f;
		setState (gameStates.Countdown);
	}

	public void enemyGetShot() {
		addScore (1);
		enemySpeed += 0.5f;
		enemy.setSpeed(enemySpeed);
		enemy.teleport ();
		resetTimer ();
	}

	public void playerGetShot() {
		loseLife ();
		enemy.teleport ();
		resetTimer ();
	}

	// ==== private functions ====

	private void resetGame() {
		score = 0;
		scoreText.text = score.ToString();

		lifeLeft = initLife;
		updateLifeText ();

		enemySpeed = initEnemySpeed;
		enemy.setDistance (initEnemyDist);
		enemy.setSpeed (enemySpeed);
		enemy.resetPosition();

		resetTimer ();
	}

	private void startPlaying() {
		resetGame ();
		setState (gameStates.Playing);
	}

	private void gameOver() {
		setState (gameStates.GameOver);
		gameOverText.text = "Game Over\nScore: " + score.ToString ();
	}
		
	private void loseLife() {
		lifeLeft -= 1;
		updateLifeText ();
		if (lifeLeft <= 0) {
			gameOver ();
		}
	}

	private void updateLifeText() {
		lifeText.text = new System.String('⋅', lifeLeft);
	}

	private void addScore(int scoreToAdd) {
		score += scoreToAdd;
		scoreText.text = score.ToString();
	}

	private void resetTimer() {
		roundTotalTime =  initEnemyDist / enemySpeed;
		roundElapsedTime = 0f;
	}
}
