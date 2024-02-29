using Donations.Lib.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Donations.Lib.Services;

public class ApplicationDispatcherWrapper : IDispatcherWrapper
{
	public void BeginInvoke(Delegate method) => Application.Current.Dispatcher.BeginInvoke(method);

	public void Invoke(Action callback) => Application.Current.Dispatcher.Invoke(callback);

	public async Task Yield() => await Dispatcher.Yield();
}
