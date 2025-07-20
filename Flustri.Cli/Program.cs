using Flustri.Cli;

switch (args[0])
{
    case "key":
        return KeyCommand.Handle(args);
    default:
        return 1;
}