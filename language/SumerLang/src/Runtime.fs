namespace SumerLang.Runtime

/// InputStream is the input type for all sumer parsers
type InputStream = 
    | InputStream of string 

    static member FromString (input: string) =
        InputStream input

/// Citizens can be passed as an arguments to functions and commands
type Citizen = 
    | String of string 

/// MutationBinding is a mapping from binding variable to a citizen to be used in an Mutation
type MutationBinding = Map<string, Citizen>

type ParsingError = string
/// CommandParser might turn an input stream into a mapping of bindings
type CommandParser = (InputStream) -> Option<Result<MutationBinding, ParsingError>>

/// Mutation<'T> is the description of a mutation on an object of type 'T
type 'T Mutation = 
    /// An Mutation that instructs the runtime to change its state to a specific value
    | UpdateState of 'T
    
/// Command<'ResultT> associates a parser with an Mutation to perform if there's a match
type 'ResultT Command = Command of CommandParser * Mutation<'ResultT>

/// Runtime<StateT> handles incoming strings, checking them against the list of known commands 
/// and executing the commands against its internal state.
type Runtime<'StateT when 'StateT : equality> (state: 'StateT, commands: List<Command<'StateT>>) = 
    // make the state available
    member val State = state with get, set 
    // as well as the list of commands
    member val Commands = commands with get, set 

    /// Execute takes an InputStream and might update its internal state if there is 
    /// a matching command
    member this.Execute (command: InputStream): Result<unit, string> =
        // only look at the first command of the ones that match
        let matches = this.Commands |> 
                            List.choose (fun (Command (parser, mutation)) -> 
                                match parser command with
                                | None -> None
                                | Some(Error(_)) -> None
                                | Some(Ok(binding)) -> Some (binding, mutation)
                            ) 

        // the result of the execution depends on the commands that were matched
        match matches with 
        // if there were no matches
        | [] -> Error("Did not understand command " + command.ToString())
        // otherwise grab the bindings and Mutation of the first match
        | (binding, mutation)::_ -> 
            match mutation with
            // if all we have to do is update the state
            | UpdateState state -> 
                // nothing can go wrong
                Ok(this.State <- state)                

module Runtime =

    // the BlackHoleParser always passes and never returns any bindings
    let BlackHoleParser = fun (input: InputStream) -> Some(Ok(Map.empty))