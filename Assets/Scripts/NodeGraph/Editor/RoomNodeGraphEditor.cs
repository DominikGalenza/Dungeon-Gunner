using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;
using static PlasticGui.LaunchDiffParameters;

public class RoomNodeGraphEditor : EditorWindow
{
	private static RoomNodeGraphSO currentRoomNodeGraph;
	private RoomNodeTypeListSO roomNodeTypeList;
	private RoomNodeSO currentRoomNode = null;
	private GUIStyle roomNodeStyle;
	private GUIStyle roomNodeSelectedStyle;
	private const float nodeWidth = 160f;
	private const float nodeHeight = 75f;
	private const int nodePadding = 25;
	private const int nodeBorder = 12;
	private const float connectingLineWidth = 3f;
	private const float connectingLineArrowSize = 6f;

	[MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
	private static void OpenWindow()
	{
		GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
	}

	private void OnEnable()
	{
		Selection.selectionChanged += InspectorSelectionChanged;

		roomNodeStyle = new GUIStyle();
		roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
		roomNodeStyle.normal.textColor = Color.white;
		roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		roomNodeSelectedStyle = new GUIStyle();
		roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
		roomNodeSelectedStyle.normal.textColor = Color.white;
		roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		roomNodeTypeList = GameResources.Instance.roomNodetypeList;
	}

	private void OnDisable()
	{
		Selection.selectionChanged -= InspectorSelectionChanged;
	}

	/// <summary>
	/// Selection changed in the inspector
	/// </summary>
	private void InspectorSelectionChanged()
	{
		RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

		if (roomNodeGraph != null)
		{
			currentRoomNodeGraph = roomNodeGraph;
			GUI.changed = true;
		}
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
			DrawDraggedLine();
			ProcessEvent(Event.current);
			DrawRoomConnections();
			DrawRoomNodes();
		}
		if (GUI.changed)
		{
			Repaint();
		}
	}
	
	private void DrawDraggedLine()
	{
		if (currentRoomNodeGraph.linePosition != Vector2.zero)
		{
			Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, 
				currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
		}
	}

	private void ProcessEvent(Event current)
	{
		if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
		{
			currentRoomNode = IsMouseOverRoomNode(current);
		}

		if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
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
	/// Process mouse down events on the room node graph (not over a node)
	/// </summary>
	private void ProcessMouseDownEvent(Event current)
	{
		if (current.button == 0)
		{
			ClearLineDrag();
			ClearAllSelectedRoomNodes();
		}
		else if (current.button == 1)
		{
			ShowContextMenu(current.mousePosition);
		}
	}

	/// <summary>
	/// Process mouse up event
	/// </summary>
	private void ProcessMouseUpEvent(Event current)
	{
		if (current.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			RoomNodeSO roomNode = IsMouseOverRoomNode(current);

			if (roomNode != null)
			{
				if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
				{
					roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
				}
			}

			ClearLineDrag();
		}
	}

	/// <summary>
	/// Clear line drag rom a room node
	/// </summary>
	private void ClearLineDrag()
	{
		currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
		currentRoomNodeGraph.linePosition = Vector2.zero;
		GUI.changed = true;
	}

	/// <summary>
	/// Clear selection from all room nodes
	/// </summary>
	private void ClearAllSelectedRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected)
			{
				roomNode.isSelected = false;
				GUI.changed = true;
			}
		}
	}

	/// <summary>
	/// Process mouse drag event
	/// </summary>
	private void ProcessMouseDragEvent(Event current)
	{
		if (current.button == 1)
		{
			ProcessRightMouseDragEvent(current);
		}
	}

	/// <summary>
	/// Process right mouse drag event - draw line
	/// </summary>
	private void ProcessRightMouseDragEvent(Event current)
	{
		if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			DragConnectingLine(current.delta);
			GUI.changed = true;
		}
	}

	/// <summary>
	/// Drag connecting line from room node
	/// </summary>
	private void DragConnectingLine(Vector2 delta)
	{
		currentRoomNodeGraph.linePosition += delta;
	}

	/// <summary>
	/// Show the context menu
	/// </summary>
	private void ShowContextMenu(Vector2 mousePosition)
	{
		GenericMenu menu = new GenericMenu();
		menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
		menu.ShowAsContext();
	}

	/// <summary>
	/// Draw connections in the graph window between room nodes
	/// </summary>
	private void DrawRoomConnections()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.childRoomNodeIDList.Count > 0)
			{
				foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
				{
					if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
					{
						DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
						GUI.changed = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Draw connection line between the parent room node and child room node
	/// </summary>
	private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
	{
		Vector2 startPosition = parentRoomNode.rect.center;
		Vector2 endPosition = childRoomNode.rect.center;

		Vector2 midPosition = (startPosition + endPosition) / 2;
		Vector2 direction = endPosition - startPosition;
		
		Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
		Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
		Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

		Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);
		GUI.changed = true;
	}

	/// <summary>
	/// Select all room nodes
	/// </summary>
	private void SelectAllRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			roomNode.isSelected = true;
		}
		GUI.changed = true;
	}


	/// <summary>
	/// Create a room node at the mouse position
	/// </summary>
	private void CreateRoomNode(object mousePositionObject)
	{
		if (currentRoomNodeGraph.roomNodeList.Count == 0)
		{
			CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
		}

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

		currentRoomNodeGraph.OnValidate();
	}

	/// <summary>
	/// Draw room nodes in the graph window
	/// </summary>
	private void DrawRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected)
			{
				roomNode.Draw(roomNodeSelectedStyle);
			}
			else
			{
				roomNode.Draw(roomNodeStyle);
			}
		}
		GUI.changed = true;
	}
}
