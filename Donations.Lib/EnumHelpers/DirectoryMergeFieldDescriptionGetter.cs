using Donations.Lib.Model;
using System;
using System.Linq;
using System.Windows.Markup;

namespace Donations.Lib.EnumHelpers;

/// <summary>
/// This class is provided to allow the Description attribute of the enumMergeField to be shown in
/// the ComboBox rather than just the enum value names.
/// </summary>
public class DirectoryMergeFieldDescriptionGetter : MarkupExtension
{
	private readonly Type _type;

	public DirectoryMergeFieldDescriptionGetter(Type type)
	{
		_type = type;
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		return Enum.GetValues(_type)
			.Cast<object>()
			.Select(e => new { Value = e, DisplayName = Helper.GetDescription((enumDirectoryMergeFields)e) });
	}
}
