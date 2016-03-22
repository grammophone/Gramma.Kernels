using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gramma.Kernels
{
	/// <summary>
	/// Contract for kernels.
	/// </summary>
	/// <typeparam name="T">The type of the kernel arguments.</typeparam>
	/// <remarks>
	/// The kernel is expected to meet the Mercer's requirements.
	/// </remarks>
	[Serializable]
	public abstract class Kernel<T>
	{
		#region Public properties

		/// <summary>
		/// Returns true if the kernel contains at least one component
		/// participating in the summation of kernels
		/// performed by method <see cref="ComputeSum"/>.
		/// This is the summation specified by the Representer Theorem for
		/// kernel methods.
		/// </summary>
		/// <remarks>
		/// If this property isn't true,
		/// invokation of <see cref="ComputeSum"/> should return zero.
		/// </remarks>
		public abstract bool HasComponents { get; }

		#endregion

		#region Public methods

		/// <summary>
		/// Compute the kernel between two items of type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="arg1">The first argument.</param>
		/// <param name="arg2">The second argument.</param>
		/// <returns>Returns the kernel value.</returns>
		public abstract double Compute(T arg1, T arg2);

		/// <summary>
		/// Compute the weighted sum of the kernels between the supplied
		/// argument <paramref name="arg"/> and the items added via <see cref="AddComponent"/>
		/// method. 
		/// This is the summation specified by the Representer Theorem for
		/// kernel methods.
		/// </summary>
		/// <param name="arg">
		/// The first argument of each kernel in the summation.
		/// The second one is supplied to method <see cref="AddComponent"/> per each kernel.
		/// </param>
		/// <returns>
		/// Returns the weighted sum of kernels of the supplied <paramref name="arg"/>
		/// and the components specified by <see cref="AddComponent"/>.
		/// </returns>
		/// <remarks>
		/// This method can take advantage of preprocessing the items added via
		/// method <see cref="AddComponent"/> in order to achieve efficiency, instead of
		/// computing individual kernels using method <see cref="Compute"/> 
		/// and summing one by one.
		/// </remarks>
		public abstract double ComputeSum(T arg);

		/// <summary>
		/// Add a component to participate in the summation of kernels
		/// performed by method <see cref="ComputeSum"/>.
		/// This is the summation specified by the Representer Theorem for
		/// kernel methods.
		/// </summary>
		/// <param name="weight">The weight applied to the kernel result in the summation.</param>
		/// <param name="arg">
		/// The second argument of the kernel. The first argument is the one supplied to
		/// method <see cref="ComputeSum"/>.
		/// </param>
		public abstract void AddComponent(double weight, T arg);

		/// <summary>
		/// Clear all the components participating in the summation of kernels
		/// performed by method <see cref="ComputeSum"/>.
		/// This is the summation specified by the Representer Theorem for
		/// kernel methods.
		/// </summary>
		/// <remarks>
		/// Invokation of <see cref="ComputeSum"/> should return zero after this call.
		/// </remarks>
		public abstract void ClearComponents();

		/// <summary>
		/// Create a clone of this kernel, having the exact settings of the prototype 
		/// but empty, without any components.
		/// </summary>
		/// <returns>A duplicate without any components.</returns>
		public abstract Kernel<T> ForkNew();

		#endregion

		#region Operators
		
		/// <summary>
		/// Define a kernel as the sum of kernels.
		/// </summary>
		public static SumKernel<T> operator +(Kernel<T> k1, Kernel<T> k2)
		{
			if (k1 == null) throw new ArgumentNullException("k1");
			if (k2 == null) throw new ArgumentNullException("k2");

			var sumKernel = new SumKernel<T>();
			
			sumKernel.KernelSummands.Add(k1);
			sumKernel.KernelSummands.Add(k2);

			return sumKernel;
		}

		/// <summary>
		/// Define a kernel as the sum of kernels.
		/// </summary>
		public static SumKernel<T> operator +(SumKernel<T> k1, Kernel<T> k2)
		{
			if (k1 == null) throw new ArgumentNullException("k1");
			if (k2 == null) throw new ArgumentNullException("k2");

			k1.KernelSummands.Add(k2);

			return k1;
		}

		/// <summary>
		/// Define a kernel as the sum of kernels.
		/// </summary>
		public static SumKernel<T> operator +(Kernel<T> k1, SumKernel<T> k2)
		{
			if (k1 == null) throw new ArgumentNullException("k1");
			if (k2 == null) throw new ArgumentNullException("k2");

			k2.KernelSummands.Add(k1);

			return k2;
		}

		/// <summary>
		/// Define a kernel as the scaling of a kernel.
		/// </summary>
		public static ScaledKernel<T> operator *(double factor, Kernel<T> k)
		{
			if (k == null) throw new ArgumentNullException("k");

			return new ScaledKernel<T>(factor, k);
		}

		/// <summary>
		/// Define a kernel as the scaling of a kernel.
		/// </summary>
		public static ScaledKernel<T> operator *(Kernel<T> k, double factor)
		{
			if (k == null) throw new ArgumentNullException("k");

			return new ScaledKernel<T>(factor, k);
		}

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="baseKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to add to the the output of the base kernel.</param>
		public static OffsetKernel<T> operator +(Kernel<T> baseKernel, double offset)
		{
			return new OffsetKernel<T>(baseKernel, offset);
		}

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="baseKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to add to the the output of the base kernel.</param>
		public static OffsetKernel<T> operator +(double offset, Kernel<T> baseKernel)
		{
			return new OffsetKernel<T>(baseKernel, offset);
		}

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="baseKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to subtract from the the output of the base kernel.</param>
		public static OffsetKernel<T> operator -(Kernel<T> baseKernel, double offset)
		{
			return new OffsetKernel<T>(baseKernel, -offset);
		}

		/// <summary>
		/// Define a kernel as the offset of the output of another kernel.
		/// </summary>
		/// <param name="baseKernel">The base kernel whose output is offset.</param>
		/// <param name="offset">The offset to subtract from the the output of the base kernel.</param>
		public static OffsetKernel<T> operator -(double offset, Kernel<T> baseKernel)
		{
			return new OffsetKernel<T>(baseKernel, -offset);
		}

		#endregion
	}
}
