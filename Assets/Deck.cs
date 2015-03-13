﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	
	public List<GameObject> cards;			// collection of gameObjects references (cards). List should be quite similar to Vector
	public int deckID;
	private Transform referenceTransform;	// this class includes reference position, scale and rotation of entire deck. Individual cards will be positioned based on this reference transform
	private int currentLayoutType;	//current layout type


	public void transferTopCardTo(Deck another)
	{
		if (cards.Count>0)
		{
			Debug.Log ("Card count = " + cards.Count);
			Debug.Log ("card to be transferred : Rank = " + cards[cards.Count-1].GetComponent <Card>().Rank);
			another.addExistingCard (cards[cards.Count-1]);
			cards.Remove (cards [cards.Count - 1]);
			setupLayout(currentLayoutType);
		}
	}

	/*
	 * type 0 = collapse
	 * type 1 = spread horizontal, right
	 * type 2 = spread horizontal, from middle
	 * type 3 = complete seperation, to the right
	 * type 4 = complete seperation, from middle
	 * type 5 = hand spread, with angle. Spread to right.
	 * type 6 = hand spread, with angle. Spreads from middle.
	 * */
	public void setupLayout(int type)
	{
		setScaleAndOrder ();
		currentLayoutType = type;
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().setTargetPos(computeIndividualCardTargetPos (type, i));
		//Debug.Log ("Layout "+currentLayoutType+" setup for Deck ID = " + deckID);
	}
	public void setLayoutType(int type)
	{
		currentLayoutType = type;
	}

	public void shuffle()
	{
		for (int i=0; i<cards.Count; i++)
		{
			int swap_target = Random.Range (0,cards.Count);
			GameObject temp = cards[i];
			cards[i] = cards[swap_target];
			cards[swap_target] = temp;
		}
		setScaleAndOrder ();
		setupLayout (currentLayoutType);
	}

	public void openDeck()	// show face of all cards in deck
	{
		setScaleAndOrder ();
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().showFace ();
	}

	public void hideDeck()	// show background for all cards in deck
	{
		setScaleAndOrder ();
		for (int i=0; i<cards.Count; i++)
			cards[i].GetComponent<Card>().showBackground ();
	}

	public void generateFullCardDeck()
	{
		for (int i=1; i<=4; ++i)
			for (int j=1; j<=13; ++j)
				new_card(j,i);
	}

	private void setScaleAndOrder()
	{
		for (int i=0; i<cards.Count; i++)
		{
			cards [i].GetComponent<Card> ().setSortingOrder (i);
			cards[i].GetComponent<Transform>().localScale = referenceTransform.localScale;
		}
	}

	public void new_card(int rank, int suit)	// create a new card object into the scene, and adds its reference to the cardlist.
	{
		GameObject newCard = (GameObject)Instantiate (Resources.Load ("prefab/card"), referenceTransform.localPosition, Quaternion.identity);
		// instantiate() creates an object into the scene. Then, it returns Object class.
		newCard.GetComponent<Card>().rank = rank;	//set rank and suit
		newCard.GetComponent<Card>().suit = suit;
		newCard.GetComponent <Transform>().parent = (Transform)this.GetComponent <Transform>();// let all new cards become a child of this deck.
		newCard.GetComponent<Card>().showFace ();
		cards.Add (newCard);	// add newCard to list of cards.
		//Debug.Log (cards.Count + " cards count ");
		setupLayout(currentLayoutType);
	}

	public void addExistingCard(GameObject card)	//add existing card. It should be distinguished from new_card
	{
		cards.Add (card);
		card.GetComponent <Transform>().parent = (Transform)this.GetComponent <Transform>();
		setupLayout (currentLayoutType);
		Debug.Log ("control transfer successful");
	}


	public void destroyAll()
	{
		while (cards.Count>0)
		{
			Destroy (cards[0].gameObject);
			cards.Remove (cards[0]);
		}
		Debug.Log ("After destroying everything, "+cards.Count + " cards Left ");
	}
	
	private Vector3[] computeIndividualCardTargetPos(int orientationType, int indexReference)	//returns target vector for each cards. Temporary solution
	{
		// 0th index represents position.
		// 1th index represents Euler rotation.
		Vector3[] pos = new Vector3[2];
		pos[0] = new Vector3(0,0,0);
		pos [1] = new Vector3 (0,0,0);
		switch (orientationType)
		{
		case 1:
			pos[0] = new Vector3 (indexReference*0.3f,0,0.001f);
			break;
		case 2:
			pos[0] = new Vector3 (indexReference*0.3f - (0.3f*(cards.Count-1))*0.5f,0,0.001f);
			break;
		case 3:
			pos[0] = new Vector3 (indexReference*1f,0,0.001f);
			break;
		case 4:
			pos[0] = new Vector3 (indexReference*1f - (1f*(cards.Count-1))*0.5f,0,0.001f);
			break;
		case 5:
			pos[0] = new Vector3 (indexReference*0.3f,0,0);
			pos[1] = new Vector3 (indexReference*(-120f)/(cards.Count),0,0.001f);
			break;
		case 6:			// DO NOT USE THIS WITH ANY MORE THAN 100 CARDS
			float maximumTilt = 30f;
			maximumTilt *= Mathf.Sqrt (cards.Count)/7;		//maximum angle normalization
			float middleCardIndex = cards.Count/2f-0.5f;		//reference point for yDist=0
			float yDist = Mathf.Abs(indexReference-middleCardIndex);	//Y distance from middle
			yDist = (1-Mathf.Cos(yDist/(30f)))*3f;		// y distance transformed from linear to curve
			pos[0] = new Vector3 (indexReference*0.3f - (0.3f*(cards.Count-1))*0.5f,-yDist,0.001f);
			pos[1] = new Vector3 (0,0,((indexReference+0.5f)*(-maximumTilt))/(cards.Count) + (maximumTilt/2));
			break;
		case 7:			// Non-normalized version of case 6
			float maximumTilt1 = 50f;
			float middleCardIndex1 = cards.Count/2f-0.5f;		//reference point for yDist=0
			float yDist1 = Mathf.Abs(indexReference-middleCardIndex1);	//Y distance from middle
			yDist1 = (1-Mathf.Cos(yDist1/(10f)))*3f;		// y distance transformed from linear to curve
			pos[0] = new Vector3 (indexReference*0.2f - (0.2f*(cards.Count-1))*0.5f,-yDist1,0.001f);
			pos[1] = new Vector3 (0,0,((indexReference+0.5f)*(-maximumTilt1))/(cards.Count) + (maximumTilt1/2));
			break;
		default:
			pos[0] = new Vector3 (indexReference*0.0015f,indexReference*0.0015f,0);
			break;
		}
		return pos;
	}

	// Use this for initialization
	void Start () {
		//deckID = 99;
		cards = new List<GameObject>();
		referenceTransform = GetComponent<Transform> ();
		GameMaster.reportDeckToGameMaster (this.gameObject);

	}
	
	// Update is called once per frame
	void Update () {
	}


}
