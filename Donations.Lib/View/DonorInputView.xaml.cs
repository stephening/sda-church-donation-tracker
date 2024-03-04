using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for DonorInputView.xaml
	/// </summary>
	public partial class DonorInputView : UserControl
	{
		private DispatcherTimer _timer = new DispatcherTimer();
		private DonorInputViewModel? _viewModel;

		public DonorInputView()
		{
			InitializeComponent();

			_timer.Tick += new EventHandler(Timer_Tick);
			_timer.Interval = new TimeSpan(0, 0, 1);
		}

		private bool _ignoreClick = false;
		private void Timer_Tick(object sender, EventArgs e)
		{
			_timer.Stop();
			_ignoreClick = false;
		}

		private async void Click_Submit(object sender, System.Windows.RoutedEventArgs e)
		{
			await _viewModel?.SubmitDonor();
			SummaryGrid.SelectedIndex = -1;
		}

		private void ChangeCategory(object sender, MouseButtonEventArgs e)
		{
			// This is needed if the user enters the dollar amount and then clicks to change the category without entering the $ amount first
			EnvelopeBody.CommitEdit(DataGridEditingUnit.Row, true);

			if (_ignoreClick)
			{
				// double click to close category selection leaves mouse up event in the queue, ignore it
				return;
			}

			try
			{
				if (0 < EnvelopeBody.SelectedCells.Count)
				{
					Donation? row = EnvelopeBody.SelectedCells[0].Item as Donation;
					if (null == row)
					{
						row = new Donation() { Category = "", Value = 0 };
						_viewModel?.IndividualDonations?.Add(row);
					}

					if (null != row)
					{
						CategorySelectionView dlg = DependencyInjection.CategorySelectionView;

						var ret = dlg.ShowDialog();

						// double-clicking to close the category selection dialog over the category list leaves a mouse click
						// in the queue, which gets handled and results in poping up the category selection window again, so
						// ignore mouse clicks for a second on entering this function
						_timer.Start();
						_ignoreClick = true;

						if (ret == true)
						{
							Category? cat = dlg.CategoryGrid?.SelectedItem as Category;
							var rowindex = _viewModel?.ChangeCategory(row, cat);
							if (null != rowindex)
							{
								Helper.SelectCellByIndex(EnvelopeBody, rowindex.Value, 1);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void EnvelopeBody_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				// everytime ENTER is hit in the grid, recalc the sum
				_viewModel?.ValueChanged();
			}
		}

		private void CheckOptionSelected(object sender, RoutedEventArgs e)
		{
			_viewModel?.UpdateSubmitEnabled();
			CheckNumberTextBox.Focus();
		}

		private void ChooseDonor_Click(object sender, RoutedEventArgs e)
		{
			DonorSelectionView dlg = DependencyInjection.DonorSelectionView;
			if (true == dlg.ShowDialog())
			{
				var SelectedName = (Donor)dlg.DonorGrid.SelectedItem;
				_viewModel?.ChooseDonor(SelectedName.Id);
				if (0 == EnvelopeBody.Items.Count)
				{
					MessageBox.Show("It is highly recommended that you add some categories to your data input design. Go to the Maintenance Tab, and then the Design title envelope sub tab.");
				}
				else
				{
					Helper.SelectCellByIndex(EnvelopeBody, 0, 1);
				}
			}
		}

		#region these two functions are used to select all in BatchTotal TextBox
		private void BatchTotal_SelectAll(object sender, MouseButtonEventArgs e)
		{
			(sender as TextBox)?.SelectAll();
		}

		private void BatchTotal_SelectAll(object sender, KeyboardFocusChangedEventArgs e)
		{
			(sender as TextBox)?.SelectAll();
		}

		private void BatchTotal_SelectivelyIgnoreMouseClick(object sender, MouseButtonEventArgs e)
		{
			if (false == (sender as TextBox)?.IsKeyboardFocusWithin)
			{
				e.Handled = true;

				(sender as TextBox)?.Focus();
			}
		}
		#endregion

		private void CheckNumber_TextChanged(object sender, TextChangedEventArgs e)
		{
			// we do this rather than just depend on normal binding because this will often
			// be the last entry before submit and the submit won't be enabled until leaving
			// this field to trigger the binding
			_viewModel?.CheckNumberChanged((sender as TextBox)?.Text);
		}

		private void EnvelopeBody_LostFocus(object sender, RoutedEventArgs e)
		{
			_viewModel?.ValueChanged();
		}

		private void DataGridRow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = (DataGridRow)sender;

			var ret = _viewModel?.SummarySelectionChanged(row.GetIndex());
			if (null != ret)
			{
				if (MessageBoxResult.Yes == MessageBox.Show(ret, "Overwrite current entry?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation))
				{
					_viewModel?.SummarySelectionChanged(row.GetIndex(), true);
				}
				else
				{
					Helper.SelectCellByIndex(EnvelopeBody, 0, 1);
					SummaryGrid.SelectedIndex = -1;
				}
			}
		}

		private void BatchTotalTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				// enter from BatchTotal will transfer control to the BatchDate
				BatchDatePicker.Focus();
			}
		}

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBoxResult.Yes == MessageBox.Show("Once reset, the intermidiate entries cannot be recovered. Are you sure you want to continue with the reset?", "Confirm reset", MessageBoxButton.YesNo, MessageBoxImage.Question))
			{
				_viewModel?.Reset();
			}
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as DonorInputViewModel;
		}
	}
}
