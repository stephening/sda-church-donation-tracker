using System.Windows;

namespace Donations
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			di.Data.LoadData();
		}
	}
}
