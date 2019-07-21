// externals
use nom::{
    IResult,
    bytes::complete::{ tag, take_while1, take_until },
    branch::{ alt },
    multi::{ many1 },
};
use std::collections::*;


/// A Citizen is anything that can be referenced
#[derive(Debug, Clone)]
pub enum Citizen {
    Identifier(String),
    Str(String),
}

impl PartialEq for Citizen {
    fn eq(&self, other: &Self) -> bool {
        match (self, other) {
            // if we're comparing two idenfitiers their value has to be the same
            (Citizen::Identifier(s), Citizen::Identifier(o)) => s == o,
            // if we're comparing two strings their value has to be the same
            (Citizen::Str(s), Citizen::Str(o)) => s == o,
            // anything else and its not the same
            _ => false,
        }
    }
}


/// Bindings are stored as a mapping from the spoken word (represented as a string) and
/// the citizen that is bound to it.
type Binding = HashMap<String, Citizen>;

/// CommandParser is a function that takes a string and if the string matches the command it
/// returns a mapping of parameter values.
type CommandParser = fn (input: &str) -> IResult<&str, Binding>;

/// Command is a 2-pair that consists of a parser to run and a function to invoke if the parser matches
type Command<'a, T> = (CommandParser, (&'a Fn(Binding, &T) -> Result<T, String>));

/// Runtime is a singleton that interprets commands and tracks the current state of the program.
pub struct Runtime<'a, StateT> {
    /// the state of the runtime
    pub state: StateT,

    /// commands are a list of (parser, callback) pairs that are checked against when
    /// interpreting words spoken by the user.
    commands: Vec<Command<'a, StateT>>,
}

impl <'a, StateT> Runtime<'a, StateT> {
    // execute a spoken command in this runtime
    fn exec(&mut self, command: String) {
        // for each possible parser/callback combo
        for (parser, callback) in &self.commands {
            // attempt to parse the command
            match parser(&command) {
                // if there was an error, skip it
                Err(_) => (),
                // if we found a match
                Ok((_, bindings)) => {
                    // update the internal state
                    match callback(bindings, &self.state) {
                        Ok(new_state) => self.state = new_state,
                        Err(_) => ()
                    };

                    // stop looking for commands
                    break;
                }
            }
        }

    }
}

fn space(input: &str) -> IResult<&str, &str> {
    take_while1(|c| c == ' ')(input)
}

fn word(input: &str) -> IResult<&str, &str> {
    take_while1(|c: char | { c.is_alphanumeric() })(input)
}

fn quote(input: &str) -> IResult<&str, &str> {
    tag("quote")(input)
}

fn word_sequence(input: &str) -> IResult<&str, String> {
    // a squenece is a series of words and spaces
    let (input, id) = many1(alt((space, word)))(input)?;

    return Ok((input, id.join("")))
}

fn identifier(input: &str) -> IResult<&str, Citizen> {
    // an identifier is a word sequence
    let (input, id) = word_sequence(input)?;

    // wrap the result in the right enum value
    Ok((input, Citizen::Identifier(id.to_string())))
}

fn string(input: &str) -> IResult<&str, Citizen> {
    // a string is a sequence of words in between two quotes
    let (input, _) = quote(input)?;
    let (input, _) = space(input)?;
    let (input, value) = take_until(" quote")(input)?;
    let (input, _) = space(input)?;
    let (input, _) = quote(input)?;

    // return the string we grabbed
    Ok((input, Citizen::Str(value.to_string())))
}

fn citizen(input: &str) -> IResult<&str, Citizen> {
    alt((
        string,
        identifier
    ))(input)
}


#[cfg(test)]
mod tests {
    // define a macro to quickly make maps
    macro_rules! hashmap {
        (@single $($x:tt)*) => (());
        (@count $($rest:expr),*) => (<[()]>::len(&[$(hashmap!(@single $rest)),*]));

        ($($key:expr => $value:expr,)+) => { hashmap!($($key => $value),+) };
        ($($key:expr => $value:expr),*) => {
            {
                let _cap = hashmap!(@count $($key),*);
                let mut _map = ::std::collections::HashMap::with_capacity(_cap);
                $(
                    let _ = _map.insert($key, $value);
                )*
                _map
            }
        };
    }

    use super::*;

    #[test]
    fn citizen() {
        // the list of citizens we can parse
        let table = vec![
            ( "identifier", "hello", Citizen::Identifier("hello".to_string()) ),
            ( "string", "quote hello world quote", Citizen::Str("hello world".to_string()) ),
        ];

        for ( title, input, expected ) in table {
            // make sure we can parse the string value
            match super::citizen(input) {
                Err(_err) => assert!(false, title),
                Ok((left_to_parse, res)) => {
                    assert_eq!(left_to_parse, "");
                    assert_eq!(expected, res);
                }
            }
        }

    }

    #[test]
    fn runtime_command() {
        // the parser to run
        fn test_command(input: &str) -> IResult<&str, Binding> {
            // a string is a sequence of words in between two quotes
            let (input, _) = tag("add")(input)?;

            // we only have one binding (the argument)
            Ok((input, hashmap!{  }))
        }

        // the function to call if the command matches
        fn callback(_args: Binding, initial: &String) -> Result<String, String> {
            Ok(format!("{} world", initial).to_string())
        }

        // create a runtime with a parse for "hello <argument>"
        let mut runtime = Runtime{
            state: "hello".to_string(),
            commands: vec![
                (test_command, &callback)
            ]
        };

        // execute the command
        runtime.exec("add".to_string());

        // make sure that we updated the state
        assert_eq!(runtime.state, "hello world".to_string());
    }
}
