// <copyright file="EnumWithNumberLookup.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System.ComponentModel.DataAnnotations.Schema;

	public class EnumWithNumberLookup<T>
	{
		public EnumWithNumberLookup()
		{
		}

		public EnumWithNumberLookup(T value)
		{
			Id = value;
			Name = value.ToString();
		}

		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public T Id { get; set; }

		public string Name { get; set; }
	}
}