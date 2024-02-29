using Donations.Lib.Interfaces;
using System;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDispatcherWrapper : IDispatcherWrapper
{
	public void BeginInvoke(Delegate method)
	{
		method.DynamicInvoke();
	}

	public void Invoke(Action callback)
	{
		callback.Invoke();
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task Yield()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
	}
}
