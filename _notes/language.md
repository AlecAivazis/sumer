# Language

At its core, Sumer is a programming environment with a computation model very similar
to a modern statically-typed functional language. Users interact with the development
environment through a verbal interface. A command is roughly broken up into a few parts:

## Action

The action is the core behavior of the command. These include:

### variable delcartion

"define bar with value foo plus one"

### control structures

"if foo is 12 then log quote big quote"

### type declarations

"create type called user with fields ..."

"add a field with name foo and type string to ..."

### pattern matching

"if foo is a Vector two with myX set to X then ..."
"if foo is a vector two with x equal 10 then ..."

### modifying existing expressions

"change the definition of bar to foo plus two"

### Wait commands

"wait" pauses sumer to allow you to do other things with your life.

### FUnction definition

"Define add a and b with parameters a and b as a plus b"
