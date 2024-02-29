using Donations.Lib.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace Donations.Lib.Services;

public class ReflectionHelpers : IReflectionHelpers
{
	public T CopyModel<T>(T source)
	{
		var copy = Activator.CreateInstance<T>();
		var properties = copy.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		foreach (var property in properties)
		{
			if (property.CanWrite)
			{
				property.SetValue(copy, property.GetValue(source));
			}
		}
		return copy;
	}

	public bool SameModel<T>(T arg1, T arg2)
	{
		var properties = arg1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		foreach (var property in properties)
		{
			var val1 = property.GetValue(arg1);
			var val2 = property.GetValue(arg2);

			if (false == val1?.Equals(val2) && val1 != val2)
			{
				return false;
			}
		}
		return true;
	}

	public PropertyInfo[]? ModelProperties<T>(T? model)
	{
		return model?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
	}

	public string ShowChanges<T>(T original, T current, string[]? ignore = null)
	{
		string changes = "";

		var properties = original.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		foreach (var property in properties)
		{
			if (true == ignore?.Contains(property.Name)) continue;

			var val1 = property.GetValue(original);
			var val2 = property.GetValue(current);

			if (true == val1?.Equals(val2)) continue;
			if (val1 == val2) continue;

			changes += $"{property.Name}: {val1} => {val2}\r\n";
		}
		return changes[..^2];
	}
}
