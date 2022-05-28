# Custodian

Custodian is a DCS server administrator appication that allows administrators
access using the new Discord Slash commands. Current features allow you to do
all of the following from within Discord.

* Display text to players on servers. Including by coalition or direct to a
  specific player
* Kick players from servers
* Ban and unban players using the built-in DCS ban system (No SLMod support
  as yet) as well as list banned players

# Installation

## Create Discord Bot account

Create a Discord bot account by following the instructions on the Discord
development site [getting started](https://discord.com/developers/docs/getting-started)
guide.

Make sure to give the bot `Slash Command` permissions when inviting it into your
Discord.

Once the bot has been invited make sure to restrict permissions regarding who
is allowed to run the commands and in what channels. This can be done from
within the administration menu for your Discord server in the `Integrations`
tab.

## Setup the bot

1. Download Custodian from URL and extract into a folder of your choice.
2. Modify the `configuration.yaml` file to suit your installation. The file
   has comments that explain the various options.
3. Run the bot using the `Service.exe` or optionally run as a Windows Service
   (See below). For the initial runs we recommend not running as a service
   to make sure everything is setup correctly.

## Install as a windows service

Run the following command in a Powershell window with administrator
permissions, making sure to change the path to point to the correct location.

```
New-Service -Name Custodian -BinaryPathName C:\YOUR\PATH\TO\CUSTODIAN\Service.exe -Description "Discord Bot for DCS Administration" -DisplayName "Custodian" -StartupType Automatic
```
