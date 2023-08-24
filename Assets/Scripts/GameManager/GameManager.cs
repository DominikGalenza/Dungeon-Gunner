using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
	#region Header DUNGEON LEVELS
	[Space(10)]
	[Header("DUNGEON LEVELS")]
	#endregion Header DUNGEON LEVELS
	#region Tooltip
	[Tooltip("Populate with the dungeon level scriptable objects")]
	#endregion Tooltip
	[SerializeField] private List<DungeonLevelSO> dungeonLevelList;
	#region Tooltip
	[Tooltip("Populate with the starting dungeon level for testing, first level = 0")]
	#endregion Tooltip
	[SerializeField] private int currentDungeonLevelListIndex = 0;
	[HideInInspector] public GameState gameState;

	private void Start()
	{
		gameState = GameState.gameStarted;
	}

	private void Update()
	{
		HandleGameState();
	}

	/// <summary>
	/// Handle game state
	/// </summary>
	private void HandleGameState()
	{
		switch (gameState)
		{
			case GameState.gameStarted:
				PlayDungeonLevel(currentDungeonLevelListIndex);
				gameState = GameState.playingLevel; 
				break;
		}
	}

	private void PlayDungeonLevel(int currentDungeonLevelListIndex)
	{
		throw new NotImplementedException();
	}

	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
	}
#endif
	#endregion Validation
}
