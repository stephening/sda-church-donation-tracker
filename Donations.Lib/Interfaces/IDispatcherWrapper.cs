using System;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IDispatcherWrapper
{
	void BeginInvoke(Delegate method);
	public void Invoke(Action callback);
	Task Yield();
}
