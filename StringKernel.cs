using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gramma.Indexing;
using Gramma.Indexing.KernelWeightFunctions;

using System.Runtime.Serialization;

namespace Gramma.Kernels
{
	/// <summary>
	/// String kernel. Takes into account all substrings of the string arguments.
	/// Follows Vishwanathan and Smola (2004).
	/// All strings must terminate with sentinel characters.
	/// </summary>
	/// <typeparam name="C">The type of the character of the strings.</typeparam>
	[Serializable]
	public class StringKernel<C> : Kernel<C[]>
	{
		#region Private fields

		private WeightFunction weightFunction;

		private KernelSuffixTree<C> suffixTree;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="λ">
		/// Implies a weight of λ^|s| for string lengths |s|.
		/// </param>
		public StringKernel(double λ)
		{
			if (Math.Abs(λ - 1.0) <= 1E-6)
				this.weightFunction = new SumWeightFunction();
			else
				this.weightFunction = new ExpSumWeightFunction(λ);

			this.suffixTree = new KernelSuffixTree<C>(this.weightFunction);
		}

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="weightFunction">
		/// A cumulative weight function. 
		/// Must have O(1) time performance in order to preserve O(n) kernel performance.
		/// </param>
		public StringKernel(WeightFunction weightFunction)
		{
			if (weightFunction == null) throw new ArgumentNullException("weightFunction");

			this.weightFunction = weightFunction;

			this.suffixTree = new KernelSuffixTree<C>(this.weightFunction);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The cumulative weight function used in the string kernel.
		/// </summary>
		public WeightFunction WeightFunction
		{
			get
			{
				return weightFunction;
			}
		}

		#endregion

		#region Kernel<C[]> implementation

		public override bool HasComponents
		{
			get { return this.suffixTree.Root.Children.Any(); }
		}

		public override double Compute(C[] arg1, C[] arg2)
		{
			if (arg1 == null) throw new ArgumentNullException("arg1");
			if (arg2 == null) throw new ArgumentNullException("arg2");

			KernelSuffixTree<C> suffixTree = new KernelSuffixTree<C>(this.weightFunction);

			suffixTree.AddWord(arg1, 1.0);

			return suffixTree.ComputeKernel(arg2);
		}

		public override double ComputeSum(C[] arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (this.suffixTree == null) return 0.0;

			return this.suffixTree.ComputeKernel(arg);
		}

		public override void AddComponent(double weight, C[] arg)
		{
			if (arg == null) throw new ArgumentNullException("arg");

			if (this.suffixTree == null) 
				this.suffixTree = new KernelSuffixTree<C>(this.weightFunction);

			this.suffixTree.AddWord(arg, weight);
		}

		public override Kernel<C[]> ForkNew()
		{
			return new StringKernel<C>(this.weightFunction);
		}

		public override void ClearComponents()
		{
			this.suffixTree.Clear();
		}

		#endregion

	}
}
