using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    PlayerController[] Players;
    string s;

    // Use this for initialization
    void Start () {
        Players = FindObjectsOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        Players = FindObjectsOfType<PlayerController>();
        GetComponent<Text>().text = "Player\tKills\tDeaths";
        foreach (PlayerController p in Players)
        {
            GetComponent<Text>().text += "\n" + p.PlayerName + "\t" + p.Kills + "\t" + p.Deaths;
        }
    }
}
