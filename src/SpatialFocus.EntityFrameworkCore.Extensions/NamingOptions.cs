// <copyright file="NamingOptions.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System;
	using Humanizer;
	using Microsoft.EntityFrameworkCore.Metadata;

	public class NamingOptions
	{
		private Func<string, string> constraintNamingFunction;

		private Func<IMutableEntityType, bool> entitiesToSkipEntirely;

		private Func<IMutableEntityType, bool> entitiesToSkipTableNaming;

		private Func<string, string> postProcessingTableNamingFunction = name => name;

		private Func<string, string> propertyNamingFunction;

		private Func<string, string> tableNamingFunction;

		public static NamingOptions Default =>
			new NamingOptions().SetTableNamingSource(NamingSource.DbSet).SetNamingScheme(NamingScheme.SnakeCase);

		public Func<string, string> ConstraintNamingFunction
		{
			get => this.constraintNamingFunction ?? NamingFunction;
			set => this.constraintNamingFunction = value;
		}

		public Func<IMutableEntityType, bool> EntitiesToSkipEntirely
		{
			get => this.entitiesToSkipEntirely ?? (type => false);
			set => this.entitiesToSkipEntirely = value;
		}

		public Func<IMutableEntityType, bool> EntitiesToSkipTableNaming
		{
			get => this.entitiesToSkipTableNaming ?? (type => false);
			set => this.entitiesToSkipTableNaming = value;
		}

		public Func<string, string> NamingFunction { get; set; }

		public Func<string, string> PropertyNamingFunction
		{
			get => this.propertyNamingFunction ?? NamingFunction;
			set => this.propertyNamingFunction = value;
		}

		public Func<IMutableEntityType, string> TableNameSource { get; set; }

		public Func<string, string> TableNamingFunction
		{
			get =>
				name => this.postProcessingTableNamingFunction(this.tableNamingFunction != null
					? this.tableNamingFunction(name)
					: NamingFunction(name));
			set => this.tableNamingFunction = value;
		}

		public NamingOptions OverrideConstraintNaming(Func<string, string> namingFunc)
		{
			ConstraintNamingFunction = namingFunc;
			return this;
		}

		public NamingOptions OverridePropertyNaming(Func<string, string> namingFunc)
		{
			PropertyNamingFunction = namingFunc;
			return this;
		}

		public NamingOptions OverrideTableNaming(Func<string, string> namingFunc)
		{
			TableNamingFunction = namingFunc;
			return this;
		}

		public NamingOptions Pluralize()
		{
			this.postProcessingTableNamingFunction = name => name.Pluralize(false);
			return this;
		}

		public NamingOptions SetNamingScheme(Func<string, string> namingFunc)
		{
			NamingFunction = namingFunc;
			return this;
		}

		public NamingOptions SetTableNamingSource(Func<IMutableEntityType, string> namingFunc)
		{
			TableNameSource = namingFunc;
			return this;
		}

		public NamingOptions Singularize()
		{
			this.postProcessingTableNamingFunction = name => name.Singularize(false);
			return this;
		}

		public NamingOptions SkipEntireEntities(Func<IMutableEntityType, bool> skipFunction)
		{
			EntitiesToSkipEntirely = skipFunction;
			return this;
		}

		public NamingOptions SkipTableNamingForEntities(Func<IMutableEntityType, bool> skipFunction)
		{
			EntitiesToSkipTableNaming = skipFunction;
			return this;
		}

		public NamingOptions SkipTableNamingForGenericEntityTypes()
		{
			return SkipTableNamingForEntities(entity =>
				entity.ClrType.IsGenericType && (entity.ClrType.GetGenericTypeDefinition() == typeof(EnumWithNumberLookup<>) ||
					entity.ClrType.GetGenericTypeDefinition() == typeof(EnumWithStringLookup<>)));
		}
	}
}