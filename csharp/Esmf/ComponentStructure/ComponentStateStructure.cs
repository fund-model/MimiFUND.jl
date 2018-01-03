// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.IO;

namespace Esmf.ComponentStructure
{
    public class StateStructure
    {
        #region Static StateStructure cache
        private static Dictionary<Type, StateStructure> _stateStructureCache;

        private static IDictionary<Type, StateStructure> StateStructureCache
        {
            get
            {
                if (_stateStructureCache == null)
                {
                    _stateStructureCache = new Dictionary<Type, StateStructure>();
                }
                return _stateStructureCache;
            }
        }
        #endregion

        private string _interfaceName;
        private string _namespaceName;
        private Type _interfaceType;
        private List<StateFieldStructure> _fields = new List<StateFieldStructure>();
        private Type _proxyTypeCache = null;

        private StateStructure(Type type)
        {
            if (!type.Name.StartsWith("I", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The interface for the component state needs to start with I");
            }

            _interfaceName = type.Name;
            _namespaceName = type.Namespace;
            _interfaceType = type;

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo pi in properties)
            {
                if (pi.PropertyType.IsGenericType)
                {
                    Type baseGenericType = pi.PropertyType.GetGenericTypeDefinition();
                    if (baseGenericType == typeof(Esmf.IVariable1Dimensional<,>))
                    {
                        Type[] dimensionTypes = pi.PropertyType.GetGenericArguments();

                        StateFieldStructure field = new StateFieldStructure();
                        field.Name = pi.Name;
                        field.CanWrite = true;
                        field.Type = dimensionTypes[1];

                        StateFieldDimensionStructure d1 = new StateFieldDimensionStructure();
                        d1.Type = dimensionTypes[0];
                        d1.Name = d1.Type.Name;
                        field.Dimensions.Add(d1);

                        _fields.Add(field);
                    }
                    else if (baseGenericType == typeof(Esmf.IVariable2Dimensional<,,>))
                    {
                        Type[] dimensionTypes = pi.PropertyType.GetGenericArguments();

                        StateFieldStructure field = new StateFieldStructure();
                        field.Name = pi.Name;
                        field.CanWrite = true;
                        field.Type = dimensionTypes[2];

                        StateFieldDimensionStructure d1 = new StateFieldDimensionStructure();
                        d1.Type = dimensionTypes[0];
                        d1.Name = d1.Type.Name;
                        field.Dimensions.Add(d1);

                        StateFieldDimensionStructure d2 = new StateFieldDimensionStructure();
                        d2.Type = dimensionTypes[1];
                        d2.Name = d1.Type.Name;
                        field.Dimensions.Add(d2);

                        _fields.Add(field);
                    }
                    else if (baseGenericType == typeof(Esmf.IParameter1Dimensional<,>))
                    {
                        Type[] dimensionTypes = pi.PropertyType.GetGenericArguments();

                        StateFieldStructure field = new StateFieldStructure();
                        field.Name = pi.Name;
                        field.CanWrite = false;
                        field.Type = dimensionTypes[1];

                        StateFieldDimensionStructure d1 = new StateFieldDimensionStructure();
                        d1.Type = dimensionTypes[0];
                        d1.Name = d1.Type.Name;
                        field.Dimensions.Add(d1);

                        _fields.Add(field);
                    }
                    else if (baseGenericType == typeof(Esmf.IParameter2Dimensional<,,>))
                    {
                        Type[] dimensionTypes = pi.PropertyType.GetGenericArguments();

                        StateFieldStructure field = new StateFieldStructure();
                        field.Name = pi.Name;
                        field.CanWrite = false;
                        field.Type = dimensionTypes[2];

                        StateFieldDimensionStructure d1 = new StateFieldDimensionStructure();
                        d1.Type = dimensionTypes[0];
                        d1.Name = d1.Type.Name;
                        field.Dimensions.Add(d1);

                        StateFieldDimensionStructure d2 = new StateFieldDimensionStructure();
                        d2.Type = dimensionTypes[1];
                        d2.Name = d1.Type.Name;
                        field.Dimensions.Add(d2);

                        _fields.Add(field);
                    }
                }
                else
                {
                    StateFieldStructure field = new StateFieldStructure();
                    field.Name = pi.Name;
                    field.CanWrite = pi.CanWrite;
                    field.Type = pi.PropertyType;

                    _fields.Add(field);
                }
            }

        }

        public static StateStructure LoadFromInterface(Type type)
        {
            if (StateStructureCache.ContainsKey(type))
            {
                return StateStructureCache[type];
            }
            else
            {
                StateStructure stateStructure = new StateStructure(type);
                StateStructureCache.Add(type, stateStructure);
                return stateStructure;
            }
        }

        public object ConnectToState(ModelOutput mf, string componentName)
        {
            Type t = GetProxyTypeForStateInterface();
            object connector = Activator.CreateInstance(t);

            IStateObjectConnections soc = (IStateObjectConnections)connector;

            foreach (StateFieldStructure field in _fields)
            {
                if (field.Dimensions.Count == 0)
                {

                    if (field.CanWrite)
                    {
                        soc.AddNonDimensionalField(field.Name, mf.GetNonDimensionalVariableGetter(componentName, field.Name), mf.GetNonDimensionalVariableSetter(componentName, field.Name));
                    }
                    else
                    {
                        soc.AddNonDimensionalField(field.Name, mf.GetNonDimensionalVariableGetter(componentName, field.Name));
                    }
                }
                else
                {
                    soc.AddDimensionalField(field.Name, mf.GetDimensionalField(componentName, field.Name));

                }
            }
            return connector;
        }

        public Type GetProxyTypeForStateInterface()
        {
            if (_proxyTypeCache == null)
            {
                List<Type> typesRequired = new List<Type>();
                typesRequired.Add(_interfaceType);

                // Create a code compile unit and a namespace
                CodeCompileUnit ccu = new CodeCompileUnit();
                CodeNamespace ns = new CodeNamespace("Esmf.StateTypes");

                // Add some imports statements to the namespace
                ns.Imports.Add(new CodeNamespaceImport("System"));
                // ns.Imports.Add(new CodeNamespaceImport("System.Drawing"));

                // Add the namespace to the code compile unit
                ccu.Namespaces.Add(ns);

                string implementationTypeName = _interfaceName.StartsWith("I", StringComparison.OrdinalIgnoreCase) ? _interfaceName.Substring(1) : String.Format("{0}Implementation", _interfaceName);
                CodeTypeDeclaration ctd = new CodeTypeDeclaration(implementationTypeName);
                ctd.BaseTypes.Add(_interfaceType);
                ctd.BaseTypes.Add(typeof(IStateObjectConnections));


                ns.Types.Add(ctd);

                foreach (StateFieldStructure field in _fields)
                {
                    if (!typesRequired.Contains(field.Type))
                    {
                        typesRequired.Add(field.Type);
                    }

                    if (field.Dimensions.Count == 0)
                    {
                        CodeTypeReference tt = new CodeTypeReference(field.Type);

                        CodeTypeReference ttGetter = new CodeTypeReference(typeof(NonDimensionalFieldGetter<>));
                        ttGetter.TypeArguments.Add(tt);
                        string _fieldGetter = String.Format("_{0}FieldGetter", field.Name);
                        CodeMemberField newGetterField = new CodeMemberField(ttGetter, _fieldGetter);
                        ctd.Members.Add(newGetterField);

                        CodeMemberProperty p = new CodeMemberProperty();
                        p.Name = String.Format("{0}.{1}", _interfaceType, field.Name);
                        p.Type = tt;
                        p.Attributes -= MemberAttributes.Private;
                        CodeStatementCollection getStatements = p.GetStatements;

                        getStatements.Add(
                            new CodeConditionStatement(
                                new CodeBinaryOperatorExpression(
                                    new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(),
                                            _fieldGetter),
                                        CodeBinaryOperatorType.IdentityInequality,
                                        new CodePrimitiveExpression(null)),
                                    new CodeStatement[] { new CodeMethodReturnStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(new CodeThisReferenceExpression(), _fieldGetter))) },
                                    new CodeStatement[] { new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(InvalidOperationException)))) }));

                        if (field.CanWrite)
                        {
                            CodeTypeReference ttSetter = new CodeTypeReference(typeof(NonDimensionalFieldSetter<>));
                            ttSetter.TypeArguments.Add(tt);
                            string _fieldSetter = String.Format("_{0}FieldSetter", field.Name);
                            CodeMemberField newSetterField = new CodeMemberField(ttSetter, _fieldSetter);
                            ctd.Members.Add(newSetterField);

                            CodeStatementCollection setStatements = p.SetStatements;

                            setStatements.Add(
                                new CodeConditionStatement(
                                    new CodeBinaryOperatorExpression(
                                        new CodeFieldReferenceExpression(
                                            new CodeThisReferenceExpression(),
                                                _fieldSetter),
                                            CodeBinaryOperatorType.IdentityInequality,
                                            new CodePrimitiveExpression(null)),
                                        new CodeStatement[] { new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(new CodeThisReferenceExpression(), _fieldSetter), new CodeExpression[] { new CodePropertySetValueReferenceExpression() })) },
                                        new CodeStatement[] { new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(InvalidOperationException)))) }));

                        }

                        ctd.Members.Add(p);

                    }
                    else if (field.Dimensions.Count > 0)
                    {
                        CodeTypeReference tt;
                        if (field.CanWrite)
                        {
                            if (field.Dimensions.Count == 1)
                            {
                                tt = new CodeTypeReference(typeof(Esmf.IVariable1Dimensional<,>));
                            }
                            else if (field.Dimensions.Count == 2)
                            {
                                tt = new CodeTypeReference(typeof(Esmf.IVariable2Dimensional<,,>));
                            }
                            else
                            {
                                throw new InvalidOperationException("Can't have parameters with more than 2 dimensions");
                            }
                        }
                        else
                        {
                            if (field.Dimensions.Count == 1)
                            {
                                tt = new CodeTypeReference(typeof(Esmf.IParameter1Dimensional<,>));
                            }
                            else if (field.Dimensions.Count == 2)
                            {
                                tt = new CodeTypeReference(typeof(Esmf.IParameter2Dimensional<,,>));
                            }
                            else
                            {
                                throw new InvalidOperationException("Can't have fields with more than 2 dimensions");
                            }
                        }

                        foreach (StateFieldDimensionStructure sfds in field.Dimensions)
                        {
                            tt.TypeArguments.Add(sfds.Type);

                            if (!typesRequired.Contains(sfds.Type))
                            {
                                typesRequired.Add(sfds.Type);
                            }
                        }
                        tt.TypeArguments.Add(field.Type);

                        string fieldName = String.Format("_{0}Field", field.Name);
                        CodeMemberField newField = new CodeMemberField(tt, fieldName);
                        ctd.Members.Add(newField);

                        CodeMemberProperty p = new CodeMemberProperty();
                        p.Name = String.Format("{0}.{1}", _interfaceType, field.Name);
                        p.Type = tt;
                        p.Attributes -= MemberAttributes.Private;
                        CodeStatementCollection statements = p.GetStatements;

                        statements.Add(
                            new CodeConditionStatement(
                                new CodeBinaryOperatorExpression(
                                    new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(),
                                            fieldName),
                                        CodeBinaryOperatorType.IdentityInequality,
                                        new CodePrimitiveExpression(null)),
                                    new CodeStatement[] { new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)) },
                                    new CodeStatement[] { new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(InvalidOperationException)))) }));
                        ctd.Members.Add(p);
                    }

                }


                GenerateCodeForIStateObjectConnection(ctd);

                CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
                CompilerParameters cp = new CompilerParameters();
                cp.ReferencedAssemblies.Add("System.dll");
                cp.ReferencedAssemblies.Add(typeof(IParameter1Dimensional<,>).Module.FullyQualifiedName);

                List<string> modules = new List<string>();
                foreach (Type t in typesRequired)
                {
                    if (!modules.Contains(t.Module.FullyQualifiedName))
                    {
                        modules.Add(t.Module.FullyQualifiedName);
                    }
                }

                foreach (string s in modules)
                {
                    cp.ReferencedAssemblies.Add(s);
                }


                cp.GenerateInMemory = true;
                string assemblyDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Temp");
                if (!Directory.Exists(assemblyDirectory))
                {
                    Directory.CreateDirectory(assemblyDirectory);
                }
                cp.OutputAssembly = Path.Combine(assemblyDirectory, string.Format("AutoGenerated{0}.dll", Guid.NewGuid()));
                CompilerResults results = provider.CompileAssemblyFromDom(cp, ccu);
                if (results.Errors.HasErrors)
                {
                    foreach (CompilerError err in results.Errors)
                    {
                        Console.WriteLine();
                        Console.WriteLine(err);
                    }
                }

                //CodeDomProvider provider2 = new Microsoft.CSharp.CSharpCodeProvider();
                //using (System.IO.TextWriter output = System.IO.File.CreateText(@"C:\Users\davidanthoff\Documents\Visual Studio 2005\Projects\test.cs"))
                //{
                //    provider2.GenerateCodeFromCompileUnit(ccu, output, new CodeGeneratorOptions());
                //}

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                _proxyTypeCache = results.CompiledAssembly.GetType("Esmf.StateTypes." + implementationTypeName);

                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                return _proxyTypeCache;
            }
            else
            {
                return _proxyTypeCache;
            }
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("FundComponents,"))
            {
                string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "FundComponents.dll");
                var a = Assembly.LoadFile(path);
                return a;
            }
            else if (args.Name.StartsWith("Esmf,"))
            {
                string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Esmf.dll");
                var a = Assembly.LoadFile(path);
                return a;
            }
            else if (args.Name.StartsWith("Fund,"))
            {
                string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Fund.exe");
                var a = Assembly.LoadFile(path);
                return a;
            }
            else
                return null;
        }

        private void GenerateCodeForIStateObjectConnection(CodeTypeDeclaration ctd)
        {
            GenerateCodeForIStateObjectConnectionAddNonDimensionalFieldGetterOnly(ctd);
            GenerateCodeForIStateObjectConnectionAddNonDimensionalFieldGetterAndSetter(ctd);
            GenerateCodeForIStateObjectConnectionAddDimensionalField(ctd);
            //GenerateCodeForIStateObjectConnectionAddReadWriteTimeDimensionalField(modell, ctd);
        }

        private void GenerateCodeForIStateObjectConnectionAddDimensionalField(CodeTypeDeclaration ctd)
        {
            // Implementation of IStateObjectConnection
            CodeMemberMethod m = new CodeMemberMethod();
            m.Name = "AddDimensionalField";
            m.PrivateImplementationType = new CodeTypeReference(typeof(IStateObjectConnections));
            m.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            CodeTypeReference bb = new CodeTypeReference(typeof(Object));
            m.Parameters.Add(new CodeParameterDeclarationExpression(bb, "field"));

            CodeStatementCollection statements2 = m.Statements;

            CodeStatementCollection statements3 = statements2;
            foreach (StateFieldStructure field in _fields)
            {
                if (field.Dimensions.Count > 0)
                {
                    CodeTypeReference fieldType;

                    if (field.Dimensions.Count == 1 && !field.CanWrite)
                    {
                        fieldType = new CodeTypeReference(typeof(IParameter1Dimensional<,>));
                    }
                    else if (field.Dimensions.Count == 1 && field.CanWrite)
                    {
                        fieldType = new CodeTypeReference(typeof(IVariable1Dimensional<,>));
                    }
                    else if (field.Dimensions.Count == 2 && !field.CanWrite)
                    {
                        fieldType = new CodeTypeReference(typeof(IParameter2Dimensional<,,>));
                    }
                    else if (field.Dimensions.Count == 2 && field.CanWrite)
                    {
                        fieldType = new CodeTypeReference(typeof(IVariable2Dimensional<,,>));
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    foreach (StateFieldDimensionStructure d in field.Dimensions)
                    {
                        fieldType.TypeArguments.Add(d.Type);
                    }
                    fieldType.TypeArguments.Add(field.Type);

                    CodeConditionStatement condition = new CodeConditionStatement();
                    statements3.Add(condition);
                    condition.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("name"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(field.Name));
                    condition.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), String.Format("_{0}Field", field.Name)), new CodeCastExpression(fieldType, new CodeArgumentReferenceExpression("field"))));
                    statements3 = condition.FalseStatements;
                }
            }

            //statements2.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "testtestdelegate"), new CodeArgumentReferenceExpression("method")));
            ctd.Members.Add(m);
        }


        private void GenerateCodeForIStateObjectConnectionAddNonDimensionalFieldGetterOnly(CodeTypeDeclaration ctd)
        {
            // Implementation of IStateObjectConnection
            CodeMemberMethod m = new CodeMemberMethod();
            m.Name = "AddNonDimensionalField";
            m.PrivateImplementationType = new CodeTypeReference(typeof(IStateObjectConnections));
            m.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            CodeTypeReference bb = new CodeTypeReference(typeof(Object));
            m.Parameters.Add(new CodeParameterDeclarationExpression(bb, "getterMethod"));

            CodeStatementCollection statements2 = m.Statements;

            CodeStatementCollection statements3 = statements2;

            foreach (StateFieldStructure field in _fields)
            {
                if (field.Dimensions.Count == 0 && !field.CanWrite)
                {
                    CodeTypeReference getterFieldType = new CodeTypeReference(typeof(NonDimensionalFieldGetter<>));
                    getterFieldType.TypeArguments.Add(field.Type);

                    CodeConditionStatement condition = new CodeConditionStatement();
                    statements3.Add(condition);
                    condition.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("name"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(field.Name));
                    condition.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), String.Format("_{0}FieldGetter", field.Name)), new CodeCastExpression(getterFieldType, new CodeArgumentReferenceExpression("getterMethod"))));
                    statements3 = condition.FalseStatements;
                }
            }

            ctd.Members.Add(m);
        }

        private void GenerateCodeForIStateObjectConnectionAddNonDimensionalFieldGetterAndSetter(CodeTypeDeclaration ctd)
        {
            // Implementation of IStateObjectConnection
            CodeMemberMethod m = new CodeMemberMethod();
            m.Name = "AddNonDimensionalField";
            m.PrivateImplementationType = new CodeTypeReference(typeof(IStateObjectConnections));
            m.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            CodeTypeReference bb = new CodeTypeReference(typeof(Object));
            m.Parameters.Add(new CodeParameterDeclarationExpression(bb, "getterMethod"));
            m.Parameters.Add(new CodeParameterDeclarationExpression(bb, "setterMethod"));

            CodeStatementCollection statements2 = m.Statements;

            CodeStatementCollection statements3 = statements2;

            foreach (StateFieldStructure field in _fields)
            {
                if (field.Dimensions.Count == 0 && field.CanWrite)
                {
                    CodeTypeReference getterFieldType = new CodeTypeReference(typeof(NonDimensionalFieldGetter<>));
                    getterFieldType.TypeArguments.Add(field.Type);

                    CodeTypeReference setterFieldType = new CodeTypeReference(typeof(NonDimensionalFieldSetter<>));
                    setterFieldType.TypeArguments.Add(field.Type);


                    CodeConditionStatement condition = new CodeConditionStatement();
                    statements3.Add(condition);
                    condition.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("name"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(field.Name));
                    condition.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), String.Format("_{0}FieldGetter", field.Name)), new CodeCastExpression(getterFieldType, new CodeArgumentReferenceExpression("getterMethod"))));
                    condition.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), String.Format("_{0}FieldSetter", field.Name)), new CodeCastExpression(setterFieldType, new CodeArgumentReferenceExpression("setterMethod"))));
                    statements3 = condition.FalseStatements;
                }
            }

            ctd.Members.Add(m);
        }

    }
}
