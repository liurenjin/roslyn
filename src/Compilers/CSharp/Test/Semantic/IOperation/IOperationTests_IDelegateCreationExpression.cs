﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = () => { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null) (Syntax: 'Action a = () => { };')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null) (Syntax: 'a = () => { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null) (Syntax: '= () => { }')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsImplicit) (Syntax: '() => { }')
          Target: 
            IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null) (Syntax: '() => { }')
              IBlockOperation (1 statements) (OperationKind.Block, Type: null) (Syntax: '{ }')
                IReturnOperation (OperationKind.Return, Type: null, IsImplicit) (Syntax: '{ }')
                  ReturnedValue: 
                    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitLambdaConversion_InitializerBindingReturnsJustAnonymousFunction()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/() => { }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null) (Syntax: '() => { }')
  IBlockOperation (1 statements) (OperationKind.Block, Type: null) (Syntax: '{ }')
    IReturnOperation (OperationKind.Return, Type: null, IsImplicit) (Syntax: '{ }')
      ReturnedValue: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LambdaExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = () => 1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action a = () => 1;')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = () => 1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= () => 1')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid, IsImplicit) (Syntax: '() => 1')
          Target: 
            IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '() => 1')
              IBlockOperation (2 statements) (OperationKind.Block, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
                IExpressionStatementOperation (OperationKind.ExpressionStatement, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
                  Expression: 
                    ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: '1')
                IReturnOperation (OperationKind.Return, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
                  ReturnedValue: 
                    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         /*<bind>*/Action a = () => 1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 36)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = (int i) => { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action a =  ...  i) => { };')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = (int i) => { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= (int i) => { }')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid, IsImplicit) (Syntax: '(int i) => { }')
          Target: 
            IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '(int i) => { }')
              IBlockOperation (0 statements) (OperationKind.Block, Type: null, IsInvalid) (Syntax: '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         /*<bind>*/Action a = (int i) => { };/*</bind>*/
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)(() => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: '(Action)(() => { })')
  Target: 
    IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null) (Syntax: '() => { }')
      IBlockOperation (1 statements) (OperationKind.Block, Type: null) (Syntax: '{ }')
        IReturnOperation (OperationKind.Return, Type: null, IsImplicit) (Syntax: '{ }')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)(() => 1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)(() => 1)')
  Target: 
    IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '() => 1')
      IBlockOperation (2 statements) (OperationKind.Block, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
        IExpressionStatementOperation (OperationKind.ExpressionStatement, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
          Expression: 
            ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: '1')
        IReturnOperation (OperationKind.Return, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         Action a = /*<bind>*/(Action)(() => 1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 45)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)((int i) => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)((int i) => { })')
  Target: 
    IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '(int i) => { }')
      IBlockOperation (0 statements) (OperationKind.Block, Type: null, IsInvalid) (Syntax: '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         Action a = /*<bind>*/(Action)((int i) => { })/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 39)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_DelegateExpression()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = delegate() { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null) (Syntax: 'Action a =  ... gate() { };')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null) (Syntax: 'a = delegate() { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null) (Syntax: '= delegate() { }')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsImplicit) (Syntax: 'delegate() { }')
          Target: 
            IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null) (Syntax: 'delegate() { }')
              IBlockOperation (1 statements) (OperationKind.Block, Type: null) (Syntax: '{ }')
                IReturnOperation (OperationKind.Return, Type: null, IsImplicit) (Syntax: '{ }')
                  ReturnedValue: 
                    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_DelegateExpression_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = delegate() { return 1; };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action a =  ... eturn 1; };')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = delegat ... return 1; }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= delegate( ... return 1; }')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid, IsImplicit) (Syntax: 'delegate() { return 1; }')
          Target: 
            IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: 'delegate() { return 1; }')
              IBlockOperation (1 statements) (OperationKind.Block, Type: null, IsInvalid) (Syntax: '{ return 1; }')
                IReturnOperation (OperationKind.Return, Type: null, IsInvalid) (Syntax: 'return 1;')
                  ReturnedValue: 
                    ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 1) (Syntax: '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS8030: Anonymous function converted to a void returning delegate cannot return a value
                //         /*<bind>*/Action a = delegate() { return 1; };/*</bind>*/
                Diagnostic(ErrorCode.ERR_RetNoObjectRequiredLambda, "return").WithLocation(7, 43)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_DelegateExpression_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = delegate(int i) { };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action a =  ... int i) { };')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = delegate(int i) { }')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= delegate(int i) { }')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid, IsImplicit) (Syntax: 'delegate(int i) { }')
          Target: 
            IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: 'delegate(int i) { }')
              IBlockOperation (0 statements) (OperationKind.Block, Type: null, IsInvalid) (Syntax: '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         /*<bind>*/Action a = delegate(int i) { };/*</bind>*/
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "delegate(int i) { }").WithArguments("System.Action", "1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
    void M1() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null) (Syntax: 'Action a = M1;')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null) (Syntax: 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null) (Syntax: '= M1')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsImplicit) (Syntax: 'M1')
          Target: 
            IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
              Instance Receiver: 
                IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(15513, "https://github.com/dotnet/roslyn/issues/15513")]
        public void DelegateCreationExpression_ImplicitMethodBinding_InitializerBindingReturnsJustMethodReference()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
    static void M1() { }
}
";

            string expectedOperationTree = @"
IMethodReferenceOperation: void Program.M1() (Static) (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
  Instance Receiver: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action a = M1;')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= M1')
        IConversionOperation (Implicit, TryCast: False, Unchecked) (OperationKind.Conversion, Type: System.Action, IsInvalid, IsImplicit) (Syntax: 'M1')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvalidOperation (OperationKind.Invalid, Type: ?, IsInvalid) (Syntax: 'M1')
              Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         /*<bind>*/Action a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidIdentifier_InitializerBindingReturnsJustInvalidExpression()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
}
";

            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: ?, IsInvalid) (Syntax: 'M1')
  Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action a = M1;')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= M1')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid, IsImplicit) (Syntax: 'M1')
          Target: 
            IMethodReferenceOperation: System.Int32 Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
              Instance Receiver: 
                IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         /*<bind>*/Action a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_BadRetType, "M1").WithArguments("Program.M1()", "int").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidReturnType_InitializerBindingReturnsJustMethodReference()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
    int M1() => 1;
}
";

            string expectedOperationTree = @"
IMethodReferenceOperation: System.Int32 Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
  Instance Receiver: 
    IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "M1").WithArguments("Program.M1()", "int").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action a = M1;/*</bind>*/
    }
    void M1(object o) { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action a = M1;')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = M1')
    Variables: Local_1: System.Action a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= M1')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid, IsImplicit) (Syntax: 'M1')
          Target: 
            IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M1')
              Children(1):
                  IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'M1' matches delegate 'Action'
                //         /*<bind>*/Action a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "M1").WithArguments("M1", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_InvalidArgumentType_InitializerBindingReturnsJustNoneOperation()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/M1/*</bind>*/;
    }
    void M1(object o) { }
}
";

            string expectedOperationTree = @"
IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M1')
  Children(1):
      IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0123: No overload for 'M1' matches delegate 'Action'
                //         Action a = /*<bind>*/M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "M1").WithArguments("M1", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: '(Action)M1')
  Target: 
    IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IConversionOperation (Explicit, TryCast: False, Unchecked) (OperationKind.Conversion, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
  Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Operand: 
    IInvalidOperation (OperationKind.Invalid, Type: ?, IsInvalid) (Syntax: 'M1')
      Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/(Action)M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidIdentifierWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        object o = new object();
        Action a = /*<bind>*/(Action)o.M1/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IConversionOperation (Explicit, TryCast: False, Unchecked) (OperationKind.Conversion, Type: System.Action, IsInvalid) (Syntax: '(Action)o.M1')
  Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Operand: 
    IInvalidOperation (OperationKind.Invalid, Type: ?, IsInvalid) (Syntax: 'o.M1')
      Children(1):
          ILocalReferenceOperation: o (OperationKind.LocalReference, Type: System.Object) (Syntax: 'o')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1061: 'object' does not contain a definition for 'M1' and no extension method 'M1' accepting a first argument of type 'object' could be found (are you missing a using directive or an assembly reference?)
                //         Action a = /*<bind>*/(Action)o.M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoSuchMemberOrExtension, "M1").WithArguments("object", "M1").WithLocation(8, 40)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
  Target: 
    IMethodReferenceOperation: System.Int32 Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/(Action)M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "(Action)M1").WithArguments("Program.M1()", "int").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidReturnTypeWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Program p = new Program();
        Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)p.M1')
  Target: 
    IMethodReferenceOperation: System.Int32 Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'p.M1')
      Instance Receiver: 
        ILocalReferenceOperation: p (OperationKind.LocalReference, Type: Program, IsInvalid) (Syntax: 'p')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "(Action)p.M1").WithArguments("Program.M1()", "int").WithLocation(8, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/(Action)M1/*</bind>*/;
    }
    void M1(object o) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
  Target: 
    IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M1')
      Children(1):
          IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // CS0030: Cannot convert type 'method' to 'Action'
                //         Action a = /*<bind>*/(Action)M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(Action)M1").WithArguments("method", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitMethodBinding_InvalidArgumentTypeWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Program p = new Program();
        Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
    }
    void M1(object o) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)p.M1')
  Target: 
    IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'p.M1')
      Children(1):
          ILocalReferenceOperation: p (OperationKind.LocalReference, Type: Program, IsInvalid) (Syntax: 'p')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0030: Cannot convert type 'method' to 'Action'
                //         Action a = /*<bind>*/(Action)p.M1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(Action)p.M1").WithArguments("method", "System.Action").WithLocation(8, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CastExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(() => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: 'new Action(() => { })')
  Target: 
    IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null) (Syntax: '() => { }')
      IBlockOperation (1 statements) (OperationKind.Block, Type: null) (Syntax: '{ }')
        IReturnOperation (OperationKind.Return, Type: null, IsImplicit) (Syntax: '{ }')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(() => 1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action(() => 1)')
  Target: 
    IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '() => 1')
      IBlockOperation (2 statements) (OperationKind.Block, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
        IExpressionStatementOperation (OperationKind.ExpressionStatement, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
          Expression: 
            ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: '1')
        IReturnOperation (OperationKind.Return, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         Action a = /*<bind>*/new Action(() => 1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 47)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((int i) => { })/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action( ...  i) => { })')
  Target: 
    IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '(int i) => { }')
      IBlockOperation (1 statements) (OperationKind.Block, Type: null, IsInvalid) (Syntax: '{ }')
        IReturnOperation (OperationKind.Return, Type: null, IsInvalid, IsImplicit) (Syntax: '{ }')
          ReturnedValue: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         Action a = /*<bind>*/new Action((int i) => { })/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitLambdaConversion_InvalidMultipleParameters()
        {
            string source = @"
using System;

class C
{
    void M1()
    {
        Action action = /*<bind>*/new Action((o) => { }, new object())/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action( ... w object())')
  Children(2):
      IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '(o) => { }')
        IBlockOperation (0 statements) (OperationKind.Block, Type: null, IsInvalid) (Syntax: '{ }')
      IObjectCreationOperation (Constructor: System.Object..ctor()) (OperationKind.ObjectCreation, Type: System.Object, IsInvalid) (Syntax: 'new object()')
        Arguments(0)
        Initializer: 
          null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0149: Method name expected
                //         Action action = /*<bind>*/new Action((o) => { }, new object())/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethodNameExpected, "(o) => { }, new object()").WithLocation(8, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1()
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: 'new Action(M1)')
  Target: 
    IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(15513, "https://github.com/dotnet/roslyn/issues/15513")]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitStaticMethodBindingConversion_01()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    static void M1()
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: 'new Action(M1)')
  Target: 
    IMethodReferenceOperation: void Program.M1() (Static) (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
      Instance Receiver: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(15513, "https://github.com/dotnet/roslyn/issues/15513")]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitStaticMethodBindingConversion_02()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(this.M1)/*</bind>*/;
    }

    static void M1()
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action(this.M1)')
  Target: 
    IMethodReferenceOperation: void Program.M1() (Static) (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'this.M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid) (Syntax: 'this')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // (7,41): error CS0176: Member 'Program.M1()' cannot be accessed with an instance reference; qualify it with a type name instead
                //         Action a = /*<bind>*/new Action(this.M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ObjectProhibited, "this.M1").WithArguments("Program.M1()").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversionWithReceiver()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        var p = new Program();
        Action a = /*<bind>*/new Action(p.M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: 'new Action(p.M1)')
  Target: 
    IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null) (Syntax: 'p.M1')
      Instance Receiver: 
        ILocalReferenceOperation: p (OperationKind.LocalReference, Type: Program) (Syntax: 'p')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion_InvalidMissingIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action(M1)')
  Children(1):
      IInvalidOperation (OperationKind.Invalid, Type: ?, IsInvalid) (Syntax: 'M1')
        Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    int M1() => 1;
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action(M1)')
  Target: 
    IMethodReferenceOperation: System.Int32 Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "M1").WithArguments("Program.M1()", "int").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndImplicitMethodBindingConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1(object o)
    { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action(M1)')
  Target: 
    IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M1')
      Children(1):
          IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'M1' matches delegate 'Action'
                //         Action a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "new Action(M1)").WithArguments("M1", "System.Action").WithLocation(7, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateCreation_InvalidMultipleParameters()
        {
            string source = @"
using System;

class C
{
    void M1()
    {
        Action action = /*<bind>*/new Action(M2, M3)/*</bind>*/;
    }

    void M2()
    {
    }
    void M3()
    {
    }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action(M2, M3)')
  Children(2):
      IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M2')
        Children(1):
            IInstanceReferenceOperation (OperationKind.InstanceReference, Type: C, IsInvalid, IsImplicit) (Syntax: 'M2')
      IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M3')
        Children(1):
            IInstanceReferenceOperation (OperationKind.InstanceReference, Type: C, IsInvalid, IsImplicit) (Syntax: 'M3')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0149: Method name expected
                //         Action action = /*<bind>*/new Action(M2, M3)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethodNameExpected, "M2, M3").WithLocation(8, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_InvalidTargetArguments()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action(M1)')
  Target: 
    IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Action<string>'
                //         Action<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action(M1)").WithArguments("System.Action", "System.Action<string>").WithLocation(7, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_InvalidTargetReturn()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Func<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action(M1)')
  Target: 
    IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Func<string>'
                //         Func<string> a = /*<bind>*/new Action(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action(M1)").WithArguments("System.Action", "System.Func<string>").WithLocation(7, 36)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitLambdaConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)(() => { }))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: 'new Action( ... () => { }))')
  Target: 
    IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: '(Action)(() => { })')
      Target: 
        IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null) (Syntax: '() => { }')
          IBlockOperation (1 statements) (OperationKind.Block, Type: null) (Syntax: '{ }')
            IReturnOperation (OperationKind.Return, Type: null, IsImplicit) (Syntax: '{ }')
              ReturnedValue: 
                null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitLambdaConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)(() => 1))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action( ... )(() => 1))')
  Children(1):
      IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)(() => 1)')
        Target: 
          IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '() => 1')
            IBlockOperation (2 statements) (OperationKind.Block, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
              IExpressionStatementOperation (OperationKind.ExpressionStatement, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
                Expression: 
                  ILiteralOperation (OperationKind.Literal, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: '1')
              IReturnOperation (OperationKind.Return, Type: null, IsInvalid, IsImplicit) (Syntax: '1')
                ReturnedValue: 
                  null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         Action a = /*<bind>*/new Action((Action)(() => 1))/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IllegalStatement, "1").WithLocation(7, 56)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitLambdaConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)((int i) => { }))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action( ... i) => { }))')
  Children(1):
      IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)((int i) => { })')
        Target: 
          IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '(int i) => { }')
            IBlockOperation (0 statements) (OperationKind.Block, Type: null, IsInvalid) (Syntax: '{ }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1593: Delegate 'Action' does not take 1 arguments
                //         Action a = /*<bind>*/new Action((Action)((int i) => { }))/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDelArgCount, "(int i) => { }").WithArguments("System.Action", "1").WithLocation(7, 50)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
    void M1() {}
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: 'new Action((Action)M1)')
  Target: 
    IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action) (Syntax: '(Action)M1')
      Target: 
        IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
          Instance Receiver: 
            IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidMissingIdentifier()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action((Action)M1)')
  Children(1):
      IConversionOperation (Explicit, TryCast: False, Unchecked) (OperationKind.Conversion, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
        Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          IInvalidOperation (OperationKind.Invalid, Type: ?, IsInvalid) (Syntax: 'M1')
            Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'M1' does not exist in the current context
                //         Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M1").WithArguments("M1").WithLocation(7, 49)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidReturnType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
    int M1() => 1;
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action((Action)M1)')
  Children(1):
      IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
        Target: 
          IMethodReferenceOperation: System.Int32 Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
            Instance Receiver: 
              IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0407: 'int Program.M1()' has the wrong return type
                //         Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadRetType, "(Action)M1").WithArguments("Program.M1()", "int").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidArgumentType()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }
    void M1(int i) { }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action, IsInvalid) (Syntax: 'new Action((Action)M1)')
  Children(1):
      IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
        Target: 
          IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M1')
            Children(1):
                IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0030: Cannot convert type 'method' to 'Action'
                //         Action a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(Action)M1").WithArguments("method", "System.Action").WithLocation(7, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidTargetArgument()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action((Action)M1)')
  Target: 
    IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
      Target: 
        IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
          Instance Receiver: 
            IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Action<string>'
                //         Action<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action((Action)M1)").WithArguments("System.Action", "System.Action<string>").WithLocation(7, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidTargetReturn()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Func<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: 'new Action((Action)M1)')
  Target: 
    IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
      Target: 
        IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
          Instance Receiver: 
            IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'System.Action' to 'System.Func<string>'
                //         Func<string> a = /*<bind>*/new Action((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new Action((Action)M1)").WithArguments("System.Action", "System.Func<string>").WithLocation(7, 36)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorAndExplicitMethodBindingConversion_InvalidConstructorArgument()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<int> a = /*<bind>*/new Action<int>((Action)M1)/*</bind>*/;
    }

    void M1() { }
}
";
            string expectedOperationTree = @"
IInvalidOperation (OperationKind.Invalid, Type: System.Action<System.Int32>, IsInvalid) (Syntax: 'new Action< ... (Action)M1)')
  Children(1):
      IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action, IsInvalid) (Syntax: '(Action)M1')
        Target: 
          IMethodReferenceOperation: void Program.M1() (OperationKind.MethodReference, Type: null, IsInvalid) (Syntax: 'M1')
            Instance Receiver: 
              IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'Action' matches delegate 'Action<int>'
                //         Action<int> a = /*<bind>*/new Action<int>((Action)M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "new Action<int>((Action)M1)").WithArguments("Action", "System.Action<int>").WithLocation(7, 35)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ConversionExpression_Implicit_ReferenceLambdaToDelegateConversion_InvalidSyntax()
        {
            string source = @"
class Program
{
    delegate void DType();
    void Main()
    {
        DType /*<bind>*/d1 = () =>/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'd1 = () =>/*</bind>*/')
  Variables: Local_1: Program.DType d1
  Initializer: 
    IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= () =>/*</bind>*/')
      IDelegateCreationOperation (OperationKind.DelegateCreation, Type: Program.DType, IsInvalid, IsImplicit) (Syntax: '() =>/*</bind>*/')
        Target: 
          IAnonymousFunctionOperation (Symbol: lambda expression) (OperationKind.AnonymousFunction, Type: null, IsInvalid) (Syntax: '() =>/*</bind>*/')
            IBlockOperation (2 statements) (OperationKind.Block, Type: null, IsInvalid, IsImplicit) (Syntax: '')
              IExpressionStatementOperation (OperationKind.ExpressionStatement, Type: null, IsInvalid, IsImplicit) (Syntax: '')
                Expression: 
                  IInvalidOperation (OperationKind.Invalid, Type: null, IsInvalid) (Syntax: '')
                    Children(0)
              IReturnOperation (OperationKind.Return, Type: null, IsInvalid, IsImplicit) (Syntax: '')
                ReturnedValue: 
                  null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ';'
                //         DType /*<bind>*/d1 = () =>/*</bind>*/;
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ";").WithArguments(";").WithLocation(7, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclaratorSyntax>(source, expectedOperationTree, expectedDiagnostics,
                additionalOperationTreeVerifier: new ExpectedSymbolVerifier().Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_MultipleCandidates_InvalidNoMatch()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action<int> a = M1;/*</bind>*/
    }
    void M1(Program o) { }
    void M1(string s) { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null, IsInvalid) (Syntax: 'Action<int> a = M1;')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null, IsInvalid) (Syntax: 'a = M1')
    Variables: Local_1: System.Action<System.Int32> a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null, IsInvalid) (Syntax: '= M1')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action<System.Int32>, IsInvalid, IsImplicit) (Syntax: 'M1')
          Target: 
            IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M1')
              Children(1):
                  IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'Program.M1(object)' matches delegate 'Action<int>'
                //         /*<bind>*/Action<int> a = M1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "M1").WithArguments("M1", "System.Action<int>").WithLocation(7, 35)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ImplicitMethodBinding_MultipleCandidates()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        /*<bind>*/Action<int> a = M1;/*</bind>*/
    }
    void M1(object o) { }
    void M1(int i) { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationsOperation (1 declarations) (OperationKind.VariableDeclarations, Type: null) (Syntax: 'Action<int> a = M1;')
  IVariableDeclarationOperation (1 variables) (OperationKind.VariableDeclaration, Type: null) (Syntax: 'a = M1')
    Variables: Local_1: System.Action<System.Int32> a
    Initializer: 
      IVariableInitializerOperation (OperationKind.VariableInitializer, Type: null) (Syntax: '= M1')
        IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action<System.Int32>, IsImplicit) (Syntax: 'M1')
          Target: 
            IMethodReferenceOperation: void Program.M1(System.Int32 i) (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
              Instance Receiver: 
                IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_MultipleCandidates()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<string> a = /*<bind>*/new Action<string>(M1)/*</bind>*/;
    }

    void M1(object o) { }

    void M1(string s) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action<System.String>) (Syntax: 'new Action<string>(M1)')
  Target: 
    IMethodReferenceOperation: void Program.M1(System.String s) (OperationKind.MethodReference, Type: null) (Syntax: 'M1')
      Instance Receiver: 
        IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DelegateCreationExpression_ExplicitDelegateConstructorImplicitMethodBinding_MultipleCandidates_InvalidNoMatch()
        {
            string source = @"
using System;
class Program
{
    void Main()
    {
        Action<int> a = /*<bind>*/new Action<int>(M1)/*</bind>*/;
    }

    void M1(Program o) { }

    void M1(string s) { }
}
";
            string expectedOperationTree = @"
IDelegateCreationOperation (OperationKind.DelegateCreation, Type: System.Action<System.Int32>, IsInvalid) (Syntax: 'new Action<int>(M1)')
  Target: 
    IOperation:  (OperationKind.None, Type: null, IsInvalid) (Syntax: 'M1')
      Children(1):
          IInstanceReferenceOperation (OperationKind.InstanceReference, Type: Program, IsInvalid, IsImplicit) (Syntax: 'M1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0123: No overload for 'Program.M1(object)' matches delegate 'Action<int>'
                //         Action<int> a = /*<bind>*/new Action<int>(M1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MethDelegateMismatch, "new Action<int>(M1)").WithArguments("M1", "System.Action<int>").WithLocation(7, 35)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
