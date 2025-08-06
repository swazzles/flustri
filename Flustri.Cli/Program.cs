using Flustri.Cli;

switch (args[0])
{
    case "key":
        return await KeyCommand.Handle(args);
    case "server":
        return await ServerCommand.Handle(args);
    default:
        return 1;
}