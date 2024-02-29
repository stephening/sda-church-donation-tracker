using Donations.Lib.Model;
using System;
using System.Linq;
using System.Windows.Markup;

namespace Donations.Lib.EnumHelpers;

/// <summary>
/// This class is provided to allow the Description attribute of the enumAddtessType to be shown in
/// the ComboBox rather than just the enum value names.
/// </summary>
public class AddressTypeDescriptionGetter : MarkupExtension
{
	private readonly Type _type;

	public AddressTypeDescriptionGetter(Type type)
	{
		_type = type;
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		return Enum.GetValues(_type)
			.Cast<object>()
			.Select(e => new { Value = e, DisplayName = Helper.GetDescription((enumAddressType)e) });
	}
}
