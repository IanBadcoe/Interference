using Godot;
using System;
using System.Collections.Generic;
using Interference.Code.QuantumMaths;

public partial class Level : Node2D
{
	/// <summary>
	/// All quantum simulation components in the level that need to be part of the quantum state calculation
	/// </summary>
	public HashSet<Component> QuantumComponents { get; } = [];

	private bool _quantumStateDirty = true;
	
	public void MarkQuantumStateDirty()
	{
		_quantumStateDirty = true;
	}

	public override void _Process(double delta)
	{
		if (_quantumStateDirty)
		{
			Calculator.Calculate(QuantumComponents);
			_quantumStateDirty = false;
		}
	}
}
