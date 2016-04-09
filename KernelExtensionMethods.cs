using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grammophone.Kernels
{
	/// <summary>
	/// Extension methods for kernels.
	/// </summary>
	public static class KernelExtensionMethods
	{
		/// <summary>
		/// Extension method that returns a kernel as the sum of kernels.
		/// </summary>
		public static SumKernel<T> Sum<T>(this IEnumerable<Kernel<T>> kernels)
		{
			var sumKernel = new SumKernel<T>(kernels);

			return sumKernel;
		}
	}
}
