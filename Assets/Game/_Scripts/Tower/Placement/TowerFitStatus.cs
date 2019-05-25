namespace TargetDefense.Targets.Placement
{
	/// <summary>
	/// Enum representing the state of how a tower fits into a placement area
	/// </summary>
	public enum TowerFitStatus
	{
		/// <summary>
		/// Target fits in this location
		/// </summary>
		Fits,

		/// <summary>
		/// Target overlaps another tower in the placement area
		/// </summary>
		Overlaps,

		/// <summary>
		/// Target exceeds bounds of the placement area
		/// </summary>
		OutOfBounds
	}
}