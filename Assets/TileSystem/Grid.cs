using System;
using UnityEngine;

public class Grid
{
	private Tile[,] tiles;
	public int width { get; }
	public int height{ get; }

	public Grid(int width, int height)
	{
		this.width = width;
		this.height = height;
		tiles = new Tile[this.width, this.height];

		for (var x = 0; x < this.width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				tiles[x, y] = new Tile(this);
			//	Debug.Log($"New tile at {x}:{y}");
			}
		}
		Debug.Log($"Successful grid creating with {this.width * this.height} tiles");
	}

	public Tile GetTileAt(int x, int y)
	{
		if (x >= width || x < 0 || y >= height || y < 0)
			throw new IndexOutOfRangeException($"Tile {x}:{y} not exists!");	
		return tiles[x, y];
	}
	public bool GetTile(int x, int y)
	{
		return x < width && x >= 0 && y < height && y >= 0;
	}
}
