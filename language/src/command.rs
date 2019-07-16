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

#[derive(Debug)]
pub struct Command {
    pub action: String,
    pub arguments: Vec<Citizen>,
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

type Parser = fn (input: &str) -> IResult<&str, &str>;

/// Runtime is a singleton that interprets commands and tracks the current state of the program.
pub struct Runtime<'a, RuntimeT> {
    /// commands are a list of (parser, callback) pairs that are checked against when 
    /// interpretting commands. A callback is a function that takes the runtime and any argument bindings
    /// and returns an updated runtime
    commands: Vec<(Parser, (&'a Fn(HashMap<&str, Citizen>, RuntimeT) -> RuntimeT))>
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

fn command_binding(input: &str) -> IResult<&str, Vec<Citizen>> {
    // the list of arguments to a command are proceeded by a space and then many citizens
    // separated by the word "and"
    let (input, (_, arguments)) = tuple((space, separated_list(tag("&"), citizen)))(input)?;

    // return the list of arguments
    Ok((input, arguments))
}

fn parse_command(input: &str) -> IResult<&str, Command> {
    // look for the action and optionally a
    let (input, (action, args)) = tuple((word, opt(command_binding)))(input)?;

    // return the command we just made
    Ok((input, Command {
        action: action.to_string(),
        arguments: match args {
            None => vec![],
            Some(args) => args,
        },
    }))
}


/// parse a string into the corresponding Command
pub fn parse(cmd: String) -> Result<Command, &'static str> {
    // if we weren't given a string
    if cmd.len() == 0 {
        // yell loudly
        return Err("Encountered empty command");
    }

    match parse_command(&cmd) {
        Ok((_left_to_parse, cmd)) => Ok(cmd),
        _ => Err("Encountered error while parsing command"),
    }
}


#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parse_empty_command_errs() {
        match super::parse("".to_string()) {
            // we got a command (no error)
            Ok(_) => assert!(false, "did not encounter an error"),
            // if we did encounter an error, the tests passed!
            Err(_) => (),
        };
    }

    #[test]
    fn parse_run_command() {
        match super::parse("hello".to_string()) {
            // if we encountered an error the test failed
            Err(err) => assert!(false, err),
            Ok(cmd) => {
                // make sure we got a command with the right action
                assert_eq!(cmd.action, "hello");
                // and there are no arguments
                assert_eq!(cmd.arguments.len(), 0);
            }
        };
    }

    #[test]
    fn parse_single_argument() {
        let table = vec![
            (
                "hello world",
                Citizen::Identifier("hello world".to_string()),
            ),
            (
                "quote hello world quote",
                Citizen::Str("hello world".to_string()),
            ),
        ];

        for row in table {
            match super::parse("run ".to_string() + row.0) {
                // if we encountered an error the test failed
                Err(err) => assert!(false, err),
                Ok(cmd) => {
                    // make sure we got a command with the right action
                    assert_eq!(cmd.action, "run");
                    // and there is only one argument
                    assert_eq!(cmd.arguments.len(), 1);
                    // and it is what we expected
                    assert_eq!(cmd.arguments[0], row.1);
                }
            };
        }
    }
}
