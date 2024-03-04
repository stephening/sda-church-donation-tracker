using Dapper;
using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlPictureServices : SqlHelper, IPictureServices
{
	private string _pictureDbName => "dbo.OrganizationLogo";
	private Picture? _pictureCache;
	private readonly SqlCreateTables _sqlCreateTables;

	public SqlPictureServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	)
		: base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	public Picture GetLogo()
	{
		try
		{
			if (null != _pictureCache)
			{
				return _pictureCache;
			}

			var ret = SelectFromTable<Picture>(_pictureDbName);

			if (0 < ret.Count)
			{
				_pictureCache = ret.Single();

				return _pictureCache;
			}

			return null;
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_pictureDbName}.");
			return null;
		}
	}

	public async Task<int> SaveLogo(Picture picture)
	{
		// write the new T record
		try
		{
			await DropTable(_pictureDbName);

			using IDbConnection conn = new SqlConnection(_connString);
			{
				await _sqlCreateTables.CreateOrganizationLogoTable();

				var properties = Helper.PublicProperties<Picture>().ToList();
				properties.Remove("Id");
				string command = "";
				command += $"INSERT INTO {_pictureDbName} ({string.Join(',', properties)}) OUTPUT INSERTED.Id VALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";

				int id = await conn.ExecuteScalarAsync<int>(command, picture);
				return id;
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught writing to {_pictureDbName}.");
		}

		return -1;
	}
}
