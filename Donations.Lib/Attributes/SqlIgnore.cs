using System;

namespace Donations.Lib.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class SqlIgnoreAttribute : Attribute
{
}

