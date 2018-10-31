// <copyright file="NamingSource.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;

	public static class NamingSource
	{
		public static Func<IMutableEntityType, string> ClrType => entity => entity.ClrType.Name;

		public static Func<IMutableEntityType, string> DbSet => entity => entity.Relational().TableName;
	}
}