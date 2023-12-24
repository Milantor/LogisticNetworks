using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelController))]
public class LevelControllerUI : Editor
{
	private int GridId;
	private float randomizeEmptyChance = 20f;

	public override void OnInspectorGUI()
	{
		var origin = (LevelController)target;
		DrawDefaultInspector();	
		
		EditorGUILayout.Space(4);
		EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
		
		randomizeEmptyChance = EditorGUILayout.FloatField("Randomize empty chance: ", randomizeEmptyChance);
		if (GUILayout.Button("Randomize tiles"))
		{
			origin.RandomizeTiles(randomizeEmptyChance);
		}
		
		EditorGUILayout.Space(4);
		GridId = EditorGUILayout.IntField("Selected grid: ", GridId);
		
		if (GUILayout.Button("Create new Grid"))
		{
			if (GridId < 0)
			{
				return;
			}
			if (GridId>=origin.allLevels.Count)
			{
				origin.CreateGrid();
				GridId = origin.allLevels.Count-1;
			}
			else
			{
				throw new ArgumentOutOfRangeException($"Grid with id {GridId} already exists!");
			}
		}
		
		if (GUILayout.Button("Load Grid"))
		{
			if(GridId<origin.allLevels.Count)
				origin.LoadGrid(origin.allLevels[GridId]);
			else
			{
				throw new IndexOutOfRangeException($"Grid with id {GridId} do not exists!");
			}
		}

		if (GUILayout.Button("Unload Grid"))
		{
			origin.UnloadActiveGrid();
		}
	}
}
