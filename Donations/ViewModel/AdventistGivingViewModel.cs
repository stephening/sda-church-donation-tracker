using Donations.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the AdventistGivingView.xaml. This is the top level
	/// page which doesn't do much besides handling the batch date and note. Most of the work is done by the user controls
	/// under the tab items, and their view models. There are three tabs:
	///   - Donor resolution
	///   - Category resolution
	///   - Verify and submit
	/// Both the resolution tabs run in parallel and you can go back an forth between them until thay are both resolved.
	/// When both resolved, the verify tab will show a list of categories and their totals which should match up with the
	/// Adventist Giving report summary provided.
	/// </summary>
	public class AdventistGivingViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<AdventistGiving>? TransactionList { get; set; }

		private string _batchNote = "";
		/// <summary>
		/// The BatchNode property gets transferred to the Note member and will show up in a column in the batch browser.
		/// </summary>
		public string BatchNote
		{
			get { return _batchNote; }
			set
			{
				_batchNote = value;
				OnPropertyChanged();
			}
		}

		private double _targetTotal;

		/// <summary>
		/// The TargetTotal property should contain the expected total from the batch being imported. When the batch import
		/// is complete, there will be a computed total which will also be saved with the batch. If the two totals don't
		/// match, the will be highlighted in red in the batch browser view.
		/// </summary>
		public double TargetTotal
		{
			get { return _targetTotal; }
			set
			{
				_targetTotal = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Constructor. This reference is saved to Global.AG for other view model use.
		/// </summary>
		public AdventistGivingViewModel()
		{
			di.AG = this;
		}

		/// <summary>
		/// import task which is called from the Adventist Giving tab to import a batch of
		/// donations through the Adventist Giving (AG) system. This is an async function so the
		/// UI thread is not blocked when it is running. After importing the AG records, the
		/// name resolution is kicked off by calling the StartNameResolution() method.
		/// </summary>
		/// <param name="filePath">File path to the *.csv file</param>
		/// <returns></returns>
		public async Task Import(string? filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				throw new Exception("Null or empty filename passed to Import");

			await Task.Run(() =>
			{
				TransactionList = AdventistGiving.Import(filePath);

				di.DonorResolution?.StartNameResolution();
				di.CategoryResolution?.StartCategoryResolution();
				di.DonationSummary?.Loaded();
			});
		}

		/// <summary>
		/// Called after Submit() to return this tab to it's original ready state.
		/// </summary>
		public void Reset()
		{
			TransactionList?.Clear();
			BatchNote = "";
			di.DonorResolution?.ResolutionComplete(false);
			di.CategoryResolution?.ResolutionComplete(false);
		}
	}
}
