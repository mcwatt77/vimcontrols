using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Navigator.Repository
{
    public static class ImplementationBuilder
    {
        public static Type Implement(Type baseType)
        {
            var fileInfo = new FileInfo(baseType.Name + ".dll");
            if (fileInfo.Exists)
                return Assembly.LoadFile(fileInfo.FullName).GetTypes().First();

            var assemblyBuilder = GetAssemblyBuilder(baseType.Name);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(baseType.Name, baseType.Name + ".dll", true);

            var newType = moduleBuilder.DefineType(baseType.Name + "Implementation", TypeAttributes.Public, null, new[] { baseType });

            foreach (var property in baseType.GetProperties())
                AddAutoProperty(newType, property);

            newType.CreateType();

            assemblyBuilder.Save(fileInfo.Name);

            var newAssembly = Assembly.LoadFile(fileInfo.FullName);
            var firstType = newAssembly.GetTypes().First();

            return firstType;
        }

        private static string FormatBackingField(string name)
        {
            return "_" + name.Substring(0, 1).ToLower() + name.Substring(1, name.Length - 1);
        }

        private static void AddAutoProperty(TypeBuilder newType, PropertyInfo property)
        {
            var propertyBuilder = newType.DefineProperty(property.Name, PropertyAttributes.HasDefault, property.PropertyType, null);
            var fieldBuilder = newType.DefineField(FormatBackingField(property.Name), property.PropertyType, FieldAttributes.Private);

            DefineGetMethod(newType, propertyBuilder, fieldBuilder, property);
            DefineSetMethod(newType, propertyBuilder, fieldBuilder, property);
        }

        private static void DefineGetMethod(TypeBuilder newType, PropertyBuilder propertyBuilder, FieldInfo fieldBuilder, PropertyInfo property)
        {
            var getMethod = property.GetGetMethod();
            var methodBuilder = newType.DefineMethod(getMethod.Name,
                                                     MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual,
                                                     property.PropertyType,
                                                     Type.EmptyTypes);

            var ilg = methodBuilder.GetILGenerator();
            
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Ldfld, fieldBuilder);
            ilg.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(methodBuilder);

            var methodInfo = methodBuilder.GetBaseDefinition();

            newType.DefineMethodOverride(methodInfo, getMethod);
        }

        private static void DefineSetMethod(TypeBuilder newType, PropertyBuilder propertyBuilder, FieldInfo fieldBuilder, PropertyInfo property)
        {
            var setMethod = property.GetSetMethod();
            if (setMethod == null) return;

            var methodBuilder = newType.DefineMethod(setMethod.Name,
                                                     MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual,
                                                     null,
                                                     new[] {property.PropertyType});

            var ilg = methodBuilder.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Ldarg_1);
            ilg.Emit(OpCodes.Stfld, fieldBuilder);
            ilg.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(methodBuilder);

            var methodInfo = methodBuilder.GetBaseDefinition();

            newType.DefineMethodOverride(methodInfo, setMethod);
        }

        public interface ITestI
        {
            string Name { get; set; }
        }

        public static Type BuildDynamicTypeWithProperties()
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName,
                                                            AssemblyBuilderAccess.RunAndSave);
            // Generate a persistable single-module assembly.
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");

            TypeBuilder myTypeBuilder = myModBuilder.DefineType("CustomerData",
                                                                TypeAttributes.Public,
                                                                null,
                                                                new[] {typeof (ITestI)});

            FieldBuilder customerNameBldr = myTypeBuilder.DefineField("customerName",
                                                            typeof(string),
                                                            FieldAttributes.Private);

            // The last argument of DefineProperty is null, because the
            // property has no parameters. (If you don't specify null, you must
            // specify an array of Type objects. For a parameterless property,
            // use an array with no elements: new Type[] {})
            PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty("Name",
                                                             PropertyAttributes.HasDefault,
                                                             typeof(string),
                                                             null);

            // The property set and property get methods require a special
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder custNameGetPropMthdBldr =
                myTypeBuilder.DefineMethod("get_Name",
                                           getSetAttr,
                                           typeof(string),
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder custNameSetPropMthdBldr =
                myTypeBuilder.DefineMethod("set_Name",
                                           getSetAttr,
                                           null,
                                           new Type[] { typeof(string) });

            ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);
            custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively. 
            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);


            Type retval = myTypeBuilder.CreateType();

            // Save the assembly so it can be examined with Ildasm.exe,
            // or referenced by a test program.
            myAsmBuilder.Save(myAsmName.Name + ".dll");
            return retval;
        }

        private static AssemblyBuilder GetAssemblyBuilder(string name)
        {
            var assemblyName = new AssemblyName { Name = name + "Assembly" };
            return Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        }
    }
}