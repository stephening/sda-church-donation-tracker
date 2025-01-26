using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for PrintPreviewView.xaml
/// </summary>
public partial class PrintPreviewView : Window
{
	private readonly PrintPreviewViewModel _viewModel;
	private readonly HelpView _helpView;
	private string _helpTarget;

	public delegate PrintPreviewView Factory(enumPrintout printoutType, string helpTarget, Action<FlowDocument, string?, double, double> render);

	public PrintPreviewView(
		PrintPreviewViewModel printPreviewViewModel,
		enumPrintout printoutType,
		string helpTarget,
		HelpView helpView,
		Action<FlowDocument, string?, double, double> render
		)
	{
		InitializeComponent();
		DataContext = printPreviewViewModel;

		_viewModel = printPreviewViewModel;
		_helpView = helpView;
		_helpTarget = helpTarget;
		_viewModel.SetRenderAction(printoutType, render, PrintPreview);
	}

	private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (e.Key == Key.F1)
		{
			_helpView.ShowTarget(_helpTarget);
		}
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		Width = (0 <= Persist.Default.PrintPreviewWidth) ? Persist.Default.PrintPreviewWidth : Width;
		Height = (0 < Persist.Default.PrintPreviewHeight) ? Persist.Default.PrintPreviewHeight : Height;
		Top = Persist.Default.PrintPreviewTop;
		Left = Persist.Default.PrintPreviewLeft;
		if (!string.IsNullOrEmpty(Persist.Default.PrintPreviewWindowState))
			WindowState = Enum.Parse<WindowState>(Persist.Default.PrintPreviewWindowState);
	}

	private void Window_Unloaded(object sender, RoutedEventArgs e)
	{
		_viewModel?.SaveSettings();

		Persist.Default.PrintPreviewTop = Top;
		Persist.Default.PrintPreviewLeft = Left;
		Persist.Default.PrintPreviewWidth = Width;
		Persist.Default.PrintPreviewHeight = Height;
		Persist.Default.PrintPreviewWindowState = WindowState.ToString();
		Persist.Default.Save();

		_helpView.ForceClose();
	}
}
