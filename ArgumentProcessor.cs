using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MontyHallSimulator
{
    internal class ArgumentProcessor
    {
        public static ParseResults Parse(string[] args, int defaultTrials, int defaultThreads)
        {
            int numberOfTrials = defaultTrials;
            int numberOfTreads = defaultThreads;
            bool badTrials = false;
            bool badThreads = false;

            ParseResults results = new ParseResults();
            
            if (args.Length == 0)
            {
                results = new ParseResults(true, numberOfTrials, numberOfTreads);
            }
            else // User has supplied the number of trials.
            {
                bool gotIt = int.TryParse(args[0], out numberOfTrials);
                gotIt &= (numberOfTrials > 0) ? true : false;

                if (!gotIt)
                {
                    badTrials = true;
                }
            }

            if (args.Length == 2)
            {
                bool gotIt = int.TryParse(args[1], out numberOfTreads);
                gotIt &= (numberOfTreads > 0) ? true : false;

                if (gotIt)
                {
                    numberOfTreads = Math.Min(Environment.ProcessorCount / 2, numberOfTreads);
                    results = new ParseResults(numberOfTrials, numberOfTreads);
                }
            }

            return results;
        }
    }

    internal class ParseResults
    {
        /// <summary>
        /// Gets if the incoming arugments where successfully parsed.
        /// </summary>
        public bool ValidArguments
        {
            get
            {
                return NumberOfThreads.HasValue & NumberOfTrials.HasValue;
            }
        }

        public bool InvalidNumberOfTrials
        {
            get { return !NumberOfTrials.HasValue; }
        }

        public bool InvalidNumberOfThreads
        {
            get { return !NumberOfThreads.HasValue; }
        }

        public bool UsedDefaultValues { get; private set; }

        public int? NumberOfTrials { get; private set; }

        public int? NumberOfThreads { get; private set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public ParseResults()
        {
            UsedDefaultValues = false;
        }

        public ParseResults(int? numberOfTrials, int? numberOfThreads) :this()
        {
            NumberOfTrials = numberOfTrials;
            NumberOfThreads = numberOfThreads;
        }

        public ParseResults(bool usedDefaultValues, int? numberOfTrials, int? numberOfThreads) : this(numberOfTrials, numberOfThreads)
        {
            UsedDefaultValues = usedDefaultValues;
        }
    }
}
