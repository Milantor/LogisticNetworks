using System;
using UnityEngine;

public enum TileType
{
	empty,
	ground,
	water
}

public class Tile
{
	private Grid parent;
	private TileType tileType = TileType.empty;

	private Action<Tile> TTCCB;

	public TileType TileType
	{
		get => tileType;
		set
		{
			var oldTT = tileType;
			tileType = value;
			if (oldTT != value)
				TTCCB(this);
		}
	}

	public Tile(Grid parent)
	{
		this.parent = parent;
	}

	public void setTTCCB(Action<Tile> cb)
	{
		TTCCB = cb;
	}
}
