// <copyright file="NamingScheme.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System;
	using System.Text.RegularExpressions;

	public static class NamingScheme
	{
		public static Func<string, string> KebabCase =>
			(name) =>
			{
				Regex underscoreRegex = new Regex(@"(?<=[a-z0-9])([A-Z])(?![A-Z])");
				return underscoreRegex.Replace(name, @"-$0").ToLower();
			};

		public static Func<string, string> ScreamingSnakeCase =>
			(name) =>
			{
				Regex underscoreRegex = new Regex(@"(?<=[a-z0-9])([A-Z])(?![A-Z])");
				return underscoreRegex.Replace(name, @"_$0").ToUpper();
			};

		public static Func<string, string> SnakeCase =>
			(name) =>
			{
				Regex underscoreRegex = new Regex(@"(?<=[a-z0-9])([A-Z])(?![A-Z])");
				return underscoreRegex.Replace(name, @"_$0").ToLower();
			};
	}
}