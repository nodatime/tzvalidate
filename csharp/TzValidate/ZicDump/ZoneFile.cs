// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NodaTime.TzValidate.ZicDump
{
    /// <summary>
    /// Class representing the raw contents of one portion of a zic output file.
    /// (An actual zic file consists of multiple parts of a similar kind.)
    /// </summary>
    internal sealed class ZoneFile
    {
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private readonly long[] transitionTimestamps;
        private readonly byte[] transitionTypeIndexes;
        private readonly TransitionType[] transitionTypes;
        // We don't have anything about leap seconds or how the transition was
        // specified. We won't need that here.

        private ZoneFile(long[] transitionTimestamps, byte[] transitionTypeIndexes, TransitionType[] types)
        {
            this.transitionTimestamps = transitionTimestamps;
            this.transitionTypeIndexes = transitionTypeIndexes;
            this.transitionTypes = types;
        }

        internal static ZoneFile FromStream(Stream input)
        {
            var binaryReader = new BinaryReader(input);
            var signature = binaryReader.ReadBytes(20);
            if (!signature.Take(4).SequenceEqual(Encoding.ASCII.GetBytes("TZif")))
            {
                throw new InvalidDataException("Unexpected signature for zic output file");
            }
            if (signature[4] != '2' && signature[4] != '3')
            {
                throw new InvalidDataException("Expecting zic output version 2 or 3");
            }

            // Names as per man page for tzfile.
            int ttisgmtcnt = ReadInt32(binaryReader);
            int ttisstdcnt = ReadInt32(binaryReader);
            int leapcnt = ReadInt32(binaryReader);
            int timecnt = ReadInt32(binaryReader);
            int typecnt = ReadInt32(binaryReader);
            int charcnt = ReadInt32(binaryReader);

            // Skip the 32-bit-based values
            binaryReader.ReadBytes(timecnt * 4 + timecnt + typecnt * 6 + charcnt + leapcnt * 8 + ttisstdcnt + ttisgmtcnt);

            // Then read the second header
            binaryReader.ReadBytes(20); // Not sure why we've got a second signature, but...
            ttisgmtcnt = ReadInt32(binaryReader);
            ttisstdcnt = ReadInt32(binaryReader);
            leapcnt = ReadInt32(binaryReader);
            timecnt = ReadInt32(binaryReader);
            typecnt = ReadInt32(binaryReader);
            charcnt = ReadInt32(binaryReader);

            long[] transitionTimestamps = new long[timecnt];
            for (int i = 0; i < timecnt; i++)
            {
                transitionTimestamps[i] = ReadInt64(binaryReader);
            }

            var transitionTypeIndexes = binaryReader.ReadBytes(timecnt);

            var types = new TransitionType[typecnt];            
            for (int i = 0; i < typecnt; i++)
            {
                types[i] = new TransitionType
                {
                    Offset = ReadInt32(binaryReader),
                    IsDst = binaryReader.ReadByte() != 0,
                    AbbreviationIndex = binaryReader.ReadByte()
                };
            }
            byte[] abbreviations = binaryReader.ReadBytes(charcnt);
            // Ignore the rest of the data

            for (int i = 0; i < typecnt; i++)
            {
                types[i].Abbreviation = ResolveAbbreviation(abbreviations, types[i].AbbreviationIndex);
            }
            return new ZoneFile(transitionTimestamps, transitionTypeIndexes, types);
        }

        internal IEnumerable<Transition> GetTransitions(Options options)
        {
            if (transitionTypes.Length == 0)
            {
                throw new Exception("No transition types, so unable to get any data at all");
            }
            var lowerBound = (long) (DateTimeOffset.MinValue - UnixEpoch).TotalSeconds;
            var transitions = transitionTimestamps.Zip(transitionTypeIndexes,
                (tt, tti) => new
                {
                    timestamp = tt < lowerBound ? (DateTimeOffset?) null : UnixEpoch.AddSeconds(tt),
                    type = transitionTypes[tti]
                })
                .Select(pair => new Transition(pair.timestamp, TimeSpan.FromSeconds(pair.type.Offset), pair.type.IsDst, pair.type.Abbreviation))
                .ToList();
            if (options.FakeInitialTransition && transitions.Count > 0 && transitions[0].Instant != null)
            {
                // Note: in some very odd cases (e.g. tzdata94f, MET) the first transition is into standard time.
                // We'll still add one for the big bang here, and let the next part de-dupe.

                // Work out the initial type according to the localtime man page:
                // - First standard type (by transition type array) if there are any
                // - First type if not
                var initialType = transitionTypes.FirstOrDefault(t => !t.IsDst) ?? transitionTypes[0];
                transitions.Insert(0, new Transition(null, TimeSpan.FromSeconds(initialType.Offset), initialType.IsDst, initialType.Abbreviation));
            }
            Transition previousTransition = null;
            foreach (var transition in transitions)
            {
                // There may be types which only differ in ways we don't care about, e.g. whether
                // the transition is specified as wall, standard or UTC.
                if (previousTransition?.Abbreviation == transition.Abbreviation &&
                    previousTransition.Offset == transition.Offset &&
                    previousTransition.IsDaylight == transition.IsDaylight)
                {
                    continue;
                }
                previousTransition = transition;

                if (transition.Instant != null)
                {
                    var year = transition.Instant.Value.Year;
                    if (options.FromYear != null && year < options.FromYear.Value)
                    {
                        continue;
                    }
                    if (year >= options.ToYear)
                    {
                        yield break;
                    }
                }

                yield return transition;
            }
        }

        private static string ResolveAbbreviation(byte[] abbreviations, int index)
        {
            for (int i = index; i < abbreviations.Length; i++)
            {
                if (abbreviations[i] == 0)
                {
                    return Encoding.ASCII.GetString(abbreviations, index, i - index);
                }
            }
            return Encoding.ASCII.GetString(abbreviations, index, abbreviations.Length - index);
        }

        private static int ReadInt32(BinaryReader reader)
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                result = (result << 8) + reader.ReadByte();
            }
            return result;
        }

        private static long ReadInt64(BinaryReader reader)
        {
            long result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = (result << 8) + reader.ReadByte();
            }
            return result;
        }

        private class TransitionType
        {
            internal int Offset { get; set; }
            internal bool IsDst { get; set; }
            internal int AbbreviationIndex { get; set; }
            internal string Abbreviation { get; set; }
        }
    }
}
