namespace DunGen
{
	public sealed class TileConnectionRule
	{
		/// <summary>
		/// The result of evaluating a TileConnectionRule
		/// </summary>
		public enum ConnectionResult
		{
			/// <summary>
			/// The connection is allowed
			/// </summary>
			Allow,
			/// <summary>
			/// The connection is not allowed
			/// </summary>
			Deny,
			/// <summary>
			/// Let any lower priority rules decide whether this connection is allowed or not
			/// </summary>
			Passthrough,
		}

		/// <summary>
		/// A delegate to determine if two tiles can be connected by the given doorways
		/// </summary>
		/// <param name="tileA">The first tile</param>
		/// <param name="tileB">The second tile</param>
		/// <param name="doorwayA">The first doorway</param>
		/// <param name="doorwayB">The second doorway</param>"
		/// <returns>Whether to allow or deny this connection, or to delegate the decision to lower priority rules</returns>
		public delegate ConnectionResult CanTilesConnectDelegate(Tile tileA, Tile tileB, Doorway doorwayA, Doorway doorwayB);

		/// <summary>
		/// This rule's prioty. Higher priority rules are evaluated first. Lower priority rules are
		/// only evaluated if the delegate returns 'Passthrough' as the result
		/// </summary>
		public int Priority = 0;

		/// <summary>
		/// The delegate to evaluate to determine if two tiles can connect using a given doorway pairing.
		/// Returning 'Passthrough' will allow lower priority rules to evaluate. If no rule handles the connection,
		/// the default method is used (only matching doorways are allowed to connect).
		/// </summary>
		public CanTilesConnectDelegate Delegate;


		public TileConnectionRule(CanTilesConnectDelegate connectionDelegate, int priority = 0)
		{
			Delegate = connectionDelegate;
			Priority = priority;
		}
	}
}
