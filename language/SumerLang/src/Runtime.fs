namespace SumerLang.Runtime

/// InputStream is the input type for all sumer parsers
type InputStream = 
    | InputStream of string 

    static member FromString (input: string) =
        InputStream input

/// Citizens can be passed as an arguments to functions and commands
type Citizen = 
    | String of string 

/// OperationBinding is a mapping from binding variable to a citizen to be used in an operation
type OperationBinding = Map<string, Citizen>

type ParsingError = string
/// CommandParser might turn an input stream into a mapping of bindings
type CommandParser = (InputStream) -> Result<OperationBinding, ParsingError>
    
/// Command<'ResultT> associates a parser with an operation to perform if there's a match
type 'ResultT Command = Command of CommandParser * Operation<'ResultT>

/// Operation<'T> is the description of a mutation on an object of type 'T
and 'T Operation = 
    /// An operation that instructs the runtime to change its state to a specific value
    | NewState of 'T


/// Runtime<StateT> handles incoming strings, checking them against the list of known commands 
/// and executing the commands against its internal state.
type Runtime<'StateT>(initialState: 'StateT, initialCommands: List<Command<'StateT>>) = 
    // make the state available
    member this.State = initialState
    // as well as the list of commands
    member this.Commands = initialCommands

    /// Execute takes an InputStream and might update its internal state if there is 
    /// a matching command
    member this.Execute (command: InputStream): Result<Runtime<'StateT>, string> =


        // nothing went wrong, nothing was updated
        Ok(this)
