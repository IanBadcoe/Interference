using Godot;
using Interference.Code.QuantumMaths;

namespace Interference.LevelObject.QuantumComponent;

public partial class LaserBeam : Node2D
{
	public static LaserBeam Create(Vector2 start, Vector2 end, Beam beam1,
		Beam beam2)
	{
		var beam = PackedScenes.Instance.LaserBeam.Instantiate<LaserBeam>();
		beam._start = start;
		beam._end = end;
		beam._beam1 = beam1;
		beam._beam2 = beam2;
		return beam;
	}
	
	[Export]
	private Line2D _line;

	private Vector2 _start;
	private Vector2 _end;
	private Beam _beam1;
	private Beam _beam2;
	

	public override void _Ready()
	{
		_line.Points = [_start, _end];
		_line.DefaultColor = Colors.Red;
	}

	public override void _Process(double delta)
	{
		_line.DefaultColor = new Color(_line.DefaultColor, (float)(_beam1.Start.Magnitude + _beam2.Start.Magnitude));
	}
}