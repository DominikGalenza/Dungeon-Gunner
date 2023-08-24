using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
	#region Header BASIC LEVEL DETAILS
	[Space(10)]
	[Header("BASIC LEVEL DETAILS")]
	#endregion Header BASIC LEVEL DETAILS
	#region Tooltip
	[Tooltip("The name for the level")]
	#endregion Tooltip
	public string levelName;
	#region Header ROOM TEMPLATES FOR LEVEL
	[Space(10)]
	[Header("ROOM TEMPLATES FOR LEVEL")]
	#endregion Header ROOM TEMPLATES FOR LEVEL
	#region Tooltip
	[Tooltip("Populate the list with the room templates that you want to be part of the level. You need to ensure that room templates are included for all room node " +
		"types that are specified in the room node graphs for the level.")]
	#endregion Tooltip
	public List<RoomTemplateSO> roomTemplateList;
	#region Header ROOM NODE GRAPHS FOR LEVEL
	[Space(10)]
	[Header("ROOM NODE GRAPHS FOR LEVEL")]
	#endregion Header ROOM NODE GRAPHS FOR LEVEL
	#region Tooltip
	[Tooltip("Populate this list with the room node graphs which should be randomly selected from for the level.")]
	#endregion Tooltip
	public List<RoomNodeGraphSO> roomNodeGraphList;

	
	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
		if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
		{
			return;
		}
		if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
		{
			return;
		}

		bool isEWCorridor = false;
		bool isNSCorrdor = false;
		bool isEntrance = false;

		foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
		{
			if (roomTemplateSO == null)
			{
				return;
			}
			if (roomTemplateSO.roomNodeType.isCorridorEW)
			{
				isEWCorridor = true;
			}
			if (roomTemplateSO.roomNodeType.isCorridorNS)
			{
				isNSCorrdor = true;
			}
			if (roomTemplateSO.roomNodeType.isEntrance)
			{
				isEntrance = true;
			}
		}

		if (isEWCorridor == false)
		{
			Debug.Log($"In {name} : no E/W corridor room type specified.");
		}

		if (isNSCorrdor == false)
		{
			Debug.Log($"In {name} : no N/S corridor room type specified.");
		}

		if (isEntrance == false)
		{
			Debug.Log($"In {name} : no entrance corridor room type specified.");
		}

		foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
		{
			if (roomNodeGraph == null)
			{
				return;
			}

			foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
			{
				if (roomNode == null)
				{
					continue;
				}

				if (roomNode.roomNodeType.isEntrance || roomNode.roomNodeType.isCorridorEW || roomNode.roomNodeType.isCorridorNS || roomNode.roomNodeType.isCorridor || roomNode.roomNodeType.isNone)
				{
					continue;
				}

				bool isRoomNodeTypeFound = false;

				foreach (RoomTemplateSO roomTemplate in roomTemplateList)
				{
					if (roomTemplate == null)
					{
						continue;
					}

					if (roomTemplate.roomNodeType == roomNode.roomNodeType)
					{
						isRoomNodeTypeFound = true;
						break;
					}
				}

				if (!isRoomNodeTypeFound)
				{
					Debug.Log($"In {name} : no room template {roomNode.roomNodeType.name} found for nod graph {roomNodeGraph.name}.");
				}
			}
		}
	}
#endif
	#endregion Validation
}
