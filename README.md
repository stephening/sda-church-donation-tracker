# sda-church-donation-tracker
Windows (C#/WPF) desktop application for tracking donations and outputting category subtotals for entry to GL software. It is a simple tab based application with a few subtabs.

The title contains sda (Seventh-Day Adventist) because one of the input methods is a *.csv from a giving platform called Adventist Giving (AG). The import from an AG csv file will be partially automated at first as new donors and donation categories are added or mapped. After that, imports will generally be extremely easy, with the occasional mapping of a new donor or donation category. The other input method is a manual process centered around a tithe envelope. Donations collected in an offering plate that are from specific donors, to specific categories will usually be given in a tithe envelope with details supplied.

Donations will be entered in batches, and the category subtotals from a batch will be entered into a General Ledger (GL) accounting program. Batches can be viewed edited edited after they are entered. There will be two types of batches, AG and manually entered donations collected at church.

This application as is writes all data to the local file system in xml format file. You can start with no data files and build up your donor database by adding donors as AG batches are entered. Of course donors can also be added or edited within the application. Donors, donations, and categories can be imported via csv files to form a starting point for the database going forward. There are future plans to enhance this software to work with a Microsoft SQL database backend to allow multiuser access.

The Software was developed using Visual Studio 2022 in C#/WPF/.NET 6.0. It is written with an MVVM pattern and a simple static class for dependency injection. It also has numerous unit tests written to work with xunit. File system mocking is done by using System.IO.Abstractions and the accompanying System.IO.Abstractions.TestingHelpers. Current NuGet packages added:

* CommunityToolkit.Mvvm
* System.IO.Abstractions

The unit test projects requires the following NuGet packages:

* Moq
* System.IO.Abstractions.TestingHelpers
* xunit
* xunit.runner.console
* xunit.runner.visualstudio


