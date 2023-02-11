# Custodian

Custodian is a DCS server administrator appication that allows administrators
access using the new Discord Slash commands. Current features allow you to do
all of the following from within Discord.

* Display text to players on servers. Including by coalition or direct to a
  specific player
* Send chat messages to players on servers. Including by coalition or direct to a
  specific player
* Transmit to players using SRS on servers. Including by coalition.
* Kick players from servers
* Ban and unban players using the built-in DCS ban system (No SLMod support
  as yet) as well as list banned players
* Run artbitrary lua code in the server (Requires `evalEnabled = true` in `DCS-gRPC`)
  using code typed directly into Discord or by reading a lua file.

The connection with the DCS server is handled using `DCS-gRPC` which allows
clients written in any gRPC suported language to interact with a running DCS
server.

# Installation

## Install DCS-gRPC

Make sure that [DCS-gRPC](https://github.com/DCS-gRPC/rust-server/releases) is
installed. This version of Custodian has been tested against DCS-gRPC version
0.7.X.

## Create Discord Bot account

Create a Discord bot account by following the instructions on the Discord
development site [getting started](https://discord.com/developers/docs/getting-started)
guide.

Make sure to give the bot `Slash Command` permissions when inviting it into your
Discord.

**Once the bot has been invited make sure to restrict permissions regarding who
is allowed to run the commands and in what channels**. This can be done from
within the administration menu for your Discord server in the `Integrations`
tab.

## Setup the bot

1. Download Custodian from URL and extract into a folder of your choice.
2. Modify the `configuration.yaml` file to suit your installation. The file
   has comments that explain the various options.
3. Run the bot using the `Custodian.exe` or optionally run as a Windows Service
   (See below). For the initial runs we recommend not running as a service
   to make sure everything is setup correctly.

## Install as a windows service

Run the following command in a Powershell window with administrator
permissions, making sure to change the path to point to the correct location.

```
New-Service -Name Custodian -BinaryPathName C:\YOUR\PATH\TO\Custodian.exe -Description "Discord Bot for DCS Administration" -DisplayName "Custodian" -StartupType Automatic
```

# Usage

All of the commands are available by typing `/` into the Discord channel that
the bot has access to. All of the options should be self-explanatory aside
from the snippet eval.

## Snippet Eval

`.lua` files in the `Snippets` directory will be available for running in your
DCS servers. You can have an entire directory hierarchy under the `Snippets`
directory if you prefer. The AutoCompletion is based on the name of the file;
not the full path.

In order to have a return value make sure that the last command in your snippet
is `return x` where `x` is the value that you want to see.
