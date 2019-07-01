module Sumer.Executor
// open the
open Sumer.Command;

/// The standard error for a sumer execution
type ExecutionError = string

/// A symbol table relevant to a particular execution context
type Scope = Map<string, Symbol>

type Executor(globalScope: Scope) =
    member this.Execute<'T> (initialState: 'T) (cmd: Command): ('T * ExecutionError list) =
        // each task can just run in series for now
        cmd.Tasks |> List.fold (fun ((state, errors): ('T * ExecutionError list)) (task: Task) ->
            match globalScope.TryFind task.Action with
            | None -> (initialState, "unknown identifier " + task.Action :: errors)
            | Some foo -> (initialState, errors)
        ) (initialState, [])
