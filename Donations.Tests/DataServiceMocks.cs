using Autofac;
using Donations.Lib.Interfaces;
using Donations.Lib.TestDataServices;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Donations.Tests;

public static class DataServiceMocks
{
	public static ContainerBuilder? Builder { get; private set; }

	public static ContainerBuilder Register(this
		ContainerBuilder builder)
	{
		Builder = builder;
		Mock dataHelper = new Mock<IDataHelpers>();

		// https://github.com/TestableIO/System.IO.Abstractions
		IFileSystem mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
		{
			{ @"summarylist.xml", new MockFileData("") },
			{ @"date_total.txt", new MockFileData("") }
		});
		mockfs.File.Delete("summarylist.xml");
		mockfs.File.Delete("date_total.txt");

		builder.RegisterInstance(mockfs).As<IFileSystem>();
		builder.RegisterInstance(dataHelper.Object).As<IDataHelpers>();
		builder.RegisterType<TestDataCategoryServices>().As<ICategoryServices>().SingleInstance();
		builder.RegisterType<TestDataDonorMapServices>().As<IDonorMapServices>().SingleInstance();
		builder.RegisterType<TestDataCategoryMapServices>().As<ICategoryMapServices>().SingleInstance();
		builder.RegisterType<TestDataDonorServices>().As<IDonorServices>().SingleInstance();
		builder.RegisterType<TestDataBatchServices>().As<IBatchServices>().SingleInstance();
		builder.RegisterType<TestDataDonationServices>().As<IDonationServices>().SingleInstance();
		builder.RegisterType<TestDataIndividualReportServices>().As<IIndividualReportServices>().SingleInstance();
		builder.RegisterType<TestDataPictureServices>().As<IPictureServices>().SingleInstance();
		builder.RegisterType<TestDataTitheEnvelopeServices>().As<ITitheEnvelopeServices>().SingleInstance();

		return builder;
	}
}
