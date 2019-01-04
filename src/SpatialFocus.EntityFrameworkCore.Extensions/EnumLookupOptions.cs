// <copyright file="EnumLookupOptions.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System;
	using Humanizer;
	using Microsoft.EntityFrameworkCore;

	public class EnumLookupOptions
	{
		private Func<string, string> namingFunction;

		private Func<string, string> postProcessingTableNamingFunction = name => name;

		public static EnumLookupOptions Default
		{
			get
			{
				EnumLookupOptions enumOptions = new EnumLookupOptions();
				enumOptions.SetNamingScheme(NamingScheme.SnakeCase);
				enumOptions.UseNumberLookup = true;
				enumOptions.UseEnumsWithAttributesOnly = false;
				enumOptions.DeleteBehavior = DeleteBehavior.Cascade;

				return enumOptions;
			}
		}

		internal Func<string, string> NamingFunction => name => this.postProcessingTableNamingFunction(this.namingFunction(name));

		internal DeleteBehavior DeleteBehavior { get; private set; }

		internal bool UseEnumsWithAttributesOnly { get; private set; }

		internal bool UseNumberLookup { get; private set; }

		public EnumLookupOptions Pluralize()
		{
			this.postProcessingTableNamingFunction = name => name.Pluralize(false);

			return this;
		}

		public EnumLookupOptions SetDeleteBehavior(DeleteBehavior deleteBehavior)
		{
			DeleteBehavior = deleteBehavior;

			return this;
		}

		public EnumLookupOptions SetNamingScheme(Func<string, string> namingFunc)
		{
			this.namingFunction = namingFunc;

			return this;
		}

		public EnumLookupOptions Singularize()
		{
			this.postProcessingTableNamingFunction = name => name.Singularize(false);

			return this;
		}

		public EnumLookupOptions UseEnumsWithAttributeOnly()
		{
			UseEnumsWithAttributesOnly = true;

			return this;
		}

		public EnumLookupOptions UseNumberAsIdentifier()
		{
			UseNumberLookup = true;

			return this;
		}

		public EnumLookupOptions UseStringAsIdentifier()
		{
			UseNumberLookup = false;

			return this;
		}
	}
}