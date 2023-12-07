using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day16
    {
        internal static void Problem1()
        {
            int sum = 0;

            foreach (Packet p in Packet.AllPackets())
            {
                Console.WriteLine($"Packet version {p.Version}; type {p.TypeId}");
                sum += p.Version;
            }

            Console.WriteLine(sum);
        }

        internal static void Problem2()
        {
            Packet root = Packet.RootPacket();

            Console.WriteLine(root.Evaluate());
        }

        private class Packet
        {
            public int Version { get; private set; }
            public int TypeId { get; private set; }
            public long Value { get; private set; }
            public List<Packet> ChildPackets { get; private set; }

            public static IEnumerable<Packet> AllPackets()
            {
                string line = Data.Enumerate().Single();
                BitStream bitStream = new BitStream(Convert.FromHexString(line));

                foreach (Packet pkt in ReadNextPacket(bitStream))
                {
                    yield return pkt;
                }
            }

            public static Packet RootPacket()
            {
                string line = Data.Enumerate().Single();
                BitStream bitStream = new BitStream(Convert.FromHexString(line));
                return ReadStructuredPacket(bitStream);
            }

            private static IEnumerable<Packet> ReadNextPacket(BitStream bitStream)
            {
                int version = bitStream.TakeBits(3);
                int typeId = bitStream.TakeBits(3);

                if (typeId == 4)
                {
                    int tmp;
                    BigInteger bigInteger = new BigInteger();

                    do
                    {
                        tmp = bitStream.TakeBits(5);
                        bigInteger <<= 4;
                        bigInteger |= (tmp & 0b0000_1111);
                    } while ((tmp & 0b0001_0000) != 0);

                    yield return new Packet
                    {
                        Version = version,
                        TypeId = typeId,
                        Value = (long)bigInteger,
                    };
                }
                else
                {
                    int lengthTypeId = bitStream.TakeBits(1);

                    if (lengthTypeId == 0)
                    {
                        int subRange = bitStream.TakeBits(15);
                        int stopAt = bitStream.BitsRemaining - subRange;

                        yield return new Packet
                        {
                            Version = version,
                            TypeId = typeId,
                        };

                        while (bitStream.BitsRemaining > stopAt)
                        {
                            foreach (Packet sub in ReadNextPacket(bitStream))
                            {
                                yield return sub;
                            }
                        }

                        Debug.Assert(bitStream.BitsRemaining == stopAt);
                    }
                    else
                    {
                        int subPackets = bitStream.TakeBits(11);

                        yield return new Packet
                        {
                            Version = version,
                            TypeId = typeId,
                        };

                        for (int sub = 0; sub < subPackets; sub++)
                        {
                            foreach (Packet subPkt in ReadNextPacket(bitStream))
                            {
                                yield return subPkt;
                            }
                        }
                    }
                }
            }

            private static Packet ReadStructuredPacket(BitStream bitStream)
            {
                int version = bitStream.TakeBits(3);
                int typeId = bitStream.TakeBits(3);

                if (typeId == 4)
                {
                    int tmp;
                    BigInteger bigInteger = new BigInteger();

                    do
                    {
                        tmp = bitStream.TakeBits(5);
                        bigInteger <<= 4;
                        bigInteger |= (tmp & 0b0000_1111);
                    } while ((tmp & 0b0001_0000) != 0);

                    return new Packet
                    {
                        Version = version,
                        TypeId = typeId,

                        Value = (long)bigInteger,
                    };
                }
                else
                {
                    int lengthTypeId = bitStream.TakeBits(1);

                    if (lengthTypeId == 0)
                    {
                        int subRange = bitStream.TakeBits(15);
                        int stopAt = bitStream.BitsRemaining - subRange;

                        List<Packet> children = new List<Packet>();

                        while (bitStream.BitsRemaining > stopAt)
                        {
                            children.Add(ReadStructuredPacket(bitStream));
                        }

                        Debug.Assert(bitStream.BitsRemaining == stopAt);

                        return new Packet
                        {
                            Version = version,
                            TypeId = typeId,

                            ChildPackets = children,
                        };
                    }
                    else
                    {
                        int subPackets = bitStream.TakeBits(11);
                        List<Packet> children = new List<Packet>(subPackets);

                        for (int sub = 0; sub < subPackets; sub++)
                        {
                            children.Add(ReadStructuredPacket(bitStream));
                        }

                        return new Packet
                        {
                            Version = version,
                            TypeId = typeId,

                            ChildPackets = children,
                        };
                    }
                }
            }

            public long Evaluate()
            {
                return TypeId switch
                {
                    4 => Value,
                    0 => ChildPackets.Sum(cp => cp.Evaluate()),
                    1 => ChildPackets.Product(cp => cp.Evaluate()),
                    2 => ChildPackets.Min(cp => cp.Evaluate()),
                    3 => ChildPackets.Max(cp => cp.Evaluate()),
                    5 => ChildPackets[0].Evaluate() > ChildPackets[1].Evaluate() ? 1 : 0,
                    6 => ChildPackets[0].Evaluate() < ChildPackets[1].Evaluate() ? 1 : 0,
                    7 => ChildPackets[0].Evaluate() == ChildPackets[1].Evaluate() ? 1 : 0,
                    _ => throw new InvalidOperationException(),
                };
            }
        }
    }

    public class BitStream
    {
        private BitArray _bitArray;
        private int _offset;

        public BitStream(byte[] dataStream)
        {
            BitArray bitArray = new BitArray(dataStream.Length * 8);

            int writePos = 0;

            foreach (byte b in dataStream)
            {
                for (int i = 7; i >= 0; i--)
                {
                    if ((b & (1 << i)) != 0)
                    {
                        bitArray.Set(writePos, true);
                    }

                    writePos++;
                }
            }

            //BitArray reversed = new BitArray(bitArray.Length);

            //for (int i = 0; i < bitArray.Length; i++)
            //{
            //    reversed.Set(bitArray.Length - 1 - i, bitArray.Get(i));
            //}

            _bitArray = bitArray;
        }

        public int BitsRemaining => _bitArray.Length - _offset;

        public int ReadBits(int bitCount)
        {
            Debug.Assert(bitCount >= 0);
            Debug.Assert(bitCount <= BitsRemaining);
            Debug.Assert(bitCount <= 32);

            int accum = 0;

            for (int i = 0; i < bitCount; i++)
            {
                accum <<= 1;

                if (_bitArray.Get(i + _offset))
                {
                    accum |= 1;
                }
            }

            return accum;
        }

        public int TakeBits(int bitCount)
        {
            int ret = ReadBits(bitCount);
            _offset += bitCount;
            return ret;
        }
    }
}
