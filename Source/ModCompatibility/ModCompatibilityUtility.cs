using System;
using System.Reflection;

namespace PathfindingFramework.ModCompatibility
{
	/// <summary>
	/// Utility functions for handling mod compatibility through reflection.
	/// </summary>
	public static class ModCompatibilityUtility
	{
		private const string ErrorPrefix = "Mod compatibility issue";

		/// <summary>
		/// Obtain a type from a specific assembly. Caller is responsible for ensuring that assembly is valid.
		/// Logs an error if the type could not be found.
		/// </summary>
		/// <param name="assembly">Assembly to search for types.</param>
		/// <param name="typeName">Type to find.</param>
		/// <returns>Found type.</returns>
		public static Type TypeFromAssembly(Assembly assembly, string typeName)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.FullName != null && type.FullName.Contains(typeName))
				{
					return type;
				}
			}

			Report.ErrorOnce($"{ErrorPrefix}: Could not find type {typeName} in assembly {assembly}.");
			return null;
		}

		/// <summary>
		/// Obtain a method from a specific assembly and type. Caller is responsible for ensuring that assembly is valid.
		/// Logs an error if the method could not be found.
		/// </summary>
		/// <param name="assembly">Assembly to search for types.</param>
		/// <param name="typeName">Type to find.</param>
		/// <param name="methodName">Name of the method to return.</param>
		/// <returns>Found method.</returns>
		public static MethodInfo MethodFromAssembly(Assembly assembly, string typeName, string methodName)
		{
			Type type = TypeFromAssembly(assembly, typeName);

			if (type == null)
			{
				// Type errors are logged in TypeFromAssembly.
				return null;
			}

			foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
				         BindingFlags.Instance | BindingFlags.Static))
			{
				if (method.Name == methodName)
				{
					return method;
				}
			}

			Report.ErrorOnce($"{ErrorPrefix}: Could not find method {typeName}:{methodName} in assembly {assembly}.");

			return null;
		}

		/// <summary>
		/// Obtain a field from a specific assembly and type. Caller is responsible for ensuring that assembly is valid.
		/// Logs an error if the field could not be found.
		/// </summary>
		/// <param name="assembly">Assembly to search for types.</param>
		/// <param name="typeName">Type to find.</param>
		/// <param name="fieldName">Name of the field to return.</param>
		/// <returns>Found field.</returns>
		public static FieldInfo FieldFromAssembly(Assembly assembly, string typeName, string fieldName)
		{
			Type type = TypeFromAssembly(assembly, typeName);

			if (type == null)
			{
				// Type errors are logged in TypeFromAssembly.
				return null;
			}

			foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
				         BindingFlags.Instance | BindingFlags.Static))
			{
				if (field.Name == fieldName)
				{
					return field;
				}
			}

			Report.ErrorOnce($"{ErrorPrefix}: Could not find field {typeName}:{fieldName} in assembly {assembly}.");
			return null;
		}
	}
}