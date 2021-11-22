// <copyright file="NamingExtension.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System.Linq;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;

	public static class NamingExtension
	{
		public static void ConfigureNames(this ModelBuilder modelBuilder, NamingOptions namingOptions)
		{
			foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
			{
				if (namingOptions.EntitiesToSkipEntirely(entity))
				{
					continue;
				}

				// Entity / Table
				if (!namingOptions.EntitiesToSkipTableNaming(entity))
				{
					string tableName = namingOptions.TableNameSource(entity);

					entity.SetTableName(namingOptions.TableNamingFunction(tableName));
				}

				// Properties
				entity.GetProperties()
					.ToList()
					.ForEach(x => x.SetColumnName(namingOptions.ColumnNamingFunction(x.GetColumnBaseName())));

				// Primary and Alternative keys
				entity.GetKeys().ToList().ForEach(x => x.SetName(namingOptions.ConstraintNamingFunction(x.GetName())));

				// Foreign keys
				entity.GetForeignKeys()
					.ToList()
					.ForEach(x => x.SetConstraintName(namingOptions.ConstraintNamingFunction(x.GetConstraintName())));

				// Indices
				entity.GetIndexes()
					.ToList()
					.ForEach(x => x.SetDatabaseName(namingOptions.ConstraintNamingFunction(x.GetDatabaseName())));
			}
		}
	}
}