using Serilog;
using System;
using System.Runtime.CompilerServices;

namespace Donations.Lib.Extensions;

public static class LoggerExtensions
{
	public static void Info(this ILogger logger,
		string message,
		[CallerMemberName] string func = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		logger
			.ForContext("Func", func)
			.ForContext("File", file)
			.ForContext("Line", line)
			.Information(message);
	}

	public static void Err(this ILogger logger,
		string message,
		[CallerMemberName] string func = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		logger
			.ForContext("Func", func)
			.ForContext("File", file)
			.ForContext("Line", line)
			.Error(message);
	}

	public static void Err(this ILogger logger,
		Exception ex,
		string message,
		[CallerMemberName] string func = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		logger
			.ForContext("Func", func)
			.ForContext("File", file)
			.ForContext("Line", line)
			.Error(ex, message);
	}

	public static void Dbg(this ILogger logger,
		string message,
		[CallerMemberName] string func = "",
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0)
	{
		logger
			.ForContext("Func", func)
			.ForContext("File", file)
			.ForContext("Line", line)
			.Debug(message);
	}

}
