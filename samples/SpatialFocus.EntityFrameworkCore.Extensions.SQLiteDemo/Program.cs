// <copyright file="Program.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions.SQLiteDemo
{
	using System;
	using System.Linq;
	using SpatialFocus.EntityFrameworkCore.Extensions.SQLiteDemo.Data;

	public class Program
	{
		private static void Main(string[] args)
		{
			using (DemoContext context = new DemoContext())
			{
				Console.WriteLine($"Found {context.Products.Count()} products.");
			}

			Console.WriteLine("--- press a key ---");
			Console.ReadKey();
		}
	}
}