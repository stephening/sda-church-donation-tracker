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

	public async Task Yield()
	{
	}
}
