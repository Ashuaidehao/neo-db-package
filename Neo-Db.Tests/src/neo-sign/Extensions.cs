using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using ECCurve = Neo.Cryptography.ECC.ECCurve;
using ECPoint = Neo.Cryptography.ECC.ECPoint;

namespace Neo
{
    public static class Extensions
    {
        /// <summary>
        /// text string is null or whitespaces=>true
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNull(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }


        /// <summary>
        /// text string is not null nor whitespaces=>true
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool NotNull(this string text)
        {
            return !IsNull(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="input"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(this HashAlgorithm hashAlgorithm, byte[] input, int times = 1)
        {
            byte[] buffer = input;
            for (int i = 0; i < times; i++)
            {
                buffer = hashAlgorithm.ComputeHash(input);
            }
            return buffer;
        }

        public static ContractParametersContext.ContextItem GetOrCreateItem(this ContractParametersContext context,
            Contract contract)
        {
            if (context.ContextItems.ContainsKey(contract.ScriptHash))
            {
                return context.ContextItems[contract.ScriptHash];
            }
            var item = new ContractParametersContext.ContextItem(contract);
            context.ContextItems[contract.ScriptHash] = item;
            return item;
        }

        public static bool SetParameter(this ContractParametersContext context,Contract contract, int index, object parameter)
        {
            var item = context.GetOrCreateItem(contract);
            item.Parameters[index].Value = parameter;
            return true;
        }

        public static bool AddSign(this ContractParametersContext context, Contract contract, ECPoint pubkey, byte[] signature)
        {
            if (contract.Script.IsMultiSigContract())
            {
                var item = new ContractParametersContext.ContextItem(contract);
                if (item.Parameters.All(p => p.Value != null)) return false;
                if (item.Signatures == null)
                    item.Signatures = new Dictionary<ECPoint, byte[]>();
                else if (item.Signatures.ContainsKey(pubkey))
                    return false;
                List<ECPoint> points = new List<ECPoint>();
                {
                    int i = 0;
                    switch (contract.Script[i++])
                    {
                        case 1:
                            ++i;
                            break;
                        case 2:
                            i += 2;
                            break;
                    }
                    while (contract.Script[i++] == 33)
                    {
                        points.Add(ECPoint.DecodePoint(contract.Script.Skip(i).Take(33).ToArray(), ECCurve.Secp256r1));
                        i += 33;
                    }
                }
                if (!points.Contains(pubkey)) return false;
                item.Signatures.Add(pubkey, signature);
                if (item.Signatures.Count == contract.ParameterList.Length)
                {
                    Dictionary<ECPoint, int> dic = points.Select((p, i) => new
                    {
                        PublicKey = p,
                        Index = i
                    }).ToDictionary(p => p.PublicKey, p => p.Index);
                    byte[][] sigs = item.Signatures.Select(p => new
                    {
                        Signature = p.Value,
                        Index = dic[p.Key]
                    }).OrderByDescending(p => p.Index).Select(p => p.Signature).ToArray();
                    for (int i = 0; i < sigs.Length; i++)
                    {
                        if (!context.SetParameter(contract, i, sigs[i]))
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    item.Signatures = null;
                }
                return true;
            }
            else
            {
                int index = -1;
                for (int i = 0; i < contract.ParameterList.Length; i++)
                    if (contract.ParameterList[i] == ContractParameterType.Signature)
                        if (index >= 0)
                            throw new NotSupportedException();
                        else
                            index = i;

                if (index == -1)
                {
                    // unable to find ContractParameterType.Signature in contract.ParameterList 
                    // return now to prevent array index out of bounds exception
                    return false;
                }
                return context.SetParameter(contract, index, signature);
            }
        }


        public static Witness[] GetWitness(this ContractParametersContext context, List<UInt160> scripthashes)
        {
            Witness[] witnesses = new Witness[scripthashes.Count];
            for (int i = 0; i < scripthashes.Count; i++)
            {
                ContractParametersContext.ContextItem item = context.ContextItems[scripthashes[i]];
                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    foreach (ContractParameter parameter in item.Parameters.Reverse())
                    {
                        sb.EmitPush(parameter);
                    }
                    witnesses[i] = new Witness
                    {
                        InvocationScript = sb.ToArray(),
                        VerificationScript = item.Script ?? new byte[0]
                    };
                }
            }
            return witnesses;
        }
    }
}
