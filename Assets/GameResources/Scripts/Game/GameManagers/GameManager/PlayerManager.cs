using Game.Bar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour {
	/* Fields */
	const int PLAYER_COUNT = 2;

	[SerializeField] PlayerInputManager manager;
	[SerializeField] PlayerInput playerPrefab;
	[SerializeField] Transform playerParent;

	[SerializeField] float startPlayerPos = 15;

	List<PlayerInput> players = new List<PlayerInput>();

	//-------------------------------------------------------------------
	/* Properties */

	//-------------------------------------------------------------------
	/* Messages */
	void Start()
	{
		for (int i = 0; i < PLAYER_COUNT; i++) {
			JoinPlayer();
		}
	}

	//-------------------------------------------------------------------
	/* Methods */
	void JoinPlayer()
	{
		string schemeName = (manager.playerCount == 0) ? "KeyboardLeft" : "KeyboardRight";
		PlayerInput player = JoinProcess(schemeName);
		PlayerInit(player);
	}

	PlayerInput JoinProcess(string schemeName)
	{
		var player = PlayerInput.Instantiate(playerPrefab.gameObject, controlScheme: schemeName, pairWithDevice: Keyboard.current);
		player.defaultControlScheme = schemeName;
		player.gameObject.transform.SetParent(playerParent);

		players.Add(player);
		return player;
	}

	void PlayerInit(PlayerInput player)
	{
		var signOfPosX = -1;
		var rotZ = 0;

		if (manager.playerCount == manager.maxPlayerCount) {
			signOfPosX = 1;
			rotZ = 180;
		}

		// Set position
		var playerPos = player.transform.position;
		playerPos.x = signOfPosX * startPlayerPos;
		player.transform.position = playerPos;

		// Set rotation
		player.transform.rotation = Quaternion.Euler(0, 0, rotZ);
	}
}
