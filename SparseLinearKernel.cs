using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gramma.Vectors;

namespace Gramma.Kernels
{
	/// <summary>
	/// The simplest kernel, the dot product between two sparse vectors.
	/// </summary>
	[Serializable]
	public class SparseLinearKernel : Kernel<SparseVector>
	{
		#region Private fields

		private SparseVector componentAccumulator;

		#endregion

		#region Kernel<T> implementation

		public override bool HasComponents
		{
			get
			{
				return this.componentAccumulator != null;
			}
		}

		public override double Compute(SparseVector arg1, SparseVector arg2)
		{
			if (arg1 == arg2) return arg1.Norm2;

			return arg1 * arg2;
		}

		public override double ComputeSum(SparseVector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (this.componentAccumulator == null)
				return 0.0;
			else
				return arg * this.componentAccumulator;
		}

		public override void AddComponent(double weight, SparseVector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (this.componentAccumulator == null)
			{
				this.componentAccumulator = weight * arg;
			}
			else
			{
				this.componentAccumulator += weight * arg;
			}
		}

		public override void ClearComponents()
		{
			this.componentAccumulator = null;
		}

		public override Kernel<SparseVector> ForkNew()
		{
			return new SparseLinearKernel();
		}

		#endregion
	}
}
