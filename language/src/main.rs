mod command;
mod executor;

fn main() {
    match command::parse("hello".to_string()) {
        Err(err) => println!("{}", err),
        Ok(cmd) => println!("{}", cmd.action),
    }
}
