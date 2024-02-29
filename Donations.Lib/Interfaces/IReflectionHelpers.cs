using System.Reflection;

namespace Donations.Lib.Interfaces;

public interface IReflectionHelpers
{
	T CopyModel<T>(T source);
	bool SameModel<T>(T arg1, T arg2);
	string ShowChanges<T>(T original, T current, string[]? ignore = null);
	PropertyInfo[]? ModelProperties<T>(T? model);
}
