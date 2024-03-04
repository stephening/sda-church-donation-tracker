using Autofac;
using Donations.Lib.Interfaces;
using Donations.Lib.Services;
using Donations.Lib.TestDataServices;
using Donations.Lib.View;
using Donations.Lib.ViewModel;
using Serilog;
using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Donations.Lib;

public static class DependencyInjection
{
	private static string _outputTemplate = "{Timestamp:HH:mm:ss.fff}, {Level}, {Func}, {File}, {Line}, {Exception}, {Message}{NewLine}";

	public static ContainerBuilder? Builder { get; private set; }

	private static ILifetimeScope _scope;

	public static ILifetimeScope Scope
	{
		get { return _scope; }
		set
		{
			_scope = value;

			CategoryServices = _scope.Resolve<ICategoryServices>();
			DonorServices = _scope.Resolve<IDonorServices>();
		}
	}

	public static ContainerBuilder SetupDonationsLib(this
		ContainerBuilder builder)
	{
		Builder = builder;

		Builder.Register(logger => new LoggerConfiguration()
			.WriteTo.File(
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Donation tracker/log-.log"),
				outputTemplate: _outputTemplate,
				rollingInterval: RollingInterval.Day)
			.CreateLogger())
			.As<ILogger>().SingleInstance();
		Builder.RegisterType<MainWindowControl>();
		Builder.RegisterType<MainWindowViewModel>();
		Builder.RegisterType<ApplicationDispatcherWrapper>().As<IDispatcherWrapper>().SingleInstance();
		Builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
		Builder.RegisterType<DataHelpers>().As<IDataHelpers>().InstancePerDependency();
		Builder.RegisterType<ReflectionHelpers>().As<IReflectionHelpers>().InstancePerDependency();
		Builder.RegisterType<SqlAppSettingsServices>().As<IAppSettingsServices>().SingleInstance();
		Builder.RegisterType<SqlTitheEnvelopeServices>().As<ITitheEnvelopeServices>().SingleInstance();
		Builder.RegisterType<SqlCategoryMapServices>().As<ICategoryMapServices>().SingleInstance();
		Builder.RegisterType<SqlDonorMapServices>().As<IDonorMapServices>().SingleInstance();
		Builder.RegisterType<SqlBatchServices>().As<IBatchServices>().SingleInstance();
		Builder.RegisterType<SqlDonorServices>().As<IDonorServices>().SingleInstance();
		Builder.RegisterType<SqlDonorReportsServices>().As<IDonorReportServices>().SingleInstance();
		Builder.RegisterType<SqlDonorChangeServices>().As<IDonorChangeServices>().SingleInstance();
		Builder.RegisterType<SqlDonationServices>().As<IDonationServices>().SingleInstance();
		Builder.RegisterType<SqlCategoryServices>().As<ICategoryServices>().SingleInstance();
		Builder.RegisterType<SqlIndividualReportServices>().As<IIndividualReportServices>().SingleInstance();
		Builder.RegisterType<SqlPictureServices>().As<IPictureServices>().SingleInstance();
		Builder.RegisterType<SqlPrintSettingsServices>().As<IPrintSettingsServices>().SingleInstance();
		Builder.RegisterType<SqlCreateTables>().SingleInstance();
		Builder.RegisterType<BatchBrowserViewModel>().SingleInstance();
		Builder.RegisterType<DonationBrowserViewModel>().SingleInstance();
		Builder.RegisterType<DonorInputViewModel>();
		Builder.RegisterType<GeneralViewModel>().SingleInstance();
		Builder.RegisterType<BatchReviewView>();
		Builder.RegisterType<BatchReviewViewModel>().SingleInstance();
		Builder.RegisterType<CategoryReviewView>();
		Builder.RegisterType<CategoryReviewViewModel>().SingleInstance();
		Builder.RegisterType<ConfirmDonorMergeView>();
		Builder.RegisterType<ConfirmDonorMergeViewModel>();
		Builder.RegisterType<BatchPrintViewModel>().SingleInstance();
		Builder.RegisterType<DonationPopupView>();
		Builder.RegisterType<DonationPopupViewModel>().SingleInstance();
		Builder.RegisterType<DonorModalView>();
		Builder.RegisterType<DonorViewModel>();
		Builder.RegisterType<DonorMapViewModel>().SingleInstance();
		Builder.RegisterType<DonorSelectionView>();
		Builder.RegisterType<DonorSelectionViewModel>();
		Builder.RegisterType<CategorySelectionView>();
		Builder.RegisterType<CategorySelectionViewModel>();
		Builder.RegisterType<CategoryMapViewModel>().SingleInstance();
		Builder.RegisterType<EnvelopeDesignViewModel>().SingleInstance();
		Builder.RegisterType<CategoryView>().SingleInstance();
		Builder.RegisterType<CategoryViewModel>().SingleInstance();
		Builder.RegisterType<ReportsViewModel>().SingleInstance();
		Builder.RegisterType<AdventistGivingViewModel>().SingleInstance();
		Builder.RegisterType<AGDonorResolutionViewModel>().SingleInstance();
		Builder.RegisterType<AGCategoryResolutionViewModel>().SingleInstance();
		Builder.RegisterType<AGDonationSummaryViewModel>().SingleInstance();
		Builder.RegisterType<DonorModalView>();
		Builder.RegisterType<ScreenShotHelper>();
		Builder.RegisterType<EmailAccountPasswordView>();
		Builder.RegisterType<TableHelper>().SingleInstance();
		Builder.RegisterType<PrintPreviewView>();
		Builder.RegisterType<PrintPreviewViewModel>().SingleInstance();
		Builder.RegisterType<HelpView>();
		Builder.RegisterType<HelpViewModel>();

		return Builder;
	}

	public static ContainerBuilder RegisterTestDataServices(this
		ContainerBuilder builder)
	{
		Builder = builder;

		Builder.Register(logger => new LoggerConfiguration()
			.WriteTo.File(
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Donation tracker/testdata log-.log"),
				outputTemplate: _outputTemplate,
				rollingInterval: RollingInterval.Day)
			.CreateLogger())
			.As<ILogger>().SingleInstance();
		Builder.RegisterType<TestDispatcherWrapper>().As<IDispatcherWrapper>().SingleInstance();
		Builder.RegisterType<MockFileSystem>().As<IFileSystem>().SingleInstance();
		Builder.RegisterType<TestDataAppSettingsServices>().As<IAppSettingsServices>().SingleInstance();
		Builder.RegisterType<TestDataCategoryServices>().As<ICategoryServices>().SingleInstance();
		Builder.RegisterType<TestDataDonorMapServices>().As<IDonorMapServices>().SingleInstance();
		Builder.RegisterType<TestDataCategoryMapServices>().As<ICategoryMapServices>().SingleInstance();
		Builder.RegisterType<TestDataDonorServices>().As<IDonorServices>().SingleInstance();
		Builder.RegisterType<TestDataDonorReportServices>().As<IDonorReportServices>().SingleInstance();
		Builder.RegisterType<TestDataDonorChangeServices>().As<IDonorChangeServices>().SingleInstance();
		Builder.RegisterType<TestDataBatchServices>().As<IBatchServices>().SingleInstance();
		Builder.RegisterType<TestDataDonationServices>().As<IDonationServices>().SingleInstance();
		Builder.RegisterType<TestDataIndividualReportServices>().As<IIndividualReportServices>().SingleInstance();
		Builder.RegisterType<TestDataPictureServices>().As<IPictureServices>().SingleInstance();
		Builder.RegisterType<TestDataPrintSettingsServices>().As<IPrintSettingsServices>().SingleInstance();
		Builder.RegisterType<TestDataTitheEnvelopeServices>().As<ITitheEnvelopeServices>().SingleInstance();

		return Builder;
	}

	public static ContainerBuilder SetupWizard(this
		ContainerBuilder builder)
	{
		Builder = builder;
		Builder.Register(logger => new LoggerConfiguration()
			.WriteTo.File(
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Donation tracker/wizzard log-.log"),
				outputTemplate: _outputTemplate,
				rollingInterval: RollingInterval.Day)
			.CreateLogger())
			.As<ILogger>().SingleInstance();
		Builder.RegisterType<ApplicationDispatcherWrapper>().As<IDispatcherWrapper>().SingleInstance();
		Builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
		Builder.RegisterType<DataHelpers>().As<IDataHelpers>().InstancePerDependency();
		Builder.RegisterType<ReflectionHelpers>().As<IReflectionHelpers>().InstancePerDependency();
		Builder.RegisterType<WizardMainWindow>().SingleInstance();
		Builder.RegisterType<WizardMainWindowViewModel>().SingleInstance();
		Builder.RegisterType<WizardSpecifyConnectionStringViewModel>().SingleInstance();
		Builder.RegisterType<WizardSqlChoiceViewModel>().SingleInstance();
		Builder.RegisterType<WizardSqlConnectViewModel>().SingleInstance();
		Builder.RegisterType<WizardSpecifyLogoViewModel>().SingleInstance();
		Builder.RegisterType<SqlCreateTables>().SingleInstance();
		Builder.RegisterType<SqlBatchServices>().As<IBatchServices>().SingleInstance();
		Builder.RegisterType<SqlPictureServices>().As<IPictureServices>().SingleInstance();
		Builder.RegisterType<SqlCategoryServices>().As<ICategoryServices>().SingleInstance();
		Builder.RegisterType<SqlDonorServices>().As<IDonorServices>().SingleInstance();
		Builder.RegisterType<SqlDonorReportsServices>().As<IDonorReportServices>().SingleInstance();
		Builder.RegisterType<SqlDonorChangeServices>().As<IDonorChangeServices>().SingleInstance();
		Builder.RegisterType<SqlDonationServices>().As<IDonationServices>().SingleInstance();
		Builder.RegisterType<SqlTitheEnvelopeServices>().As<ITitheEnvelopeServices>().SingleInstance();
		Builder.RegisterType<SqlDonorReportsServices>().As<IDonorReportServices>().SingleInstance();
		Builder.RegisterType<SqlCreateTables>().SingleInstance();
		Builder.RegisterType<WizardImportCategoriesViewModel>().SingleInstance();
		Builder.RegisterType<WizardImportDonorsViewModel>().SingleInstance();
		Builder.RegisterType<WizardImportDonationsViewModel>().SingleInstance();
		Builder.RegisterType<BatchBrowserViewModel>().SingleInstance();
		Builder.RegisterType<BatchReviewView>();
		Builder.RegisterType<BatchReviewViewModel>();
		Builder.RegisterType<DonorInputViewModel>();
		Builder.RegisterType<BatchPrintViewModel>().SingleInstance();
		Builder.RegisterType<EnvelopeDesignViewModel>().SingleInstance();
		Builder.RegisterType<HelpView>();
		Builder.RegisterType<HelpViewModel>();

		return builder;
	}

	public static T Resolve<T>()
	{
		return _scope.Resolve<T>();
	}

	public static CategorySelectionView? CategorySelectionView => _scope?.Resolve<CategorySelectionView>();
	public static DonorSelectionView? DonorSelectionView => _scope?.Resolve<DonorSelectionView>();

	public static ICategoryServices? CategoryServices { get; private set; }
	public static IDonorServices? DonorServices { get; private set; }
}
