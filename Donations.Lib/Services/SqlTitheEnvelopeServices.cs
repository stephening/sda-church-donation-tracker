using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlTitheEnvelopeServices : SqlHelper, ITitheEnvelopeServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	private string _envelopeDesignDbName => "dbo.EnvelopeDesign";
	public ObservableCollection<EnvelopeEntry>? TitheEnvelopeDesign { get; set; } = new ObservableCollection<EnvelopeEntry>();

	public SqlTitheEnvelopeServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	)
		: base(logger)
	{
		try
		{
			TitheEnvelopeDesign = SelectFromTable<EnvelopeEntry>(_envelopeDesignDbName);

			if (null == TitheEnvelopeDesign)
			{
				throw new OutOfMemoryException("TitheEnvelopeDesign is null");
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while loading from {_envelopeDesignDbName}.");
		}
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<string?> SaveTitheEnvelopeDesign(ObservableCollection<EnvelopeEntry> envelope, bool force = false)
	{
		try
		{
			await DropTable(_envelopeDesignDbName);

			await _sqlCreateTables.CreateEnvelopeDesignTable();

			return await WriteEntireTableAsync<EnvelopeEntry>(false, envelope, _envelopeDesignDbName, force, reseedOnDelete: true);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while saving envelope design to {_envelopeDesignDbName}.");
		}

		return null;
	}
}
