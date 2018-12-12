// <copyright file="SpecialOccasion.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions.SQLiteDemo.Entities
{
	using System.ComponentModel;

	public enum SpecialOccasion
	{
		[Description("Your birth anniversary")]
		Birthday = 1,

		[Description("Jesus' birth anniversary")]
		Christmas,

		[Description("Jesus' resurrection anniversary")]
		Easter,

		[Description("Florist holiday")]
		Valentines,

		[Description("Marriage anniversary")]
		WeddingDay,
	}
}