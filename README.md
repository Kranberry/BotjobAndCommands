# BotjobAndCommands
A project I made for fun without any specific reason.

## The project is originally written in C# 10 .Net 6
This project is a ***Console Application*** that stays alive until the ***ManualResetEvent*** has been .Set().
> Current versions
> C# 10 .Net 6

## Create a new bot job
Create a new class that implements the IBotJob interface. And that's it, it will now automatically start following the given Cron expression.

## Create a new Command
Create a new class that implements the ICommand interface. And that's it, it will be automatically added as a command and show up with the help command.
Every command sent by the user will be trimmed and treated as lowecase. The ***AvailableCommands*** dictionary have their key be the Command or the ShortCommand 
of the command converted to lowercase. The value will be a new instance (shared instance if the class both has Command and ShortCommand) of said Command object.
