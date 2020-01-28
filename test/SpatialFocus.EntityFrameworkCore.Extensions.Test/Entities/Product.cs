// <copyright file="Product.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions.Test.Entities
{
	using System;

	public class Product
	{
		public Product()
		{
			Created = DateTime.Now;
		}

		public DateTime Created { get; set; }

		public SpecialOccasion? IdealForSpecialOccasion { get; set; }

		public string Name { get; set; }

		public double Price { get; set; }

		public ProductCategory ProductCategory { get; set; }

		public int ProductId { get; set; }

		public DateTime ReleaseDate { get; set; }
	}
}