using System;
using System.Reflection;

namespace PathfindingFramework.Patches.ModCompatibility
{
	public class ModCompatibilityUtility
	{
		public static MethodInfo MethodFromAssembly(Assembly assembly, string typeName, string methodName)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.FullName == null || typeName.Contains(type.FullName))
				{
					continue;
				}

				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
					         BindingFlags.Instance | BindingFlags.Static))
				{
					if (method.Name == methodName)
					{
						return method;
					}
				}
			}

			return null;
		}
	}
}