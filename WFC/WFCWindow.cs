using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace WFC;

public sealed class WFCWindow(int width, int height) : PixelWindow.PixelWindow(
	new NativeWindowSettings
	{
		Title = "WFC",
		ClientSize = new Vector2i(width, height),
		IsEventDriven = true,
		MaximumClientSize = new Vector2i(width, height),
		MinimumClientSize = new Vector2i(width, height),
	})
{
	private const int Resolution = 8;

	private readonly int[][,] _templates =
	[
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
			{ 1, 1, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 2, 2, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 2, 2, 2, 2, 1, 0 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		}
	];

	public enum TileType
	{
		Empty                = 0,
		Vertical             = 1,
		Horizontal           = 2,
		TopLeftCorner        = 3,
		TopRightCorner       = 4,
		BottomLeftCorner     = 5,
		BottomRightCorner    = 6,
		UpTJoint             = 7,
		DownTJoint           = 8,
		LeftTJoint           = 9,
		RightTJoint          = 10
	}

	protected override unsafe void OnLoad()
	{
		base.OnLoad();
		TextureSize = ClientSize;
		fixed (byte* dataPtr = Pixels)
			Data = dataPtr;
		for (var i = 0; i < width / Resolution; i++)
		{
			for (var j = 0; j < height / Resolution; j++)
			{
				Tiles[i, j] = (TileType)Random.Shared.Next(0, 11);
			}
		}

		BuildPixels();
	}

	public readonly byte[] Pixels = new byte[width * height * 4];

	public readonly TileType[,] Tiles = new TileType[width / Resolution, height / Resolution];

	public unsafe void BuildPixels()
	{
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
							0 => ~0xFF00FF00,
							1 => ~0xFFFFFFFF,
							2 => ~0xFF0000FF, 
							_ => throw new ArgumentOutOfRangeException()
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
									Marshal.WriteInt32((IntPtr)pixelPtr, pixelIndex, *(int*)&color);
							}
						}

					}
				}
			}
		}

		UpdateTexture();
	}

}