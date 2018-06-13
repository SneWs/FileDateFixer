# FileDateFixer

I managed to screw up some files so that they ended up being far into the future, this really messed with a few applications. 
This C# script/console app helps fixing all files from given folders.

To build it you need either dotnet core or regular dotnet/mono. Once built you can run it like this:
`DateTimeFixer.exe /home/<user_name>/Folder /drives/Blue/Important-Files`

You just give the binary a range of paths and it will recursivly look through all folders for files 
having last modified/last written dates past current date.
