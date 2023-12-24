using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
	public static LevelController _instance;
	[SerializeField] private int width, height;
	[SerializeField] private Sprite groundSprite;
	[SerializeField] private GameObject tilePrefab;
	private Dictionary<Tile, GameObject> tileGOMap;
	public List<Grid> allLevels;
	private Grid activeGrid;
	public Action<Grid> activeGridChangedCB;

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		tileGOMap = new Dictionary<Tile, GameObject>();
		allLevels = new List<Grid>();
		LoadGrid(CreateGrid(width, height));
	}

	public Grid CreateGrid(int Width = 25, int Height = 25)
	{
		var grid = new Grid(Width, Height);
		allLevels.Add(grid);
		return grid;
	}

	public Grid LoadGrid(Grid grid)
	{
		if (activeGrid != null)
		{
			UnloadActiveGrid();
		}
		activeGrid = grid;
		SimplePool.Preload(tilePrefab, activeGrid.width * activeGrid.height);
		for (var x = 0; x < activeGrid.width; x++)
		{
			for (var y = 0; y < activeGrid.height; y++)
			{
				var tile = SimplePool.Spawn(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
				var data = activeGrid.GetTileAt(x, y);
				tile.name = $"tile_{x}_{y}";
				tileGOMap.Add(data, tile);
				data.setTTCCB(UpdateTile);
				UpdateTile(data);
			}
		}
		activeGridChangedCB(activeGrid);
		return activeGrid;
	}

	public void UnloadActiveGrid()
	{
		while (tileGOMap.Count>0)
		{
			SimplePool.Despawn(tileGOMap.First().Value);
			tileGOMap.Remove(tileGOMap.First().Key);
		}
		activeGrid = null;
		activeGridChangedCB(activeGrid);
	}

	public bool GetTile(int x, int y)
	{
		return activeGrid.GetTile(x, y);
	}
	
	public void ChangeTileType(TileType tileType,int x, int y)
	{
		activeGrid.GetTileAt(x, y).TileType = tileType;
	}

	public void RandomizeTiles(float randomizeEmptyChance)
	{
		for (var x = 0; x < activeGrid.width; x++)
		{
			for (var y = 0; y < activeGrid.height; y++)
			{
				activeGrid.GetTileAt(x, y).TileType = Random.Range(0, 100) > randomizeEmptyChance ? TileType.ground : TileType.empty;
			}
		}
	}
	
	private void UpdateTile(Tile data)
	{
		var go = tileGOMap[data];
		var sr = go.GetComponent<SpriteRenderer>();
		sr.sprite = data.TileType switch
		{
			TileType.ground => groundSprite,
			TileType.empty => null,
			_ => sr.sprite
		};
	}
}