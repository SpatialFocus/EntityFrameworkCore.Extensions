// <copyright file="EnumWithNumberLookupAndDescription.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	public class EnumWithNumberLookupAndDescription<T> : EnumWithNumberLookup<T>
	{
		public EnumWithNumberLookupAndDescription()
			: base()
		{
		}

		public EnumWithNumberLookupAndDescription(T value)
			: base(value)
		{
		}

		public string Description { get; set; }
	}
}