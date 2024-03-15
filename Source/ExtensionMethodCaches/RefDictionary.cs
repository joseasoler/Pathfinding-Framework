using System.Collections;
using System.Collections.Generic;

namespace PathfindingFramework.ExtensionMethodCaches
{
	/// <summary>
	/// Class used to return references to the intended value that can be changed from the outside without affecting the
	/// integrity of the dictionary.
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	internal class RefHolder<TValue>
	{
		internal TValue value;
	}


	public class RefDictionary<TValue>
	{
		private Dictionary<int, RefHolder<TValue>> _dict = new();

		public ref TValue Get(int uniqueID)
		{
			if (!_dict.TryGetValue(uniqueID, out RefHolder<TValue> holder))
			{
				holder = new RefHolder<TValue>();
				_dict[uniqueID] = holder;
			}

			return ref holder.value;
		}
	}
}