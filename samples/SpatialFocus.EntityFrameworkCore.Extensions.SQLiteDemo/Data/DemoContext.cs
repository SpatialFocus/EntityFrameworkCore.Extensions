// <copyright file="DemoContext.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions.SQLiteDemo.Data
{
	using System;
	using Microsoft.EntityFrameworkCore;
	using SpatialFocus.EntityFrameworkCore.Extensions.SQLiteDemo.Entities;

	public partial class DemoContext : DbContext
	{
		public DemoContext()
		{
			Database.EnsureDeleted();
			Database.EnsureCreated();
		}

		public DemoContext(DbContextOptions options)
			: base(options)
		{
			Database.EnsureDeleted();
			Database.EnsureCreated();
		}

		public DbSet<Product> Products { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlite("Data Source=demo.db");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Any custom model configuration should come before calling ConfigureEnumLookup and ConfigureNames
			modelBuilder.Entity<Product>()
				.OwnsMany<Review>(nameof(Product.Reviews),
					builder =>
					{
						builder.ConfigureOwnedEnumLookup(EnumLookupOptions.Default.Pluralize().UseStringAsIdentifier(), modelBuilder);
					});

			modelBuilder.ConfigureEnumLookup(EnumLookupOptions.Default.Pluralize().UseStringAsIdentifier());

			modelBuilder.ConfigureNames(NamingOptions.Default.Pluralize()
				////.SetTableNamingSource(NamingSource.ClrType)
				////.SetNamingScheme(NamingScheme.SnakeCase)
				////.OverrideTableNaming(NamingScheme.SnakeCase)
				.SkipTableNamingForGenericEntityTypes());

			modelBuilder.Entity<Product>()
				.HasData(
					new Product
					{
						ProductId = 1,
						ProductCategory = ProductCategory.Book,
						Name = "Robinson Crusoe",
						ReleaseDate = new DateTime(1719, 4, 25),
						Price = 14.99,
					},
					new Product
					{
						ProductId = 2,
						ProductCategory = ProductCategory.Bluray,
						Name = "Rogue One: A Star Wars Story",
						ReleaseDate = new DateTime(2017, 5, 4),
						Price = 11.99,
					},
					new Product
					{
						ProductId = 3,
						ProductCategory = ProductCategory.CD,
						Name = "Wham! - Last Christmas",
						ReleaseDate = new DateTime(1984, 12, 3),
						Price = 6.97,
						IdealForSpecialOccasion = SpecialOccasion.Christmas,
					});
		}
	}
}