using Donations.Lib;
using Donations.Lib.ViewModel;
using Markdig;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace MarkdownToHtml
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private string ReplaceFirstOnly(string text, string search, string replace)
		{
			int pos = text.IndexOf(search);
			if (pos < 0)
			{
				return text;
			}
			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Length < 1 || !File.Exists(e.Args[0]))
			{
				MessageBox.Show("MarkdownToHtml <README.md>");
				Current.Shutdown();
			}
			try
			{
				string str = "";

				using (var sr = new StreamReader(e.Args[0]))
				{
					str = sr.ReadToEnd();
				}

				var parsed = Markdown.Parse(str);

				var HelpNavigation = new ObservableCollection<HelpNavigationViewModel>();
				Dictionary<int, ObservableCollection<HelpNavigationViewModel>> levels = new Dictionary<int, ObservableCollection<HelpNavigationViewModel>>();
				Dictionary<int, string> path = new Dictionary<int, string>();
				Dictionary<string, string> anchors = new Dictionary<string, string>();
				path[0] = "#";
				int prevLevel = -1;
				var currNode = HelpNavigation;

				string html = Markdown.ToHtml(str.Replace("./Documentation/screens/", ""));

				foreach (var item in parsed)
				{
					if (item.GetType() == typeof(HeadingBlock))
					{
						HeadingBlock headingBlock = (HeadingBlock)item;
						string? label = headingBlock.Inline?.FirstChild?.ToString();
						string? target = label?.Replace(" ", "-");

						if (-1 != prevLevel)
						{
							for (int i = prevLevel; i > headingBlock.Level - 1; i--)
							{
								path.Remove(i);
							}
						}

#pragma warning disable CS8601 // Possible null reference assignment.
						path[headingBlock.Level] = target;
#pragma warning restore CS8601 // Possible null reference assignment.

						string? fullTarget = string.Join('-', path.Values);

						html = ReplaceFirstOnly(html, $"<h{headingBlock.Level}>{label}</h{headingBlock.Level}>", $"<h{headingBlock.Level} id=\"{fullTarget[1..]}\">{label}</h{headingBlock.Level}>");

#pragma warning disable CS8601 // Possible null reference assignment.
						anchors[fullTarget] = label;
#pragma warning restore CS8601 // Possible null reference assignment.

						if (-1 == prevLevel || prevLevel == headingBlock.Level)
						{
							currNode.Add(new HelpNavigationViewModel() { Label = label, Level = headingBlock.Level, Target = fullTarget });
						}
						else if (prevLevel < headingBlock.Level)
						{
							levels[prevLevel] = currNode;
							currNode = currNode.Last().Children!;
							currNode.Add(new HelpNavigationViewModel() { Label = label, Level = headingBlock.Level, Target = fullTarget });
						}
						else if (prevLevel > headingBlock.Level)
						{
							currNode = levels[headingBlock.Level];
							currNode.Add(new HelpNavigationViewModel() { Label = label, Level = headingBlock.Level, Target = fullTarget });
						}
						prevLevel = headingBlock.Level;
					}
				}

				using (var outputStream = new StreamWriter(Persist.Default.HtmlHelpFile))
				{
					await outputStream.WriteAsync(html);
				}

				JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };

				var jstring = JsonSerializer.Serialize(HelpNavigation, options: options);
				using (var navTreeWriter = new StreamWriter(Persist.Default.NavTreeJsonFile))
				{
					await navTreeWriter.WriteAsync(jstring);
				}

				jstring = JsonSerializer.Serialize(anchors, options: options);
				using (var navAnchorsWriter = new StreamWriter(Persist.Default.NavAnchorsJsonFile))
				{
					await navAnchorsWriter.WriteAsync(jstring);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			Current.Shutdown();
		}
	}
}
