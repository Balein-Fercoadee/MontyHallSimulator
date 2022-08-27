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

            bool gotIt = false;

            switch (args.Length)
            {
                case 0: // nothing was provided, use the default values
                    results = new ParseResults(0, numberOfTrials, numberOfTreads);
                    break;

                case 1: // one value was provided, assume it's number of trials
                    gotIt = int.TryParse(args[0], out numberOfTrials);
                    gotIt &= (numberOfTrials > 0);

                    if (gotIt)
                    {
                        results = new ParseResults(1, numberOfTrials, numberOfTreads);
                    }
                    else
                        badTrials = true;
                    break;

                case 2: // both number of trials and threads was provided
                    gotIt = int.TryParse(args[0], out numberOfTrials);
                    gotIt &= (numberOfTrials > 0);

                    if (!gotIt)
                        badTrials = true;

                    gotIt = int.TryParse(args[1], out numberOfTreads);
                    gotIt &= (numberOfTreads > 0);

                    if (gotIt)
                    {
                        numberOfTreads = Math.Min(Environment.ProcessorCount / 2, numberOfTreads);
                        results = new ParseResults(2, numberOfTrials, numberOfTreads);
                    }
                    else
                        badThreads = true;

                    break;

                default: // too many arguments sent
                    results = new ParseResults();
                    break;
            }

            return results;
        }
    }

    internal class ParseResults
    {
        /// <summary>
        /// Gets whether the incoming arugments where successfully parsed.
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

        public int? NumberOfTrials { get; private set; }

        public int? NumberOfThreads { get; private set; }

        public int NumberOfArguments { get; private set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public ParseResults()
        {
        }

        public ParseResults(int numberOfArguments, int? numberOfTrials, int? numberOfThreads) : this()
        {
            NumberOfArguments = numberOfArguments;
            NumberOfTrials = numberOfTrials;
            NumberOfThreads = numberOfThreads;
        }
    }
}
