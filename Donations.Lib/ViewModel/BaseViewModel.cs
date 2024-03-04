using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Donations.Lib.ViewModel;

public class BaseViewModel : ObservableObject
{
	public async Task Loading() { }

	public async Task Leaving() { }
}
