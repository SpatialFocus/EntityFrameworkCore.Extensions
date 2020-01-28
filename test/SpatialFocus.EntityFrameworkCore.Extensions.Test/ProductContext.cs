// <copyright file="ProductContext.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions.Test
{
	using System;
	using Microsoft.EntityFrameworkCore;
	using SpatialFocus.EntityFrameworkCore.Extensions.Test.Entities;

	public class ProductContext : DbContext
	{
		public ProductContext(DbContextOptions options, EnumLookupOptions enumLookupOptions, NamingOptions namingOptions)
			: base(options)
		{
			EnumLookupOptions = enumLookupOptions;
			NamingOptions = namingOptions;
		}

		public DbSet<Product> Products { get; set; }

		public DbSet<ProductTag> ProductTags { get; set; }

		protected EnumLookupOptions EnumLookupOptions { get; }

		protected NamingOptions NamingOptions { get; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

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

			modelBuilder.Entity<ProductTag>().HasIndex("ProductId", "Name").IsUnique();

			if (EnumLookupOptions != null)
			{
				modelBuilder.ConfigureEnumLookup(EnumLookupOptions);
			}

			if (NamingOptions != null)
			{
				modelBuilder.ConfigureNames(NamingOptions);
			}
		}
	}
}