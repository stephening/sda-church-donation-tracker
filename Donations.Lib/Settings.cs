namespace Donations.Lib
{
	public class Settings
	{
		public static void Save()
		{
			Persist.Default.Save();
		}
	}
}
