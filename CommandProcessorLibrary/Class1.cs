﻿using System;
using System.Collections.Generic;

namespace WMCommandFramework
{
    public sealed class CommandUtils
    {
        private static string currentToken = "";
        /// <summary>
        /// The string of text to display in the prompt.
        /// </summary>
        public static string CurrentToken
        {
            get { return currentToken; }
            set { currentToken = value; }
        }

        private static bool debugMode = false;
        /// <summary>
        /// Shows or hides specific debug data.
        /// </summary>
        public static bool DebugMode
        {
            get { return debugMode; }
            set { debugMode = value; }
        }
    }
    public class CommandProcessor
    {
        private CommandInvoker invoker = null;
        public CommandProcessor()
        {
            invoker = new CommandInvoker();
        }
        
        /// <summary>
        /// Gets the pre-set invoker that is used for registering and using commands within the command processor.
        /// </summary>
        /// <returns>CommandInvoker - The internal command invoker.</returns>
        public CommandInvoker GetInvoker()
        {
            return invoker;
        }

        /// <summary>
        /// Starts the Command Processor.
        /// </summary>
        public void Process()
        {
            try
            {
                Console.Write($"{CommandUtils.CurrentToken} > ");
                var input = Console.ReadLine();
                if (!(input == null) || !(input == "")) invoker.InvokeCommand(input);
            }
            catch (Exception ex)
            {
                if (CommandUtils.DebugMode == true)
                    Console.WriteLine($"An unknown error has occurred! Please restart your computer! Error: {ex.ToString()}");
                else
                    Console.WriteLine("An unknown error has occurred! Please restart your computer!");
            }
        }
    }
    public interface Command
    {
        /// <summary>
        /// The default name of the command!
        /// </summary>
        /// <returns>The name of the command.</returns>
        string CommandName();
        /// <summary>
        /// The description of the command that will display in help.
        /// </summary>
        /// <returns>The description of the command.</returns>
        string CommandDesc();
        /// <summary>
        /// The syntax of the command.
        /// Note: Add syntax after the command as the command that was inputted is placed before the syntax.
        /// </summary>
        /// <returns>The syntax of the command.</returns>
        string CommandSynt();
        /// <summary>
        /// Alliases of the command. If this name or value is typed the same command will also run.
        /// </summary>
        /// <returns>Allieses for the command.</returns>
        string[] CommandAliases();
        /// <summary>
        /// Code that is ran when the command is invoked by the parent command invoker.
        /// </summary>
        /// <param name="invoker">The invoker that invoked the command.</param>
        /// <param name="args">The argument that where processed.</param>
        void OnCommandInvoked(CommandInvoker invoker, CommandArgs args);
    }

    public class CommandArgs
    {
        private List<string> args = new List<string>();

        public CommandArgs() { new CommandArgs(new string[] { }); }

        public CommandArgs(string[] argsx)
        {
            foreach (string s in argsx)
            {
                args.Add(s);
            }
        }

        /// <summary>
        /// Checks to see if there are no args.
        /// </summary>
        /// <returns>Whether there are args.</returns>
        public bool isEmpty()
        {
            if (args.Count == 0) return true;
            return false;
        }

        /// <summary>
        /// Gets the argument at the following position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public string GetArgAtPosition(int pos)
        {
            return args[pos];
        }

        /// <summary>
        /// Checks if there is a switch anywhere in the arg string.
        /// </summary>
        /// <param name="switch_key">The swich key to check for.</param>
        /// <returns>Whether there is a switch value.</returns>
        public bool ContainsSwitch(string switch_key)
        {
            if (args.Contains($"-{switch_key}") || args.Contains($"/{switch_key}")) return true;
            return false;
        }

        /// <summary>
        /// Checks if there is a segmented switch anywhere in the arg array.
        /// </summary>
        /// <param name="segment">The segment to search for.</param>
        /// <returns>True if the array contains the segmented switch.</returns>
        public bool ContainsSegmentedSwitch(string segment)
        {
            if (args.Contains($"-{segment}:")) return true;
            return false;
        }

        /// <summary>
        /// Checks to see if the specified arg is anywhere in the argument stirng.
        /// </summary>
        /// <param name="arg">The arg to search for.</param>
        /// <returns>Whether the arg was found.</returns>
        public bool ContainsArg(string arg)
        {
            if (args.Contains(arg)) return true;
            return false;
        }

        /// <summary>
        /// Gets the index of the specified arg.
        /// </summary>
        /// <param name="arg">The arg to get the index of.</param>
        /// <returns>The index of the specified arg.</returns>
        public int GetIndexOfArg(String arg)
        {
            if (ContainsArg(arg))
            {
                var index = args.IndexOf(arg);
                return index;
            }
            return 0;
        }
        
        /// <summary>
        /// Gets the index of the swithc witht he following name.
        /// </summary>
        /// <param name="switch_key">The name of the switch key.</param>
        /// <returns>The index of the switch with the key.</returns>
        public int GetIndexOfSwitch(string switch_key)
        {
            if (ContainsSwitch(switch_key))
            {
                if (args.Contains($"-{switch_key}"))
                {
                    var index = args.IndexOf($"-{switch_key}");
                    return index;
                }
                else if (args.Contains($"/{switch_key}"))
                {
                    var index = args.IndexOf($"/{switch_key}");
                    return index;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the index of the segmented switch with the following segment.
        /// </summary>
        /// <param name="segment">The segment of the segmented switch.</param>
        /// <returns>The segmented switch value.</returns>
        public string GetSegmentedSwitchValue(string segment)
        {
            if (ContainsSegmentedSwitch(segment))
            {
                string data = "";
                foreach (string segmented in args)
                {
                    if (segmented.StartsWith($"-{segment}:"))
                    {
                        var x = segmented.Split(':');
                        data = x[1];
                        break;
                    }
                }
                return data;
            }
            return null;
        }

        /// <summary>
        /// Skips the argument at the following location.
        /// </summary>
        /// <param name="index">The index to split from.</param>
        /// <returns>A reformmated string array.</returns>
        public string[] Skip(int index)
        {
            var x = args.ToString();
            var l = x.Substring(index, x.Length);
            var s = l.Split(' ');
            var ls = new List<string>();
            foreach (string sx in s)
                ls.Add(sx);
            return ls.ToArray();
        }

        /// <summary>
        /// Skips the argument with the following name.
        /// </summary>
        /// <param name="arg">The string to skip from.</param>
        /// <returns>The newly formatted string only containing the data that was skipped from.</returns>
        public string[] Skip(string arg)
        {
            return Skip(GetIndexOfArg(arg));
        }
    }

    public class CommandInvoker
    {
        private List<Command> commands = new List<Command>();

        public CommandInvoker()
        {
            //TODO: Register Internal Commands.
            AddCommand(new Help());
            AddCommand(new Clear());
            AddCommand(new Echo());
        }

        /// <summary>
        /// Adds a command to the Command Registry.
        /// </summary>
        /// <param name="c">The command to add to the Command Registry.</param>
        public void AddCommand(Command c)
        {
            commands.Add(c);
        }

        /// <summary>
        /// Removes a pre-existing command from the Command Registry.
        /// </summary>
        /// <param name="c">The command to remove from the Command Registry.</param>
        public void RemoveCommand(Command c)
        {
            commands.Remove(c);
        }

        /// <summary>
        /// Gets a list of all commands in the Command Registry.
        /// </summary>
        /// <returns>A list of commands in the Command Registry.</returns>
        public List<Command> GetCommands()
        {
            return commands;
        }

        /// <summary>
        /// Gets a command with the specified name.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <returns>The command with the specified name. Null if none.</returns>
        public Command GetCommandByName(string name)
        {
            Command out_x = null;
            foreach (Command c in commands)
            {
                if (c.CommandName().ToLower() == name.ToLower())
                {
                    out_x = c;
                    break;
                }
            }
            return out_x;
        }

        /// <summary>
        /// Gets a command with the specified alias.
        /// </summary>
        /// <param name="alias">The alias of the command.</param>
        /// <returns>The command with the specified alias. Null if none.</returns>
        public Command GetCommandByAlias(string alias)
        {
            Command out_x = null;
            foreach (Command c in commands)
            {
                var next = false;
                foreach (string s in c.CommandAliases())
                {
                    if (s.ToLower() == alias.ToLower())
                    {
                        out_x = c;
                        next = true;
                        break;
                    }
                }
                if (next == true) break;
            }
            return out_x;
        }

        /// <summary>
        /// Gets a command based on the specified value.
        /// </summary>
        /// <param name="value">The name or alias of the command.</param>
        /// <returns>The command with the specified value. Null if none.</returns>
        public Command GetCommand(string value)
        {
            var cname = GetCommandByName(value);
            var calias = GetCommandByAlias(value);
            Command out_x = null;
            if ((cname != null) && (calias == null))
                out_x = cname;
            else if ((cname == null) && (calias != null))
                out_x = calias;
            else
                out_x = null;
            return out_x;
        }

        /// <summary>
        /// Parses the input string and if a command exists it gets invoked.
        /// </summary>
        /// <param name="input">The string to parse containing command and args.</param>
        public void InvokeCommand(string input)
        {
            var x = input.Split(' ');
            var name = x[0];
            CommandArgs args = null;
            if (x.Length > name.Length) args = new CommandArgs();
            else args = new CommandArgs(new CommandArgs(x).Skip(name));
            var cmd = GetCommand(name);
            if (cmd == null)
            {
                //Not a command.
                Console.WriteLine($"\"{name.ToLower()}\", is not a valid command.");
            }
            else
            {
                try
                {
                    //Invoke Command.
                    cmd.OnCommandInvoked(this, args);
                    return;
                }
                catch
                {
                    //Unknown Error Print Syntax.
                    Console.WriteLine($"Syntax Error:\nUsage: {cmd.CommandSynt().ToLower()}");
                    return;
                }
            }
        }
    }

    //Internal Commands.
    public class Help : Command
    {
        public string[] CommandAliases()
        {
            return new string[] { "cmds", "cmd", "command", "commands" };
        }

        public string CommandDesc()
        {
            return "This displays command information.";
        }

        public string CommandName()
        {
            return "help";
        }

        public string CommandSynt()
        {
            return "help [command]";
        }

        public void OnCommandInvoked(CommandInvoker invoker, CommandArgs args)
        {
            if (args.isEmpty())
            {
                //List all commands.
                foreach (Command c in invoker.GetCommands())
                {
                    Console.WriteLine($"{c.CommandName().ToLower()}: {c.CommandDesc()}");
                }
            }
            else
            {
                //List all info about the single command.
                var cmd = args.GetArgAtPosition(0);
                var command = invoker.GetCommand(cmd);
                Console.WriteLine($"===============\nName: {command.CommandName().ToLower()}\nDescription: {command.CommandDesc()}\nSyntax Usage: {command.CommandSynt()}\nAliases: {FormatString(command.CommandAliases())}\n===============");
            }
        }

        private string FormatString(string[] input)
        {
            string x = "";
            foreach (string s in input)
            {
                if (x == "")
                    x = s.ToLower();
                else
                    x += $", {s.ToLower()}";
            }
            return x;
        }
    }
    public class Clear : Command
    {
        public string[] CommandAliases()
        {
            return new string[] { "cls", "clr" };
        }

        public string CommandDesc()
        {
            return "Clears the terminal.";
        }

        public string CommandName()
        {
            return "clear";
        }

        public string CommandSynt()
        {
            return "cls";
        }

        public void OnCommandInvoked(CommandInvoker invoker, CommandArgs args)
        {
            Console.Clear();
        }
    }
    public class Echo : Command
    {
        public string[] CommandAliases()
        {
            return new string[] { };
        }

        public string CommandDesc()
        {
            return "Says the the data to output.";
        }

        public string CommandName()
        {
            return "echo";
        }

        public string CommandSynt()
        {
            return "echo <value>";
        }

        public void OnCommandInvoked(CommandInvoker invoker, CommandArgs args)
        {
            if (args.isEmpty()) throw new Exception();
            var x = args.GetArgAtPosition(0);
            Console.WriteLine(x);
        }
    }
}
