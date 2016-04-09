using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grammophone.Vectors;

namespace Grammophone.Kernels
{
	/// <summary>
	/// Gaussian RBF kernel for vectors.
	/// </summary>
	[Serializable]
	public class RbfKernel : Kernel<Vector>
	{
		#region Auxilliary types

		[Serializable]
		private struct Component
		{
			public double Weight;
			public Vector Vector;
		}

		#endregion

		#region Private members

		private IList<Component> components;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="σ2">The variance of the Gaussian.</param>
		/// <param name="dimensionality">
		/// The dimensionality of the vectors processed by the kernel.
		/// </param>
		public RbfKernel(double σ2, int dimensionality)
		{
			if (σ2 <= 0.0) throw new ArgumentException("The variance must be positive.", "σ2");

			if (dimensionality < 0)
				throw new ArgumentException("dimensionality must be non-negative.", "dimensionality");

			this.σ2 = σ2;
			this.Dimensionality = dimensionality;

			this.components = new List<Component>();
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The variance of the Gaussian.
		/// </summary>
		public double σ2 { get; private set; }

		/// <summary>
		/// The dimensionality of the vectors processed by the kernel.
		/// </summary>
		public int Dimensionality { get; private set; }

		#endregion

		#region Kernel<Vector> implementation

		public override bool HasComponents
		{
			get { return this.components.Count > 0; }
		}

		public override double Compute(Vector arg1, Vector arg2)
		{
			if (arg1 == null) throw new ArgumentNullException("arg1");
			if (arg2 == null) throw new ArgumentNullException("arg2");

			if (arg1.Length != this.Dimensionality)
				throw new ArgumentException(
					"The supplied vector is not compatible with the kernel's dimensionality",
					"arg1");

			return Math.Exp(-(arg1.Norm2 + arg2.Norm2 - 2 * arg1 * arg2) / (2 * σ2));
		}

		public override double ComputeSum(Vector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (arg.Length != this.Dimensionality)
				throw new ArgumentException(
					"The supplied vector is not compatible with the kernel's dimensionality",
					"arg");

			double innerArgDotArg = arg.Norm2;

			return this.components
				.Sum(c => c.Weight * Math.Exp(-(innerArgDotArg + c.Vector.Norm2 - 2 * arg * c.Vector) / (2 * σ2)));
		}

		public override void AddComponent(double weight, Vector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (arg.Length != this.Dimensionality)
				throw new ArgumentException(
					"The supplied vector is not compatible with the kernel's dimensionality",
					"arg");

			this.components.Add(new Component() { Weight = weight, Vector = arg });
		}

		public override void ClearComponents()
		{
			this.components.Clear();
		}

		public override Kernel<Vector> ForkNew()
		{
			return new RbfKernel(this.σ2, this.Dimensionality);
		}

		#endregion
	}
}
