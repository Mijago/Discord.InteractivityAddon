# Discord.InteractivityAddon

[![NuGet](https://img.shields.io/nuget/vpre/Discord.InteractivityAddon.svg?style=plastic)](https://www.nuget.org/packages/Discord.InteractivityAddon)
[![BotFacility](https://img.shields.io/discord/512366986383065088.svg?style=flat-square&label=discord)](https://discord.gg/gAmSyVD)

This is an addon for the Discord API Wrapper [Discord.Net](https://github.com/discord-net/Discord.Net) which makes it easy to add interactivity to your discord bot.

## Installation
The package is available to download on [NuGet](https://www.nuget.org/packages/Discord.InteractivityAddon).

## Features
 - Waiting for a message / reaction which passes your filter
   - Run actions on filtered messages / reactions
 - Delayed sending and deleting messages & files
 - A powerful fully customizable Paginator
   - Send multi page messages
   - Move through pages using reactions
   - You can choose between a static and a lazy loaded one
   - Get more customizability by creating your own child of the `Paginator` & `PaginatorBuilder` classes
 - Fully customizable selection from a list of objects
   - Works with messages or reactions
   - Makes user input easy
   - Get more customizability by creating your own child of the `Selection` & `SelectionBuilder` classes
 - Confirmation
 - Uptime counter
 
## Usage
To properly use the features this addon provides you need to add the `InteractivityService` to your service provider.

```cs
var provider = new ServiceCollection()
                .AddSingleton(new InteractivityService(Client, TimeSpan.FromMinutes(3)))
                ....
```
This addon does not include a custom `ModuleBase` in order to support every command framework. (Discord.Net.Command/Qmmands/...)

### Example: Waiting for Message
```cs
[Command("nextmessage")]
public async Task ExampleReplyNextMessageAsync()
{
    var result = await Interactivity.NextMessageAsync(x => x.Author == Context.User);

    if (result.IsSuccess == true) {
        Interactivity.DelayedSendMessageAndDeleteAsync(
                        Context.Channel,
                        deleteDelay: TimeSpan.FromSeconds(20), 
                        text: result.Value.Content, 
                        embed: result.Value.Embeds.FirstOrDefault());
    }
}
```

### Example: Selection
```cs
[Command("select")]
public async Task ExampleSelectionAsync()
{
    var builder = new ReactionSelectionBuilder<string>()
        .WithValues("Hi", "How", "Hey", "Huh?!")
        .WithEmotes(new Emoji("💵"), new Emoji("🍭"), new Emoji("😩"), new Emoji("💠"))
        .WithUsers(Context.User)
        .WithDeletion(DeletionOption.AfterCapturedContext | DeletionOption.Invalids);

    var result = await Interactivity.SendSelectionAsync(builder.Build(), Context.Channel, TimeSpan.FromSeconds(50));

    if (result.IsSuccess == true) {
        await Context.Channel.SendMessageAsync(result.Value.ToString());
    }
}
```

### Example: Paginator
```cs
[Command("paginator")]
public Task PaginatorAsync()
{
    var pages = new PageBuilder[] {
        new PageBuilder().WithTitle("I"),
        new PageBuilder().WithTitle("am"),
        new PageBuilder().WithTitle("cool"),
        new PageBuilder().WithTitle(":sunglasses:"),
        new PageBuilder().WithText("I am cool :crown:")
    };

    var paginator = new StaticPaginatorBuilder()
        .WithUsers(Context.User)
        .WithPages(pages)
        .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
        .WithDefaultEmotes()
        .Build();

    return Interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));
}
```

### Example: Confirmation
```cs
[Command("confirm")]
public async Task ConfirmAsync()
{
    var request = new ConfirmationBuilder()
        .WithContent(new PageBuilder().WithText("Please Confirm"))
        .Build();

    var result = await Interactivity.SendConfirmationAsync(request, Context.Channel);

    if (result.Value == true)
    {
        await Context.Channel.SendMessageAsync("Confirmed :thumbsup:!");
    }
    else
    {
        await Context.Channel.SendMessageAsync("Declined :thumbsup:!");
    }
}
```
