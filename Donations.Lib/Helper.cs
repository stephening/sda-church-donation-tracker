using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Donations.Lib;

/// <summary>
/// Fuzzy compare flags. See Equal function.
/// </summary>
public enum eFlags
{
	None,
	FirstGroup,
	Length,
	State,
	Date,
}

public class Helper
{
	static public char[] ro_firstNameSplitters = new char[] { ' ', '/', '&' };

	static public Dictionary<string, string> StateAbbrMapping = new Dictionary<string, string>()
	{
		{ "AK", "Alaska" },
		{ "AL", "Alabama" },
		{ "AZ", "Arizona" },
		{ "AR", "Arkansas" },
		{ "AS", "American Samoa" },
		{ "CA", "California" },
		{ "CO", "Colorado" },
		{ "CT", "Connecticut" },
		{ "DE", "Deleware" },
		{ "DC", "Disctrict of Columbia" },
		{ "FL", "Florida" },
		{ "GA", "Georgia" },
		{ "GU", "Guam" },
		{ "HI", "Hawaii" },
		{ "ID", "Idaho" },
		{ "IL", "Illinois" },
		{ "IN", "Indiana" },
		{ "IA", "Iowa" },
		{ "KS", "Kansas" },
		{ "KY", "Kentucky" },
		{ "LA", "Louisiana" },
		{ "ME", "Maine" },
		{ "MD", "Maryland" },
		{ "MA", "Massachusetts" },
		{ "MI", "Michigan" },
		{ "MN", "Minnesota" },
		{ "MS", "Mississippi" },
		{ "MO", "Missouri" },
		{ "MT", "Montana" },
		{ "NE", "Nebraska" },
		{ "NV", "Nevada" },
		{ "NH", "New Hampshire" },
		{ "NJ", "New Jersey" },
		{ "NM", "New Mexico" },
		{ "NY", "New York" },
		{ "NC", "North Carolina" },
		{ "ND", "North Dakota" },
		{ "MP", "Northern Mariana Islands" },
		{ "OH", "Ohio" },
		{ "OK", "Oklahoma" },
		{ "OR", "Oregon" },
		{ "PA", "Pennsylvania" },
		{ "PR", "Puerto Rico" },
		{ "RI", "Rhode Island" },
		{ "SC", "South Carolina" },
		{ "SD", "South Dakota" },
		{ "TN", "Tennessee" },
		{ "TX", "Texas" },
		{ "TT", "Trust Territories" },
		{ "UT", "Utah" },
		{ "VT", "Vermont" },
		{ "VA", "Virginia" },
		{ "VI", "Virgin Islands" },
		{ "WA", "Washington" },
		{ "WV", "West Virginia" },
		{ "WI", "Wisconsin" },
		{ "WY", "Wyoming" },
	};

	static public string GetStateAbbreviation(string state)
	{
		foreach (var st in StateAbbrMapping)
		{
			if (st.Value.Equals(state, StringComparison.OrdinalIgnoreCase))
			{
				return st.Key;
			}
		}

		return state;
	}

	/// <summary>
	/// This function does the fuzzy comparison between two strings. Strings can be
	/// null or "". The flag is used to allow different fuzzy rules to be applied for
	/// the comparison.
	/// </summary>
	/// <param name="s1">String one.</param>
	/// <param name="s2">String two.</param>
	/// <param name="flag">Allows the function to know which fuzzy rules to apply.
	/// - none(default) - regular case insensitive comparison.
	/// - firstGroup - split strings by delimiters {' ', '/', '&'} in
	///   and only compare first word.</param>
	/// - length - Only compare the first len characters.
	/// <returns>Returns true for a match and false otherwise.</returns>
	static public bool Equal(string? s1, string? s2, eFlags flag = eFlags.None, char[]? delim = null, int? len = null)
	{
		if (eFlags.FirstGroup == flag)
		{
			s1 = (string.IsNullOrEmpty(s1)) ? "" : s1.Split(delim)[0];
			s2 = (string.IsNullOrEmpty(s2)) ? "" : s2.Split(delim)[0];
		}
		else if (eFlags.Length == flag && null != len)
		{
			s1 = (string.IsNullOrEmpty(s1)) ? "" : s1[..len.Value];
			s2 = (string.IsNullOrEmpty(s2)) ? "" : s2[..len.Value];
		}
		else if (eFlags.State == flag)
		{
			s1 = (string.IsNullOrEmpty(s1)) ? "" : (2 == s1.Length) ? s1 : GetStateAbbreviation(s1);
			s2 = (string.IsNullOrEmpty(s2)) ? "" : (2 == s2.Length) ? s2 : GetStateAbbreviation(s2);
		}
		else if (eFlags.Date == flag)
		{
			try
			{
				s1 = string.IsNullOrEmpty(s1) ? "" : DateOnly.Parse(s1).ToString("yyyy/MM/dd");
			}
			catch
			{
				s1 = "";
			}
			try
			{
				s2 = string.IsNullOrEmpty(s2) ? "" : DateOnly.Parse(s2).ToString("yyyy/MM/dd");
			}
			catch
			{
				s2 = "";
			}
		}

		return string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2) || (s1 != null && s1.Equals(s2, StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// This function compares the string from the parameter list with the properties in transaction.
	/// It uses the Equal() method to do the actual comparison.
	/// </summary>
	/// <param name="comment">A string parameter that can be null, or return a description of what matched or didn't match.</param>
	/// <param name="partial">A bool, where a partial match is defined to be a LastName match but no first name match.</param>
	/// <param name="donor"></param>
	/// <param name="transaction"></param>
	/// <returns></returns>
	static public bool Compare(ref string? comment, ref int? partial, Donor donor,
		string? lastName,
		string? firstName,
		string? address,
		bool checkFullFirst = false,
		string? address2 = null,
		string? city = null,
		string? state = null,
		string? zip = null,
		char[]? delim = null)
	{
		if (Helper.Equal(donor.LastName, lastName))
		{
			if (Helper.Equal(donor.FirstName, firstName, checkFullFirst ? eFlags.None : eFlags.FirstGroup, delim: delim))
			{
				comment = $"First and last name matched: {donor.FirstName} {donor.LastName}\n";

				var b_address1 = (null == address) ? true : Helper.Equal(donor?.Address?.Trim('.'), address.Trim('.'));
				var b_address2 = (null == address2) ? true : Helper.Equal(donor?.Address2, address2);
				var b_city = (null == city) ? true : Helper.Equal(donor?.City, city);
				var b_state = (null == state) ? true : Helper.Equal(donor?.State, state, eFlags.State);
				var b_zip = (null == zip) ? true : Helper.Equal(donor?.Zip, zip, eFlags.Length, len: 5); // only check first 5 chars of zip code

				if (b_address1 && b_address2 && b_city && b_state && b_zip)
				{
					if (Helper.Equal(donor?.FirstName, firstName))
					{
						// Even first name field is same
						comment = null;
					}
					else
					{
						comment = "Very close match";
					}

					return true;
				}
				else
				{
					if (!b_address1) comment += $"Address mismatch:\n  {donor?.Address}\n  {address}\n";
					if (!b_address2) comment += $"Address2 mismatch:\n  {donor?.Address2}\n  {address2}\n";
					if (!b_city) comment += $"City mismatch:\n  {donor?.City}\n  {city}\n";
					if (!b_state) comment += $"State mismatch:\n  {donor?.State}\n  {state}\n";
					if (!b_zip) comment += $"Postal code mismatch:\n  {donor?.Zip}\n  {zip}\n";

					return true;
				}
			}
			else
			{
				comment += $"Last name matched: {donor?.LastName}\n";
				comment += $"First name mismatch:\n  {donor?.FirstName}\n  {firstName}\n";

				partial = donor.Id;
			}
		}
		return false;
	}

	public static string ParseString(int lineNumber, string[] values, Dictionary<string, int> dict, string name)
	{
		if (!dict.ContainsKey(name))
		{
			return "";
		}

		var value = values[dict[name]].Trim().Trim('"');

		try
		{
			return value;
		}
		catch
		{
			throw new Exception($"Failed parsing line number: {lineNumber}, value: \"{value}\" for string ({name})");
		}
	}

	public static int ParseInt(int lineNumber, string[] values, Dictionary<string, int> dict, string name, bool requires = false)
	{
		if (!dict.ContainsKey(name))
		{
			return 0;
		}

		string value = values[dict[name]].Trim('"');
		try
		{
			return int.Parse(value);
		}
		catch
		{
			throw new Exception($"Failed parsing line number: {lineNumber}, value: \"{value}\" for int ({name})");
		}
	}

	public static int? ParseNullableInt(int lineNumber, string[] values, Dictionary<string, int> dict, string name)
	{
		if (!dict.ContainsKey(name))
		{
			return null;
		}

		string value = values[dict[name]].Trim('"');
		try
		{
			if (string.IsNullOrEmpty(value))
				return null;
			else
				return int.Parse(value);
		}
		catch
		{
			throw new Exception($"Failed parsing line number: {lineNumber}, value: \"{value}\" for int? ({name})");
		}
	}

	public static bool ParseBool(int lineNumber, string[] values, Dictionary<string, int> dict, bool def, string name)
	{
		if (!dict.ContainsKey(name))
		{
			return def;
		}

		string value = values[dict[name]].Trim('"');
		try
		{
			if (string.IsNullOrEmpty(value))
				return def;
			else
				return bool.Parse(value);
		}
		catch
		{
			throw new Exception($"Failed parsing line number: {lineNumber}, value: \"{value}\" for bool? ({name})");
		}
	}

	public static bool? ParseNullableBool(int lineNumber, string[] values, Dictionary<string, int> dict, string name)
	{
		if (!dict.ContainsKey(name))
		{
			return null;
		}

		string value = values[dict[name]].Trim('"');
		try
		{
			if (string.IsNullOrEmpty(value))
				return null;
			else
				return bool.Parse(value);
		}
		catch
		{
			throw new Exception($"Failed parsing line number: {lineNumber}, value: \"{value}\" for bool? ({name})");
		}
	}

	public static Tenum ParseEnum<Tenum>(int lineNumber, string[] values, Dictionary<string, int> dict, string name, Tenum def) where Tenum : struct
	{
		if (!dict.ContainsKey(name))
		{
			return def;
		}

		string value = values[dict[name]].Trim('"');
		try
		{
			return string.IsNullOrEmpty(value) ? def : Enum.Parse<Tenum>(value);
		}
		catch
		{
			throw new Exception($"Failed parsing line number: {lineNumber}, value: \"{value}\" for {typeof(Tenum).Name.ToString()} ({name})");
		}
	}

	// https://csharpforums.net/threads/convert-object-properties-to-array.6464/
	public static string[] PublicProperties<T>(bool rw = true)
	{
		PropertyInfo[] myProperties = typeof(T).GetProperties(BindingFlags.Public |
															BindingFlags.SetProperty |
															BindingFlags.Instance);

		List<string> ret = new List<string>();

		foreach (PropertyInfo item in myProperties)
		{
			if (item.CustomAttributes.Any())
			{
				bool dont_add = false;
				foreach (var attribute in item.CustomAttributes)
				{
					if (attribute.AttributeType == typeof(Attributes.SqlIgnoreAttribute))
					{
						dont_add = true;
						break;
					}
				}
				if (dont_add) continue;
			}
			if (rw)
			{
				if (item.CanRead && item.CanWrite)
				{
					ret.Add(item.Name);
				}
			}
			else
			{
				ret.Add(item.Name);
			}
		}

		return ret.ToArray();
	}

	public static string CsvLine<T>(T line)
	{
		return string.Join(",", line.GetType()
									.GetProperties(BindingFlags.Instance | BindingFlags.Public)
									.Select((pi) =>
									{
										var str = pi.GetValue(line)?.ToString();
										if (null == str) return "";
										if (str.Contains(',')) return $"\"{str}\"";
										else return $"{str}";
									})
									.ToArray());
	}

	/// <summary>
	/// This method takes an AdventistGiving object and produces a hash by concatenating:
	///   LastName + FirstName + Address + Zip
	/// This string has should hopefully be unique, otherwise how would donors be distinguied
	/// if the have the same first and last name and live at the same address?
	/// </summary>
	/// <param name="tx"></param>
	/// <returns></returns>
	public static string AGHash(AdventistGiving tx)
	{
		return String.Concat(tx.LastName, tx.FirstName, tx.Address, tx.Zip);
	}

	public static string? GetDescription(Enum value)
	{
		FieldInfo? fieldInfo = value?.GetType()?.GetField(value.ToString());
		if (fieldInfo == null) return null;
		var attribute = (DescriptionAttribute?)fieldInfo?.GetCustomAttribute(typeof(DescriptionAttribute));
		return attribute?.Description;
	}

	#region cell select
	// The following few functions were borrowed from 
	// https://social.technet.microsoft.com/wiki/contents/articles/21202.wpf-programmatically-selecting-and-focusing-a-row-or-cell-in-a-datagrid.aspx
	public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
	{
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(obj, i);
			if (child != null && child is T)
				return (T)child;
			else
			{
				T childOfChild = FindVisualChild<T>(child);
				if (childOfChild != null)
					return childOfChild;
			}
		}
		return null;
	}

	public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
	{
		if (rowContainer != null)
		{
			DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
			if (presenter == null)
			{
				/* if the row has been virtualized away, call its ApplyTemplate() method
				 * to build its visual tree in order for the DataGridCellsPresenter
				 * and the DataGridCells to be created */
				rowContainer.ApplyTemplate();
				presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
			}
			if (presenter != null)
			{
				DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
				if (cell == null)
				{
					/* bring the column into view
					 * in case it has been virtualized away */
					dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
					cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
				}
				return cell;
			}
		}
		return null;
	}

	public static void SelectCellByIndex(DataGrid dataGrid, int rowIndex, int columnIndex)
	{
		if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.Cell))
			throw new ArgumentException("The SelectionUnit of the DataGrid must be set to Cell.");

		if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
			throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

		if (columnIndex < 0 || columnIndex > (dataGrid.Columns.Count - 1))
			throw new ArgumentException(string.Format("{0} is an invalid column index.", columnIndex));

		dataGrid.SelectedCells.Clear();

		object item = dataGrid.Items[rowIndex]; //=Product X
		DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
		if (row == null)
		{
			dataGrid.ScrollIntoView(item);
			row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
		}
		if (row != null)
		{
			DataGridCell cell = GetCell(dataGrid, row, columnIndex);
			if (cell != null)
			{
				DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(cell);
				dataGrid.SelectedCells.Add(dataGridCellInfo);
				cell.Focus();
			}
		}
	}
	#endregion
}
