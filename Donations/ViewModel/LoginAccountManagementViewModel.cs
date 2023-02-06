using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Donations.ViewModel
{
	class LoginAccountManagementViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
