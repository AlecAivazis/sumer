task "build" {
    description = "Compile the sumer dll and copy it into the unity project"
    pipeline = [
        // compile the library
        "dotnet publish Sumer",
        // copy it into the unity project
        "cp Sumer/obj/Debug/netstandard2.0/sumer.dll SumerGameEngine/Assets/Sumer.dll",
    ]
}

task "tests" {
    description = "Run the tests associated with this project"
    pipeline = [
        "run build",
        "dotnet test Sumer.Tests"
    ]
}
