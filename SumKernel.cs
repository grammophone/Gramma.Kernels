using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grammophone.Kernels
{
	/// <summary>
	/// Kernel that consists of the sum of kernels.
	/// </summary>
	/// <typeparam name="T">The type of the kernel arguments.</typeparam>
	[Serializable]
	public class SumKernel<T> : Kernel<T>
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		public SumKernel()
		{
			this.KernelSummands = new List<Kernel<T>>();
		}

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="kernels">The kernels forming the sum.</param>
		public SumKernel(IEnumerable<Kernel<T>> kernels)
		{
			if (kernels == null) throw new ArgumentNullException("kernels");

			this.KernelSummands = new List<Kernel<T>>(kernels);
		}

		#endregion

		#region Public properties

		public ICollection<Kernel<T>> KernelSummands { get; private set; }

		#endregion

		#region Kernel<T> implementation

		public override bool HasComponents
		{
			get { return this.KernelSummands.Any(k => k.HasComponents); }
		}

		public override double Compute(T arg1, T arg2)
		{
			if (arg1 == null) throw new ArgumentNullException("arg1");
			if (arg2 == null) throw new ArgumentNullException("arg2");

			return this.KernelSummands
				.Sum(s => s.Compute(arg1, arg2));
		}

		public override double ComputeSum(T arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			return this.KernelSummands
				.Sum(s => s.ComputeSum(arg));
		}

		public override void AddComponent(double weight, T arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			foreach (var kernel in this.KernelSummands)
			{
				kernel.AddComponent(weight, arg);
			}
		}

		public override void ClearComponents()
		{
			foreach (var kernel in this.KernelSummands)
			{
				kernel.ClearComponents();
			}
		}

		public override Kernel<T> ForkNew()
		{
			return new SumKernel<T>(this.KernelSummands.Select(k => k.ForkNew()));
		}

		#endregion
	}
}
