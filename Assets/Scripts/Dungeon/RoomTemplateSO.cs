using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;
	#region Header ROOM PREFAB
	[Space(10)]
	[Header("ROOM PREFAB")]
	#endregion Header ROOM PREFAB
	#region Tooltip
	[Tooltip("The game object prefab for the room (this will contain all the tilemaps for the room and environment game objects")]
	#endregion Tooltip
	public GameObject roomPrefab;
	[HideInInspector] public GameObject previousPrefab; //this is used to regenerate the guid if the SO is copied and the prefab is changed
	#region Header ROOM CONFIGURATION
	[Space(10)]
	[Header("ROOM CONFIGURATION")]
	#endregion Header ROOM CONFIGURATION
	#region Tooltip
	[Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph. The exceptions being with corridors. " +
		"In the room node graph there is just one corridor type 'Corridor'. For the room templates there are 2 corridor node types - CorridorNS and CorrdorEW.")]
	#endregion Tooltip
	public RoomNodeTypeSO roomNodeType;
	#region Tooltip
	[Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. " +
		"This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner " +
		"(note: this is the local tilemap position and NOT world position).")]
	#endregion Tooltip
	public Vector2Int lowerBounds;
	#region Tooltip
	[Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the top right corner of that rectangle. " +
		"This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner " +
		"(note: this is the local tilemap position and NOT world position).")]
	#endregion Tooltip
	public Vector2Int upperBounds;
	#region Tooltip
	[Tooltip("There should be a maximum of four doorways for a room - one for each compass direction. These should have a consistent 3 tile opening size, with the middle " +
		"tile position being the doorway coordinate 'position;")]
	#endregion Tooltip
	[SerializeField] public List<Doorway> doorwayList;
	#region Tooltip
	[Tooltip("Each possible spawn position (used for enemies and chests) for the room in tilemap coordinates should be added to this array")]
	#endregion Tooltip
	public Vector2Int[] spawnPositionArray;

	/// <summary>
	/// Returns the list of entrances for the room template
	/// </summary>
	public List<Doorway> GetDoorwayList()
	{
		return doorwayList;
	}

	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		if (guid == "" || previousPrefab != roomPrefab)
		{
			guid = GUID.Generate().ToString();
			previousPrefab = roomPrefab;
			EditorUtility.SetDirty(this);
		}
		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);
		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
	}
#endif
	#endregion Validation
}
