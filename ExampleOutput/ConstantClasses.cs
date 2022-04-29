﻿using System.CommandLine;
using System.CommandLine.Completions;
using System.CommandLine.Invocation;

namespace Jackfruit
{
    public partial class ConsoleApplication
    {
        public static ConsoleApplication Create() { return new ConsoleApplication(); }
        public void SetRootCommand(Delegate action) { }

        public void AddCommand(Delegate action) { }


    }

    public class GeneratedCommandBase
    {
        private readonly Command sclCommand;
        protected GeneratedCommandBase(string name, string? description = null)
        {
            sclCommand = new Command(name, description);
        }

        internal Command Command => sclCommand;

        /// <summary>
        /// Gets the set of strings that can be used on the command line to specify the symbol.
        /// </summary>
        public IReadOnlyCollection<string> Aliases => sclCommand.Aliases;

        /// <inheritdoc/>
        public string Name
        {
            get => sclCommand.Name;
            set => sclCommand.Name = value;
        }

        /// <summary>
        /// Determines whether the specified alias has already been defined.
        /// </summary>
        /// <param name="alias">The alias to search for.</param>
        /// <returns><see langword="true" /> if the alias has already been defined; otherwise <see langword="false" />.</returns>
        public bool HasAlias(string alias) => sclCommand.HasAlias(alias);

        /// <summary>
        /// Represents all of the arguments for the command.
        /// </summary>
        public IReadOnlyList<Argument> Arguments => sclCommand.Arguments;

        /// <summary>
        /// Represents all of the options for the command, including global options.
        /// </summary>
        public IReadOnlyList<Option> Options => sclCommand.Options;

        /// <summary>
        /// Represents all of the global options for the command
        /// </summary>
        public IReadOnlyList<Option> GlobalOptions => sclCommand.Options;


        /// <summary>
        /// Adds an alias to the command. Multiple aliases can be added to a command, most often used to provide a
        /// shorthand alternative.
        /// </summary>
        /// <param name="alias">The alias to add to the command.</param>
        public void AddAlias(string alias) => sclCommand.AddAlias(alias);

        /// <summary>
        /// Gets or sets a value that indicates whether unmatched tokens should be treated as errors. For example,
        /// if set to <see langword="true"/> and an extra command or argument is provided, validation will fail.
        /// </summary>
        public bool TreatUnmatchedTokensAsErrors { get; set; } = true;

        /// <summary>
        /// Represents all of the symbols for the command.
        /// </summary>
        public IEnumerator<Symbol> GetEnumerator() => sclCommand.GetEnumerator();

        /// <summary>
        /// Gets completions for the symbol.
        /// </summary>
        public IEnumerable<CompletionItem> GetCompletions(CompletionContext context) => sclCommand.GetCompletions(context);

        /// <summary>
        /// Adds an <see cref="Option"/> to the command.
        /// </summary>
        /// <param name="option">The option to add to the command.</param>
        private protected void Add(Option option) => sclCommand.AddOption(option);

        /// <summary>
        /// Adds an <see cref="Argument"/> to the command.
        /// </summary>
        /// <param name="argument">The argument to add to the command.</param>
        private protected void Add(Argument argument) => sclCommand.AddArgument(argument);

        /// <summary>
        /// Adds a subcommand to the command.
        /// </summary>
        /// <param name="command">The subcommand to add to the command.</param>
        /// <remarks>Commands can be nested to an arbitrary depth.</remarks>
        private protected void Add(Command command) => sclCommand.AddCommand(command);

        /// <summary>
        /// Gets or sets the <see cref="ICommandHandler"/> for the command. The handler represents the action
        /// that will be performed when the command is invoked.
        /// </summary>
        /// <remarks>
        /// <para>Use one of the <see cref="Handler.SetHandler(Command, Action)" /> overloads to construct a handler.</para>
        /// <para>If the handler is not specified, parser errors will be generated for command line input that
        /// invokes this command.</para></remarks>
        private protected ICommandHandler? Handler
        {
            get => sclCommand.Handler;
            set => sclCommand.Handler = value;
        }


    }
}
