// externals
use nom::{
    IResult,
    bytes::complete::{ tag, take_while1, take_until },
    combinator::{ opt },
    sequence::{ tuple },
    branch::{ alt },
    multi::{ many1, separated_list },
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

/// CommandParser is a function that takes a string and returns a binding mappings if 
/// the string matches the command. 
type CommandParser = fn (input: &str) -> IResult<Binding, &str>;

/// Command is a 2-pair that consists of a parser to run and a function to invoke if the parser matches
type Command<'a, T> = (CommandParser, (&'a Fn(Binding, T) -> T));

/// Runtime is a singleton that interprets commands and tracks the current state of the program.
pub struct Runtime<'a, StateT> {
    /// the state of the runtime
    state: StateT,
    
    /// commands are a list of (parser, callback) pairs that are checked against when 
    /// interpretting commands. A callback is a function that takes the runtime and any argument bindings
    /// and returns an updated runtime
    commands: Vec<Command<'a, StateT>>,
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
    use super::*;

    #[test]
    fn citizen() {
        // the list of citizens we can parse
        let table = vec![
            ( "identifier", "hello", Citizen::Identifier("hello".to_string()) ),
            ( "string", "quote hello world quote", Citizen::Str("hello world".to_string()) ),
        ];

        // the runtime
        let runtime = Runtime{
            state: "",
            commands: vec![],
        };

        for ( title, input, expected ) in table {
            // make sure we can parse the string value
            match super::citizen(input) {
                Err(err) => assert!(false, title),
                Ok((_left_to_parse, res)) => {
                    assert_eq!(expected, res)
                }
            }
        }

    }
}
