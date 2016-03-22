using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gramma.Vectors;

namespace Gramma.Kernels
{
	/// <summary>
	/// The simplest kernel, the dot product between two vectors.
	/// </summary>
	[Serializable]
	public class LinearKernel : Kernel<Vector>
	{
		#region Private fields

		private Vector componentAccumulator;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="dimensionality">
		/// The dimensionality of the vectors processed by the kernel.
		/// </param>
		public LinearKernel(int dimensionality)
		{
			if (dimensionality < 0)
				throw new ArgumentException("dimensionality must be non-negative.", "dimensionality");

			this.Dimensionality = dimensionality;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The dimensionality of the vectors processed by the kernel.
		/// </summary>
		public int Dimensionality { get; private set; }

		#endregion

		#region Kernel<Vector> implementation

		public override bool HasComponents
		{
			get { return this.componentAccumulator != null; }
		}

		public override double Compute(Vector arg1, Vector arg2)
		{
			if (arg1 == null) throw new ArgumentNullException("arg1");
			if (arg2 == null) throw new ArgumentNullException("arg2");

			if (arg1.Length != this.Dimensionality)
				throw new ArgumentException(
					"The supplied vector is not compatible with the kernel's dimensionality",
					"arg1");

			if (arg1 == arg2) return arg1.Norm2;

			return arg1 * arg2;
		}

		public override double ComputeSum(Vector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (arg.Length != this.Dimensionality)
				throw new ArgumentException(
					"The supplied vector is not compatible with the kernel's dimensionality",
					"arg");

			if (this.componentAccumulator == null) 
				return 0.0;
			else
				return arg * this.componentAccumulator;
		}

		public override void AddComponent(double weight, Vector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (arg.Length != this.Dimensionality)
				throw new ArgumentException(
					"The supplied vector is not compatible with the kernel's dimensionality",
					"arg");

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

		public override Kernel<Vector> ForkNew()
		{
			return new LinearKernel(this.Dimensionality);
		}

		#endregion

	}
}
