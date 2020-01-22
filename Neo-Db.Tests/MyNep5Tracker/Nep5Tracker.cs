using System;
using System.Collections.Generic;
using System.Numerics;
using MyNep5Tracker.DB;
using Neo;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.Plugins;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;
using VMArray = Neo.VM.Types.Array;


namespace MyNep5Tracker
{
    public class Nep5Tracker : Plugin, IPersistencePlugin
    {

        private TrackDB _db = new TrackDB();
        public override void Configure()
        {
        }



        public void OnCommit(Snapshot snapshot)
        {
        }



        public void OnPersist(Snapshot snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList)
        {
            Header header = snapshot.GetHeader(snapshot.CurrentBlockHash);
            foreach (Blockchain.ApplicationExecuted appExecuted in applicationExecutedList)
            {
                foreach (var executionResults in appExecuted.ExecutionResults)
                {
                    // Executions that fault won't modify storage, so we can skip them.
                    if (executionResults.VMState.HasFlag(VMState.FAULT)) continue;
                    foreach (var notifyEventArgs in executionResults.Notifications)
                    {
                        if (!(notifyEventArgs?.State is VMArray stateItems)
                            || stateItems.Count == 0
                            || !(notifyEventArgs.ScriptContainer is Transaction transaction))
                        {
                            continue;
                        }

                        HandleNotification(snapshot, transaction, notifyEventArgs.ScriptHash, stateItems, header);
                    }
                }
            }

        }



        public bool ShouldThrowExceptionFromCommit(Exception ex)
        {
            return true;
        }


        private void HandleNotification(Snapshot snapshot, Transaction transaction, UInt160 scriptHash, VMArray stateItems, Header header)
        {
            if (stateItems.Count == 0) return;
            // Event name should be encoded as a byte array.
            if (!(stateItems[0] is ByteArray)) return;
            var eventName = stateItems[0].GetString();
            if (eventName != "Transfer") return;
            if (stateItems.Count < 4) return;

            var fromItem = stateItems[1];
            if (!(fromItem is ByteArray)) return;
            //if (!(fromItem is null) && !(fromItem is VM.Types.ByteArray)) return;

            var toItem = stateItems[2];
            if (toItem != null && !(toItem is ByteArray)) return;

            var amountItem = stateItems[3];
            if (!(amountItem is ByteArray || amountItem is Integer)) return;

            byte[] fromBytes = fromItem?.GetByteArray();
            if (fromBytes?.Length != 20) fromBytes = null;
            byte[] toBytes = toItem?.GetByteArray();
            if (toBytes?.Length != 20) toBytes = null;
            if (fromBytes == null && toBytes == null) return;
            var from = new UInt160(fromBytes);
            var to = new UInt160(toBytes);
            var amount = amountItem.GetBigInteger();
            var fromBalance = GetBalanceOf(from,scriptHash, snapshot);
            var toBalance = GetBalanceOf(to,scriptHash, snapshot);

            var record = new Nep5TransferInfo
            {
                BlockHeight = header.Index,
                From = from,
                FromBalance = fromBalance,
                To = to,
                ToBalance = toBalance,
                AssetId = scriptHash,
                Amount = amount,
                TxId = transaction.Hash,
                TimeStamp = header.Timestamp,
            };
            _db.Add(record);

            //Console.WriteLine($"[{from}:{fromBalance}]=>[{to}:{toBalance}]:{amount}");

        }


        public BigInteger GetBalanceOf(UInt160 address,UInt160 assetId, Snapshot snapshot)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitAppCall(assetId, "balanceOf",
                    address.ToArray());
                script = sb.ToArray();
            }

            ApplicationEngine engine = ApplicationEngine.Run(script, snapshot, testMode:true);
            if (engine.State.HasFlag(VMState.FAULT)) return 0;
            if (engine.ResultStack.Count <= 0) return 0;
            return  engine.ResultStack.Pop().GetBigInteger();
        }
    }
}
