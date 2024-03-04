using System;

namespace Donations.Lib.Model;

public class DonorChange
{
	public int Id { get; set; }
	public int DonorId { get; set; }
	public string Name { get; set; }
	public DateTime WhenChanged { get; set; }
	public string WhatChanged { get; set; }
	public string WhoChanged { get; set; }
}
