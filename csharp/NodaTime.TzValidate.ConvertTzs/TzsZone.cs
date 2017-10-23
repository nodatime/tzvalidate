// Copyright 2016 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;

namespace NodaTime.TzValidate.ConvertTzs
{
    public class TzsZone
    {
        public string Id { get; }
        public IEnumerable<Transition> Transitions { get; }

        public TzsZone(string id, IEnumerable<Transition> transitions)
        {
            Id = id;
            Transitions = transitions;
        }

        internal IEnumerable<Transition> GetTransitions(Options options)
        {
            Transition previousTransition = null;
            foreach (var transition in Transitions)
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
    }
}
