﻿using UnityEngine;
using System.Collections;

/*
 * AuctionTimer has two parts. As soon as this object is instantiated, it starts counting.
 * Also, Button UI is also bound to this object.
 * 
 * This Class contains hardCoded part:
 * 1. Button pressing action does not detects the player
 * 2. Button pressing action cannot be done automatically
 * 
 * 
 * After done counting, it destroys itself.
 * 
 * */
public class AuctionTimer : MonoBehaviour {

	private float timeRemaining;	// time remaining
	private float speedMultiplier;
	private bool auctionInProcess;
	private bool buttonClicked;		
	private int transferID;

	private double timerStopTime;	//when buttonClicked=true, timer stops to 
	const double BUTTON_DELAY = 1;		// delay before destroying itself.

	// Use this for initialization. Default time and speed settings
	void Start () {
		timeRemaining = 100;
		speedMultiplier = 10;
		auctionInProcess = true;
		buttonClicked = false;
	}
	
	// Update is Implicitly called once per frame
	void Update () {
		if (!buttonClicked && GameMaster.getHighestBidValue () >= (int)timeRemaining && ((PlayerHand)GameMaster.searchDeckByID (GameMaster.getHighestBidderID ())).bidForAuction((int)timeRemaining)) {
			buttonClicked=true;
			auctionInProcess=false;
			timerStopTime = Time.time;
			transferID = GameMaster.getHighestBidderID ();
		}


		if (timeRemaining >= 10 && auctionInProcess) {
			timeRemaining -= Time.deltaTime * speedMultiplier;

		}
		else if (buttonClicked && timerStopTime+BUTTON_DELAY < Time.time)	//After delay time, it transfers card from auction deck to player hand.
		{
			//Debug.Log ("Deliver!!!");
			GameMaster.requestCardTransfer (100,transferID, true);
			((PlayerHand)GameMaster.searchDeckByID (transferID)).takeAuctionCard((int)timeRemaining);
			buttonClicked=false;
			Destroy (this,0.2f);
			GameMaster.terminateCurrentAuction();
		}
		else if (timeRemaining < 10)	// if timer counts down to zero, the object is destroyed and tells GameMaster to terminate auction.
		{
			Destroy (this,0.2f);
			GameMaster.terminateCurrentAuction();
		}
	}

	// override OnGUI() from monobehavior. Implicitly called by unity.
	// apparently, GUI is refreshed every frame.
	void OnGUI()
	{
		// Converts localPosition of the transform to position vector in screenSpace, then to GUI space. I found some unidentified bug, and it needs rectification.
		Vector3 pos = transform.localPosition;//(Vector2)Camera.WorldToScreenPoint(pos)
		Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
		if (timeRemaining>=10)
		{
			//If button is clicked, stops timer and waits for BUTTON_DELAY seconds to destroy itself..
			if (GUI.Button (new Rect(screenPos.x, Camera.main.pixelHeight-screenPos.y-100, 70, 35), "Bid for "+(int)timeRemaining) && !buttonClicked)
			{
				//Debug.Log ("Pressed!!!");
				//buttonClicked = true;
				((PlayerHand)GameMaster.searchDeckByID (GameMaster.UserID)).setBidValue ((int)timeRemaining);
				//auctionInProcess=false;
				//timerStopTime = Time.time;
			}
		}
		else{
			GUI.Button (new Rect(screenPos.x, Camera.main.pixelHeight-screenPos.y, 70, 35), "Bid over");
		}
	}
}
