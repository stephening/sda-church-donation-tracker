﻿using Donations.Lib.ViewModel;
using Microsoft.Win32;
using System.Windows.Controls;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for DirectoryPdfView.xaml
/// </summary>
public partial class DirectoryPdfView : UserControl
{
	private DirectoryPdfViewModel? _viewModel;
	public DirectoryPdfView()
	{
		InitializeComponent();
	}

	private async void SavePdf(object sender, System.Windows.RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Pdf file|*.pdf";

		if (true == saveFileDialog.ShowDialog())
		{
			await _viewModel!.SavePdf(saveFileDialog.FileName);
		}
	}

	private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
	{
		_viewModel = DataContext as DirectoryPdfViewModel;
		_viewModel?.SetDocument(DirectoryDocument, Rich.RTB);
	}

	private void ImageBrowse(object sender, System.Windows.RoutedEventArgs e)
	{
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Filter = "Images (*.jpg;*.png)|*.jpg;*.png";

		if (dlg.ShowDialog() == true)
		{
			_viewModel?.SetCoverImageFile(dlg.FileName);
		}
	}
}
