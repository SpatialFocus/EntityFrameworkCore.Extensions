// <copyright file="EnumWithStringLookupAndDescription.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	public class EnumWithStringLookupAndDescription<T> : EnumWithStringLookup<T>
	{
		public EnumWithStringLookupAndDescription()
			: base()
		{
		}

		public EnumWithStringLookupAndDescription(T value)
			: base(value)
		{
		}

		public string Description { get; set; }
	}
}