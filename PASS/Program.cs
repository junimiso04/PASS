using PASS.Entities;
using PASS.Routines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS
{
    class Program
    {
        static void Initialize()
        {
            if (!File.Exists(@"probs.db"))
                DatabaseManager.CreateDatabase(@"probs.db");

            if (!File.Exists(@"answers.db"))
                DatabaseManager.CreateDatabase(@"answers.db");
        }

        static ArgumentInformation ArgumentInitialize(string[] args)
        { 
            string argsLine = string.Empty;
            string problemCode = string.Empty;
            string directory = string.Empty;

            // Argument parsing.
            for (int num = 0; num<args.Length; num++)
            {
                switch (args[num].ToLower())
                {
                    case string str when (str == "-c" || str == "--code" || str == "/c"):
                        problemCode = args[num + 1];
                        break;
                    case string str when (str == "-p" || str == "--path" || str == "/p"):
                        directory = args[num + 1];
                        break;
                }
            }

            // Exception handling.
            if (string.IsNullOrEmpty(problemCode))
                throw new Exception("Problem code input error.");

            if (string.IsNullOrEmpty(directory))
                throw new Exception("Directory input error.");

            ArgumentInformation argInfo = new ArgumentInformation();
            argInfo.ProblemCode = problemCode;
            argInfo.Path = directory;

            return argInfo;
        }

        static void SwitchToConsole()
        {
            Console.Write(">");
            string command = Console.ReadLine();
            string[] cmdArr = command.Split(' ');

            // Console command switch
            switch (command)
            {
                case "crset": // Create problem set.
                    CreateSet();
                    break;
                case "addprob": // Add problem.
                    AddProblem();
                    break;
                case "viewprob": // View problem list.
                    ViewProblems();
                    break;
                case "addans": // Add answers to set.
                    AddAnswer();
                    break;
                case "viewans": // View answer list.
                    ViewAnswer();
                    break;
                case "scoring": // Scoring problem.
                    Scoring(null, null, true);
                    break;
                case "help": // Show commands.
                    Console.WriteLine("ADDANS          Add an answer.\n" +
                        "ADDPROB         Add an problem.\n" +
                        "CRSET           Create a new problem set.\n" +
                        "SCORING         Scoring problem.\n" +
                        "VIEWANS         Shows a list of answers to a specific problem.\n" +
                        "VIEWPROB        Shows a list of problems in the problem set.\n");
                    SwitchToConsole();
                    break;
                default: // Default
                    if (string.IsNullOrWhiteSpace(command))
                        Console.WriteLine("Enter 'help' to see which commands are available.");
                    SwitchToConsole();
                    break;
            }
        }

        static void CreateSet()
        {
            // Database initialize.
            if (DatabaseManager.IsOpen())
                DatabaseManager.RestartDatabase(@"probs.db");
            else
                DatabaseManager.OpenDatabase(@"probs.db");

            // Input
            Console.WriteLine("Specify the name for the problem set.");
            Console.Write("SET-NAME>");
            string set = Console.ReadLine();

            // Problem set create process.
            if (!DatabaseManager.IsTableExists(set))
            {
                if (DatabaseManager.CreateSet(set))
                    Console.WriteLine("Problem set created successfully!");
                else
                    Console.WriteLine("An error occurred while creating the problem set.");
            }
            else
            {
                Console.WriteLine("Problem set already exists.");
            }

            Console.WriteLine();
            SwitchToConsole();
        }

        static void AddProblem()
        {
            // Database initialize.
            if (DatabaseManager.IsOpen())
                DatabaseManager.RestartDatabase(@"probs.db");
            else
                DatabaseManager.OpenDatabase(@"probs.db");

            // Input
            Problem prob = new Problem(); // Declare a new problem information class.
            Console.WriteLine("Specify the set to which you want to add.");
            Console.Write("SET-NAME>");
            prob.Set = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Set the items below.\nIf you want to exit, press Ctrl+C to cancel.");

            Console.Write("NUMBER>");
            prob.Code = int.Parse(Console.ReadLine());
            Console.Write("TITLE>");
            prob.Title = Console.ReadLine();
            Console.Write("DESCRIPTION>");
            prob.Description = Console.ReadLine();
            Console.Write("MEMORY-LIMIT(Byte)>");
            prob.Memory = int.Parse(Console.ReadLine());
            Console.Write("TIME-LIMIT(Milliseconds)>");
            prob.TimeLimit = int.Parse(Console.ReadLine());

            // Problem add process.
            if (DatabaseManager.IsTableExists(prob.Set))
            {
                if (!DatabaseManager.IsRowExists(prob.Set, prob.Code))
                {
                    if (DatabaseManager.AddProblem(prob)) // Add problem.
                        Console.WriteLine("Problem added successfully!");
                }
                else
                {
                    Console.WriteLine("An error occurred while adding the problem.");
                }
            }
            else
            {
                Console.WriteLine("Problem set does not exist.");
            }

            Console.WriteLine();
            SwitchToConsole();
        }

        static void ViewProblems()
        {
            // Database initialize.
            if (DatabaseManager.IsOpen())
                DatabaseManager.RestartDatabase(@"probs.db");
            else
                DatabaseManager.OpenDatabase(@"probs.db");

            // Input
            Console.WriteLine("Enter the name of the set you want to see the problem list.");
            Console.Write("SET-NAME>");
            string set = Console.ReadLine();

            // View problems process.
            if (DatabaseManager.IsTableExists(set))
            {
                List<Problem> probList = DatabaseManager.GetProblemList(set);
                int count = 0;

                foreach(Problem prob in probList)
                {
                    Console.WriteLine($"[{set}-{prob.Code}|{prob.Title}] MEMORY : {prob.Memory}, TIMELIMIT : {prob.TimeLimit}");
                    count++;
                }
                Console.WriteLine($"Done! Total number of problems is {count}.");
            }
            else
            {
                Console.WriteLine("Problem set does not exist.");
            }

            Console.WriteLine();
            SwitchToConsole();
        }

        static void AddAnswer()
        {
            // Database initialize.
            if (DatabaseManager.IsOpen())
                DatabaseManager.RestartDatabase(@"answers.db");
            else
                DatabaseManager.OpenDatabase(@"answers.db");

            // Input
            Console.WriteLine("Enter the setcode you want to add answers.");
            Console.Write("SET-CODE>");
            string setCode = Console.ReadLine();

            if (!setCode.Contains("-"))
                SwitchToConsole();

            // Split set code.
            string set = setCode.Split('-')[0];
            string code = setCode.Split('-')[1];

            DatabaseManager.CreateAnswerTable(setCode); // Create answer table.

            // Add answers process.
            if (DatabaseManager.IsTableExists(setCode))
            {
                Console.WriteLine("Enter the input and output of the problem in the form of A~&~B.\nIf you want to exit, please enter #END#.");

                // IO Values Input part.
                Dictionary<string, string> answers = new Dictionary<string, string>();
                while(true)
                {
                    string IO = Console.ReadLine();

                    if (string.IsNullOrEmpty(IO))
                        continue;

                    if (IO.ToUpper() == "#END#")
                        break;

                    if (!IO.Contains("~&~"))
                        continue;

                    string[] temp = IO.Split(new string[] { "~&~" }, StringSplitOptions.None);
                    answers.Add(temp[0], temp[1]); // Input/Output values add to Dictionary.
                    continue;
                }

                // Show the check value.
                foreach (var pair in answers)
                {
                    Console.WriteLine($"[CHECK]Input : ({pair.Key}) -> Output : ({pair.Value})");
                }

                // Synchronize IO values to database.
                Console.WriteLine("Please make sure the I/O values are correct.");
                while(true)
                {
                    Console.Write("Would you like to add these?(Y/N)>");
                    string agree = Console.ReadLine().ToLower();
                    if (agree == "y" || agree == "yes") // If you agree to this.
                    {
                        int count = 0;
                        foreach (var pair in answers)
                        {
                            Console.Write($"[ADD]Input : [{pair.Key}] -> Output : [{pair.Value}]... ");
                            if (DatabaseManager.AddAnswer(setCode, pair.Key, pair.Value)) // Add
                                Console.WriteLine("OK.");
                            else
                                Console.WriteLine("FAILURE.");
                            count++;
                        }
                        Console.WriteLine($"Done! Total number of answers is {count}."); // Done
                        break;
                    }
                    else if (agree == "n" || agree == "no")
                    {
                        SwitchToConsole();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                Console.WriteLine("Answer set does not exist.");
            }

            Console.WriteLine();
            SwitchToConsole();
        }

        static void ViewAnswer()
        {
            // Database initialize.
            if (DatabaseManager.IsOpen())
                DatabaseManager.RestartDatabase(@"answers.db");
            else
                DatabaseManager.OpenDatabase(@"answers.db");

            // Input
            Console.WriteLine("Enter the setcode you want to see answers.");
            Console.Write("SET-CODE>");
            string setCode = Console.ReadLine();

            if (!setCode.Contains("-"))
                SwitchToConsole();

            // Split set code.
            string set = setCode.Split('-')[0];
            string code = setCode.Split('-')[1];

            // View answers process.
            if (DatabaseManager.IsTableExists(setCode))
            {
                List<Answer> answerList = DatabaseManager.GetAnswerList(setCode); // Get answers.
                int count = 0;

                foreach(Answer answer in answerList) // Calling up problems in the list.
                {
                    Console.WriteLine($"[ANSWER]Input : ({answer.Input}) -> Output : ({answer.Output})");
                    count++;
                }
                Console.WriteLine($"Done! Total number of answers is {count}.");
            }
            else
            {
                Console.WriteLine("Answer set does not exist.");
            }

            Console.WriteLine();
            SwitchToConsole();
        }

        static void Scoring(string path = null, string setCode = null, bool inputMode = true)
        {
            // If the input mode is true.
            if (inputMode == true)
            {
                Console.WriteLine("Enter the path that contains the file to be scored.");
                Console.Write("PATH>");
                path = Console.ReadLine();
                Console.WriteLine("Enter the setcode in question.");
                Console.Write("SET-CODE>");
                setCode = Console.ReadLine();
            }

            // Exception handling.
            if (string.IsNullOrEmpty(setCode))
                throw new Exception("Invalid setcode format.");


            // Split set code
            string set = setCode.Split('-')[0];
            int code = int.Parse(setCode.Split('-')[1]);

            // Problems Database initialize.
            if (DatabaseManager.IsOpen())
                DatabaseManager.RestartDatabase(@"probs.db");
            else
                DatabaseManager.OpenDatabase(@"probs.db");

            Problem prob = new Problem();

            // Print problem information.
            if (DatabaseManager.IsTableExists(set))
            {
                prob = DatabaseManager.GetProblem(set, code);
                Console.WriteLine("--------------------------------------------------\n" +
                    $"TITLE : [{setCode}]{prob.Title}\n" +
                    $"DESCRIPTION : {prob.Description}\n" +
                    $"MEMORY LIMIT : {prob.Memory} Byte.\n" +
                    $"TIME LIMIT : {prob.TimeLimit} Milliseconds.\n" +
                    $"--------------------------------------------------");
            }
            else
            {
                Console.WriteLine("Problem set does not exist.");
            }

            // Answers Database initialize.
            if (DatabaseManager.IsOpen())
                DatabaseManager.RestartDatabase(@"answers.db");
            else
                DatabaseManager.OpenDatabase(@"answers.db");

            // Scoring process.
            if (DatabaseManager.IsTableExists(setCode))
            {
                List<Answer> answerList = DatabaseManager.GetAnswerList(setCode);

                foreach(Answer answer in answerList)
                {
                    ResultInformation result = ProcessRunner.Run(path, answer.Input, prob.TimeLimit); // Run program.

                    if (result.OutputResult != answer.Output) // If the output value is different.
                    {
                        Console.WriteLine($"Invalid output value(Normal:{answer.Output}, Current:{result.OutputResult}).");
                    }

                    if (result.ErrorResult != null) // If an error has occurred.
                    {
                        Console.WriteLine($"An error occured while running process({result.ErrorResult}).");
                    }

                    if (result.PrivilegedProcessorTime.TotalMilliseconds > prob.TimeLimit) // If the time is exceeded.
                    {
                        Console.WriteLine($"The timeout has been exceeded.\n({result.PrivilegedProcessorTime})");
                    }

                    if (result.PeakWorkingSet > prob.Memory) // If memory usage is exceeded.
                    {
                        Console.WriteLine($"Memory usage exceeded({result.PeakWorkingSet}).");
                    }

                    if (result.ExitCode != 0) // If it is not a normal exit code.
                    {
                        Console.WriteLine($"The exit code is not 0(Exit code : {result.ExitCode}).");
                    }
                }
            }
            else
            {
                Console.WriteLine("Answer set does not exist.");
            }

            Console.WriteLine();
            SwitchToConsole();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("PASS(Problem Automated Scoring System).\nCopyright <c> 2019 Syri all rights reserved.");

            Initialize(); // Program initialize.

            ArgumentInformation argInfo = new ArgumentInformation();

            // If there is no argument-
            if (args.Length == 0)
                SwitchToConsole(); // Switch to console mode.
            else
                argInfo = ArgumentInitialize(args); // Argument initialize.

            Scoring(argInfo.Path, argInfo.ProblemCode, false);
        }
    }
}
