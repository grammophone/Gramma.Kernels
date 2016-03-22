using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gramma.Kernels
{
	/// <summary>
	/// Kernel operating on items of type <typeparamref name="T"/> by
	/// mapping them to type <typeparamref name="S"/> and delegating to
	/// another kernel for items of type <typeparamref name="S"/>.
	/// </summary>
	/// <typeparam name="T">The source type being mapped.</typeparam>
	/// <typeparam name="S">The destination type of the mapping.</typeparam>
	/// <remarks>
	/// This kernel allows, in conjunction with other helper kernels such as <see cref="SumKernel{T}"/>
	/// or <see cref="ScaledKernel{T}"/>, to make kernels for complex 
	/// data structures of a type <typeparamref name="T"/>
	/// consisting of many fields or data types. For example, we could have a kernel
	/// for a complex structure, defined as a sum, possibly weighted, of mapping kernels 
	/// acting on the different fields of diverse data types of the data structure.
	/// </remarks>
	[Serializable]
	public class MappingKernel<T, S> : Kernel<T>
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="mappingFunction">
		/// The function mapping <typeparamref name="T" /> instances to <typeparamref name="S" /> ones.
		/// </param>
		/// <param name="mappedKernel">
		/// The kernel acting on <typeparamref name="S" /> instances.
		/// </param>
		public MappingKernel(Func<T, S> mappingFunction, Kernel<S> mappedKernel)
		{
			if (mappingFunction == null) throw new ArgumentNullException("mappingFunction");
			if (mappedKernel == null) throw new ArgumentNullException("mappedKernel");

			this.MappingFunction = mappingFunction;
			this.MappedKernel = mappedKernel;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The function mapping <typeparamref name="T" /> instances to <typeparamref name="S" /> ones.
		/// </summary>
		public Func<T, S> MappingFunction { get; private set; }

		/// <summary>
		/// The kernel acting on <typeparamref name="S" /> instances.
		/// </summary>
		public Kernel<S> MappedKernel { get; private set; }

		#endregion

		#region Kernel<T> implementation

		public override bool HasComponents
		{
			get
			{
				return this.MappedKernel.HasComponents;
			}
		}

		public override double Compute(T arg1, T arg2)
		{
			return this.MappedKernel.Compute(this.MappingFunction(arg1), this.MappingFunction(arg2));
		}

		public override double ComputeSum(T arg)
		{
			return this.MappedKernel.ComputeSum(this.MappingFunction(arg));
		}

		public override void AddComponent(double weight, T arg)
		{
			this.MappedKernel.AddComponent(weight, this.MappingFunction(arg));
		}

		public override void ClearComponents()
		{
			this.MappedKernel.ClearComponents();
		}

		public override Kernel<T> ForkNew()
		{
			return new MappingKernel<T, S>(this.MappingFunction, this.MappedKernel.ForkNew());
		}

		#endregion
	}
}
