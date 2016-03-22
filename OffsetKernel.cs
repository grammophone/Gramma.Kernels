using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gramma.Kernels
{
	/// <summary>
	/// Define a kernel as the offset of the output of another kernel.
	/// </summary>
	/// <typeparam name="T">The type of the kernel arguments.</typeparam>
	/// <remarks>
	/// The kernel is expected to meet the Mercer's requirements.
	/// </remarks>
	[Serializable]
	public class OffsetKernel<T> : Kernel<T>
	{
		#region Private fields

		private double componentsTotalOffset;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="baseKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to subtract from the the output of the base kernel.</param>
		public OffsetKernel(Kernel<T> baseKernel, double offset)
		{
			if (baseKernel == null) throw new ArgumentNullException("baseKernel");

			this.componentsTotalOffset = 0.0;
			this.Offset = offset;
			this.BaseKernel = baseKernel;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The offset added to the output of the <see cref="BaseKernel"/>.
		/// </summary>
		public double Offset { get; private set; }

		/// <summary>
		/// The base kernel whose output is offset.
		/// </summary>
		public Kernel<T> BaseKernel { get; private set; }

		#endregion

		#region Kernel implementation

		public override bool HasComponents
		{
			get 
			{
				return this.BaseKernel.HasComponents;
			}
		}

		public override double Compute(T arg1, T arg2)
		{
			if (arg1 == null) throw new ArgumentNullException("arg1");
			if (arg2 == null) throw new ArgumentNullException("arg2");

			return this.BaseKernel.Compute(arg1, arg2) + this.Offset;
		}

		public override double ComputeSum(T arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			return this.BaseKernel.ComputeSum(arg) + this.componentsTotalOffset;
		}

		public override void AddComponent(double weight, T arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			this.componentsTotalOffset += this.Offset * weight;
			this.BaseKernel.AddComponent(weight, arg);
		}

		public override void ClearComponents()
		{
			this.componentsTotalOffset = 0.0;
			this.BaseKernel.ClearComponents();
		}

		public override Kernel<T> ForkNew()
		{
			return new OffsetKernel<T>(this.BaseKernel.ForkNew(), this.Offset);
		}

		#endregion

		#region Additional operators

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="offsetKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to add to the the output of the base kernel.</param>
		public static OffsetKernel<T> operator +(OffsetKernel<T> offsetKernel, double offset)
		{
			return new OffsetKernel<T>(offsetKernel.BaseKernel, offset + offsetKernel.Offset);
		}

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="offsetKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to add to the the output of the base kernel.</param>
		public static OffsetKernel<T> operator +(double offset, OffsetKernel<T> offsetKernel)
		{
			return new OffsetKernel<T>(offsetKernel.BaseKernel, offset + offsetKernel.Offset);
		}

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="offsetKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to subtract from the the output of the base kernel.</param>
		public static OffsetKernel<T> operator -(OffsetKernel<T> offsetKernel, double offset)
		{
			return new OffsetKernel<T>(offsetKernel.BaseKernel, offsetKernel.Offset - offset);
		}

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="offsetKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to subtract from the the output of the base kernel.</param>
		public static OffsetKernel<T> operator -(double offset, OffsetKernel<T> offsetKernel)
		{
			return new OffsetKernel<T>(offsetKernel.BaseKernel, offset - offsetKernel.Offset);
		}

		#endregion
	}
}
