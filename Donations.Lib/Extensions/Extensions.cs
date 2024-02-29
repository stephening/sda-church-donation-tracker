using System;
using System.ComponentModel;
using System.Reflection;

namespace Donations.Lib.Extensions
{
	public static class AttributeExtensions
	{
		public static string? GetDescription(this Enum value)
		{
			FieldInfo? fieldInfo = value?.GetType()?.GetField(value.ToString());
			if (fieldInfo == null) return null;
			var attribute = (DescriptionAttribute?)fieldInfo?.GetCustomAttribute(typeof(DescriptionAttribute));
			return attribute?.Description;
		}
	}
}
