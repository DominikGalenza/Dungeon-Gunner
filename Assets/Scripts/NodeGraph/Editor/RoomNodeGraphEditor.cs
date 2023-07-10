using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
	private static RoomNodeGraphSO currentRoomNodeGraph;
	private RoomNodeTypeListSO roomNodeTypeList;
	private RoomNodeSO currentRoomNode = null;
	private GUIStyle roomNodeStyle;
	private const float nodeWidth = 160f;
	private const float nodeHeight = 75f;
	private const int nodePadding = 25;
	private const int nodeBorder = 12;

	[MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
	private static void OpenWindow()
	{
		GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
	}

	private void OnEnable()
	{
		roomNodeStyle = new GUIStyle();
		roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
		roomNodeStyle.normal.textColor = Color.white;
		roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		roomNodeTypeList = GameResources.Instance.roomNodetypeList;
	}

	/// <summary>
	/// Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
	/// </summary>
	[OnOpenAsset(0)]
	public static bool OnDoubleClickAsset(int instanceID, int line)
	{
		RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

		if (roomNodeGraph != null)
		{
			OpenWindow();
			currentRoomNodeGraph = roomNodeGraph;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Draw Editor GUI
	/// </summary>
	private void OnGUI()
	{
		if (currentRoomNodeGraph != null)
		{
			ProcessEvent(Event.current);
			DrawRoomNodes();
		}
		if (GUI.changed)
		{
			Repaint();
		}
	}	

	private void ProcessEvent(Event current)
	{
		if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
		{
			currentRoomNode = IsMouseOverRoomNode(current);
		}

		if (currentRoomNode == null)
		{
			ProcessRoomNodeGraphEvents(current);
		}
		else
		{
			currentRoomNode.ProcessEvents(current);
		}
	}

	/// <summary>
	/// Check to see if mouse is over a room node - if so then return the room node else return null
	/// </summary>
	private RoomNodeSO IsMouseOverRoomNode(Event current)
	{
		for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
		{
			if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(current.mousePosition))
			{
				return currentRoomNodeGraph.roomNodeList[i];
			}
		}
		return null;
	}

	/// <summary>
	/// Process Room Node Graph Events
	/// </summary>
	private void ProcessRoomNodeGraphEvents(Event current)
	{
		switch (current.type)
		{
			case EventType.MouseDown:
				ProcessMouseDownEvent(current);
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Process mouse down events on the room node graph (not over a node)
	/// </summary>
	private void ProcessMouseDownEvent(Event current)
	{
		if (current.button == 1)
		{
			ShowContextMenu(current.mousePosition);
		}
	}
	
	/// <summary>
	/// Show the context menu
	/// </summary>
	private void ShowContextMenu(Vector2 mousePosition)
	{
		GenericMenu menu = new GenericMenu();
		menu.AddItem(new GUIContent("Creat Room Node"), false, CreateRoomNode, mousePosition);
		menu.ShowAsContext();
	}

	/// <summary>
	/// Create a room node at the mouse position
	/// </summary>
	private void CreateRoomNode(object mousePositionObject)
	{
		CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
	}

	/// <summary>
	/// Create a room node at the mouse position - overloaded to also pass in RoomNodeType
	/// </summary>
	private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
	{
		Vector2 mousePositon = (Vector2)mousePositionObject;
		RoomNodeSO roomNode = CreateInstance<RoomNodeSO>();
		currentRoomNodeGraph.roomNodeList.Add(roomNode);

		roomNode.Initialise(new Rect(mousePositon, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);
		AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
		AssetDatabase.SaveAssets();
	}

	/// <summary>
	/// Draw room nodes in the graph window
	/// </summary>
	private void DrawRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			roomNode.Draw(roomNodeStyle);
		}
		GUI.changed = true;
	}
}
