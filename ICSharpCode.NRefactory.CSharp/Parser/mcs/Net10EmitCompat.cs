//
// Net10EmitCompat.cs
//
// net10 migration: NRefactory embeds the Mono C# compiler (mcs) but uses it ONLY as a parser /
// type-system front end -- it never emits an assembly. .NET Core/.NET 10 removed the Reflection.Emit
// *persisted assembly* APIs (AssemblyBuilder.Save, AddResourceFile, DefineUnmanagedResource,
// SetEntryPoint(PEFileKinds), *Builder.AddDeclarativeSecurity, *Builder.GetToken, PEFileKinds,
// AssemblyBuilderAccess.Save/RunAndSave, AppDomain.DefineDynamicAssembly).
//
// These shims keep the mcs back end compiling. Every member here is on a code path NRefactory does
// not execute; the ones that cannot be meaningfully emulated throw NotSupportedException.
//

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ICSharpCode.NRefactory.MonoCSharp
{
	// Removed from .NET; mcs only uses it to describe the output kind.
	enum PEFileKinds
	{
		Dll = 1,
		ConsoleApplication = 2,
		WindowApplication = 3
	}

	static class Net10EmitCompat
	{
		const string Msg = "Reflection.Emit assembly persistence is not supported on .NET 10; " +
			"NRefactory uses the Mono C# compiler as a parser only.";

		// --- AssemblyBuilder ---
		public static ModuleBuilder DefineDynamicModule (this AssemblyBuilder builder, string name, string fileName, bool emitSymbolInfo)
			=> builder.DefineDynamicModule (name);

		public static void DefineUnmanagedResource (this AssemblyBuilder builder, string resourceFileName)
			=> throw new NotSupportedException (Msg);

		public static void DefineVersionInfoResource (this AssemblyBuilder builder)
			=> throw new NotSupportedException (Msg);

		public static void DefineVersionInfoResource (this AssemblyBuilder builder, string product, string productVersion, string company, string copyright, string trademark)
			=> throw new NotSupportedException (Msg);

		public static void AddResourceFile (this AssemblyBuilder builder, string name, string fileName)
			=> throw new NotSupportedException (Msg);

		public static void AddResourceFile (this AssemblyBuilder builder, string name, string fileName, ResourceAttributes attribute)
			=> throw new NotSupportedException (Msg);

		public static void Save (this AssemblyBuilder builder, string assemblyFileName)
			=> throw new NotSupportedException (Msg);

		public static void Save (this AssemblyBuilder builder, string assemblyFileName, PortableExecutableKinds portableExecutableKind, ImageFileMachine imageFileMachine)
			=> throw new NotSupportedException (Msg);

		public static void SetEntryPoint (this AssemblyBuilder builder, MethodInfo entryMethod, PEFileKinds fileKind)
			=> throw new NotSupportedException (Msg);

		// --- ModuleBuilder ---
		public static ModuleBuilder DefineDynamicModule (this AssemblyBuilder builder, string name, bool emitSymbolInfo)
			=> builder.DefineDynamicModule (name);

		public static void DefineManifestResource (this ModuleBuilder builder, string name, System.IO.Stream stream, ResourceAttributes attribute)
			=> throw new NotSupportedException (Msg);

		// --- declarative security / metadata tokens (CAS and token APIs are gone) ---
		public static void AddDeclarativeSecurity (this TypeBuilder builder, System.Security.Permissions.SecurityAction action, object pset)
			=> throw new NotSupportedException (Msg);

		public static void AddDeclarativeSecurity (this MethodBuilder builder, System.Security.Permissions.SecurityAction action, object pset)
			=> throw new NotSupportedException (Msg);

		public static void AddDeclarativeSecurity (this ConstructorBuilder builder, System.Security.Permissions.SecurityAction action, object pset)
			=> throw new NotSupportedException (Msg);

		public static MetadataTokenShim GetToken (this MethodBuilder builder) => new MetadataTokenShim (builder.MetadataToken);

		public static MetadataTokenShim GetToken (this ConstructorBuilder builder) => new MetadataTokenShim (builder.MetadataToken);

		// --- AppDomain-based assembly definition (removed; use the static factory) ---
		public static AssemblyBuilder DefineDynamicAssembly (this AppDomain domain, AssemblyName name, AssemblyBuilderAccess access)
			=> AssemblyBuilder.DefineDynamicAssembly (name, access);

		public static AssemblyBuilder DefineDynamicAssembly (this AppDomain domain, AssemblyName name, AssemblyBuilderAccess access, string dir)
			=> AssemblyBuilder.DefineDynamicAssembly (name, access);
	}

	// Stands in for the removed MethodToken/ConstructorToken structs (only .Token was consumed).
	struct MetadataTokenShim
	{
		public readonly int Token;
		public MetadataTokenShim (int token) { Token = token; }
	}
}
