using System.Collections;
using System.Reflection;
using Verse;

namespace PathfindingFramework.ModCompatibility
{
	/// <summary>
	/// Compatibility with the Giddy-Up 2 mod.
	/// </summary>
	public static class GiddyUp2Compat
	{
		/// <summary>
		/// Reference to the ExtendedDataStorage.GUComp._store dictionary.
		/// Initialized by the postfix in ExtendedDataStorage_FinalizeInit_Patch.
		/// Pawn.thingIDNumber is used as the key of this dictionary.
		/// </summary>
		private static IDictionary _store;

		/// <summary>
		/// Obtain the mount Pawn from the ExtendedPawnData instances returned by the _store dictionary.
		/// </summary>
		private static FieldInfo _mountField;

		/// <summary>
		/// Check for Giddy-Up 2 presence and initialize required data.
		/// </summary>
		public static void Initialize()
		{
			Assembly giddyUp2Assembly = ModAssemblyInfo.GiddyUp2Assembly;
			if (giddyUp2Assembly == null)
			{
				// Giddy-Up 2 is assumed to not be present at this point.
				return;
			}

			// Mount field of the ExtendedPawnData type.
			_mountField = ModCompatibilityUtility.FieldFromAssembly(giddyUp2Assembly, "ExtendedPawnData", "mount");
		}

		/// <summary>
		/// Called after Giddy-Up 2 finishes setting up its world component. Takes a reference to the internal storage and
		/// stores it in the PF code.
		/// </summary>
		/// <param name="store">Dictionary containing Giddy-Up's internal cache.</param>
		public static void SetGiddyUpStorage(IDictionary store)
		{
			_store = store;
			if (_store == null)
			{
				Report.Error(
					$"Giddy-Up 2 compatibility patch error: could not find internal storage.");
			}
		}

		/// <summary>
		/// Obtain the mount of a pawn.
		/// </summary>
		/// <param name="pawn">Pawn to check as rider.</param>
		/// <returns>Mount pawn, if any has been found.</returns>
		public static Pawn GetMount(Pawn pawn)
		{
			if (_store != null && _store.Contains(pawn.thingIDNumber))
			{
				object extendedPawnData = _store[pawn.thingIDNumber];
				return _mountField.GetValue(extendedPawnData) as Pawn;
			}

			return null;
		}
	}
}