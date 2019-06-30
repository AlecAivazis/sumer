module Sumer.Language

// Task is the atomic unit of mutation that the user can perform
type Task(words: string list) =
    member this.Action
        with get() =
            // for now the action is just the first word in the command
            words.[0]

    member this.Arguments
        with get() =
            match words with
            // if there is only one element in the list, there are no arguments to the action
            | [_] -> []
            // the arguments are everything after the action
            | _ -> words.[1..words.Length - 1]

// A single command contains many tasks
type Command(tasks: Task list) =
    // the lists of tasks associated with this command
    member this.Tasks
        with get() : Task list =
            tasks

// takes a string assumed to be spoken text and creates the underlying command
// that needs to be executed
let ParseCommand (sentence: string): Command = Command ([Task (List.ofSeq(sentence.Split ' '))]);
