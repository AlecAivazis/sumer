namespace SumerLang.Parser

/// Input is the input type for all sumer parsers
type InputStream = string

type Citizen = 
    | String of string 
    | Idetifier of string

/// CommandParser is something that might turn a string into a mapping of bindings
type CommandParser = (InputStream) -> Result<Map<string, Citizen>, string>

/// Command<ResultT> is hold the necessary information to register a command with a runtime
type 'ResultT Command = Command of 'ResultT * int

/// Runtime<StateT> is responsible for handing incoming strings, checking them against
/// the list of known commands and executing the commands and its internal state.
type 'StateT Runtime = {
    /// The internal state of the runtime
    State: 'StateT;
    /// The top level list of commands that the user can perform on the environment
    Commands: List<Command<'StateT>>;
}

