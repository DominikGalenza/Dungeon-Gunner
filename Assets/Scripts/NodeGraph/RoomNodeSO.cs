using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
	[HideInInspector] public string id;
	[HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
	[HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    public RoomNodeTypeSO roomNodeType;

    #region Editor Code

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    /// <summary>
    /// Initialise node
    /// </summary>
    public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodetypeList;
    }

    /// <summary>
    /// Draw node with the nodestyle
    /// </summary>
    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);
        EditorGUI.BeginChangeCheck();

        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
			int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
			int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
			roomNodeType = roomNodeTypeList.list[selection];
		}

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Populate a string array with the room node types to display that can be selected
    /// </summary>
	private string[] GetRoomNodeTypesToDisplay()
	{
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
	}

    /// <summary>
    /// Process events for the node
    /// </summary>
    public void ProcessEvents(Event current)
    {
        switch (current.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(current);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(current);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(current);
                break;
            default:
                break;
        }
    }

	/// <summary>
	/// Process mouse down events
	/// </summary>
	private void ProcessMouseDownEvent(Event current)
	{
        if (current.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if (current.button == 1)
        {
            ProcessRightClickDownEvent(current);
        }
	}

	/// <summary>
	/// Process left click down event
	/// </summary>
	private void ProcessLeftClickDownEvent()
	{
        Selection.activeObject = this;
        isSelected = !isSelected;
	}

    /// <summary>
    /// Process right click down event
    /// </summary>
	private void ProcessRightClickDownEvent(Event current)
	{
		roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, current.mousePosition);
	}

	/// <summary>
	/// Process mouse up event
	/// </summary>
	private void ProcessMouseUpEvent(Event current)
	{
        if (current.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
	}

    /// <summary>
    /// Process left click up event
    /// </summary>
	private void ProcessLeftClickUpEvent()
	{
		if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
	}

    /// <summary>
    /// Proces mouse drag event
    /// </summary>
	private void ProcessMouseDragEvent(Event current)
	{
		if (current.button == 0)
        {
            ProcessLeftMouseDragEvent(current);
        }
	}

    /// <summary>
    /// Process left mouse drag event
    /// </summary>
	private void ProcessLeftMouseDragEvent(Event current)
	{
        isLeftClickDragging = true;
        DragNode(current.delta);
        GUI.changed = true;
	}

    /// <summary>
    /// Drag node
    /// </summary>
	private void DragNode(Vector2 delta)
	{
        rect.position += delta;
        EditorUtility.SetDirty(this);
	}

    /// <summary>
    /// Add childID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
			childRoomNodeIDList.Add(childID);
			return true;
		}
        return false;
    }

    /// <summary>
    /// Check if the child node can be validly added to the parent node - return true if it can
    /// </summary>
	private bool IsChildRoomValid(string childID)
	{
        bool isConnectedBossNodeAlready = false;
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossNodeAlready = true;
            }
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
        {
            return false;
        }
        if (childRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        if (id == childID)
        {
            return false;
        }
        if (parentRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            return false;
        }
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
		{
			return false;
		}
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
		{
			return false;
		}
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
        {
            return false;
        }
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            return false;
        }
        return true;
	}

	/// <summary>
	/// Add parentID to the node (returns true if the node has been added, false otherwise)
	/// </summary>
	public bool AddParentRoomNodeIDToRoomNode(string parentID)
	{
		parentRoomNodeIDList.Add(parentID);
		return true;
	}
#endif

	#endregion
}
