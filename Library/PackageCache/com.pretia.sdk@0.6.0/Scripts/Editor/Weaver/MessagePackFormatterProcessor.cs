using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

using System.Runtime.CompilerServices;
using Mono.Collections.Generic;
using UnityEngine;

[assembly: InternalsVisibleTo("Tests")]

namespace PretiaArCloud.Networking.Weaver
{
    internal class MessagePackFormatterProcessor
    {
        private const string KEY_ATTRIBUTE_NAME = "KeyAttribute";
        private static readonly string[] PRIMITIVE_TYPES_DEFINITION = new string[]
        {
            "Int16",
            "Int32",
            "Int64",
            "UInt16",
            "UInt32",
            "UInt64",
            "Single",
            "Double",
            "Boolean",
            "Byte",
            "SByte",
            "Char",
        };

        private static readonly string[] LDC_I4_COMPATIBLE_TYPES = new string[]
        {
            "Int16",
            "Int32",
            "UInt16",
            "UInt32",
            "Boolean",
            "Byte",
            "SByte",
            "Char",
        };

        private static readonly string[] LDC_I8_COMPATIBLE_TYPES = new string[]
        {
            "Int64",
            "UInt64",
        };

        internal class IndexSequence
        {
            public int StartIndex;
            public int TotalIndicesCount;
            public int LastIndex;

            public IndexSequence LeftNode;
            public IndexSequence RightNode;

            public bool HasChildNode
            {
                get { return LeftNode != null; }
            }

            public IndexSequence[] Sequences;

            public Instruction Instruction;
            public Instruction[] JumpTable;
            public Instruction StartingPoint;
        }

        private static ModuleDefinition _pretiaModule;
        private static ModuleDefinition _msgPackModule;
        private static TypeReference _int32_typeRef;
        private static TypeReference _uint8_typeRef;
        private static TypeDefinition _messagePackWriter_typeDef;
        private static TypeDefinition _messagePackReader_typeDef;
        private static TypeDefinition _messagePackSerializerOptions_typeDef;
        private static TypeDefinition _messagePackSecurity_typeDef;
        private static TypeReference _iFormatterResolver_typeRef;
        private static MethodReference _writeNil_methodDef;
        private static MethodReference _tryReadNil_methodRef;
        private static MethodReference _writeArrayHeader_methodRef;
        private static MethodReference _writeMapHeader_methodRef;
        private static MethodReference _getResolver_methodDef;
        private static MethodReference _getFormatterWithVerify_methodDef;
        private static TypeDefinition _iMessagePackFormatter_typeDef;
        private static TypeReference _iMessagePackFormatter_typeRef;
        private static TypeDefinition _iMessagePackFormatter1_typeDef;
        private static TypeReference _iMessagePackFormatter1_typeRef;
        private static MethodReference _serialize_methodDef;
        private static MethodReference _deserialize_methodDef;
        private static MethodReference _invalidOperationException_ctor_methodRef;
        private static MethodReference _getSecurity_methodRef;
        private static MethodReference _depthStep_methodRef;
        private static MethodReference _readArrayHeader_methodRef;
        private static MethodReference _readMapHeader_methodRef;
        private static MethodReference _getDepth_methodRef;
        private static MethodReference _setDepth_methodRef;
        private static MethodReference _skip_methodRef;
        private static TypeDefinition _generatedResolverGetFormatterHelper_typeDef;
        private static MethodDefinition _generatedResolverGetFormatterHelper_cctor_methodDef;
        private static MethodReference _automata_add_methodRef;
        private static MethodReference _getEncodedStringBytes_methodRef;
        private static MethodReference _byteOpImplicit_methodRef;
        private static MethodReference _writeRaw_methodRef;
        private static MethodReference _readStringSpan_methodRef;
        private static MethodReference _automata_tryGetValue_methodRef;
        private static Dictionary<string, MethodReference> _writeMethods;
        private static Dictionary<string, MethodReference> _readMethods;
        private static TypeReference _object_typeRef;
        private static TypeReference _type_typeRef;
        private static MethodReference _getTypeFromHandle_methodRef;
        private static MethodDefinition _dictionary_add_methodDef;

        internal static void Process(ModuleDefinition mainModule, ModuleDefinition baseModule, List<TypeDefinition> messagePackObjectTypes, List<TypeDefinition> unionTypes)
        {
            _pretiaModule = mainModule.AssemblyResolver.Resolve(AssemblyNameReference.Parse("PretiaSDK.Runtime")).MainModule;
            _msgPackModule = mainModule.AssemblyResolver.Resolve(AssemblyNameReference.Parse("PretiaSDK.MessagePack")).MainModule;

            _int32_typeRef = mainModule.ImportReference(
                baseModule.Types.FirstOrDefault(t => t.Name == "Int32")
            );
            _uint8_typeRef = mainModule.ImportReference(
                baseModule.Types.FirstOrDefault(t => t.Name == "Byte")
            );
            _object_typeRef = mainModule.ImportReference(baseModule.GetType("System.Object"));
            _type_typeRef = mainModule.ImportReference(baseModule.GetType("System.Type"));

            _getTypeFromHandle_methodRef = mainModule.ImportReference(
                    baseModule.Types
                    .FirstOrDefault(t => t.Name == "Type")
                    .Methods
                    .FirstOrDefault(m => m.Name == "GetTypeFromHandle")
                );

            _dictionary_add_methodDef = baseModule.Types
                .FirstOrDefault(t => t.Name == "Dictionary`2")
                .Methods
                .FirstOrDefault(m => m.Name == "Add");

            _messagePackWriter_typeDef = _msgPackModule.Types.FirstOrDefault(t => t.Name == "MessagePackWriter");
            _messagePackReader_typeDef = _msgPackModule.Types.FirstOrDefault(t => t.Name == "MessagePackReader");
            _messagePackSerializerOptions_typeDef = _msgPackModule.Types.FirstOrDefault(t => t.Name == "MessagePackSerializerOptions");
            _messagePackSecurity_typeDef = _msgPackModule.Types.FirstOrDefault(t => t.Name == "MessagePackSecurity");

            _iFormatterResolver_typeRef = mainModule.ImportReference(
                _msgPackModule.Types.FirstOrDefault(t => t.Name == "IFormatterResolver")
            );
            _writeNil_methodDef = mainModule.ImportReference(
                _messagePackWriter_typeDef.Methods.FirstOrDefault(m => m.Name == "WriteNil")
            );
            _tryReadNil_methodRef = mainModule.ImportReference(
                _messagePackReader_typeDef.Methods.FirstOrDefault(m => m.Name == "TryReadNil")
            );
            _writeArrayHeader_methodRef = mainModule.ImportReference(
                _messagePackWriter_typeDef.Methods.FirstOrDefault(m => m.Name == "WriteArrayHeader")
            );
            _writeMapHeader_methodRef = mainModule.ImportReference(
                _messagePackWriter_typeDef.Methods.FirstOrDefault(m => m.Name == "WriteMapHeader")
            );
            _getResolver_methodDef = mainModule.ImportReference(
                _msgPackModule.Types.FirstOrDefault(t => t.Name == "MessagePackSerializerOptions")
                .Methods.FirstOrDefault(m => m.Name == "get_Resolver")
            );
            _getFormatterWithVerify_methodDef = mainModule.ImportReference(
                _msgPackModule.Types.FirstOrDefault(t => t.Name == "FormatterResolverExtensions")
                .Methods.FirstOrDefault(m => m.Name == "GetFormatterWithVerify")
            );
            _iMessagePackFormatter_typeDef = _msgPackModule.Types.FirstOrDefault(t => t.Name == "IMessagePackFormatter");
            _iMessagePackFormatter_typeRef = mainModule.ImportReference(_iMessagePackFormatter_typeDef);
            _iMessagePackFormatter1_typeDef = _msgPackModule.Types.FirstOrDefault(t => t.Name == "IMessagePackFormatter`1");
            _iMessagePackFormatter1_typeRef = mainModule.ImportReference(_iMessagePackFormatter1_typeDef);
            _serialize_methodDef = mainModule.ImportReference(
                _iMessagePackFormatter1_typeDef.Methods.FirstOrDefault(m => m.Name == "Serialize")
            );
            _deserialize_methodDef = mainModule.ImportReference(
                _iMessagePackFormatter1_typeDef.Methods.FirstOrDefault(m => m.Name == "Deserialize")
            );
            _invalidOperationException_ctor_methodRef = mainModule.ImportReference(
                baseModule.Types.FirstOrDefault(t => t.Name == "InvalidOperationException")
                .Methods.FirstOrDefault(m => m.Name == ".ctor" && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.Name == "String")
            );
            _getSecurity_methodRef = mainModule.ImportReference(
                _messagePackSerializerOptions_typeDef.Methods.FirstOrDefault(m => m.Name == "get_Security")
            );
            _depthStep_methodRef = mainModule.ImportReference(
                _messagePackSecurity_typeDef.Methods.FirstOrDefault(m => m.Name == "DepthStep")
            );
            _readArrayHeader_methodRef = mainModule.ImportReference(
                _messagePackReader_typeDef.Methods.FirstOrDefault(m => m.Name == "ReadArrayHeader")
            );
            _readMapHeader_methodRef = mainModule.ImportReference(
                _messagePackReader_typeDef.Methods.FirstOrDefault(m => m.Name == "ReadMapHeader")
            );
            _getDepth_methodRef = mainModule.ImportReference(
                _messagePackReader_typeDef.Methods.FirstOrDefault(m => m.Name == "get_Depth")
            );
            _setDepth_methodRef = mainModule.ImportReference(
                _messagePackReader_typeDef.Methods.FirstOrDefault(m => m.Name == "set_Depth")
            );
            _skip_methodRef = mainModule.ImportReference(
                _messagePackReader_typeDef.Methods.FirstOrDefault(m => m.Name == "Skip")
            );
            _automata_add_methodRef = mainModule.ImportReference(
                _msgPackModule.Types.FirstOrDefault(t => t.Name == "AutomataDictionary")
                .Methods.FirstOrDefault(m => m.Name == "Add")
            );
            _getEncodedStringBytes_methodRef = mainModule.ImportReference(
                _msgPackModule.Types.FirstOrDefault(t => t.Name == "CodeGenHelpers")
                .Methods.FirstOrDefault(m => m.Name == "GetEncodedStringBytes")
            );
            _byteOpImplicit_methodRef = GetByteSpanOpImplicitMethodRef(mainModule);
            _writeRaw_methodRef = mainModule.ImportReference(
                _messagePackWriter_typeDef.Methods.FirstOrDefault(m => m.Name == "WriteRaw")
            );
            _readStringSpan_methodRef = mainModule.ImportReference(
                _msgPackModule.Types.FirstOrDefault(t => t.Name == "CodeGenHelpers")
                .Methods.FirstOrDefault(m => m.Name == "ReadStringSpan")
            );
            _automata_tryGetValue_methodRef = mainModule.ImportReference(
                _msgPackModule.Types.FirstOrDefault(t => t.Name == "AutomataDictionary")
                .Methods.LastOrDefault(m => m.Name == "TryGetValue")
            );

            _writeMethods = GetWriteMethods(mainModule, _messagePackWriter_typeDef);
            _readMethods = GetReadMethods(mainModule, _messagePackReader_typeDef);

            var intIndexedFormatterTemplate_typeDef = GetIntIndexedFormatterTemplate(_pretiaModule);
            var stringIndexedFormatterTemplate_typeDef = GetStringIndexedFormatterTemplate(_pretiaModule);
            var unionFormatterTemplate_typeDef = GetUnionFormatterTemplate(_pretiaModule);

            var initialize_methodDef = mainModule.GeneratedClass().AddMethod("__GENERATED__InitFormatterResolvers", MethodAttributes.Static | MethodAttributes.Public);
            initialize_methodDef.AddRuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType.BeforeSceneLoad);

            var initialize_instructions = initialize_methodDef.Body.Instructions;
            var initialize_iLProcessor = initialize_methodDef.Body.GetILProcessor();

            for (int i = 0; i < messagePackObjectTypes.Count; i++)
            {
                var messagePackObject_typeDef = messagePackObjectTypes[i];

                TypeDefinition formatter_typeDef = null;
                if (IsIntIndexed(messagePackObject_typeDef))
                {
                    formatter_typeDef = GenerateIntIndexedFormatterClass(mainModule, messagePackObject_typeDef, mainModule.ImportReference(intIndexedFormatterTemplate_typeDef));
                }
                else
                {
                    formatter_typeDef = GenerateStringIndexedFormatterClass(mainModule, messagePackObject_typeDef, stringIndexedFormatterTemplate_typeDef);
                }

                mainModule.Types.Add(formatter_typeDef);
                AddFormatterToGeneratedResolver(mainModule, baseModule, messagePackObject_typeDef, formatter_typeDef, initialize_instructions, initialize_iLProcessor);
            }

            for (int i = 0; i < unionTypes.Count; i++)
            {
                var union_typeDef = unionTypes[i];

                var formatter_typeDef = GenerateUnionFormatterClass(mainModule, union_typeDef, unionFormatterTemplate_typeDef);
                mainModule.Types.Add(formatter_typeDef);
                AddFormatterToGeneratedResolver(mainModule, baseModule, union_typeDef, formatter_typeDef, initialize_instructions, initialize_iLProcessor);
            }

            initialize_iLProcessor.Emit(OpCodes.Ret);
        }

        private static TypeDefinition GenerateUnionFormatterClass(ModuleDefinition mainModule, TypeDefinition union_typeDef, TypeDefinition formatterTemplate_typeDef)
        {
            TypeReference union_typeRef = mainModule.GetType(union_typeDef.FullName);
            if (union_typeRef == null)
            {
                union_typeRef = mainModule.ImportReference(union_typeDef);
            }

            var formatter_typeDef = SetupFormatterClass(mainModule, union_typeRef, formatterTemplate_typeDef);
            FieldDefinition typeToKey_fieldDef = formatter_typeDef.Fields.FirstOrDefault(f => f.Name == "typeToKey");
            foreach (var template_fieldDef in formatterTemplate_typeDef.Fields)
            {
                var fieldDef = new FieldDefinition(template_fieldDef.Name, template_fieldDef.Attributes, mainModule.ImportReference(template_fieldDef.FieldType));
                formatter_typeDef.Fields.Add(fieldDef);
                if (fieldDef.Name == "typeToKey")
                {
                    typeToKey_fieldDef = fieldDef;
                }
            }

            var (unionTypes, keys) = GetSerializableUnionTypes(union_typeDef);
            MethodDefinition serializeInternal_methodDef = null;
            MethodDefinition deserializeInternal_methodDef = null;
            foreach (var template_methodDef in formatterTemplate_typeDef.Methods)
            {
                MethodDefinition methodDef = null;
                if (template_methodDef.Name == ".ctor")
                {
                    methodDef = GenerateUnionConstructor(mainModule, template_methodDef, union_typeRef, unionTypes, keys, typeToKey_fieldDef);
                }
                else if (template_methodDef.Name == "SerializeInternal")
                {
                    methodDef = GenerateUnionSerializeInternalMethod(mainModule, template_methodDef, union_typeRef, unionTypes, keys);
                    serializeInternal_methodDef = methodDef;
                }
                else if (template_methodDef.Name == "DeserializeInternal")
                {
                    methodDef = GenerateUnionDeserializeInternalMethod(mainModule, template_methodDef, union_typeRef, unionTypes, keys);
                    deserializeInternal_methodDef = methodDef;
                }
                else if (template_methodDef.Name == "Deserialize")
                {
                    continue;
                }
                else
                {
                    methodDef = GenerateOtherMethod(mainModule, template_methodDef);
                }

                MethodBodyRocks.Optimize(methodDef.Body);
                formatter_typeDef.Methods.Add(methodDef);
            }

            var serialize_methodDef = formatter_typeDef.Methods.FirstOrDefault(m => m.Name == "Serialize");
            serialize_methodDef.Parameters[1] = new ParameterDefinition(serialize_methodDef.Parameters[1].Name, serialize_methodDef.Parameters[1].Attributes, mainModule.ImportReference(union_typeRef));
            var call_serializeInternal_instruction = serialize_methodDef.Body.Instructions.LastOrDefault(i => i.OpCode == OpCodes.Ldloc_0).Next;
            call_serializeInternal_instruction.Operand = serializeInternal_methodDef;
            var loadField_isntruction = serialize_methodDef.Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldfld);
            loadField_isntruction.Operand = typeToKey_fieldDef;

            var deserialize_methodDef = GenerateUnionDeserializeMethod(mainModule, formatterTemplate_typeDef.Methods.FirstOrDefault(m => m.Name == "Deserialize"), deserializeInternal_methodDef, union_typeRef, unionTypes, keys);
            formatter_typeDef.Methods.Add(deserialize_methodDef);

            return formatter_typeDef;
        }

        private static MethodDefinition GenerateUnionDeserializeMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef, MethodDefinition deserializeInternal_methodDef, TypeReference union_typeRef, TypeReference[] unionTypes, int[] keys)
        {
            union_typeRef = mainModule.ImportReference(union_typeRef);
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, union_typeRef);
            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
            }

            var iLProcessor = methodDef.Body.GetILProcessor();
            var instructions = methodDef.Body.Instructions;

            methodDef.Body.Variables.Add(new VariableDefinition(_int32_typeRef));
            methodDef.Body.Variables.Add(new VariableDefinition(_int32_typeRef));

            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Call, _tryReadNil_methodRef);
            var readArrayHeader_instruction = iLProcessor.Create(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Brfalse, readArrayHeader_instruction);

            iLProcessor.Emit(OpCodes.Ldnull);
            iLProcessor.Emit(OpCodes.Ret);

            iLProcessor.Append(readArrayHeader_instruction);
            iLProcessor.Emit(OpCodes.Call, _readArrayHeader_methodRef);
            iLProcessor.Emit(OpCodes.Ldc_I4_2);
            var depthStep_instruction = iLProcessor.Create(OpCodes.Ldarg_2);
            iLProcessor.Emit(OpCodes.Beq, depthStep_instruction);

            iLProcessor.Emit(OpCodes.Ldstr, $"Invalid Union data was detected. {union_typeRef.FullName}");
            iLProcessor.Emit(OpCodes.Newobj, _invalidOperationException_ctor_methodRef);
            iLProcessor.Emit(OpCodes.Throw);

            iLProcessor.Append(depthStep_instruction);
            iLProcessor.Emit(OpCodes.Callvirt, _getSecurity_methodRef);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, _depthStep_methodRef);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Call, _readMethods["System.Int32"]);
            iLProcessor.Emit(OpCodes.Stloc_0);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Ldarg_2);
            iLProcessor.Emit(OpCodes.Ldloc_0);
            iLProcessor.Emit(OpCodes.Call, deserializeInternal_methodDef);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Dup);
            iLProcessor.Emit(OpCodes.Call, _getDepth_methodRef); ;
            iLProcessor.Emit(OpCodes.Stloc_1);
            iLProcessor.Emit(OpCodes.Ldloc_1);
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Sub);
            iLProcessor.Emit(OpCodes.Call, _setDepth_methodRef); ;
            iLProcessor.Emit(OpCodes.Ret);

            return methodDef;
        }

        private static (TypeReference[] types, int[] keys) GetSerializableUnionTypes(TypeDefinition union_typeDef)
        {
            var unionAttrs =
                from attr in union_typeDef.CustomAttributes
                where attr.AttributeType.Name == "UnionAttribute"
                orderby (int)attr.ConstructorArguments[0].Value
                select attr;

            var keys =
                from attr in unionAttrs
                select (int)attr.ConstructorArguments[0].Value;

            var distinct = new HashSet<int>(keys);
            if (distinct.Count != keys.Count())
            {
                throw new Exception($"UnionAttribute requires unique keys for each type. Found duplicate keys in type {union_typeDef.FullName}");
            }

            var unionTypes =
                from attr in unionAttrs
                select (TypeReference)attr.ConstructorArguments[1].Value;

            return (unionTypes.ToArray(), keys.ToArray());
        }

        private static MethodDefinition GenerateUnionConstructor(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference union_typeRef, TypeReference[] unionTypes, int[] keys, FieldDefinition typeToKey_fieldDef)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, template_methodDef.ReturnType);

            var iLProcessor = methodDef.Body.GetILProcessor();
            var instructions = methodDef.Body.Instructions;
            foreach (var instruction in template_methodDef.Body.Instructions)
            {
                switch (instruction.Operand)
                {
                    case TypeReference typeRef:
                    iLProcessor.Emit(instruction.OpCode, mainModule.ImportReference(typeRef));
                    break;

                    case MethodReference methodRef:
                    iLProcessor.Emit(instruction.OpCode, mainModule.ImportReference(methodRef));
                    break;

                    case FieldDefinition fieldDef:
                    iLProcessor.Emit(instruction.OpCode, typeToKey_fieldDef);
                    break;

                    default:
                    iLProcessor.Append(instruction);
                    break;
                }

            }

            var insertPoint = instructions.LastOrDefault(instruction => instruction.OpCode == OpCodes.Stfld);
            insertPoint.Operand = typeToKey_fieldDef;

            for (int i = 0; i < keys.Length; i++)
            {
                var type_int32_genericInstanceType = _dictionary_add_methodDef.DeclaringType.MakeGenericInstanceType(_type_typeRef, _int32_typeRef);
                var dictionary_add_methodRef = mainModule.ImportReference(new MethodReference(_dictionary_add_methodDef.Name, _dictionary_add_methodDef.ReturnType, type_int32_genericInstanceType));
                dictionary_add_methodRef.HasThis = true;
                foreach (var parameter in _dictionary_add_methodDef.Parameters)
                {
                    dictionary_add_methodRef.Parameters.Add(parameter);
                }

                iLProcessor.InsertBefore(insertPoint, Instruction.Create(OpCodes.Dup));
                iLProcessor.InsertBefore(insertPoint, Instruction.Create(OpCodes.Ldtoken, mainModule.ImportReference(unionTypes[i])));
                iLProcessor.InsertBefore(insertPoint, Instruction.Create(OpCodes.Call, _getTypeFromHandle_methodRef));
                iLProcessor.InsertBefore(insertPoint, iLProcessor.CreateLoadIntegerInstruction(keys[i]));
                iLProcessor.InsertBefore(insertPoint, Instruction.Create(OpCodes.Callvirt, dictionary_add_methodRef));
            }

            return methodDef;
        }

        private static MethodDefinition GenerateUnionSerializeInternalMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference union_typeRef, TypeReference[] unionTypes, int[] keys)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, template_methodDef.ReturnType);
            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                if (i == 1)
                {
                    methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, union_typeRef));
                }
                else
                {
                    methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
                }
            }

            var iLProcessor = methodDef.Body.GetILProcessor();
            var instructions = methodDef.Body.Instructions;
            Instruction[] jumpTargets = new Instruction[keys.Last() + 1];

            methodDef.Body.Variables.Add(new VariableDefinition(_int32_typeRef));
            iLProcessor.Emit(OpCodes.Ldarg_S, (byte)4);
            var switchStart_instruction = iLProcessor.Create(OpCodes.Stloc_0);
            iLProcessor.Append(switchStart_instruction);
            iLProcessor.Emit(OpCodes.Ret);

            for (int i = 0; i < unionTypes.Length; i++)
            {
                var unionType_typeRef = mainModule.ImportReference(unionTypes[i]);

                var jumpTarget = iLProcessor.Create(OpCodes.Ldarg_3);
                jumpTargets[keys[i]] = jumpTarget;
                iLProcessor.Append(jumpTarget);
                iLProcessor.Emit(OpCodes.Callvirt, _getResolver_methodDef);
                var getFormatterWithVerify_genericInstanceMethod = new GenericInstanceMethod(_getFormatterWithVerify_methodDef);
                getFormatterWithVerify_genericInstanceMethod.GenericArguments.Add(unionType_typeRef);
                iLProcessor.Emit(OpCodes.Call, getFormatterWithVerify_genericInstanceMethod);

                iLProcessor.Emit(OpCodes.Ldarg_1);
                iLProcessor.Emit(OpCodes.Ldarg_2);
                iLProcessor.Emit(OpCodes.Castclass, unionType_typeRef);
                iLProcessor.Emit(OpCodes.Ldarg_3);

                var iMessagePackFormatter_typeRef = mainModule.ImportReference(
                    _serialize_methodDef.DeclaringType.MakeGenericInstanceType(unionType_typeRef)
                );
                var serialize_methodRef = new MethodReference(_serialize_methodDef.Name, _serialize_methodDef.ReturnType, iMessagePackFormatter_typeRef);
                serialize_methodRef.HasThis = true;
                foreach (var parameter in _serialize_methodDef.Parameters)
                {
                    serialize_methodRef.Parameters.Add(parameter);
                }

                iLProcessor.Emit(OpCodes.Callvirt, serialize_methodRef);
                iLProcessor.Emit(OpCodes.Ret);
            }

            if (unionTypes.Length > 0)
            {
                var indexSequences = GetIndexSequences(keys);
                var switchSequence = MakeTree(indexSequences);
                Instruction defaultInstruction = null;

                EmitSwitchStatement(switchSequence,
                                    ref jumpTargets,
                                    ref iLProcessor,
                                    ref switchStart_instruction,
                                    ref defaultInstruction,
                                    0);
            }

            return methodDef;
        }

        private static MethodDefinition GenerateUnionDeserializeInternalMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference union_typeRef, TypeReference[] unionTypes, int[] keys)
        {
            union_typeRef = mainModule.ImportReference(union_typeRef);
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, union_typeRef);
            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
            }

            var iLProcessor = methodDef.Body.GetILProcessor();
            var instructions = methodDef.Body.Instructions;
            Instruction[] jumpTargets = new Instruction[keys.Last() + 1];

            methodDef.Body.Variables.Add(new VariableDefinition(union_typeRef));
            methodDef.Body.Variables.Add(new VariableDefinition(_int32_typeRef));

            iLProcessor.Emit(OpCodes.Ldnull);
            iLProcessor.Emit(OpCodes.Stloc_0);
            iLProcessor.Emit(OpCodes.Ldarg_3);
            var switchStart_instruction = iLProcessor.Create(OpCodes.Stloc_1);
            iLProcessor.Append(switchStart_instruction);

            var skip_instruction = iLProcessor.Create(OpCodes.Ldarg_1);
            var return_instruction = iLProcessor.Create(OpCodes.Ldloc_0);

            for (int i = 0; i < unionTypes.Length; i++)
            {
                var unionType_typeRef = mainModule.ImportReference(unionTypes[i]);

                var jumpTarget = iLProcessor.Create(OpCodes.Ldarg_2);
                jumpTargets[keys[i]] = jumpTarget;
                iLProcessor.Append(jumpTarget);
                iLProcessor.Emit(OpCodes.Callvirt, _getResolver_methodDef);
                var getFormatterWithVerify_genericInstanceMethod = new GenericInstanceMethod(_getFormatterWithVerify_methodDef);
                getFormatterWithVerify_genericInstanceMethod.GenericArguments.Add(unionType_typeRef);
                iLProcessor.Emit(OpCodes.Call, getFormatterWithVerify_genericInstanceMethod);

                iLProcessor.Emit(OpCodes.Ldarg_1);
                iLProcessor.Emit(OpCodes.Ldarg_2);

                var iMessagePackFormatter_typeRef = mainModule.ImportReference(
                    _deserialize_methodDef.DeclaringType.MakeGenericInstanceType(unionType_typeRef)
                );
                var deserialize_methodRef = new MethodReference(_deserialize_methodDef.Name, _deserialize_methodDef.ReturnType, iMessagePackFormatter_typeRef);
                deserialize_methodRef.HasThis = true;
                foreach (var parameter in _deserialize_methodDef.Parameters)
                {
                    deserialize_methodRef.Parameters.Add(parameter);
                }

                iLProcessor.Emit(OpCodes.Callvirt, deserialize_methodRef);
                iLProcessor.Emit(OpCodes.Castclass, unionType_typeRef);
                iLProcessor.Emit(OpCodes.Stloc_0);
                iLProcessor.Emit(OpCodes.Br, return_instruction);
            }

            iLProcessor.Append(skip_instruction);
            iLProcessor.Emit(OpCodes.Call, _skip_methodRef);

            iLProcessor.Append(return_instruction);
            iLProcessor.Emit(OpCodes.Ret);

            if (unionTypes.Length > 0)
            {
                var indexSequences = GetIndexSequences(keys);
                var switchSequence = MakeTree(indexSequences);

                EmitSwitchStatement(switchSequence,
                                    ref jumpTargets,
                                    ref iLProcessor,
                                    ref switchStart_instruction,
                                    ref skip_instruction,
                                    1);
            }

            return methodDef;
        }

        private static bool IsUnion(TypeDefinition messagePackObject_typeDef)
        {
            var unionAttrs =
                from attr in messagePackObject_typeDef.CustomAttributes
                where attr.AttributeType.Name == "UnionAttribute"
                select attr;

            int count = unionAttrs.Count();
            bool result = false;

            if (count > 0)
            {
                var attrKeys =
                    from attr in unionAttrs
                    select (int)attr.ConstructorArguments[0].Value;

                var distinct = new HashSet<int>(attrKeys);
                if (distinct.Count != unionAttrs.Count())
                {
                    throw new Exception($"UnionAttribute requires unique key for each type. Found duplicate key for type {messagePackObject_typeDef.FullName}");
                }

                result = true;
            }

            return result;
        }

        private static MethodReference GetByteSpanOpImplicitMethodRef(ModuleDefinition mainModule)
        {
            var memoryModule = mainModule.AssemblyResolver.Resolve(AssemblyNameReference.Parse("System.Memory")).MainModule;
            var opImplicit_methodDef = memoryModule.Types.FirstOrDefault(t => t.Name == "ReadOnlySpan`1").Methods.FirstOrDefault(m => m.Name == "op_Implicit");

            var byteSpan_genericInstanceType = opImplicit_methodDef.DeclaringType.MakeGenericInstanceType(_uint8_typeRef);
            var method = new MethodReference(opImplicit_methodDef.Name, opImplicit_methodDef.ReturnType, byteSpan_genericInstanceType);
            foreach (var parameter in opImplicit_methodDef.Parameters)
            {
                method.Parameters.Add(parameter);
            }

            return mainModule.ImportReference(method);
        }

        private static TypeDefinition GetUnionFormatterTemplate(ModuleDefinition module)
        {
            return module.Types.FirstOrDefault(t => t.Name == "UnionFormatterTemplate");
        }

        private static TypeDefinition GetStringIndexedFormatterTemplate(ModuleDefinition module)
        {
            return module.Types.FirstOrDefault(t => t.Name == "StringIndexedFormatterTemplate");
        }

        private static bool IsIntIndexed(TypeDefinition messagePackObject_typeDef)
        {
            if (messagePackObject_typeDef.HasFields || messagePackObject_typeDef.HasProperties)
            {
                IMemberDefinition memberDef;
                if (messagePackObject_typeDef.HasFields)
                {
                    memberDef = messagePackObject_typeDef.Fields[0];
                }
                else
                {
                    memberDef = messagePackObject_typeDef.Properties[0];
                }

                var attr = memberDef.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == KEY_ATTRIBUTE_NAME).ConstructorArguments[0];
                return attr.Type.Name == "Int32";
            }
            else
            {
                return true;
            }
        }

        private static void AddFormatterToGetFormatterInternal(ModuleDefinition mainModule, TypeDefinition formatter_typeDef, ILProcessor iLProcessor, ref Instruction jumpTarget)
        {
            var formatter_ctor_methodDef = formatter_typeDef.Methods.FirstOrDefault(m => m.Name == ".ctor");
            jumpTarget = iLProcessor.Create(OpCodes.Newobj, formatter_ctor_methodDef);
            iLProcessor.Append(jumpTarget);
            iLProcessor.Emit(OpCodes.Ret);
        }

        private static void AddFormatterToGeneratedResolver(ModuleDefinition mainModule,
                                                            ModuleDefinition baseModule,
                                                            TypeDefinition messagePackObject_typeDef,
                                                            TypeDefinition formatter_typeDef,
                                                            Collection<Instruction> instructions,
                                                            ILProcessor iLProcessor)
        {
            TypeReference messagePackObject_typeRef = mainModule.GetType(messagePackObject_typeDef.FullName);
            if (messagePackObject_typeRef == null)
            {
                messagePackObject_typeRef = mainModule.ImportReference(messagePackObject_typeDef);
            }

            var formatter_ctor_methodDef = formatter_typeDef.Methods.FirstOrDefault(m => m.Name == ".ctor");

            var generatedResolver_typeDef = _pretiaModule.Types.FirstOrDefault(t => t.Name == "FormatterResolver");
            var add_methodRef = mainModule.ImportReference(generatedResolver_typeDef.Methods.FirstOrDefault(m => m.Name == "Add"));

            var insertPoint = instructions.LastOrDefault(instruction => instruction.OpCode == OpCodes.Stsfld);

            iLProcessor.Emit(OpCodes.Ldtoken, messagePackObject_typeRef);
            iLProcessor.Emit(OpCodes.Call, _getTypeFromHandle_methodRef);
            iLProcessor.Emit(OpCodes.Newobj, formatter_ctor_methodDef);
            iLProcessor.Emit(OpCodes.Call, add_methodRef);
        }

        private static Dictionary<string, MethodReference> GetWriteMethods(ModuleDefinition mainModule, TypeDefinition messagePackWriter_typeDef)
        {
            var methodDictionary = new Dictionary<string, MethodReference>();
            var writeMethods = messagePackWriter_typeDef.Methods.Where(m => m.Name == "Write");

            foreach (var methodDef in writeMethods)
            {
                string parameterTypeName = methodDef.Parameters[0].ParameterType.FullName;
                methodDictionary.Add(parameterTypeName, mainModule.ImportReference(methodDef));
            }

            return methodDictionary;
        }

        private static Dictionary<string, MethodReference> GetReadMethods(ModuleDefinition mainModule, TypeDefinition messagePackReader_typeDef)
        {
            var methodDictionary = new Dictionary<string, MethodReference>();

            foreach (var primitiveType in PRIMITIVE_TYPES_DEFINITION)
            {
                var readMethod = messagePackReader_typeDef.Methods.FirstOrDefault(m => m.Name == $"Read{primitiveType}");
                string returnTypeName = readMethod.ReturnType.FullName;
                methodDictionary.Add(returnTypeName, mainModule.ImportReference(readMethod));
            }

            return methodDictionary;
        }

        private static TypeDefinition GetIntIndexedFormatterTemplate(ModuleDefinition module)
        {
            return module.Types.FirstOrDefault(t => t.Name == "ObjectFormatterTemplate");
        }

        private static TypeDefinition SetupFormatterClass(ModuleDefinition mainModule, TypeReference messagePackObject_typeRef, TypeDefinition formatterTemplate_typeDef)
        {
            string formatter_className = messagePackObject_typeRef.FullName;
            int lastDotCharacterIndex = formatter_className.LastIndexOf('.');
            if (lastDotCharacterIndex != -1)
            {
                formatter_className = formatter_className.Substring(lastDotCharacterIndex + 1);
            }
            formatter_className = formatter_className.Replace('/', '_');

            var formatter_typeDef = new TypeDefinition(formatterTemplate_typeDef.Namespace, $"{formatter_className}Formatter", formatterTemplate_typeDef.Attributes);
            formatter_typeDef.BaseType = formatterTemplate_typeDef.BaseType;

            var formatterResolverInterface = new InterfaceImplementation(_iMessagePackFormatter_typeRef);
            formatter_typeDef.Interfaces.Add(formatterResolverInterface);

            var genericFormatterResolverInterface = new InterfaceImplementation(_iMessagePackFormatter1_typeRef);
            var genericInstanceFormatterResolverInterface = new GenericInstanceType(genericFormatterResolverInterface.InterfaceType.GetElementType());
            genericInstanceFormatterResolverInterface.GenericArguments.Add(messagePackObject_typeRef);
            formatter_typeDef.Interfaces.Add(new InterfaceImplementation(genericInstanceFormatterResolverInterface));

            return formatter_typeDef;
        }

        private static TypeDefinition GenerateIntIndexedFormatterClass(ModuleDefinition mainModule, TypeDefinition messagePackObject_typeDef, TypeReference formatterTemplate_typeRef)
        {
            TypeReference messagePackObject_typeRef = mainModule.GetType(messagePackObject_typeDef.FullName);
            if (messagePackObject_typeRef == null)
            {
                messagePackObject_typeRef = mainModule.ImportReference(messagePackObject_typeDef);
            }

            var formatterTemplate_typeDef = formatterTemplate_typeRef.Resolve();
            var formatter_typeDef = SetupFormatterClass(mainModule, messagePackObject_typeRef, formatterTemplate_typeDef);

            var serializableMembers = GetSerializableMembers(mainModule, messagePackObject_typeRef);
            foreach (var template_methodDef in formatterTemplate_typeDef.Methods)
            {
                MethodDefinition methodDef = null;
                if (template_methodDef.Name == "Serialize")
                {
                    methodDef = GenerateSerializeMethod(mainModule, template_methodDef, messagePackObject_typeRef, serializableMembers);
                }
                else if (template_methodDef.Name == "Deserialize")
                {
                    methodDef = GenerateDeserializeMethod(mainModule, template_methodDef, messagePackObject_typeRef, serializableMembers);
                }
                else
                {
                    methodDef = GenerateOtherMethod(mainModule, template_methodDef);
                }

                MethodBodyRocks.Optimize(methodDef.Body);
                formatter_typeDef.Methods.Add(methodDef);
            }

            return formatter_typeDef;
        }

        private static TypeDefinition GenerateStringIndexedFormatterClass(ModuleDefinition mainModule, TypeDefinition messagePackObject_typeDef, TypeDefinition formatterTemplate_typeDef)
        {
            TypeReference messagePackObject_typeRef = mainModule.GetType(messagePackObject_typeDef.FullName);
            if (messagePackObject_typeRef == null)
            {
                messagePackObject_typeRef = mainModule.ImportReference(messagePackObject_typeDef);
            }

            var formatter_typeDef = SetupFormatterClass(mainModule, messagePackObject_typeRef, formatterTemplate_typeDef);
            FieldDefinition stringByteKeys_fieldDef = null;
            FieldDefinition keyMapping_fieldDef = null;
            foreach (var template_fieldDef in formatterTemplate_typeDef.Fields)
            {
                var fieldDef = new FieldDefinition(template_fieldDef.Name, template_fieldDef.Attributes, mainModule.ImportReference(template_fieldDef.FieldType));
                formatter_typeDef.Fields.Add(fieldDef);
                if (fieldDef.Name == "____stringByteKeys")
                {
                    stringByteKeys_fieldDef = fieldDef;
                }
                else
                {
                    keyMapping_fieldDef = fieldDef;
                }
            }

            var serializableMembers = GetSerializableMembers(mainModule, messagePackObject_typeRef, IsIntIndexed: false);
            foreach (var template_methodDef in formatterTemplate_typeDef.Methods)
            {
                MethodDefinition methodDef = null;

                if (template_methodDef.Name == ".ctor")
                {
                    methodDef = GenerateStringIndexedConstructor(mainModule, template_methodDef, messagePackObject_typeRef, serializableMembers, keyMapping_fieldDef, stringByteKeys_fieldDef);
                }
                else if (template_methodDef.Name == "Serialize")
                {
                    methodDef = GenerateStringIndexedSerializeMethod(mainModule, template_methodDef, messagePackObject_typeRef, serializableMembers, stringByteKeys_fieldDef);
                }
                else if (template_methodDef.Name == "Deserialize")
                {
                    methodDef = GenerateStringIndexedDeserializeMethod(mainModule, template_methodDef, messagePackObject_typeRef, serializableMembers, keyMapping_fieldDef);
                }

                MethodBodyRocks.Optimize(methodDef.Body);
                formatter_typeDef.Methods.Add(methodDef);
            }

            return formatter_typeDef;
        }

        private static MethodDefinition GenerateStringIndexedConstructor(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference messagePackObject_typeRef, SerializableMembersDefinition serializableMembers, FieldDefinition keyMapping_fieldDef, FieldDefinition stringByteKeys_fieldDef)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, template_methodDef.ReturnType);

            var ilProcessor = methodDef.Body.GetILProcessor();
            foreach (var instruction in template_methodDef.Body.Instructions)
            {
                switch (instruction.Operand)
                {
                    case TypeReference typeRef:
                    ilProcessor.Emit(instruction.OpCode, mainModule.ImportReference(typeRef));
                    break;

                    case MethodReference methodRef:
                    ilProcessor.Emit(instruction.OpCode, mainModule.ImportReference(methodRef));
                    break;

                    case FieldDefinition fieldDef:
                    ilProcessor.Emit(instruction.OpCode, keyMapping_fieldDef);
                    break;

                    default:
                    ilProcessor.Append(instruction);
                    break;
                }
            }

            var ret_instruction = ilProcessor.Body.Instructions.Last();
            var storeStringByteKeys_instruction = ilProcessor.Create(OpCodes.Stfld, stringByteKeys_fieldDef);
            ilProcessor.InsertBefore(ret_instruction, ilProcessor.Create(OpCodes.Ldarg_0));
            ilProcessor.InsertBefore(ret_instruction, ilProcessor.CreateLoadIntegerInstruction(serializableMembers.Members.Length));
            ilProcessor.InsertBefore(ret_instruction, ilProcessor.Create(OpCodes.Newarr, _uint8_typeRef.MakeArrayType()));
            ilProcessor.InsertBefore(ret_instruction, storeStringByteKeys_instruction);

            var storeKeyMapping_instruction = methodDef.Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Stfld);
            var storeKeyMapping_nextInstruction = storeKeyMapping_instruction.Next;
            methodDef.Body.Instructions.Remove(storeKeyMapping_instruction);
            storeKeyMapping_instruction = ilProcessor.Create(OpCodes.Stfld, keyMapping_fieldDef);
            ilProcessor.InsertBefore(storeKeyMapping_nextInstruction, storeKeyMapping_instruction);

            for (int i = 0; i < serializableMembers.Members.Length; i++)
            {
                var memberDef = serializableMembers.Members[i];
                var key = serializableMembers.Keys[i];

                ilProcessor.InsertBefore(storeKeyMapping_instruction, ilProcessor.Create(OpCodes.Dup));
                ilProcessor.InsertBefore(storeKeyMapping_instruction, ilProcessor.Create(OpCodes.Ldstr, key));
                ilProcessor.InsertBefore(storeKeyMapping_instruction, ilProcessor.CreateLoadIntegerInstruction(i));
                ilProcessor.InsertBefore(storeKeyMapping_instruction, ilProcessor.Create(OpCodes.Callvirt, _automata_add_methodRef));

                ilProcessor.InsertBefore(storeStringByteKeys_instruction, ilProcessor.Create(OpCodes.Dup));
                ilProcessor.InsertBefore(storeStringByteKeys_instruction, ilProcessor.CreateLoadIntegerInstruction(i));
                ilProcessor.InsertBefore(storeStringByteKeys_instruction, ilProcessor.Create(OpCodes.Ldstr, key));
                ilProcessor.InsertBefore(storeStringByteKeys_instruction, ilProcessor.Create(OpCodes.Call, _getEncodedStringBytes_methodRef));
                ilProcessor.InsertBefore(storeStringByteKeys_instruction, ilProcessor.Create(OpCodes.Stelem_Ref));
            }

            return methodDef;
        }

        private static MethodDefinition GenerateStringIndexedSerializeMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference messagePackObject_typeRef, SerializableMembersDefinition serializableMembers, FieldDefinition stringByteKeys_fieldDef)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, template_methodDef.ReturnType);
            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                if (i == 1)
                {
                    methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, messagePackObject_typeRef));
                }
                else
                {
                    methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
                }
            }

            var iLProcessor = methodDef.Body.GetILProcessor();
            Instruction instructionAfterNullCheck = null;

            if (serializableMembers.HasNonPrimitiveTypeMember)
            {
                var getResolver_instruction = iLProcessor.Create(OpCodes.Ldarg_3);
                iLProcessor.Append(getResolver_instruction);
                iLProcessor.Emit(OpCodes.Callvirt, _getResolver_methodDef);

                instructionAfterNullCheck = getResolver_instruction;
            }

            var writeMapHeader_instruction = iLProcessor.Create(OpCodes.Ldarg_1);
            iLProcessor.Append(writeMapHeader_instruction);
            iLProcessor.Append(iLProcessor.CreateLoadIntegerInstruction(serializableMembers.Members.Length));
            iLProcessor.Emit(OpCodes.Call, _writeMapHeader_methodRef);

            if (instructionAfterNullCheck == null)
            {
                instructionAfterNullCheck = writeMapHeader_instruction;
            }

            if (!messagePackObject_typeRef.Resolve().IsValueType)
            {
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Ldarg_2));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Brtrue, instructionAfterNullCheck));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Ldarg_1));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Call, _writeNil_methodDef));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Ret));
            }

            for (int i = 0; i < serializableMembers.Members.Length; i++)
            {
                var memberDef = serializableMembers.Members[i];
                var key = serializableMembers.Keys[i];

                Instruction get_instruction = default;
                TypeReference member_typeRef = default;
                if (memberDef.GetType() == typeof(FieldDefinition))
                {
                    var member_fieldDef = (FieldDefinition)memberDef;
                    get_instruction = iLProcessor.Create(OpCodes.Ldfld, mainModule.ImportReference(member_fieldDef));
                    member_typeRef = mainModule.ImportReference(member_fieldDef.FieldType);
                }
                else if (memberDef.GetType() == typeof(PropertyDefinition))
                {
                    var member_propertyDef = (PropertyDefinition)memberDef;
                    get_instruction = iLProcessor.Create(OpCodes.Callvirt, mainModule.ImportReference(member_propertyDef.GetMethod));
                    member_typeRef = mainModule.ImportReference(member_propertyDef.PropertyType);
                }

                (bool isPrimitiveOrEnumType, bool isEnum) = IsPrimitiveOrEnumType(memberDef);

                iLProcessor.Emit(OpCodes.Ldarg_1);
                iLProcessor.Emit(OpCodes.Ldarg_0);
                iLProcessor.Emit(OpCodes.Ldfld, stringByteKeys_fieldDef);
                iLProcessor.Append(iLProcessor.CreateLoadIntegerInstruction(i));
                iLProcessor.Emit(OpCodes.Ldelem_Ref);
                iLProcessor.Emit(OpCodes.Call, _byteOpImplicit_methodRef);
                iLProcessor.Emit(OpCodes.Call, _writeRaw_methodRef);

                int generatedNonPrimitiveTypeMemberCount = 0;
                if (isPrimitiveOrEnumType)
                {
                    iLProcessor.Emit(OpCodes.Ldarg_1);
                    iLProcessor.Emit(OpCodes.Ldarg_2);
                    iLProcessor.Append(get_instruction);

                    string memberType_fullName = member_typeRef.FullName;
                    if (isEnum)
                    {
                        memberType_fullName = GetEnumTypeFullName(member_typeRef);
                    }
                    iLProcessor.Emit(OpCodes.Call, _writeMethods[memberType_fullName]);
                }
                else
                {
                    generatedNonPrimitiveTypeMemberCount++;
                    if (generatedNonPrimitiveTypeMemberCount < serializableMembers.NonPrimitiveTypeMemberCount)
                    {
                        iLProcessor.Emit(OpCodes.Dup);
                    }

                    var getFormatterWithVerify_genericInstanceMethod = new GenericInstanceMethod(_getFormatterWithVerify_methodDef);
                    getFormatterWithVerify_genericInstanceMethod.GenericArguments.Add(member_typeRef);
                    iLProcessor.Emit(OpCodes.Call, getFormatterWithVerify_genericInstanceMethod);

                    iLProcessor.Emit(OpCodes.Ldarg_1);
                    iLProcessor.Emit(OpCodes.Ldarg_2);
                    iLProcessor.Append(get_instruction);
                    iLProcessor.Emit(OpCodes.Ldarg_3);

                    var iMessagePackFormatter_typeRef = mainModule.ImportReference(
                        _serialize_methodDef.DeclaringType.MakeGenericInstanceType(member_typeRef)
                    );
                    var serialize_methodRef = new MethodReference(_serialize_methodDef.Name, _serialize_methodDef.ReturnType, iMessagePackFormatter_typeRef);
                    serialize_methodRef.HasThis = true;
                    foreach (var parameter in _serialize_methodDef.Parameters)
                    {
                        serialize_methodRef.Parameters.Add(parameter);
                    }

                    iLProcessor.Emit(OpCodes.Callvirt, serialize_methodRef);
                }
            }

            iLProcessor.Emit(OpCodes.Ret);

            return methodDef;
        }

        private static MethodDefinition GenerateStringIndexedDeserializeMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference messagePackObject_typeRef, SerializableMembersDefinition serializableMembers, FieldDefinition keyMapping_fieldDef)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, messagePackObject_typeRef);

            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
            }

            var iLProcessor = methodDef.Body.GetILProcessor();
            Instruction startingPoint = null;

            if (messagePackObject_typeRef.IsValueType)
            {
                startingPoint = iLProcessor.Create(OpCodes.Ldstr, "typecode is null, struct not supported");
                iLProcessor.Append(startingPoint);
                iLProcessor.Emit(OpCodes.Newobj, _invalidOperationException_ctor_methodRef);
                iLProcessor.Emit(OpCodes.Throw);
            }
            else
            {
                startingPoint = iLProcessor.Create(OpCodes.Ldnull);
                iLProcessor.Append(startingPoint);
                iLProcessor.Emit(OpCodes.Ret);
            }

            var after_tryReadNil_Instruction = iLProcessor.Create(OpCodes.Ldarg_2);

            iLProcessor.InsertBefore(startingPoint, iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.InsertBefore(startingPoint, iLProcessor.Create(OpCodes.Call, _tryReadNil_methodRef));
            iLProcessor.InsertBefore(startingPoint, iLProcessor.Create(OpCodes.Brfalse, after_tryReadNil_Instruction));

            iLProcessor.Append(after_tryReadNil_Instruction);
            iLProcessor.Emit(OpCodes.Callvirt, _getSecurity_methodRef);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, _depthStep_methodRef);

            VariableDefinition resolver_varDef = null;
            if (serializableMembers.HasNonPrimitiveTypeMember)
            {
                iLProcessor.Emit(OpCodes.Ldarg_2);
                iLProcessor.Emit(OpCodes.Callvirt, _getResolver_methodDef);
                iLProcessor.Emit(OpCodes.Stloc_0);
                resolver_varDef = new VariableDefinition(_iFormatterResolver_typeRef);
                methodDef.Body.Variables.Add(resolver_varDef);
            }

            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Call, _readMapHeader_methodRef);
            var num_varDef = new VariableDefinition(_int32_typeRef);
            methodDef.Body.Variables.Add(num_varDef);
            iLProcessor.Append(CreateStLocInstruction(iLProcessor, num_varDef.Index));

            var variableDefinition_instruction = iLProcessor.Create(OpCodes.Ldc_I4_0);
            iLProcessor.Append(variableDefinition_instruction);
            var i_varDef = new VariableDefinition(_int32_typeRef);
            methodDef.Body.Variables.Add(i_varDef);
            iLProcessor.Append(CreateStLocInstruction(iLProcessor, i_varDef.Index));

            var spanByte_varDef = new VariableDefinition(_readStringSpan_methodRef.ReturnType);
            methodDef.Body.Variables.Add(spanByte_varDef);
            var intKey_varDef = new VariableDefinition(_int32_typeRef);
            methodDef.Body.Variables.Add(intKey_varDef);

            // for loop
            var check_i_lessThan_num_instruction = CreateLdLocInstruction(iLProcessor, i_varDef.Index);
            var loopStart_instruction = iLProcessor.Create(OpCodes.Br, check_i_lessThan_num_instruction);
            var increment_i_instruction = CreateLdLocInstruction(iLProcessor, i_varDef.Index);
            var goto_switch_instruction = iLProcessor.Create(OpCodes.Brtrue, increment_i_instruction);
            var afterIntKey_skip_instruction = iLProcessor.Create(OpCodes.Br, increment_i_instruction);
            var loopEnd_instruction = afterIntKey_skip_instruction;
            Instruction loopBack_instruction = null;
            {
                iLProcessor.Append(loopStart_instruction);
                iLProcessor.Emit(OpCodes.Ldarg_1);
                iLProcessor.Emit(OpCodes.Call, _readStringSpan_methodRef);
                iLProcessor.Append(CreateStLocInstruction(iLProcessor, spanByte_varDef.Index));
                iLProcessor.Emit(OpCodes.Ldarg_0);
                iLProcessor.Emit(OpCodes.Ldfld, keyMapping_fieldDef);
                iLProcessor.Append(CreateLdLocInstruction(iLProcessor, spanByte_varDef.Index));
                iLProcessor.Append(CreateLdLocaInstruction(iLProcessor, intKey_varDef.Index));
                iLProcessor.Emit(OpCodes.Callvirt, _automata_tryGetValue_methodRef);
                iLProcessor.Append(goto_switch_instruction);

                iLProcessor.Emit(OpCodes.Ldarg_1);
                iLProcessor.Emit(OpCodes.Call, _skip_methodRef);
                iLProcessor.Append(afterIntKey_skip_instruction);

                iLProcessor.Emit(OpCodes.Ldarg_1);
                iLProcessor.Emit(OpCodes.Call, _skip_methodRef);

                iLProcessor.Append(increment_i_instruction);
                iLProcessor.Emit(OpCodes.Ldc_I4_1);
                iLProcessor.Emit(OpCodes.Add);
                iLProcessor.Append(CreateStLocInstruction(iLProcessor, i_varDef.Index));

                iLProcessor.Append(check_i_lessThan_num_instruction);
                iLProcessor.Append(CreateLdLocInstruction(iLProcessor, num_varDef.Index));
                loopBack_instruction = iLProcessor.Create(OpCodes.Blt, loopStart_instruction.Next);
                iLProcessor.Append(loopBack_instruction);
            }
            loopEnd_instruction = loopEnd_instruction.Next;

            VariableDefinition result_varDef = null;
            Instruction result_ctor_instruction = null;
            var tuple = TryGetStringMatchingConstructor(messagePackObject_typeRef.Resolve(), serializableMembers.Keys);
            var result_ctor_methodDef = tuple.method;
            bool ctorHasParameters = result_ctor_methodDef != null && result_ctor_methodDef.Parameters.Count > 0;
            if (messagePackObject_typeRef.IsValueType)
            {
                result_varDef = new VariableDefinition(messagePackObject_typeRef);
                methodDef.Body.Variables.Add(result_varDef);
                iLProcessor.Append(CreateLdLocaInstruction(iLProcessor, result_varDef.Index));

                if (result_ctor_methodDef == null)
                {
                    result_ctor_instruction = iLProcessor.Create(OpCodes.Initobj, messagePackObject_typeRef);
                }
                else
                {
                    result_ctor_instruction = iLProcessor.Create(OpCodes.Call, mainModule.ImportReference(result_ctor_methodDef));
                }
            }
            else
            {
                result_ctor_instruction = iLProcessor.Create(OpCodes.Newobj, mainModule.ImportReference(result_ctor_methodDef));
            }
            iLProcessor.Append(result_ctor_instruction);

            var depth_varDef = new VariableDefinition(_int32_typeRef);
            methodDef.Body.Variables.Add(depth_varDef);

            var setValue_instructions = iLProcessor.Create(OpCodes.Ldarg_1);
            iLProcessor.Append(setValue_instructions);
            iLProcessor.Emit(OpCodes.Dup);
            iLProcessor.Emit(OpCodes.Call, _getDepth_methodRef);
            iLProcessor.Append(CreateStLocInstruction(iLProcessor, depth_varDef.Index));
            iLProcessor.Append(CreateLdLocInstruction(iLProcessor, depth_varDef.Index));
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Sub);
            iLProcessor.Emit(OpCodes.Call, _setDepth_methodRef);

            if (messagePackObject_typeRef.IsValueType)
            {
                iLProcessor.Append(CreateLdLocInstruction(iLProcessor, result_varDef.Index));
            }
            iLProcessor.Emit(OpCodes.Ret);

            Instruction[] jumpTargets = new Instruction[serializableMembers.Members.Length];
            VariableDefinition[] varDefs = new VariableDefinition[serializableMembers.Members.Length];

            for (int i = 0; i < serializableMembers.Members.Length; i++)
            {
                var memberDef = serializableMembers.Members[i];

                if (memberDef == null)
                {
                    jumpTargets[i] = loopEnd_instruction;
                    continue;
                }

                TypeReference member_typeRef = null;
                Instruction stfld_instruction = null;
                bool isReadOnly = false;
                if (memberDef.GetType() == typeof(FieldDefinition))
                {
                    var member_fieldDef = (FieldDefinition)memberDef;
                    isReadOnly = member_fieldDef.IsInitOnly;
                    stfld_instruction = iLProcessor.Create(OpCodes.Stfld, mainModule.ImportReference(member_fieldDef));
                    member_typeRef = mainModule.ImportReference(member_fieldDef.FieldType);
                }
                else if (memberDef.GetType() == typeof(PropertyDefinition))
                {
                    var member_propertyDef = (PropertyDefinition)memberDef;
                    isReadOnly = member_propertyDef.SetMethod == null;
                    if (!isReadOnly)
                    {
                        stfld_instruction = iLProcessor.Create(OpCodes.Callvirt, mainModule.ImportReference(member_propertyDef.SetMethod));
                    }
                    member_typeRef = mainModule.ImportReference(member_propertyDef.PropertyType);
                }

                var member_varDef = EmitTypeDefaultValue(iLProcessor, member_typeRef, storeAsVariable: true, variableDefinition_instruction);
                varDefs[i] = member_varDef;

                (bool isPrimitiveOrEnumType, bool isEnum) = IsPrimitiveOrEnumType(memberDef);
                if (isPrimitiveOrEnumType)
                {
                    string memberType_fullName = member_typeRef.FullName;
                    if (isEnum)
                    {
                        memberType_fullName = GetEnumTypeFullName(member_typeRef);
                    }

                    jumpTargets[i] = iLProcessor.Create(OpCodes.Ldarg_1);
                    iLProcessor.InsertBefore(loopEnd_instruction, jumpTargets[i]);
                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Call, _readMethods[memberType_fullName]));
                }
                else
                {
                    var ldLocResolver_instruction = CreateLdLocInstruction(iLProcessor, resolver_varDef.Index);
                    jumpTargets[i] = ldLocResolver_instruction;
                    iLProcessor.InsertBefore(loopEnd_instruction, ldLocResolver_instruction);
                    var getFormatterWithVerify_genericInstanceMethod = new GenericInstanceMethod(_getFormatterWithVerify_methodDef);
                    getFormatterWithVerify_genericInstanceMethod.GenericArguments.Add(member_typeRef);
                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Call, getFormatterWithVerify_genericInstanceMethod));

                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Ldarg_1));
                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Ldarg_2));

                    var iMessagePackFormatter_typeRef = mainModule.ImportReference(
                        _deserialize_methodDef.DeclaringType.MakeGenericInstanceType(member_typeRef)
                    );
                    var deserialize_methodRef = new MethodReference(_deserialize_methodDef.Name, _deserialize_methodDef.ReturnType, iMessagePackFormatter_typeRef);
                    deserialize_methodRef.HasThis = true;
                    foreach (var parameter in _deserialize_methodDef.Parameters)
                    {
                        deserialize_methodRef.Parameters.Add(parameter);
                    }

                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Callvirt, deserialize_methodRef));
                }
                iLProcessor.InsertBefore(loopEnd_instruction, CreateStLocInstruction(iLProcessor, member_varDef.Index));
                iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Br, increment_i_instruction));

                if (!isReadOnly)
                {
                    if (messagePackObject_typeRef.IsValueType)
                    {
                        iLProcessor.InsertBefore(setValue_instructions, CreateLdLocaInstruction(iLProcessor, result_varDef.Index));
                    }
                    else
                    {
                        iLProcessor.InsertBefore(setValue_instructions, iLProcessor.Create(OpCodes.Dup));
                    }
                    iLProcessor.InsertBefore(setValue_instructions, CreateLdLocInstruction(iLProcessor, member_varDef.Index));
                    iLProcessor.InsertBefore(setValue_instructions, stfld_instruction);
                }
            }

            if (ctorHasParameters)
            {
                foreach (int parameterIndex in tuple.parameterIndices)
                {
                    iLProcessor.InsertBefore(result_ctor_instruction, CreateLdLocInstruction(iLProcessor, varDefs[parameterIndex].Index));
                }
            }

            if (serializableMembers.Members.Length > 0)
            {
                var indexSequences = GetIndexSequences(serializableMembers.Indices);
                var switchSequence = MakeTree(indexSequences);

                EmitSwitchStatement(switchSequence,
                                    ref jumpTargets,
                                    ref iLProcessor,
                                    ref afterIntKey_skip_instruction,
                                    ref loopEnd_instruction,
                                    intKey_varDef.Index);

                loopBack_instruction.Operand = loopStart_instruction.Next;
                goto_switch_instruction.Operand = afterIntKey_skip_instruction.Next;
            }

            return methodDef;
        }

        private struct SerializableMembersDefinition
        {
            public IMemberDefinition[] Members;
            public int NonPrimitiveTypeMemberCount;
            public int[] Indices;
            public string[] Keys;

            public bool HasNonPrimitiveTypeMember
            {
                get { return NonPrimitiveTypeMemberCount > 0; }
            }

            public Collection<ParameterDefinition> AsParameters { get; internal set; }
        }

        private static SerializableMembersDefinition GetSerializableMembers(ModuleDefinition mainModule, TypeReference messagePackObject_typeRef, bool IsIntIndexed = true)
        {
            var messagePackObject_typeDef = messagePackObject_typeRef.Resolve();

            var serializableProperties_memberDef = GetSerializableProperties(messagePackObject_typeDef);
            var serializableFields_memberDef = GetSerializableFields(messagePackObject_typeDef);

            var concatenatedSerializableMembers = (serializableProperties_memberDef as IEnumerable<IMemberDefinition>).Concat(serializableFields_memberDef);
            int nonPrimitiveTypeMemberCount = concatenatedSerializableMembers.Count(m => !IsPrimitiveOrEnumType(m).primitiveOrEnumType);
            int membersCount = concatenatedSerializableMembers.Count();

            IMemberDefinition[] members = null;
            int[] indices = null;
            string[] keys = null;
            Collection<ParameterDefinition> asParameters = new Collection<ParameterDefinition>();

            if (IsIntIndexed)
            {
                var distinct = new HashSet<int>(concatenatedSerializableMembers.Select(GetIntKeyValue));
                if (distinct.Count != membersCount)
                {
                    throw new Exception($"All keys must be unique. Found duplicate keys in type {messagePackObject_typeDef.FullName}");
                }

                concatenatedSerializableMembers = concatenatedSerializableMembers.OrderBy(GetIntKeyValue);
                foreach (var memberDef in concatenatedSerializableMembers)
                {
                    if (memberDef.GetType() == typeof(FieldDefinition))
                    {
                        asParameters.Add(new ParameterDefinition(((FieldDefinition)memberDef).FieldType));
                    }
                    else
                    {
                        asParameters.Add(new ParameterDefinition(((PropertyDefinition)memberDef).PropertyType));
                    }
                }

                if (membersCount > 0)
                {
                    membersCount = GetIntKeyValue(concatenatedSerializableMembers.Last()) + 1;
                }

                members = new IMemberDefinition[membersCount];
                indices = new int[concatenatedSerializableMembers.Count()];

                int index = 0;
                foreach (var memberDef in concatenatedSerializableMembers)
                {
                    int key = GetIntKeyValue(memberDef);
                    members[key] = memberDef;
                    indices[index] = key;
                    index++;
                }
            }
            else
            {
                var distinct = new HashSet<string>(concatenatedSerializableMembers.Select(GetStringKeyValue));
                if (distinct.Count != membersCount)
                {
                    throw new Exception($"All keys must be unique. Found duplicate keys in type {messagePackObject_typeDef.FullName}");
                }

                foreach (var propertyDef in serializableProperties_memberDef)
                {
                    asParameters.Add(new ParameterDefinition(propertyDef.PropertyType));
                }
                foreach (var fieldDef in serializableFields_memberDef)
                {
                    asParameters.Add(new ParameterDefinition(fieldDef.FieldType));
                }

                members = new IMemberDefinition[membersCount];
                indices = new int[membersCount];
                keys = new string[membersCount];

                int index = 0;
                foreach (var memberDef in concatenatedSerializableMembers)
                {
                    members[index] = memberDef;
                    indices[index] = index;
                    keys[index] = GetStringKeyValue(memberDef);
                    index++;
                }
            }

            return new SerializableMembersDefinition
            {
                Members = members,
                NonPrimitiveTypeMemberCount = nonPrimitiveTypeMemberCount,
                Indices = indices,
                AsParameters = asParameters,
                Keys = keys,
            };
        }

        private static IEnumerable<FieldDefinition> GetSerializableFields(TypeDefinition typeDef)
        {
            List<FieldDefinition> fields = new List<FieldDefinition>(typeDef.Fields.AsEnumerable());
            if (typeDef.BaseType != null)
            {
                fields.AddRange(GetSerializableFields(typeDef.BaseType.Resolve()));
            }

            return
                from field in fields
                from attr in field.CustomAttributes
                where attr.AttributeType.Name == KEY_ATTRIBUTE_NAME
                select field;
        }

        private static IEnumerable<PropertyDefinition> GetSerializableProperties(TypeDefinition typeDef)
        {
            List<PropertyDefinition> properties = new List<PropertyDefinition>(typeDef.Properties.AsEnumerable());
            if (typeDef.BaseType != null)
            {
                properties.AddRange(GetSerializableProperties(typeDef.BaseType.Resolve()));
            }

            return
                from property in properties
                from attr in property.CustomAttributes
                where attr.AttributeType.Name == KEY_ATTRIBUTE_NAME
                select property;
        }

        private static MethodDefinition GenerateSerializeMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference messagePackObject_typeRef, SerializableMembersDefinition serializableMembers)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, template_methodDef.ReturnType);
            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                if (i == 1)
                {
                    methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, messagePackObject_typeRef));
                }
                else
                {
                    methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
                }
            }

            var iLProcessor = methodDef.Body.GetILProcessor();
            Instruction instructionAfterNullCheck = default;
            if (serializableMembers.HasNonPrimitiveTypeMember)
            {
                var getResolver_instruction = iLProcessor.Create(OpCodes.Ldarg_3);
                iLProcessor.Append(getResolver_instruction);
                iLProcessor.Emit(OpCodes.Callvirt, _getResolver_methodDef);

                instructionAfterNullCheck = getResolver_instruction;
            }

            var writeArrayHeader_instruction = iLProcessor.Create(OpCodes.Ldarg_1);
            iLProcessor.Append(writeArrayHeader_instruction);
            iLProcessor.Append(iLProcessor.CreateLoadIntegerInstruction(serializableMembers.Members.Length));
            iLProcessor.Emit(OpCodes.Call, _writeArrayHeader_methodRef);

            if (instructionAfterNullCheck == null)
            {
                instructionAfterNullCheck = writeArrayHeader_instruction;
            }

            if (serializableMembers.Members.Length > 0)
            {
                int generatedNonPrimitiveTypeMemberCount = 0;
                foreach (var memberDef in serializableMembers.Members)
                {
                    if (memberDef == null)
                    {
                        iLProcessor.Emit(OpCodes.Ldarg_1);
                        iLProcessor.Emit(OpCodes.Call, _writeNil_methodDef);
                    }
                    else
                    {
                        Instruction get_instruction = default;
                        TypeReference member_typeRef = default;
                        if (memberDef.GetType() == typeof(FieldDefinition))
                        {
                            var member_fieldDef = (FieldDefinition)memberDef;
                            get_instruction = iLProcessor.Create(OpCodes.Ldfld, mainModule.ImportReference(member_fieldDef));
                            member_typeRef = mainModule.ImportReference(member_fieldDef.FieldType);
                        }
                        else if (memberDef.GetType() == typeof(PropertyDefinition))
                        {
                            var member_propertyDef = (PropertyDefinition)memberDef;
                            var get_methodDef = member_propertyDef.GetMethod;
                            if (get_methodDef == null)
                            {
                                throw new Exception($"Property {member_propertyDef.Name} of type {memberDef.DeclaringType.FullName} requires a getter");
                            }

                            get_instruction = iLProcessor.Create(OpCodes.Callvirt, mainModule.ImportReference(member_propertyDef.GetMethod));
                            member_typeRef = mainModule.ImportReference(member_propertyDef.PropertyType);
                        }

                        (bool isPrimitiveOrEnumType, bool isEnum) = IsPrimitiveOrEnumType(memberDef);

                        if (isPrimitiveOrEnumType)
                        {
                            iLProcessor.Emit(OpCodes.Ldarg_1);
                            iLProcessor.Emit(OpCodes.Ldarg_2);
                            iLProcessor.Append(get_instruction);

                            string memberType_fullName = member_typeRef.FullName;
                            if (isEnum)
                            {
                                memberType_fullName = GetEnumTypeFullName(member_typeRef);
                            }
                            iLProcessor.Emit(OpCodes.Call, _writeMethods[memberType_fullName]);
                        }
                        else
                        {
                            generatedNonPrimitiveTypeMemberCount++;
                            if (generatedNonPrimitiveTypeMemberCount < serializableMembers.NonPrimitiveTypeMemberCount)
                            {
                                iLProcessor.Emit(OpCodes.Dup);
                            }

                            var getFormatterWithVerify_genericInstanceMethod = new GenericInstanceMethod(_getFormatterWithVerify_methodDef);
                            getFormatterWithVerify_genericInstanceMethod.GenericArguments.Add(member_typeRef);
                            iLProcessor.Emit(OpCodes.Call, getFormatterWithVerify_genericInstanceMethod);

                            iLProcessor.Emit(OpCodes.Ldarg_1);
                            iLProcessor.Emit(OpCodes.Ldarg_2);
                            iLProcessor.Append(get_instruction);
                            iLProcessor.Emit(OpCodes.Ldarg_3);

                            var iMessagePackFormatter_typeRef = mainModule.ImportReference(
                                _serialize_methodDef.DeclaringType.MakeGenericInstanceType(member_typeRef)
                            );
                            var serialize_methodRef = new MethodReference(_serialize_methodDef.Name, _serialize_methodDef.ReturnType, iMessagePackFormatter_typeRef);
                            serialize_methodRef.HasThis = true;
                            foreach (var parameter in _serialize_methodDef.Parameters)
                            {
                                serialize_methodRef.Parameters.Add(parameter);
                            }

                            iLProcessor.Emit(OpCodes.Callvirt, serialize_methodRef);
                        }
                    }
                }
            }

            iLProcessor.Emit(OpCodes.Ret);

            if (!messagePackObject_typeRef.Resolve().IsValueType)
            {
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Ldarg_2));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Brtrue, instructionAfterNullCheck));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Ldarg_1));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Call, _writeNil_methodDef));
                iLProcessor.InsertBefore(instructionAfterNullCheck, iLProcessor.Create(OpCodes.Ret));
            }

            return methodDef;
        }

        private static (bool primitiveOrEnumType, bool isEnum) IsPrimitiveOrEnumType(IMemberDefinition memberDef)
        {
            var memberDefinitionType = memberDef.GetType();
            if (memberDefinitionType == typeof(FieldDefinition))
            {
                var fieldDefinition = (FieldDefinition)memberDef;
                bool isEnum = fieldDefinition.FieldType.Resolve().IsEnum;
                bool result = isEnum || PRIMITIVE_TYPES_DEFINITION.Contains(fieldDefinition.FieldType.Name);

                return (result, isEnum);
            }
            else if (memberDefinitionType == typeof(PropertyDefinition))
            {
                var propertyDefinition = (PropertyDefinition)memberDef;
                bool isEnum = propertyDefinition.PropertyType.Resolve().IsEnum;
                bool result = isEnum || PRIMITIVE_TYPES_DEFINITION.Contains(propertyDefinition.PropertyType.Name);

                return (result, isEnum);
            }

            throw new NotSupportedException($"Member definition of type ${memberDefinitionType} is not supported");
        }

        private static string GetEnumTypeFullName(TypeReference member_typeRef)
        {
            return member_typeRef.Resolve().Fields.FirstOrDefault(f => f.Name == "value__").FieldType.FullName;
        }

        private static int GetIntKeyValue(IMemberDefinition memberDef)
        {
            return (int)memberDef.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == KEY_ATTRIBUTE_NAME).ConstructorArguments[0].Value;
        }

        private static string GetStringKeyValue(IMemberDefinition memberDef)
        {
            return (string)memberDef.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == KEY_ATTRIBUTE_NAME).ConstructorArguments[0].Value;
        }

        private static Instruction CreateLdLocaInstruction(ILProcessor iLProcessor, int value)
        {
            if (value > 255)
            {
                short s_value = (short)value;
                return iLProcessor.Create(OpCodes.Ldloca, s_value);
            }
            else
            {
                byte b_value = (byte)value;
                return iLProcessor.Create(OpCodes.Ldloca_S, b_value);
            }
        }

        private static Instruction CreateLdLocInstruction(ILProcessor iLProcessor, int value)
        {
            if (value > 3)
            {
                if (value > 255)
                {
                    return iLProcessor.Create(OpCodes.Ldloc, value);
                }
                else
                {
                    return iLProcessor.Create(OpCodes.Ldloc_S, (byte)value);
                }
            }
            else
            {
                OpCode opCode = OpCodes.Nop;
                switch (value)
                {
                    case 0:
                        opCode = OpCodes.Ldloc_0;
                        break;
                    case 1:
                        opCode = OpCodes.Ldloc_1;
                        break;
                    case 2:
                        opCode = OpCodes.Ldloc_2;
                        break;
                    case 3:
                        opCode = OpCodes.Ldloc_3;
                        break;
                }

                return iLProcessor.Create(opCode);
            }
        }

        private static Instruction CreateStLocInstruction(ILProcessor iLProcessor, int value)
        {
            if (value > 3)
            {
                if (value > 255)
                {
                    return iLProcessor.Create(OpCodes.Stloc, value);
                }
                else
                {
                    return iLProcessor.Create(OpCodes.Stloc_S, (byte)value);
                }
            }
            else
            {
                OpCode opCode = OpCodes.Nop;
                switch (value)
                {
                    case 0:
                        opCode = OpCodes.Stloc_0;
                        break;
                    case 1:
                        opCode = OpCodes.Stloc_1;
                        break;
                    case 2:
                        opCode = OpCodes.Stloc_2;
                        break;
                    case 3:
                        opCode = OpCodes.Stloc_3;
                        break;
                }

                return iLProcessor.Create(opCode);
            }
        }

        private static MethodDefinition GenerateDeserializeMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef, TypeReference messagePackObject_typeRef, SerializableMembersDefinition serializableMembers)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, messagePackObject_typeRef);

            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
            }

            var iLProcessor = methodDef.Body.GetILProcessor();
            Instruction startingPoint = null;

            if (messagePackObject_typeRef.IsValueType)
            {
                startingPoint = iLProcessor.Create(OpCodes.Ldstr, "typecode is null, struct not supported");
                iLProcessor.Append(startingPoint);
                iLProcessor.Emit(OpCodes.Newobj, _invalidOperationException_ctor_methodRef);
                iLProcessor.Emit(OpCodes.Throw);
            }
            else
            {
                startingPoint = iLProcessor.Create(OpCodes.Ldnull);
                iLProcessor.Append(startingPoint);
                iLProcessor.Emit(OpCodes.Ret);
            }

            var after_tryReadNil_Instruction = iLProcessor.Create(OpCodes.Ldarg_2);

            iLProcessor.InsertBefore(startingPoint, iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.InsertBefore(startingPoint, iLProcessor.Create(OpCodes.Call, _tryReadNil_methodRef));
            iLProcessor.InsertBefore(startingPoint, iLProcessor.Create(OpCodes.Brfalse, after_tryReadNil_Instruction));

            iLProcessor.Append(after_tryReadNil_Instruction);
            iLProcessor.Emit(OpCodes.Callvirt, _getSecurity_methodRef);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, _depthStep_methodRef);

            VariableDefinition resolver_varDef = null;
            if (serializableMembers.HasNonPrimitiveTypeMember)
            {
                iLProcessor.Emit(OpCodes.Ldarg_2);
                iLProcessor.Emit(OpCodes.Callvirt, _getResolver_methodDef);
                iLProcessor.Emit(OpCodes.Stloc_0);
                resolver_varDef = new VariableDefinition(_iFormatterResolver_typeRef);
                methodDef.Body.Variables.Add(resolver_varDef);
            }

            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Call, _readArrayHeader_methodRef);
            var num_varDef = new VariableDefinition(_int32_typeRef);
            methodDef.Body.Variables.Add(num_varDef);
            iLProcessor.Append(CreateStLocInstruction(iLProcessor, num_varDef.Index));

            var variableDefinition_instruction = iLProcessor.Create(OpCodes.Ldc_I4_0);
            iLProcessor.Append(variableDefinition_instruction);
            var i_varDef = new VariableDefinition(_int32_typeRef);
            methodDef.Body.Variables.Add(i_varDef);
            iLProcessor.Append(CreateStLocInstruction(iLProcessor, i_varDef.Index));


            // for loop
            var check_i_lessThan_num_instruction = CreateLdLocInstruction(iLProcessor, i_varDef.Index);
            var loopStart_instruction = iLProcessor.Create(OpCodes.Br, check_i_lessThan_num_instruction);
            var increment_i_instruction = CreateLdLocInstruction(iLProcessor, i_varDef.Index);
            var loopEnd_instruction = iLProcessor.Create(OpCodes.Ldarg_1);
            Instruction loopBack_instruction = null;
            {
                iLProcessor.Append(loopStart_instruction);

                iLProcessor.Append(loopEnd_instruction);
                iLProcessor.Emit(OpCodes.Call, _skip_methodRef);

                iLProcessor.Append(increment_i_instruction);
                iLProcessor.Emit(OpCodes.Ldc_I4_1);
                iLProcessor.Emit(OpCodes.Add);
                iLProcessor.Append(CreateStLocInstruction(iLProcessor, i_varDef.Index));

                iLProcessor.Append(check_i_lessThan_num_instruction);
                iLProcessor.Append(CreateLdLocInstruction(iLProcessor, num_varDef.Index));
                loopBack_instruction = iLProcessor.Create(OpCodes.Blt, loopStart_instruction.Next);
                iLProcessor.Append(loopBack_instruction);
            }

            VariableDefinition result_varDef = null;
            Instruction result_ctor_instruction = null;
            var result_ctor_methodDef = TryGetMatchingConstructor(messagePackObject_typeRef.Resolve(), serializableMembers.AsParameters);
            bool ctorHasParameters = result_ctor_methodDef != null && result_ctor_methodDef.Parameters.Count > 0;
            if (messagePackObject_typeRef.IsValueType)
            {
                result_varDef = new VariableDefinition(messagePackObject_typeRef);
                methodDef.Body.Variables.Add(result_varDef);
                iLProcessor.Append(CreateLdLocaInstruction(iLProcessor, result_varDef.Index));

                if (result_ctor_methodDef == null)
                {
                    result_ctor_instruction = iLProcessor.Create(OpCodes.Initobj, messagePackObject_typeRef);
                }
                else
                {
                    result_ctor_instruction = iLProcessor.Create(OpCodes.Call, mainModule.ImportReference(result_ctor_methodDef));
                }
            }
            else
            {
                result_ctor_instruction = iLProcessor.Create(OpCodes.Newobj, mainModule.ImportReference(result_ctor_methodDef));
            }
            iLProcessor.Append(result_ctor_instruction);

            var depth_varDef = new VariableDefinition(_int32_typeRef);
            methodDef.Body.Variables.Add(depth_varDef);

            var setValue_instructions = iLProcessor.Create(OpCodes.Ldarg_1);
            iLProcessor.Append(setValue_instructions);
            iLProcessor.Emit(OpCodes.Dup);
            iLProcessor.Emit(OpCodes.Call, _getDepth_methodRef);
            iLProcessor.Append(CreateStLocInstruction(iLProcessor, depth_varDef.Index));
            iLProcessor.Append(CreateLdLocInstruction(iLProcessor, depth_varDef.Index));
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Sub);
            iLProcessor.Emit(OpCodes.Call, _setDepth_methodRef);

            if (messagePackObject_typeRef.IsValueType)
            {
                iLProcessor.Append(CreateLdLocInstruction(iLProcessor, result_varDef.Index));
            }
            iLProcessor.Emit(OpCodes.Ret);

            Instruction[] jumpTargets = new Instruction[serializableMembers.Members.Length];
            VariableDefinition[] varDefs = new VariableDefinition[serializableMembers.Indices.Length];

            for (int i = 0; i < serializableMembers.Members.Length; i++)
            {
                var memberDef = serializableMembers.Members[i];

                if (memberDef == null)
                {
                    jumpTargets[i] = loopEnd_instruction;
                    continue;
                }

                TypeReference member_typeRef = null;
                Instruction stfld_instruction = null;
                bool isReadOnly = false;
                if (memberDef.GetType() == typeof(FieldDefinition))
                {
                    var member_fieldDef = (FieldDefinition)memberDef;
                    isReadOnly = member_fieldDef.IsInitOnly;
                    stfld_instruction = iLProcessor.Create(OpCodes.Stfld, mainModule.ImportReference(member_fieldDef));
                    member_typeRef = mainModule.ImportReference(member_fieldDef.FieldType);
                }
                else if (memberDef.GetType() == typeof(PropertyDefinition))
                {
                    var member_propertyDef = (PropertyDefinition)memberDef;
                    isReadOnly = member_propertyDef.SetMethod == null;
                    if (!isReadOnly)
                    {
                        stfld_instruction = iLProcessor.Create(OpCodes.Callvirt, mainModule.ImportReference(member_propertyDef.SetMethod));
                    }
                    member_typeRef = mainModule.ImportReference(member_propertyDef.PropertyType);
                }

                var member_varDef = EmitTypeDefaultValue(iLProcessor, member_typeRef, storeAsVariable: true, variableDefinition_instruction);
                varDefs[i] = member_varDef;

                (bool isPrimitiveOrEnumType, bool isEnum) = IsPrimitiveOrEnumType(memberDef);
                if (isPrimitiveOrEnumType)
                {
                    string memberType_fullName = member_typeRef.FullName;
                    if (isEnum)
                    {
                        memberType_fullName = GetEnumTypeFullName(member_typeRef);
                    }

                    jumpTargets[i] = iLProcessor.Create(OpCodes.Ldarg_1);
                    iLProcessor.InsertBefore(loopEnd_instruction, jumpTargets[i]);
                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Call, _readMethods[memberType_fullName]));
                }
                else
                {
                    var ldLocResolver_instruction = CreateLdLocInstruction(iLProcessor, resolver_varDef.Index);
                    jumpTargets[i] = ldLocResolver_instruction;
                    iLProcessor.InsertBefore(loopEnd_instruction, ldLocResolver_instruction);
                    var getFormatterWithVerify_genericInstanceMethod = new GenericInstanceMethod(_getFormatterWithVerify_methodDef);
                    getFormatterWithVerify_genericInstanceMethod.GenericArguments.Add(member_typeRef);
                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Call, getFormatterWithVerify_genericInstanceMethod));

                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Ldarg_1));
                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Ldarg_2));

                    var iMessagePackFormatter_typeRef = mainModule.ImportReference(
                        _deserialize_methodDef.DeclaringType.MakeGenericInstanceType(member_typeRef)
                    );
                    var deserialize_methodRef = new MethodReference(_deserialize_methodDef.Name, _deserialize_methodDef.ReturnType, iMessagePackFormatter_typeRef);
                    deserialize_methodRef.HasThis = true;
                    foreach (var parameter in _deserialize_methodDef.Parameters)
                    {
                        deserialize_methodRef.Parameters.Add(parameter);
                    }

                    iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Callvirt, deserialize_methodRef));
                }
                iLProcessor.InsertBefore(loopEnd_instruction, CreateStLocInstruction(iLProcessor, member_varDef.Index));
                iLProcessor.InsertBefore(loopEnd_instruction, iLProcessor.Create(OpCodes.Br, increment_i_instruction));

                if (!isReadOnly)
                {
                    if (messagePackObject_typeRef.IsValueType)
                    {
                        iLProcessor.InsertBefore(setValue_instructions, CreateLdLocaInstruction(iLProcessor, result_varDef.Index));
                    }
                    else
                    {
                        iLProcessor.InsertBefore(setValue_instructions, iLProcessor.Create(OpCodes.Dup));
                    }
                    iLProcessor.InsertBefore(setValue_instructions, CreateLdLocInstruction(iLProcessor, member_varDef.Index));
                    iLProcessor.InsertBefore(setValue_instructions, stfld_instruction);
                }
            }

            if (ctorHasParameters)
            {
                for (int i = 0; i < result_ctor_methodDef.Parameters.Count; i++)
                {
                    iLProcessor.InsertBefore(result_ctor_instruction, CreateLdLocInstruction(iLProcessor, varDefs[i].Index));
                }
            }

            if (serializableMembers.Members.Length > 0)
            {
                var indexSequences = GetIndexSequences(serializableMembers.Indices);
                var switchSequence = MakeTree(indexSequences);

                EmitSwitchStatement(switchSequence,
                                    ref jumpTargets,
                                    ref iLProcessor,
                                    ref loopStart_instruction,
                                    ref loopEnd_instruction,
                                    i_varDef.Index);
                loopBack_instruction.Operand = loopStart_instruction.Next;
            }

            return methodDef;
        }

        private static void EmitSwitchStatement(IndexSequence indexSequence, ref Instruction[] jumpTargets, ref ILProcessor iLProcessor, ref Instruction insertPoint, ref Instruction defaultInstruction, int i_index)
        {
            if (indexSequence.HasChildNode)
            {
                EmitSwitchStatement(indexSequence.RightNode, ref jumpTargets, ref iLProcessor, ref insertPoint, ref defaultInstruction, i_index);
                EmitSwitchStatement(indexSequence.LeftNode, ref jumpTargets, ref iLProcessor, ref insertPoint, ref defaultInstruction, i_index);

                indexSequence.StartingPoint = CreateLdLocInstruction(iLProcessor, i_index);
                iLProcessor.InsertAfter(insertPoint, iLProcessor.Create(OpCodes.Bgt, indexSequence.RightNode.StartingPoint));
                iLProcessor.InsertAfter(insertPoint, iLProcessor.CreateLoadIntegerInstruction(indexSequence.LeftNode.LastIndex));
                iLProcessor.InsertAfter(insertPoint, indexSequence.StartingPoint);
            }
            else
            {
                Instruction end_instruction = null;
                var instructionStack = new Stack<Instruction>();
                for (int i = 0; i < indexSequence.Sequences.Length; i++)
                {
                    var sequence = indexSequence.Sequences[i];
                    var startingPoint = CreateLdLocInstruction(iLProcessor, i_index);
                    instructionStack.Push(startingPoint);

                    if (i == 0)
                    {
                        indexSequence.StartingPoint = startingPoint;
                    }

                    CreateBranchInstruction(ref instructionStack, ref iLProcessor, ref startingPoint, sequence.StartIndex, sequence.TotalIndicesCount, jumpTargets);
                }

                foreach (var instruction in instructionStack)
                {
                    iLProcessor.InsertAfter(insertPoint, instruction);
                }

                end_instruction = instructionStack.Peek();
                if (defaultInstruction != null)
                {
                    iLProcessor.InsertAfter(end_instruction, iLProcessor.Create(OpCodes.Br, defaultInstruction));
                }
            }
        }

        private static void CreateBranchInstruction(ref Stack<Instruction> stack, ref ILProcessor iLProcessor, ref Instruction insertPoint, int startIndex, int totalIndicesCount, Span<Instruction> jumpTargets)
        {
            if (startIndex == 0)
            {
                if (totalIndicesCount == 1)
                {
                    stack.Push(iLProcessor.Create(OpCodes.Brfalse, jumpTargets[startIndex]));
                }
                else
                {
                    var jumpTarget = jumpTargets.Slice(startIndex, totalIndicesCount).ToArray();
                    stack.Push(iLProcessor.Create(OpCodes.Switch, jumpTarget));
                }
            }
            else
            {
                stack.Push(iLProcessor.CreateLoadIntegerInstruction(startIndex));
                if (totalIndicesCount == 1)
                {
                    stack.Push(iLProcessor.Create(OpCodes.Beq, jumpTargets[startIndex]));
                }
                else
                {
                    stack.Push(iLProcessor.Create(OpCodes.Sub));
                    var jumpTarget = jumpTargets.Slice(startIndex, totalIndicesCount).ToArray();
                    stack.Push(iLProcessor.Create(OpCodes.Switch, jumpTarget));
                }
            }
        }

        private static VariableDefinition EmitTypeDefaultValue(ILProcessor iLProcessor, TypeReference typeRef, bool storeAsVariable = false, Instruction insertBefore = null)
        {
            bool isPrimitiveType = IsPrimitiveType(typeRef);
            bool isEnum = typeRef.Resolve().IsEnum;

            VariableDefinition temp_varDef = null;
            if (storeAsVariable)
            {
                temp_varDef = new VariableDefinition(typeRef);
                iLProcessor.Body.Variables.Add(temp_varDef);
            }

            if (isPrimitiveType || isEnum)
            {
                var defaultValue_instruction = CreatePrimitiveTypeDefaultValue(iLProcessor, typeRef);

                if (insertBefore == null)
                {
                    iLProcessor.Append(defaultValue_instruction);
                    if (storeAsVariable)
                    {
                        iLProcessor.Append(CreateStLocInstruction(iLProcessor, temp_varDef.Index));
                    }
                }
                else
                {
                    iLProcessor.InsertBefore(insertBefore, defaultValue_instruction);
                    if (storeAsVariable)
                    {
                        iLProcessor.InsertBefore(insertBefore, CreateStLocInstruction(iLProcessor, temp_varDef.Index));
                    }
                }

            }
            else if (typeRef.IsValueType)
            {
                if (temp_varDef == null)
                {
                    temp_varDef = new VariableDefinition(typeRef);
                    iLProcessor.Body.Variables.Add(temp_varDef);
                }

                if (insertBefore == null)
                {
                    iLProcessor.Append(CreateLdLocaInstruction(iLProcessor, temp_varDef.Index));
                    iLProcessor.Emit(OpCodes.Initobj, typeRef);
                    if (!storeAsVariable)
                    {
                        iLProcessor.Append(CreateLdLocInstruction(iLProcessor, temp_varDef.Index));
                    }
                }
                else
                {
                    iLProcessor.InsertBefore(insertBefore, CreateLdLocaInstruction(iLProcessor, temp_varDef.Index));
                    iLProcessor.InsertBefore(insertBefore, iLProcessor.Create(OpCodes.Initobj, typeRef));
                    if (!storeAsVariable)
                    {
                        iLProcessor.InsertBefore(insertBefore, CreateLdLocInstruction(iLProcessor, temp_varDef.Index));
                    }
                }
            }
            else
            {

                if (insertBefore == null)
                {
                    iLProcessor.Emit(OpCodes.Ldnull);
                    if (storeAsVariable)
                    {
                        iLProcessor.Append(CreateStLocInstruction(iLProcessor, temp_varDef.Index));
                    }
                }
                else
                {
                    iLProcessor.InsertBefore(insertBefore, iLProcessor.Create(OpCodes.Ldnull));
                    if (storeAsVariable)
                    {
                        iLProcessor.InsertBefore(insertBefore, CreateStLocInstruction(iLProcessor, temp_varDef.Index));
                    }
                }
            }

            return temp_varDef;
        }

        private static Instruction CreatePrimitiveTypeDefaultValue(ILProcessor iLProcessor, TypeReference typeRef)
        {
            var typeDef = typeRef.Resolve();
            bool isEnum = typeDef.IsEnum;

            string parameterTypeName = typeRef.Name;
            if (isEnum)
            {
                parameterTypeName = typeDef.GetEnumUnderlyingType().Name;
            }

            if (LDC_I4_COMPATIBLE_TYPES.Contains(parameterTypeName))
            {
                return iLProcessor.Create(OpCodes.Ldc_I4_0);
            }
            else if (LDC_I8_COMPATIBLE_TYPES.Contains(parameterTypeName))
            {
                return iLProcessor.Create(OpCodes.Ldc_I8, 0L);
            }
            else if (parameterTypeName == "Single")
            {
                return iLProcessor.Create(OpCodes.Ldc_R4, 0f);
            }
            else if (parameterTypeName == "Double")
            {
                return iLProcessor.Create(OpCodes.Ldc_R8, 0.0);
            }

            throw new Exception($"{typeRef.Name} is not a primitive type");

        }

        private static MethodDefinition TryGetMatchingConstructor(TypeDefinition typeDef, Collection<ParameterDefinition> parameters)
        {
            var constructors_methodDef = TypeDefinitionRocks.GetConstructors(typeDef);
            var ctor = GetCtorWithSerializationConstructorAttribute(typeDef, constructors_methodDef);

            if (ctor == null)
            {
                ctor = GetCtorWithMatchingParameters(constructors_methodDef, parameters);

                if (ctor == null)
                {
                    ctor = constructors_methodDef.Where(m => m.Parameters.Count == 0).FirstOrDefault();
                }
            }
            else
            {
                if (ctor.Parameters.Count != MostParameterTypeMatch(ctor.Parameters, parameters))
                {
                    ctor = null;
                }
            }

            if (ctor == null && !typeDef.IsValueType)
            {
                throw new Exception($"Unable to find matching constructor for type {typeDef.FullName}");
            }

            return ctor;
        }

        private static MethodDefinition GetCtorWithSerializationConstructorAttribute(TypeDefinition typeDef, IEnumerable<MethodDefinition> constructors_methodDef)
        {
            var methods =
                from method in constructors_methodDef
                where method.CustomAttributes.Any(a => a.AttributeType.Name == "SerializationConstructorAttribute")
                select method;

            if (methods.Count() > 1)
            {
                throw new Exception($"There are multiple constructors with SerializationConstructorAttribute found for type {typeDef.FullName}");
            }

            return methods.FirstOrDefault();
        }

        private static MethodDefinition GetCtorWithMatchingParameters(IEnumerable<MethodDefinition> constructors_methodDef, Collection<ParameterDefinition> parameters)
        {
            var methods =
                    from method in constructors_methodDef
                    orderby MostParameterTypeMatch(method.Parameters, parameters) descending
                    select method;

            return methods.FirstOrDefault();
        }

        private static int MostParameterTypeMatch(Collection<ParameterDefinition> parameters1, Collection<ParameterDefinition> parameters2)
        {
            if (parameters1.Count > parameters2.Count) return 0;

            int result = 0;
            for (int i = 0; i < parameters1.Count; i++)
            {
                if (parameters1[i].ParameterType.Name == parameters2[i].ParameterType.Name)
                {
                    result++;
                }
            }

            return result;
        }

        private static (MethodDefinition method, List<int> parameterIndices) TryGetStringMatchingConstructor(TypeDefinition typeDef, string[] keys)
        {
            var constructors_methodDef = TypeDefinitionRocks.GetConstructors(typeDef);
            var ctor = GetCtorWithSerializationConstructorAttribute(typeDef, constructors_methodDef);
            (MethodDefinition method, List<int> parameterIndices) tuple = (null, null);

            if (ctor == null)
            {
                tuple = GetCtorWithMatchingNameParameters(constructors_methodDef, keys);

                if (tuple.method == null)
                {
                    tuple.method = constructors_methodDef.Where(m => m.Parameters.Count == 0).FirstOrDefault();
                }
            }
            else
            {
                tuple.parameterIndices = MostParameterNameMatch(ctor.Parameters, keys);
                if (tuple.parameterIndices.Count == ctor.Parameters.Count)
                {
                    tuple.method = ctor;
                }
            }

            if (tuple.method == null && !typeDef.IsValueType)
            {
                throw new Exception($"Unable to find matching constructor for type {typeDef.FullName}");
            }

            return tuple;
        }

        private static (MethodDefinition method, List<int> parameterIndices) GetCtorWithMatchingNameParameters(IEnumerable<MethodDefinition> constructors_methodDef, string[] keys)
        {
            var tuples =
                from method in constructors_methodDef
                select (method, MostParameterNameMatch(method.Parameters, keys));

            return
                (
                    from tuple in tuples
                    orderby tuple.Item2.Count descending
                    select tuple
                ).FirstOrDefault();
        }

        private static List<int> MostParameterNameMatch(Collection<ParameterDefinition> parameters1, string[] keys)
        {
            List<int> parameterIds = new List<int>();

            if (parameters1.Count <= keys.Length)
            {
                var methodParametersName = parameters1.Select(p => p.Name.ToLower()).ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    int parameterIndex = Array.IndexOf(methodParametersName, keys[i].ToLower());
                    if (parameterIndex != -1)
                    {
                        parameterIds.Add(parameterIndex);
                    }
                }
            }

            return parameterIds;
        }

        private static MethodDefinition GetCtorWithLeastNonPrimitiveType(TypeReference typeRef)
        {
            var typeDef = typeRef.Resolve();
            return typeDef.Methods
                .Where(m => m.Name == ".ctor")
                .OrderBy(GetNonPrimitiveTypeRequiredCount)
                .FirstOrDefault();
        }

        private static bool IsPrimitiveType(TypeReference typeRef)
        {
            return PRIMITIVE_TYPES_DEFINITION.Contains(typeRef.Name);
        }

        private static int GetNonPrimitiveTypeRequiredCount(MethodDefinition methodDef)
        {
            int nonPrimitiveTypeMemberCount = 0;
            foreach (var parameter in methodDef.Parameters)
            {
                if (IsPrimitiveType(parameter.ParameterType))
                {
                    nonPrimitiveTypeMemberCount++;
                }
            }

            return nonPrimitiveTypeMemberCount;
        }

        private static MethodDefinition GenerateOtherMethod(ModuleDefinition mainModule, MethodDefinition template_methodDef)
        {
            var methodDef = new MethodDefinition(template_methodDef.Name, template_methodDef.Attributes, template_methodDef.ReturnType);

            for (int i = 0; i < template_methodDef.Parameters.Count; i++)
            {
                var parameter = template_methodDef.Parameters[i];
                methodDef.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.ImportReference(parameter.ParameterType)));
            }

            var ilProcessor = methodDef.Body.GetILProcessor();
            foreach (var instruction in template_methodDef.Body.Instructions)
            {
                switch (instruction.Operand)
                {
                    case TypeReference typeRef:
                    ilProcessor.Emit(instruction.OpCode, mainModule.ImportReference(typeRef));
                    break;

                    case MethodReference methodRef:
                    ilProcessor.Emit(instruction.OpCode, mainModule.ImportReference(methodRef));
                    break;

                    default:
                    ilProcessor.Append(instruction);
                    break;
                }
            }

            foreach (var variable in template_methodDef.Body.Variables)
            {
                mainModule.ImportReference(variable.VariableType);
                methodDef.Body.Variables.Add(new VariableDefinition(variable.VariableType));
            }

            return methodDef;
        }

        internal static IndexSequence[] GetIndexSequences(int[] array)
        {
            var indexSequences = new List<IndexSequence>();

            for (int i = 0; i < array.Length;)
            {
                int startIndex = array[i];
                int lastIndex = array[i];
                int switchableLength = 1;
                int defaultCases = 0;

                int j = i;

                for (; j < array.Length - 1; j++)
                {
                    int currentIndex = array[j];
                    int nextIndex = array[j + 1];
                    int totalIndicesCount = nextIndex - startIndex + 1;
                    defaultCases += nextIndex - currentIndex - 1;
                    int maxDefaultCases = (totalIndicesCount + 1) / 2;
                    int nonDefaultCases = j - i + 2;

                    bool switchable = defaultCases < maxDefaultCases
                        && totalIndicesCount > 2
                        && nonDefaultCases > 2;

                    if (j + 1 == array.Length - 1)
                    {
                        lastIndex = nextIndex;
                    }
                    else
                    {
                        lastIndex = currentIndex;
                    }

                    if (switchable)
                    {
                        switchableLength = totalIndicesCount;
                    }

                    if (switchableLength > 1 && !switchable
                        || (switchableLength == 1 && !switchable && defaultCases > maxDefaultCases))
                        break;
                }

                if (switchableLength > 1)
                {
                    var indexSequence = new IndexSequence
                    {
                        StartIndex = startIndex,
                        TotalIndicesCount = switchableLength,
                        LastIndex = lastIndex,
                    };
                    indexSequences.Add(indexSequence);

                    i = j + 1;
                }
                else
                {
                    while (i <= j)
                    {
                        var indexSequence = new IndexSequence
                        {
                            StartIndex = array[i],
                            TotalIndicesCount = 1,
                            LastIndex = array[i],
                        };
                        indexSequences.Add(indexSequence);

                        i++;
                    }
                }
            }

            return indexSequences.ToArray();
        }

        internal static IndexSequence MakeTree(Span<IndexSequence> sequenceSpan)
        {
            var indexSequence = new IndexSequence();

            if (sequenceSpan.Length > 3)
            {
                int splitIndex = sequenceSpan.Length / 2;
                indexSequence.LeftNode = MakeTree(sequenceSpan.Slice(0, splitIndex));
                indexSequence.RightNode = MakeTree(sequenceSpan.Slice(splitIndex));

                indexSequence.StartIndex = indexSequence.LeftNode.StartIndex;
                indexSequence.LastIndex = indexSequence.RightNode.LastIndex;
            }
            else
            {
                indexSequence.StartIndex = sequenceSpan[0].StartIndex;
                indexSequence.LastIndex = sequenceSpan[sequenceSpan.Length - 1].LastIndex;

                indexSequence.Sequences = sequenceSpan.ToArray();
            }

            return indexSequence;
        }
    }
}