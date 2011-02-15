
namespace i4o.Tests
{
	using System;
	using System.Collections;
	using NUnit.Framework;

	public delegate object MethodThatThrows();

	public static partial class FluentSpecificationExtensions
	{
		public static void ShouldBeFalse(this bool condition)
		{
			Assert.IsFalse(condition);
		}

		public static void ShouldBeTrue(this bool condition)
		{
			Assert.IsTrue(condition);
		}

		public static T ShouldEqual<T>(this T actual, T expected)
		{
			return ShouldEqual(actual, expected, string.Empty);
		}

		public static T ShouldEqual<T>(this T actual, T expected, string message)
		{
			Assert.AreEqual(expected, actual, message);
			return actual;
		}

		public static T ShouldNotEqual<T>(this T actual, T expected)
		{
			Assert.AreNotEqual(expected, actual);
			return actual;
		}

		public static T ShouldBeNull<T>(this T anObject)
		{
			Assert.IsNull(anObject);
			return anObject;
		}

		public static T ShouldNotBeNull<T>(this T anObject)
		{
			Assert.IsNotNull(anObject);
			return anObject;
		}

		public static T ShouldBeTheSameAs<T>(this T actual, object expected)
		{
			Assert.AreSame(expected, actual);
			return actual;
		}

		public static T ShouldNotBeTheSameAs<T>(this T actual, object expected)
		{
			Assert.AreNotSame(expected, actual);
			return actual;
		}

		public static T ShouldBeOfType<T>(this T actual, Type expected)
		{
			Assert.IsInstanceOfType(expected, actual);
			return actual;
		}

		public static T ShouldNotBeOfType<T>(this T actual, Type expected)
		{
			Assert.IsNotInstanceOfType(expected, actual);
			return actual;
		}

		public static IList ShouldContain(this IList actual, object expected)
		{
			Assert.Contains(expected, actual);
			return actual;
		}

		public static IList ShouldNotContain(this IList collection, object actual)
		{
			CollectionAssert.DoesNotContain(collection, actual);
			return collection;
		}

		public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
		{
			Assert.Greater(arg1, arg2);
			return arg2;
		}

		public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
		{
			Assert.Less(arg1, arg2);
			return arg2;
		}

		public static ICollection ShouldBeEmpty(this ICollection collection)
		{
			Assert.IsEmpty(collection);
			return collection;
		}

		public static string ShouldBeEmpty(this string aString)
		{
			Assert.IsEmpty(aString);
			return aString;
		}

		public static ICollection ShouldNotBeEmpty(this ICollection collection)
		{
			Assert.IsNotEmpty(collection);
			return collection;
		}

		public static ICollection ShouldBeEquivalentTo(this ICollection actual, ICollection expected)
		{
			Assert.That(expected, Is.EquivalentTo(actual));
			return actual;
		}

		public static string ShouldNotBeEmpty(this string aString)
		{
			Assert.IsNotEmpty(aString);
			return aString;
		}

		public static string ShouldContain(this string actual, string expected)
		{
			StringAssert.Contains(expected, actual);
			return actual;
		}

		public static string ShouldNotContain(this string actual, string expected)
		{
			try
			{
				StringAssert.Contains(expected, actual);
			}
			catch (AssertionException)
			{
				return actual;
			}

			throw new AssertionException(String.Format("\"{0}\" should not contain \"{1}\".", actual, expected));
		}

		public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
		{
			StringAssert.AreEqualIgnoringCase(expected, actual);
			return actual;
		}

		public static string ShouldStartWith(this string actual, string expected)
		{
			StringAssert.StartsWith(expected, actual);
			return actual;
		}

		public static string ShouldEndWith(this string actual, string expected)
		{
			StringAssert.EndsWith(expected, actual);
			return actual;
		}

		public static object ShouldBeSurroundedWith(this string actual, string expectedStartDelimiter, string expectedEndDelimiter)
		{
			StringAssert.StartsWith(expectedStartDelimiter, actual);
			StringAssert.EndsWith(expectedEndDelimiter, actual);
			return actual;
		}

		public static object ShouldBeSurroundedWith(this string actual, string expectedDelimiter)
		{
			StringAssert.StartsWith(expectedDelimiter, actual);
			StringAssert.EndsWith(expectedDelimiter, actual);
			return actual;
		}

		public static Exception ShouldContainErrorMessage(this Exception exception, string expected)
		{
			StringAssert.Contains(expected, exception.Message);
			return exception;
		}

		public static Exception ShouldBeThrownBy(this Type exceptionType, Action method)
		{
			Exception exception = method.GetException();

			Assert.IsNotNull(exception, string.Format("Exception of type[{0}] was not thrown.", exceptionType.FullName));
			Assert.AreEqual(exceptionType, exception.GetType());

			return exception;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static Exception GetException(this Action method)
		{
			Exception exception = null;

			try
			{
				method();
			}
			catch (Exception e)
			{
				exception = e;
			}

			return exception;
		}
	}
}