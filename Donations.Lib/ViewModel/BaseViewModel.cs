using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Donations.Lib.ViewModel;

public class BaseViewModel : ObservableObject
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task Loading() { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task Leaving() { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
