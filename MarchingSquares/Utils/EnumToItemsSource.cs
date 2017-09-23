﻿using System;
using System.Linq;
using System.Windows.Markup;

namespace MarchingSquares.Utils
{
	public class EnumToItemsSource : MarkupExtension
	{
		private readonly Type _type;

		public EnumToItemsSource(Type type)
		{
			_type = type;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues(_type)
				.Cast<object>()
				.Select(e => new { Value = (int)e, DisplayName = e.ToString() });
		}

	}
}
