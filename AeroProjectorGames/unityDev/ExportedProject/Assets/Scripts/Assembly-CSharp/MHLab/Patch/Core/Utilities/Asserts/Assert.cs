using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MHLab.Patch.Core.Utilities.Asserts
{
	public static class Assert
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysFail()
		{
			throw new AssertFailedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysFail(string message)
		{
			throw new AssertFailedException(message);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysFail(string message, params object[] parameters)
		{
			throw new AssertFailedException(string.Format(message, parameters));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysCheck(bool condition)
		{
			if (!condition)
			{
				AlwaysFail();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysCheck(bool condition, string message)
		{
			if (!condition)
			{
				AlwaysFail(message);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysCheck(bool condition, string message, params object[] parameters)
		{
			if (!condition)
			{
				AlwaysFail(message, parameters);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysNotNull(object obj)
		{
			if (obj == null)
			{
				AlwaysFail();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerHidden]
		public static void AlwaysNotNull(object obj, string message)
		{
			if (obj == null)
			{
				AlwaysFail(message);
			}
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Fail()
		{
			throw new AssertFailedException();
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Fail(string message)
		{
			throw new AssertFailedException(message);
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Fail(string message, params object[] parameters)
		{
			throw new AssertFailedException(string.Format(message, parameters));
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Check(bool condition)
		{
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Check(bool condition, string message)
		{
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Check(bool condition, string message, params object[] parameters)
		{
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void NotNull(object obj)
		{
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void NotNull(object obj, string message)
		{
		}
	}
}
