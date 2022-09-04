using DunGen.Editor.Validation;
using DunGen.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DunGen.Assets.DunGen.Code.Editor.Validation.Rules
{
	sealed class TileConfigurationRule : IValidationRule
	{
		public void Validate(DungeonFlow flow, DungeonValidator validator)
		{
			var tilePrefabs = flow.GetUsedTileSets()
						.SelectMany(ts => ts.TileWeights.Weights.Select(w => w.Value))
						.Where(t => t != null)
						.ToArray();

			CheckTilemaps(flow, validator, tilePrefabs);
		}

		// Checks every tile and logs a warning if any have a Tilemap and are using automatic bounds calculations
		// Unity's tilemap doesn't have accurate bounds when first instantiated and so must use overriden tile bounds
		private void CheckTilemaps(DungeonFlow flow, DungeonValidator validator, IEnumerable<GameObject> tilePrefabs)
		{
			foreach(var tileObj in tilePrefabs)
			{
				var tile = tileObj.GetComponent<Tile>();

				if (tile == null)
					continue;

				if(!tile.OverrideAutomaticTileBounds)
				{
					var tilemap = tile.GetComponentInChildren<Tilemap>();

					if (tilemap != null)
						validator.AddWarning("[Tile: {0}] Automatic tile bounds don't work correctly with Unity's tilemaps. Check 'Override Automatic Tile Bounds' on your tile component and press the 'Fit to Tile' button", tile, tile.name);
				}
			}
		}
	}
}
