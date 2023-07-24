using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

	private void Awake()
	{
        LoadRoomNodeDictionary();
	}

	/// <summary>
	/// Load the room node dictionary from the room node list.
	/// </summary>
	private void LoadRoomNodeDictionary()
	{
		roomNodeDictionary.Clear();

		foreach (RoomNodeSO node in roomNodeList)
		{
			roomNodeDictionary[node.id] = node;
		}
	}

	/// <summary>
	/// Get room node by room node ID
	/// </summary>
	public RoomNodeSO GetRoomNode(string roomNodeID)
	{
		if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
		{
			return roomNode;
		}
		return null;
	}

	#region Editor Code

#if UNITY_EDITOR
	[HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

	public void OnValidate()
	{
		LoadRoomNodeDictionary();
	}

	public void SetNodeToDrawConnectionLineFrom(RoomNodeSO roomNode, Vector2 position)
    {
        roomNodeToDrawLineFrom = roomNode;
        linePosition = position;
    }
    
#endif

	#endregion
}
