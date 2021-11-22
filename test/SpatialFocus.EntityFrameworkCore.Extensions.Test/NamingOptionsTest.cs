// <copyright file="NamingOptionsTest.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions.Test
{
	using System;
	using System.Linq;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.ChangeTracking;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Storage;
	using SpatialFocus.EntityFrameworkCore.Extensions.Test.Entities;
	using Xunit;

	public class NamingOptionsTest
	{
		protected ProductContext GetContext(EnumLookupOptions? enumLookupOptions = null, NamingOptions? namingOptions = null)
		{
			DbContextOptions<ProductContext> options = new DbContextOptionsBuilder<ProductContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString(), new InMemoryDatabaseRoot())
				.Options;

			ProductContext context = new ProductContext(options, null, namingOptions);
			context.Database.EnsureCreated();

			return context;
		}

#if NET5_0_OR_GREATER
		[Fact]
		public void OverrideColumnNaming()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().OverrideColumnNaming(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));

			Assert.Equal("product_tag_id", findEntityType.FindProperty(nameof(ProductTag.ProductTagId)).GetColumnBaseName());
		}
#else
		[Fact]
		public void OverrideColumnNaming()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().OverrideColumnNaming(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));

			Assert.Equal("product_tag_id", findEntityType.FindProperty(nameof(ProductTag.ProductTagId)).GetColumnName());
		}
#endif

#if NET5_0_OR_GREATER
		[Fact]
		public void OverrideConstraintNaming()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().OverrideConstraintNaming(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));
			Assert.Equal("ProductTag", findEntityType.GetTableName());
			Assert.Equal("ProductTagId", findEntityType.FindProperty(nameof(ProductTag.ProductTagId)).GetColumnBaseName());
			Assert.True(findEntityType.GetKeys().All(x => x.GetName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetForeignKeys().All(x => x.GetConstraintName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetIndexes().All(x => x.GetDatabaseName() == NamingScheme.SnakeCase(x.GetDefaultDatabaseName())));
		}
#else
		[Fact]
		public void OverrideConstraintNaming()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().OverrideConstraintNaming(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));
			Assert.Equal("ProductTag", findEntityType.GetTableName());
			Assert.Equal("ProductTagId", findEntityType.FindProperty(nameof(ProductTag.ProductTagId)).GetColumnName());
			Assert.True(findEntityType.GetKeys().All(x => x.GetName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetForeignKeys().All(x => x.GetConstraintName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetIndexes().All(x => x.GetName() == NamingScheme.SnakeCase(x.GetDefaultName())));
		}
#endif

		[Fact]
		public void OverrideTableNaming()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().OverrideTableNaming(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));
			Assert.Equal("product_tag", findEntityType.GetTableName());
		}

		[Fact]
		public void Pluralize()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().Pluralize());

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));
			Assert.Equal("ProductTags", findEntityType.GetTableName());
		}

		[Fact]
		public void PluralizeAndOverrideTableNaming()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().Pluralize().OverrideTableNaming(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));
			Assert.Equal("product_tags", findEntityType.GetTableName());
		}

#if NET5_0_OR_GREATER
		[Fact]
		public void SetNamingScheme()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().SetNamingScheme(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));
			Assert.Equal("product_tag", findEntityType.GetTableName());
			Assert.Equal("product_tag_id", findEntityType.FindProperty(nameof(ProductTag.ProductTagId)).GetColumnBaseName());
			Assert.True(findEntityType.GetKeys().All(x => x.GetName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetForeignKeys().All(x => x.GetConstraintName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetIndexes().All(x => x.GetDatabaseName() == NamingScheme.SnakeCase(x.GetDefaultDatabaseName())));
		}
#else
		[Fact]
		public void SetNamingScheme()
		{
			ProductContext context = GetContext(namingOptions: new NamingOptions().SetNamingScheme(NamingScheme.SnakeCase));

			IEntityType findEntityType = context.Model.FindEntityType(typeof(ProductTag));
			Assert.Equal("product_tag", findEntityType.GetTableName());
			Assert.Equal("product_tag_id", findEntityType.FindProperty(nameof(ProductTag.ProductTagId)).GetColumnName());
			Assert.True(findEntityType.GetKeys().All(x => x.GetName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetForeignKeys().All(x => x.GetConstraintName() == NamingScheme.SnakeCase(x.GetDefaultName())));
			Assert.True(findEntityType.GetIndexes().All(x => x.GetName() == NamingScheme.SnakeCase(x.GetDefaultName())));
		}
#endif

		[Fact]
		public void MultipleProviders()
		{
			DbContextOptions<ChangeTrackerInConstructorContext> inMemoryOptions = new DbContextOptionsBuilder<ChangeTrackerInConstructorContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString(), new InMemoryDatabaseRoot())
				.Options;

			ChangeTrackerInConstructorContext _ = new ChangeTrackerInConstructorContext(inMemoryOptions, EnumLookupOptions.Default, null);

			DbContextOptions<ChangeTrackerInConstructorContext> sqliteOptions = new DbContextOptionsBuilder<ChangeTrackerInConstructorContext>()
				.UseSqlite("Filename=:memory:")
				.Options;

			ChangeTrackerInConstructorContext context = new ChangeTrackerInConstructorContext(sqliteOptions, EnumLookupOptions.Default, null);
			context.Database.EnsureCreated();
			context.Dispose();
		}

		private class ChangeTrackerInConstructorContext : ProductContext
		{
			public ChangeTrackerInConstructorContext(DbContextOptions options, EnumLookupOptions enumLookupOptions, NamingOptions namingOptions)
				: base(options, enumLookupOptions, namingOptions)
			{
				ChangeTracker _ = ChangeTracker;
			}
		}
	}
}