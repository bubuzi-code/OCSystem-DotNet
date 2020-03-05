using OnlyChain.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using OnlyChain.Network.Objects;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using OnlyChain.Network;
using System.Security.Cryptography;

namespace OnlyChain {
    class Program {
        static string[] messages;

        unsafe static void Test() {
            Size256 a = default, b = default;

            var sw = new System.Diagnostics.Stopwatch();
            //sw.Restart();
            //for (int i = 0; i < 10000000; i++) {
            //    a.Equals(b);
            //}
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
            //Console.WriteLine(a.Equals(b));

            sw.Restart();
            for (int i = 0; i < 1000000000; i++) {
                new ReadOnlySpan<byte>(&a, sizeof(Size256)).SequenceEqual(new ReadOnlySpan<byte>(&b, sizeof(Size256)));
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.WriteLine(new ReadOnlySpan<byte>(&a, sizeof(Size256)).SequenceEqual(new ReadOnlySpan<byte>(&b, sizeof(Size256))));
        }


        static void Main(string[] args) {
            var sw = new System.Diagnostics.Stopwatch();
            var keys = new Address[100000];
            for (int i = 0; i < keys.Length; i++) keys[i] = Address.Random();
            //var memSize = GC.GetTotalMemory(true);

            // 019251639cb90c9d59f2108524d377315d781caf
            // 8750f447c8355658a4724a129592b5f2c21b87dc
            // 0f77cce7599bf8bb0b49f00c378410d861bad2af
            //keys[0] = "019251639cb90c9d59f2108524d377315d781caf";
            //keys[1] = "8750f447c8355658a4724a129592b5f2c21b87dc";
            //keys[2] = "0f77cce7599bf8bb0b49f00c378410d861bad2af";

            var tree = new MerklePatriciaTree<Address, string>();
            sw.Restart();
            for (int i = 0; i < keys.Length; i++) {
                tree.Add(keys[i], keys[i].ToString());
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            //Console.WriteLine((GC.GetTotalMemory(true) - memSize) / 1024.0 / 1024.0);


            sw.Restart();
            for (int i = 0; i < keys.Length; i++) {
                if (!tree.TryGetValue(keys[i], out _)) throw new Exception();
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            sw.Restart();
            foreach (var kv in tree) ;
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            sw.Restart();
            for (int i = 0; i < keys.Length; i++) {
                if (!tree.Remove(keys[i])) throw new Exception();
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine(tree.Count);

            Console.WriteLine("====================================");

            Thread.Sleep(-1);
            //Console.WriteLine(tree.Count);
            return;



            //var sw = new System.Diagnostics.Stopwatch();
            //for (int i = 0; i < 10000; i++) {
            //    var h = Ripemd160.ComputeHash(new Hash<Size256>());
            //}
            //var hash256 = new Hash<Size256>("0000000000000000000000000000000000000000000000000000000000000001");
            //sw.Restart();
            //for (int i = 0; i < 200_0000; i++) {
            //    Ripemd160.ComputeHash(hash256);
            //}
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
            //Console.WriteLine(Ripemd160.ComputeHash("0000000000000000000000000000000000000000000000000000000000000001"));
            //return;



            //var clients = new List<Client>();
            //var bindIP = IPAddress.Parse("127.0.0.1");
            //clients.Add(new Client(Address.Random(), 0, bindIP: bindIP));
            //clients[^1].ReceiveBroadcast += Program_ReceiveBroadcast(clients.Count);
            //Console.WriteLine($"{clients.Count,5} create: {clients[^1].Address}");
            //for (int i = 0; i < 1000; i++) {
            //    await Task.Delay(10);
            //    clients.Add(new Client(Address.Random(), 0, bindIP: bindIP, seeds: new[] { new IPEndPoint(bindIP, clients[0].Port) }));
            //    clients[^1].ReceiveBroadcast += Program_ReceiveBroadcast(clients.Count);
            //    Console.WriteLine($"{clients.Count,5} create: {clients[^1].Address}");
            //}
            //await Task.Delay(200);
            //clients.Add(new Client("dbaaf68ee499766bdc548e324cdd204e3a563f2c", 0, bindIP: bindIP, seeds: new[] { new IPEndPoint(bindIP, clients[0].Port) }));
            //clients[^1].ReceiveBroadcast += Program_ReceiveBroadcast(clients.Count);
            //Console.WriteLine($"{clients.Count,5} create: {clients[^1].Address}");



            //Address cmpAddress = clients[0].Address;
            //clients.Sort(Comparer<Client>.Create((a, b) => (b.Address ^ cmpAddress).CompareTo(a.Address ^ cmpAddress)));
            //foreach (var c in clients) Console.WriteLine(c.Address);

            //foreach (var c in clients.Skip(5)) {
            //    await c.DisposeAsync();
            //}
            //clients.RemoveRange(5, clients.Count - 5);

            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Restart();
            //await clients[0].DisposeAsync();
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);

            //await Task.Delay(2000);

            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Restart();
            //var node = await clients[500].Lookup("dbaaf68ee499766bdc548e324cdd204e3a563f2c");
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
            //Console.WriteLine(node);
            //return;

            //int broadcastCount = 0;
            //while (true) {
            //    await Task.Delay(1000);
            //    if (messages != null) {
            //        Console.WriteLine($"======================================= {broadcastCount}");
            //        for (int i = 1; i < messages.Length; i++) {
            //            if (messages[i] is null) {
            //                Console.WriteLine($"{i,4} miss: {clients[i].Address}");
            //            }
            //        }
            //    }
            //    messages = new string[clients.Count];
            //    clients[0].Broadcast(Encoding.UTF8.GetBytes("hello"));
            //    broadcastCount++;
            //}

            //await Task.Delay(10);
            //Console.WriteLine("=======================================");
            //foreach (var n in clients[^1].Nodes) {
            //    Console.WriteLine(n);
            //}
            //Console.WriteLine("=======================================");
            //Console.WriteLine(clients[^1].Address);
            //var random = new Random();
            //var randomAddress = new byte[Address.Size];
            //random.NextBytes(randomAddress);
            //for (int i = 0; i < 16; i++) {
            //    Console.WriteLine($"=======================================");
            //    randomAddress[0] = (byte)(i << 4);
            //    Address target = new Address(randomAddress);
            //    Console.WriteLine($"find {target}");
            //    foreach (var n in clients[^1].Nodes.FindNode(target, 10)) {
            //        Console.WriteLine(n);
            //    }
            //}

            //await Task.Delay(-1);

            //Console.WriteLine(clients.Count);
        }

        private static EventHandler<BroadcastEventArgs> Program_ReceiveBroadcast(int index) {
            return (object sender, BroadcastEventArgs e) => {
                Client client = (Client)sender;
                string message = Encoding.UTF8.GetString(e.Message);
                //Console.WriteLine($"{index,5}, {client.Address}: {message}");
                messages[index - 1] = message;
            };
        }
    }
}
