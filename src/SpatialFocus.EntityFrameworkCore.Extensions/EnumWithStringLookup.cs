// <copyright file="EnumWithStringLookup.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System.ComponentModel.DataAnnotations.Schema;

	public class EnumWithStringLookup<T>
	{
		public EnumWithStringLookup()
		{
		}

		public EnumWithStringLookup(T value)
		{
			Id = value;
		}

		public string Description { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public T Id { get; set; }
	}
}