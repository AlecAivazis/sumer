module Sumer.Command


/// First-class citizens (can be the value of a variable)
type Symbol = Identifier of string | String of string

/// Everything in a Command AST is a Node
type Node = Symbol

type Task = {
    Action: string;
    Arguments: Symbol list;
}

type private Maybe<'T> = Result of 'T | Error of string

// A single command contains many tasks
type Command(tasks: Task list) =
    // the lists of tasks associated with this command
    member this.Tasks
        with get() : Task list =
            tasks

type ParseResult = ParseResult of Command | ParseError of string

let private parseArguments (input: string list ): Maybe<Symbol list> =
    match input with
    // an empty input has no arguments
    | [] -> Result []
    // a non-empty list has arguments
    | head :: _ ->
        // otherwise treat the head as an indentifier
        Result ([Identifier head])


// takes a string assumed to be spoken text and creates the underlying command
// that needs to be executed
let ParseCommand (sentence: string): ParseResult =
    // if the sentence is empty
    if sentence.Length = 0 then
        // return an empty command
        ParseResult (Command [])

    // there is something to do
    else
        // grab the words in the sentence
        let words = sentence.Split ' '

        // the first word defines the action of the command
        let action = words.[0]

        // try to get the arguments in the rest of the sentence
        match parseArguments (List.ofSeq words.[1..words.Length - 1]) with
        | Error err -> ParseError err
        | Result args ->
            // for now assume there is only one task in the command
            ParseResult (Command [{
                Action = action;
                Arguments = args;
            }])

