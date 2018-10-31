using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grammophone.Kernels
{
	/// <summary>
	/// Makes a generalized gaussian kernel out of a supplied inner <see cref="Kernel{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of the kernel arguments, which also matches the type of the inner kernel arguments.</typeparam>
	/// <remarks>
	/// The common gaussian kernels for vectors are a subcase of this kernel, 
	/// expressed as <see cref="GaussianKernel{Vector}"/> and <see cref="GaussianKernel{SparseVector}"/>
	/// having specified as <see cref="InnerKernel"/> the <see cref="LinearKernel"/> 
	/// and <see cref="SparseLinearKernel"/> correspondingly.
	/// </remarks>
	[Serializable]
	public class GaussianKernel<T> : Kernel<T>
	{
		#region Auxilliary types

		[Serializable]
		private class Component
		{
			public Component(double weight, T item, GaussianKernel<T> gaussianKernel)
			{
				Kernel<T> innerKernelWithItemComponent = gaussianKernel.InnerKernel.ForkNew();
				innerKernelWithItemComponent.AddComponent(1.0, item);

				this.Weight = weight;
				this.Item = item;
				this.InnerKernelWithItemComponent = innerKernelWithItemComponent;
				this.InnerItemDotItem = innerKernelWithItemComponent.ComputeSum(item);
			}

			public readonly double Weight;
			public readonly T Item;
			public readonly Kernel<T> InnerKernelWithItemComponent;
			public readonly double InnerItemDotItem;
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
		/// <param name="innerKernel">The kernel being gauss-ified.</param>
		public GaussianKernel(double σ2, Kernel<T> innerKernel)
		{
			if (σ2 <= 0.0) throw new ArgumentException("The variance must be positive.", "σ2");
			if (innerKernel == null) throw new ArgumentNullException("innerKernel");

			this.σ2 = σ2;
			this.InnerKernel = innerKernel;

			this.components = new List<Component>();
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The variance of the Gaussian.
		/// </summary>
		public double σ2 { get; private set; }

		/// <summary>
		/// The kernel being gauss-ified.
		/// </summary>
		public Kernel<T> InnerKernel { get; private set; }

		#endregion

		#region Kernel<T> implementation

		public override bool HasComponents
		{
			get 
			{
				return this.components.Count > 0;
			}
		}

		public override double Compute(T arg1, T arg2)
		{
			if (arg1 == null) throw new ArgumentNullException("arg1");
			if (arg2 == null) throw new ArgumentNullException("arg2");

			return Math.Exp(-(InnerKernel.Compute(arg1, arg1) + InnerKernel.Compute(arg2, arg2) - 2 * InnerKernel.Compute(arg1, arg2)) / (2 * σ2));
		}

		public override double ComputeSum(T arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			double innerArgDotArg = InnerKernel.Compute(arg, arg);

			return this.components
				.Sum(c => 
					c.Weight * Math.Exp(
					-(innerArgDotArg + c.InnerItemDotItem - 2 * InnerKernel.ComputeSum(arg)) / (2 * σ2)));
		}

		public override void AddComponent(double weight, T arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			this.components.Add(new Component(weight, arg, this));
		}

		public override void ClearComponents()
		{
			this.components.Clear();
		}

		public override Kernel<T> ForkNew()
		{
			return new GaussianKernel<T>(σ2, InnerKernel);
		}

		#endregion
	}
}
