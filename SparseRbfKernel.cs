using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gramma.Vectors;

namespace Gramma.Kernels
{
	[Serializable]
	public class SparseRbfKernel : Kernel<SparseVector>
	{
		#region Auxilliary types

		[Serializable]
		private struct Component
		{
			public double Weight;
			public SparseVector Vector;
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
		public SparseRbfKernel(double σ2)
		{
			if (σ2 <= 0.0) throw new ArgumentException("The variance must be positive.", "σ2");

			this.σ2 = σ2;

			this.components = new List<Component>();
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The variance of the Gaussian.
		/// </summary>
		public double σ2 { get; private set; }

		#endregion

		#region Kernel<SparseVector> implementation

		public override bool HasComponents
		{
			get { return this.components.Count > 0; }
		}

		public override double Compute(SparseVector arg1, SparseVector arg2)
		{
			if (arg1 == null) throw new ArgumentNullException("arg1");
			if (arg2 == null) throw new ArgumentNullException("arg2");

			return Math.Exp(-(arg1.Norm2 + arg2.Norm2 - 2.0 * (arg1 * arg2)) / (2.0 * σ2));
		}

		public override double ComputeSum(SparseVector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			double innerArgDotArg = arg.Norm2;

			return this.components
				.Sum(c => c.Weight * Math.Exp(-(innerArgDotArg + c.Vector.Norm2 - 2.0 * (arg * c.Vector)) / (2.0 * σ2)));
		}

		public override void AddComponent(double weight, SparseVector arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			this.components.Add(new Component() { Weight = weight, Vector = arg });
		}

		public override void ClearComponents()
		{
			this.components.Clear();
		}

		public override Kernel<SparseVector> ForkNew()
		{
			return new SparseRbfKernel(this.σ2);
		}

		#endregion
	}
}
