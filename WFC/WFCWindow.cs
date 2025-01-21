using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace WFC;

public sealed class WFCWindow(int width, int height) : PixelWindow.PixelWindow(
	new NativeWindowSettings
	{
		Title = "WFC",
		ClientSize = new Vector2i(width, height),
		IsEventDriven = false,
		MaximumClientSize = new Vector2i(width, height),
		MinimumClientSize = new Vector2i(width, height),
	})
{
	private const int Resolution = 48;

	private readonly int[][,] _templates =
	[
		new[,]
		{
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1 },
		},
		// Empty
		new[,]
		{
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
		},
		// Vertical
		new[,]
		{
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		},
		// Horizontal
		new[,]
		{
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
		},
		// TopLeftCorner
		new[,]
		{
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 1, 1, 1, 1, 1, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
		},
		// TopRightCorner
		new[,]
		{
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 1 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 1, 1, 1, 1, 1, 1 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
		},
		// BottomLeftCorner
		new[,]
		{
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 1, 1, 1, 1, 1, 1, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		},
		// BottomRightCorner
		new[,]
		{
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 1, 1, 1, 1, 1, 1, 1 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 1, 1 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		},
		// UpTJoint
		new[,]
		{
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 2, 2, 2, 2, 1, 1 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
		},
		// DownTJoint
		new[,]
		{
			{ 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 1, 1, 2, 2, 2, 2, 1, 1 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		},
		// LeftTJoint
		new[,]
		{
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		},
		// RightTJoint
		new[,]
		{
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 1 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 2, 2 },
			{ 0, 1, 2, 2, 2, 2, 1, 1 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		}
	];

	public enum TileType
	{
		NotAssigned          = 0,
		Empty                = 1,
		Vertical             = 2,
		Horizontal           = 3,
		TopLeftCorner        = 4,
		TopRightCorner       = 5,
		BottomLeftCorner     = 6,
		BottomRightCorner    = 7,
		UpTJoint             = 8,
		DownTJoint           = 9,
		LeftTJoint           = 10,
		RightTJoint          = 11
	}

	protected override unsafe void OnLoad()
	{
		base.OnLoad();
		TextureSize = ClientSize;
		fixed (byte* dataPtr = Pixels)
			Data = dataPtr;
		var thread = new Thread(WFC);
		thread.Start();
	}

	private void WFC()
	{
		var neighbours = new PriorityQueue<(int X, int Y), int>();
		// pick a random square
		var x = Random.Shared.Next(width / Resolution);
		var y = Random.Shared.Next(height / Resolution);
		// pick a random square
		var enqueuedCoords = new HashSet<(int X, int Y)>();
		// collapse it to some random value
		var chosenState = (TileType)Random.Shared.Next(1, 11);
		Tiles[x, y] = chosenState;
		_updated = true;
		foreach (var neighbor in GetNeighbours(x, y))
		{
			if (enqueuedCoords.Add(neighbor)) // only add if it's not already in the set
			{
				var priority = GetPossibleStates(neighbor.X, neighbor.Y).Length;
				neighbours.Enqueue(neighbor, priority);
			}
		}

		while (neighbours.TryDequeue(out var neighbour, out _))
		{
			// Remove from our tracking set since we're processing it
			enqueuedCoords.Remove(neighbour);
        
			var states = GetPossibleStates(neighbour.X, neighbour.Y);
			if (states.Length == 0)
			{
				Console.WriteLine("fucky wucky");
				Tiles = new TileType[width / Resolution, height / Resolution];
				var thread = new Thread(WFC);
				thread.Start();
				return;
			}

			var state = states.Random();
			Tiles[neighbour.X, neighbour.Y] = state;
        
			foreach (var newNeighbor in GetNeighbours(neighbour.X, neighbour.Y))
			{
				if (enqueuedCoords.Add(newNeighbor)) // only add if not already in queue
				{
					var priority = GetPossibleStates(newNeighbor.X, newNeighbor.Y).Length;
					neighbours.Enqueue(newNeighbor, priority);
				}
			}
			_updated = true;
		}
		_done = true;
	}

	public TileType[] GetPossibleStates(int x, int y)
	{
		var possibleStates = new List<TileType>
		{
			TileType.Empty, TileType.Horizontal, TileType.Vertical, TileType.TopLeftCorner, TileType.TopRightCorner,
			TileType.BottomLeftCorner, TileType.BottomRightCorner, TileType.LeftTJoint, TileType.RightTJoint,
			TileType.UpTJoint, TileType.DownTJoint
		};
		// copy and paste ik, idc
		// check if top exists
		if (y > 0 && Tiles[x, y - 1] is not TileType.NotAssigned)
		{
			var tileType = Tiles[x, y - 1];
			switch (tileType)
			{
				case TileType.NotAssigned:
					throw new UnreachableException();
				// if the top tile is one of these then the current tile must connect with it
				case TileType.Vertical:
				case TileType.BottomRightCorner:
				case TileType.DownTJoint:
				case TileType.BottomLeftCorner:
				case TileType.LeftTJoint:
				case TileType.RightTJoint:
					// so remove all those that dont connect
					possibleStates.Remove(TileType.Empty);
					possibleStates.Remove(TileType.Horizontal);
					possibleStates.Remove(TileType.BottomLeftCorner);
					possibleStates.Remove(TileType.BottomRightCorner);
					possibleStates.Remove(TileType.DownTJoint);
					break;
				// if the top tile is one of these then the current tile must not connect with it
				case TileType.Empty:
				case TileType.Horizontal:
				case TileType.TopLeftCorner:
				case TileType.TopRightCorner:
				case TileType.UpTJoint:
					// so remove all those that connect
					possibleStates.Remove(TileType.Vertical);
					possibleStates.Remove(TileType.TopLeftCorner);
					possibleStates.Remove(TileType.TopRightCorner);
					possibleStates.Remove(TileType.LeftTJoint);
					possibleStates.Remove(TileType.RightTJoint);
					possibleStates.Remove(TileType.UpTJoint);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		// check if bottom exists
		if (y < height / Resolution - 1 && Tiles[x, y + 1] is not TileType.NotAssigned)
		{
			var tileType = Tiles[x, y + 1];
			switch (tileType)
			{
				case TileType.NotAssigned:
					throw new UnreachableException();
				// if the bottom tile is one of these then the current tile must not connect with it
				case TileType.Empty:
				case TileType.Horizontal:
				case TileType.BottomLeftCorner:
				case TileType.BottomRightCorner:
				case TileType.DownTJoint:
					// so remove all that connect
					possibleStates.Remove(TileType.Vertical);
					possibleStates.Remove(TileType.BottomLeftCorner);
					possibleStates.Remove(TileType.BottomRightCorner);
					possibleStates.Remove(TileType.LeftTJoint);
					possibleStates.Remove(TileType.RightTJoint);
					possibleStates.Remove(TileType.DownTJoint);
					break;
				// if the bottom tile is one of these then the current tile must connect with it
				case TileType.LeftTJoint:
				case TileType.RightTJoint:
				case TileType.UpTJoint:
				case TileType.TopRightCorner:
				case TileType.TopLeftCorner:
				case TileType.Vertical:
					// so remove all those that dont connect
					possibleStates.Remove(TileType.Empty);
					possibleStates.Remove(TileType.Horizontal);
					possibleStates.Remove(TileType.TopLeftCorner);
					possibleStates.Remove(TileType.TopRightCorner);
					possibleStates.Remove(TileType.UpTJoint);
					break;
			}
		}
		
		// check if left exists
		if (x > 0 && Tiles[x - 1, y] is not TileType.NotAssigned)
		{
			var tileType = Tiles[x - 1, y];
			switch (tileType)
			{
				case TileType.NotAssigned:
					throw new UnreachableException();
				// if left is one of these then the current tile must connect with it
				case TileType.TopRightCorner:
				case TileType.BottomRightCorner:
				case TileType.Horizontal:
				case TileType.UpTJoint:
				case TileType.RightTJoint:
				case TileType.DownTJoint:
					// so remove those that dont connect
					possibleStates.Remove(TileType.Empty);
					possibleStates.Remove(TileType.Vertical);
					possibleStates.Remove(TileType.TopRightCorner);
					possibleStates.Remove(TileType.BottomRightCorner);
					possibleStates.Remove(TileType.RightTJoint);
					
					break;
				// if the left is one of these then the current tile must not connect with it
				case TileType.Empty:
				case TileType.Vertical:
				case TileType.TopLeftCorner:
				case TileType.BottomLeftCorner:
				case TileType.LeftTJoint:
					// so remove those that connect
					possibleStates.Remove(TileType.Horizontal);
					possibleStates.Remove(TileType.TopLeftCorner);
					possibleStates.Remove(TileType.BottomLeftCorner);
					possibleStates.Remove(TileType.UpTJoint);
					possibleStates.Remove(TileType.DownTJoint);
					possibleStates.Remove(TileType.LeftTJoint);
					break;
				
			}
		}
		
		// check if right exists
		if (x < width / Resolution - 1 && Tiles[x + 1, y] is not TileType.NotAssigned)
		{
			var tileType = Tiles[x + 1, y];
			switch (tileType)
			{
				case TileType.NotAssigned:
					throw new UnreachableException();
				// if the right tile is one of these then the current tile must connect with it
				case TileType.Horizontal:
				case TileType.BottomLeftCorner:
				case TileType.TopLeftCorner:
				case TileType.UpTJoint:
				case TileType.DownTJoint:
				case TileType.LeftTJoint:
					// so remove that dont connect
					possibleStates.Remove(TileType.Empty);
					possibleStates.Remove(TileType.Vertical);
					possibleStates.Remove(TileType.TopLeftCorner);
					possibleStates.Remove(TileType.BottomLeftCorner);
					possibleStates.Remove(TileType.LeftTJoint);
					break;
				// if the right tile is one of these then the current tile must not connect with it
				case TileType.Empty:
				case TileType.TopRightCorner:
				case TileType.BottomRightCorner:
				case TileType.Vertical:
				case TileType.RightTJoint:
					// so remove those that connect with it
					possibleStates.Remove(TileType.Horizontal);
					possibleStates.Remove(TileType.TopRightCorner);
					possibleStates.Remove(TileType.BottomRightCorner);
					possibleStates.Remove(TileType.UpTJoint);
					possibleStates.Remove(TileType.DownTJoint);
					possibleStates.Remove(TileType.RightTJoint);
					break;
			}
		}
		return possibleStates.ToArray();
	}

	public List<(int X, int Y)> GetNeighbours(int x, int y)
	{
		var neighbours = new List<(int, int)>();
		// check if top exists
		if (y > 0 && Tiles[x, y-1] is TileType.NotAssigned)
			neighbours.Add((x, y-1));
		
		// check if bottom exists
		if (y < height/Resolution - 1 && Tiles[x, y+1] is TileType.NotAssigned)
			neighbours.Add((x, y+1));
		
		// check if left exists
		if (x > 0 && Tiles[x-1, y] is TileType.NotAssigned)
			neighbours.Add((x-1, y));
		
		// check if right exists
		if (x < width/Resolution -1 && Tiles[x+1, y] is TileType.NotAssigned)
			neighbours.Add((x+1, y));
		
		return neighbours;
	}

	public readonly byte[] Pixels = new byte[width * height * 4];

	public TileType[,] Tiles = new TileType[width / Resolution, height / Resolution];
	private bool _done;
	private bool _updated;

	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		if (_updated)
			BuildPixels();
	}

	public unsafe void BuildPixels()
	{
		_updated = false;
		const int pixelsPerTemplateCell = Resolution / 8;

		for (var tileY = 0; tileY < height / Resolution; tileY++)
		{
			for (var tileX = 0; tileX < width / Resolution; tileX++)
			{
				var tile = Tiles[tileX, tileY];
				var template = _templates[(int)tile];

				for (var templateY = 0; templateY < 8; templateY++)
				{
					for (var templateX = 0; templateX < 8; templateX++)
					{
						var templateValue = template[templateY, templateX];
						
						var color = templateValue switch
						{
							//ABGR FOR SOME FUCKING REASON (ik its cuz little endian)
							-1 => ~0x00FFFFFFU,
							0  => ~0xFF00FF00U,
							1  => ~0xFFFFFFFFU,
							2  => ~0xFF0000FFU, 
							_  => throw new ArgumentOutOfRangeException()
						};
						
						var baseX = tileX * Resolution + templateX * pixelsPerTemplateCell;
						var baseY = tileY * Resolution + templateY * pixelsPerTemplateCell;

						for (var py = 0; py < pixelsPerTemplateCell; py++)
						{
							for (var px = 0; px < pixelsPerTemplateCell; px++)
							{
								var pixelX = baseX + px;
								var pixelY = baseY + py;

								var pixelIndex = (pixelY * width + pixelX) * 4;
								fixed (byte* pixelPtr = Pixels)
									Unsafe.Write(pixelPtr + pixelIndex, color);
							}
						}

					}
				}
			}
		}

		UpdateTexture();
		if (_done)
			IsEventDriven = true;
	}
}