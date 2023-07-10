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

        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
        roomNodeType = roomNodeTypeList.list[selection];

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
#endif

	#endregion
}
