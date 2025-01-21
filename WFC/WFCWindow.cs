using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace WFC;


public sealed class WFCWindow : PixelWindow.PixelWindow
{
	private const int Resolution = 8;

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
		},
		// PlusJoint
		new[,]
		{
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
			{ 1, 1, 2, 2, 2, 2, 1, 1 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 2, 2, 2, 2, 2, 2, 2, 2 },
			{ 1, 1, 2, 2, 2, 2, 1, 1 },
			{ 0, 1, 2, 2, 2, 2, 1, 0 },
		}
	];

	public enum TileType
	{
		NotAssigned = 0,
		Empty = 1,
		Vertical = 2,
		Horizontal = 3,
		TopLeftCorner = 4,
		TopRightCorner = 5,
		BottomLeftCorner = 6,
		BottomRightCorner = 7,
		UpTJoint = 8,
		DownTJoint = 9,
		LeftTJoint = 10,
		RightTJoint = 11,
		PlusJoint = 12
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

	public readonly byte[] Pixels;

	public readonly TileType[,] Tiles;
	private bool _done;
	private bool _updated;

	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		if (_updated)
			BuildPixels();
	}

	// Pre-calculate valid connections for each tile type
	private static readonly Dictionary<TileType, HashSet<TileType>> ValidTopConnections = new()
	{
		#region should connect

		[TileType.Vertical] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.LeftTJoint,
			TileType.RightTJoint,
			TileType.PlusJoint,
		],
		[TileType.BottomLeftCorner] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.BottomRightCorner] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.LeftTJoint] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.PlusJoint,
			TileType.UpTJoint,
			TileType.LeftTJoint,
			TileType.RightTJoint
		],
		[TileType.RightTJoint] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.DownTJoint] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.PlusJoint] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		#endregion

		#region should not connect
		[TileType.Empty] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint
		],
		[TileType.Horizontal] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint
		],
		[TileType.TopLeftCorner] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint
		],
		[TileType.TopRightCorner] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint
		],
		[TileType.UpTJoint] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint
		],
		#endregion
	};

	private static readonly Dictionary<TileType, HashSet<TileType>> ValidBottomConnections = new()
	{
		#region should connect

		[TileType.Vertical] =
		[
			TileType.Vertical,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.TopRightCorner] =
		[
			TileType.Vertical,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.TopLeftCorner] =
		[
			TileType.Vertical,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],

		[TileType.LeftTJoint] =
		[
			TileType.Vertical,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.RightTJoint] =
		[
			TileType.Vertical,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.UpTJoint] =
		[
			TileType.Vertical,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint,
			TileType.LeftTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.PlusJoint] =
		[
			TileType.Vertical,
			TileType.BottomLeftCorner,
			TileType.BottomRightCorner,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.LeftTJoint,
			TileType.RightTJoint
		],

		#endregion

		#region should not connect

		[TileType.Empty] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint
		],
		[TileType.Horizontal] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint
		],
		[TileType.BottomLeftCorner] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint
		],
		[TileType.BottomRightCorner] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint
		],

		[TileType.DownTJoint] =
		[
			TileType.Empty,
			TileType.Horizontal,
			TileType.TopLeftCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint
		],

		#endregion
	};

	private static readonly Dictionary<TileType, HashSet<TileType>> ValidLeftConnections = new()
	{
		#region should connect

		[TileType.Horizontal] =
		[
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.TopLeftCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.LeftTJoint
		],
		[TileType.TopRightCorner] =
		[
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.TopLeftCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.LeftTJoint
		],
		[TileType.BottomRightCorner] =
		[
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.TopLeftCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.LeftTJoint
		],
		[TileType.UpTJoint] =
		[
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.TopLeftCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.LeftTJoint
		],
		[TileType.RightTJoint] =
		[
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.TopLeftCorner,
			TileType.UpTJoint,
			TileType.PlusJoint,
			TileType.DownTJoint,
			TileType.LeftTJoint
		],
		[TileType.DownTJoint] =
		[
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.TopLeftCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.LeftTJoint
		],
		[TileType.PlusJoint] =
		[
			TileType.Horizontal,
			TileType.BottomLeftCorner,
			TileType.TopLeftCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.LeftTJoint
		],

		#endregion

		#region should not connect

		[TileType.Empty] =
		[
			TileType.Vertical,
			TileType.TopRightCorner,
			TileType.BottomRightCorner,
			TileType.RightTJoint
		],
		[TileType.Vertical] =
		[
			TileType.Vertical,
			TileType.TopRightCorner,
			TileType.BottomRightCorner,
			TileType.RightTJoint
		],
		[TileType.TopLeftCorner] =
		[
			TileType.Vertical,
			TileType.TopRightCorner,
			TileType.BottomRightCorner,
			TileType.RightTJoint
		],
		[TileType.BottomLeftCorner] =
		[
			TileType.Vertical,
			TileType.TopRightCorner,
			TileType.BottomRightCorner,
			TileType.RightTJoint
		],
		[TileType.LeftTJoint] =
		[
			TileType.Vertical,
			TileType.TopRightCorner,
			TileType.BottomRightCorner,
			TileType.RightTJoint
		],

		#endregion
	};

	private static readonly Dictionary<TileType, HashSet<TileType>> ValidRightConnections = new()
	{
		#region should connect

		[TileType.Horizontal] =
		[
			TileType.Horizontal,
			TileType.BottomRightCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.TopLeftCorner] =
		[
			TileType.Horizontal,
			TileType.BottomRightCorner,
			TileType.TopRightCorner,
			TileType.PlusJoint,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.RightTJoint
		],

		[TileType.BottomLeftCorner] =
		[
			TileType.Horizontal,
			TileType.BottomRightCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.PlusJoint,
			TileType.DownTJoint,
			TileType.RightTJoint
		],

		[TileType.UpTJoint] =
		[
			TileType.Horizontal,
			TileType.BottomRightCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.PlusJoint,
			TileType.DownTJoint,
			TileType.RightTJoint
		],

		[TileType.LeftTJoint] =
		[
			TileType.Horizontal,
			TileType.BottomRightCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.PlusJoint,
			TileType.DownTJoint,
			TileType.RightTJoint
		],

		[TileType.DownTJoint] =
		[
			TileType.Horizontal,
			TileType.BottomRightCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.DownTJoint,
			TileType.PlusJoint,
			TileType.RightTJoint
		],
		[TileType.PlusJoint] =
		[
			TileType.Horizontal,
			TileType.BottomRightCorner,
			TileType.TopRightCorner,
			TileType.UpTJoint,
			TileType.PlusJoint,
			TileType.DownTJoint,
			TileType.RightTJoint
		],

		#endregion

		#region should not connect

		[TileType.Empty] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.BottomLeftCorner,
			TileType.LeftTJoint
		],
		[TileType.Vertical] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.BottomLeftCorner,
			TileType.LeftTJoint
		],
		[TileType.TopRightCorner] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.BottomLeftCorner,
			TileType.LeftTJoint
		],
		[TileType.BottomRightCorner] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.BottomLeftCorner,
			TileType.LeftTJoint
		],
		[TileType.RightTJoint] =
		[
			TileType.Vertical,
			TileType.TopLeftCorner,
			TileType.BottomLeftCorner,
			TileType.LeftTJoint
		],

		#endregion
	};

	// Cache common calculations
	private readonly int _gridWidth;
	private readonly int _gridHeight;
	private readonly int _pixelsPerTemplateCell;
	private readonly int _width;
	private readonly int _height;

	public WFCWindow(int width, int height) : base(new NativeWindowSettings
	{
		Title = "WFC",
		ClientSize = new Vector2i(width, height),
		IsEventDriven = false,
		MaximumClientSize = new Vector2i(width, height),
		MinimumClientSize = new Vector2i(width, height),
	})
	{
		_width = width;
		_height = height;
		Pixels = new byte[width * height * 4];
		_gridWidth = width / Resolution;
		_gridHeight = height / Resolution;
		_pixelsPerTemplateCell = Resolution / 8;
		Tiles = new TileType[_gridWidth, _gridHeight];
	}


	private void WFC()
	{
		// Use a more efficient data structure for tracking entropy
		var cellsToProcess = new SortedSet<(int Priority, int X, int Y)>(Comparer<(int Priority, int X, int Y)>.Create(
			(a, b) =>
			{
				var priorityComparison = a.Priority.CompareTo(b.Priority);
				if (priorityComparison != 0) return priorityComparison;
				var xComparison = a.X.CompareTo(b.X);
				return xComparison != 0 ? xComparison : a.Y.CompareTo(b.Y);
			}));

		// Initialize with a random starting point
		var startX = Random.Shared.Next(_gridWidth);
		var startY = Random.Shared.Next(_gridHeight);
		Tiles[startX, startY] = (TileType)Random.Shared.Next(1, 12);

		// Add neighbors with their entropy as priority
		foreach (var (x, y) in GetNeighbours(startX, startY))
		{
			var possibleStates = GetPossibleStates(x, y);
			cellsToProcess.Add((possibleStates.Length, x, y));
		}

		while (cellsToProcess.Count > 0)
		{
			var (_, x, y) = cellsToProcess.Min;
			cellsToProcess.Remove(cellsToProcess.Min);

			var states = GetPossibleStates(x, y);
			if (states.Length == 0)
			{
				// Reset and restart if we hit a contradiction
				Array.Clear(Tiles, 0, Tiles.Length);
				var thread = new Thread(WFC);
				thread.Start();
				return;
			}

			Tiles[x, y] = states[Random.Shared.Next(states.Length)];

			// Update neighbors
			foreach (var neighbor in GetNeighbours(x, y))
			{
				var possibleStates = GetPossibleStates(neighbor.X, neighbor.Y);
				cellsToProcess.Add((possibleStates.Length, neighbor.X, neighbor.Y));
			}

			_updated = true;
		}

		_done = true;
	}

	public List<(int X, int Y)> GetNeighbours(int x, int y)
	{
		var neighbours = new List<(int, int)>();
		// check if top exists
		if (y > 0 && Tiles[x, y - 1] is TileType.NotAssigned)
			neighbours.Add((x, y - 1));

		// check if bottom exists
		if (y < _height / Resolution - 1 && Tiles[x, y + 1] is TileType.NotAssigned)
			neighbours.Add((x, y + 1));

		// check if left exists
		if (x > 0 && Tiles[x - 1, y] is TileType.NotAssigned)
			neighbours.Add((x - 1, y));

		// check if right exists
		if (x < _width / Resolution - 1 && Tiles[x + 1, y] is TileType.NotAssigned)
			neighbours.Add((x + 1, y));
		return neighbours;
	}

	public TileType[] GetPossibleStates(int x, int y)
	{
		var possibleStates = new HashSet<TileType>
		{
			TileType.Empty, TileType.Horizontal, TileType.Vertical, TileType.TopLeftCorner,
			TileType.TopRightCorner, TileType.BottomLeftCorner, TileType.BottomRightCorner,
			TileType.LeftTJoint, TileType.RightTJoint, TileType.UpTJoint, TileType.DownTJoint,
			TileType.PlusJoint
		};

		// Check each direction using the pre-calculated valid connections
		if (y > 0 && Tiles[x, y - 1] != TileType.NotAssigned)
		{
			possibleStates.IntersectWith(ValidTopConnections[Tiles[x, y - 1]]);
		}

		if (y < _gridHeight - 1 && Tiles[x, y + 1] != TileType.NotAssigned)
		{
			possibleStates.IntersectWith(ValidBottomConnections[Tiles[x, y + 1]]);
		}

		if (x > 0 && Tiles[x - 1, y] != TileType.NotAssigned)
		{
			possibleStates.IntersectWith(ValidLeftConnections[Tiles[x - 1, y]]);
		}

		if (x < _gridWidth - 1 && Tiles[x + 1, y] != TileType.NotAssigned)
		{
			possibleStates.IntersectWith(ValidRightConnections[Tiles[x + 1, y]]);
		}

		return possibleStates.ToArray();
	}

	public unsafe void BuildPixels()
	{
		_updated = false;

		// Use SIMD operations for pixel filling where possible
		fixed (byte* pixelPtr = Pixels)
		{
			for (var tileY = 0; tileY < _gridHeight; tileY++)
			{
				for (var tileX = 0; tileX < _gridWidth; tileX++)
				{
					var template = _templates[(int)Tiles[tileX, tileY]];

					for (var templateY = 0; templateY < 8; templateY++)
					{
						for (var templateX = 0; templateX < 8; templateX++)
						{
							var color = GetColorForTemplateValue(template[templateY, templateX]);
							var baseX = tileX * Resolution + templateX * _pixelsPerTemplateCell;
							var baseY = tileY * Resolution + templateY * _pixelsPerTemplateCell;

							// Fill pixels in blocks using Vector operations where possible
							FillPixelBlock(pixelPtr, baseX, baseY, color);
						}
					}
				}
			}
		}

		UpdateTexture();
		if (_done)
			IsEventDriven = true;
	}

	private unsafe void FillPixelBlock(byte* pixelPtr, int baseX, int baseY, uint color)
	{
		// TODO: replace with simd
		var rowSize = _width * 4;
		for (var py = 0; py < _pixelsPerTemplateCell; py++)
		{
			var rowPtr = pixelPtr + ((baseY + py) * rowSize + baseX * 4);
			for (var px = 0; px < _pixelsPerTemplateCell; px++)
			{
				*(uint*)&rowPtr[px * 4] = color;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GetColorForTemplateValue(int value) => value switch
	{
		-1 => ~0x00FFFFFFU,
		0 => ~0xFF00FF00U,
		1 => ~0xFFFFFFFFU,
		2 => ~0xFF0000FFU,
		_ => throw new ArgumentOutOfRangeException()
	};
}