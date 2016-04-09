using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grammophone.Kernels
{
	/// <summary>
	/// Kernel that consists of a scaled kernel.
	/// </summary>
	/// <typeparam name="T">The type of the kernel arguments.</typeparam>
	[Serializable]
	public class ScaledKernel<T> : Kernel<T>
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="factor">The scaling factor.</param>
		/// <param name="originalKernel">The kernel to be scaled.</param>
		public ScaledKernel(double factor, Kernel<T> originalKernel)
		{
			if (originalKernel == null) throw new ArgumentNullException("originalKernel");

			this.Factor = factor;
			this.OriginalKernel = originalKernel;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The scaling factor.
		/// </summary>
		public double Factor { get; private set; }

		/// <summary>
		/// The kernel to be scaled.
		/// </summary>
		public Kernel<T> OriginalKernel { get; private set; }

		#endregion

		#region Kernel<T> implementation

		public override bool HasComponents
		{
			get { return this.OriginalKernel.HasComponents; }
		}

		public override double Compute(T arg1, T arg2)
		{
			return this.Factor * this.OriginalKernel.Compute(arg1, arg2);
		}

		public override double ComputeSum(T arg)
		{
			return this.Factor * this.OriginalKernel.ComputeSum(arg);
		}

		public override void AddComponent(double weight, T arg)
		{
			this.OriginalKernel.AddComponent(weight, arg);
		}

		public override void ClearComponents()
		{
			this.OriginalKernel.ClearComponents();
		}

		public override Kernel<T> ForkNew()
		{
			return new ScaledKernel<T>(this.Factor, this.OriginalKernel.ForkNew());
		}

		#endregion

		#region Supplementary operators (the basic ones are defined in Kernel<T>)

		/// <summary>
		/// Define a kernel as the scaling of a kernel.
		/// </summary>
		public static ScaledKernel<T> operator *(ScaledKernel<T> k, double factor)
		{
			if (k == null) throw new ArgumentNullException("k");

			return new ScaledKernel<T>(factor * k.Factor, k.OriginalKernel);
		}

		/// <summary>
		/// Define a kernel as the scaling of a kernel.
		/// </summary>
		public static ScaledKernel<T> operator *(double factor, ScaledKernel<T> k)
		{
			if (k == null) throw new ArgumentNullException("k");

			return new ScaledKernel<T>(factor * k.Factor, k.OriginalKernel);
		}

		#endregion
	}
}
