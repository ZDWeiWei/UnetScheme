﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine;
using UnityEngine.Networking;
using Zyq.Game.Base;

namespace Zyq.Weaver {
    public static class SendProcessor {
        public static void Weave(ModuleDefinition module, Dictionary<short, MethodDefinition> defs) {
            foreach (short key in defs.Keys) {
                MethodDefinition method = defs[key];
                ILProcessor processor = method.Body.GetILProcessor();

                method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(NetworkWriter))));

                Instruction first = method.Body.Instructions[0];

                processor.InsertBefore(first, processor.Create(OpCodes.Nop));

                processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(NetworkWriter).GetConstructor(Type.EmptyTypes))));
                processor.InsertBefore(first, processor.Create(OpCodes.Stloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldc_I4, key));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkWriter).GetMethod("StartMessage", new Type[] { typeof(short) }))));

                int index = 0;
                foreach (ParameterDefinition pd in method.Parameters) {
                    if (index > 0) {
                        processor.InsertBefore(first, processor.Create(OpCodes.Nop));

                        processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                        processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_S, pd));
                        processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkWriter).GetMethod("Write", new Type[] { Type.GetType(pd.ParameterType.ToString()) }))));
                    }
                    index++;
                }

                processor.InsertBefore(first, processor.Create(OpCodes.Nop));

                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Connection).GetMethod("Send", new Type[] { typeof(NetworkWriter) }))));
            }
        }
    }
}