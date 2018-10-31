# Grammophone.Kernels
This .NET library is used to provide kernels for kernel methods found in machine learning.

The abstract base class `Kernel<T>` expresses the definition of a kernel, where x<sub>1</sub> and x<sub>2</sub> are of type `T` and φ is some function, possibly unknown, which maps items of type `T` to a vector of finite or infinite dimensions:

![Kernel definition](http://s10.postimg.org/5534sl18p/Kernel_definition.png)

In order for a kernel implementation to be valid, meaning that there exists some function φ to express the kernel in the above dot product form, it has to be positive definite, i.e. to fulfill the Mercer's condition. The included implementations of `Kernel<T>` and their supported combinations all comply to this condition. For any new derivation of `Kernel<T>`, it is the developer's duty to fulfill the condition.

The above expression is offered by `Kernel<T>` implementations via the `Compute` method. But implementations also offer method `ComputeSum` which calculates weighted sums of kernel evaluations:

![representer form](http://s7.postimg.org/wu8zhf81z/Representer_form.png)

The above 'representer form' is what typically all kernel methods arrive to after training. In theory, it could be computed calling repeatedly the `Compute` method then scaling and summing the results, but for many kernels there can be lots of savings computing the 'representer form' directly. The c<sub>i</sub> and x<sub>i</sub> components of the formula are registered in the kernel using the `AddComponent` method. These components are the training state of the kernel method that uses the kernel. In order to save and load this state, use standard .NET serialization on the kernel.

The hierarchy of the included kernels follows:

![Kernels hierarchy](http://s24.postimg.org/bgiut5pmt/Kernels_hierarchy.png)

The included kernels are:
* `LinearKernel` and `SparseLinearKernel`: Linear kernels for dense and sparse vectors respectively.
* `RbfKernel` and `SparseRbfKernel`: Gaussian kernels for dense and sparse vectors respectively.
* `StringKernel<C>`: Kernel for generic strings where 'characters' can be of any type `C`. It is an implementation of the 'all-substrings kernel' as introduced by [Vishwanathan and Smola (2004)](http://www.stat.purdue.edu/~vishy/papers/VisSmo04.pdf). Note that in order to preserve its linear performance O(n) where n is the length of strings passed in a method, the `HashCode` and `Equals` implementations of type `C` must have O(1) performance.
* `ScaledKernel<T>`: Produces a kernel by scaling it with a value which must be positive to preserve the positive-definiteness.
* `OffsetKernel<T>`: Produces a kernel by adding to it with a value which must be positive to preserve the positive-definiteness.
* `SumKernel<T>`: Produces a kernel from a sum of other `Kernel<T>` implementations.
* `GaussianKernel<T>`: Takes as input an arbitrary `Kernel<T>` and 'gaussianizes' it. It is a generalization of `RbfKernel` and `SparseRbfKernel`, which can be seen as `GaussianKernel<Vector>` and `GaussianKernel<SparseVecetor>` respectively.
* `MappingKernel<T, S>`: Produces a `Kernel<T>` implementation by mapping instances of `T` to instances of `S` and delegating them to a `Kernel<S>` implementation.

This project depends on the following projects, which must reside in sibling directories:
* [Grammophone.Vector](https://github.com/grammophone/Grammophone.Vectors)
* [Grammophone.Indexing](https://github.com/grammophone/Grammophone.Indexing)
