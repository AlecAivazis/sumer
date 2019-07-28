namespace SumerLang.Tests

open NUnit.Framework
open SumerLang.Runtime
open FsUnit


[<TestFixture>]
type RuntimeTests () =
    [<Test>]
    member this.ExecuteParser_newState() =
        // we need to test a command that just sets the state
        let updateParser = fun (input: InputStream) -> Ok(Map.empty)
        // we do want an operation that sets the new state
        let updateOperation = NewState "updated!!!"

        // create a runtime to test against
        let initialRuntime = Runtime(
                                initialState = "",  
                                initialCommands = [Command(updateParser, updateOperation)])

        // execute a command
        match InputStream.FromString "this could be anything" |> initialRuntime.Execute with 
        | Error(str) -> should equal false true 
        | Ok(runtime) -> 
            // this should have updated the internal state of runtime
            runtime.State |> should equal "updated!!!"
