
<div align="center">
	<h1>Project Reloaded (Mod Template)</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong>All your mods are belong to us.</strong>
	<p>C# based universal mod loader framework compatible with arbitrary processes.</p>
</div>


# About This Project

The following project provides a very basic hook of what I believe is the CRIWARE filesystem inside Sonic Heroes, used to load CRI based media formats such as ADX, AFS and SFD.

While I wouldn't go in depth about the reverse engineering process involved to get to this stage; I should summarize why I am led to believe the code I am hooking is CRI owned: 

- All of the functions have not been optimized out by the compiler (are standard CDECL), with no visible inlining inside functions either. This is rare for game code and implies a linking of a 3rd party library.

- Hooking of the functions reveals that they are only used to load CRI formats.

## Why?

By default, Sonic Heroes is known to exhibit the following issues:

- Starting the game after a system restart or wake-up from hibernation takes particularly long on any system. Presumably from the overhead of calling CreateFileA on a couple of thousand files.

- As a result of the above, the game holds an open handle to every single file present in any of the game subfolders until it is shut down. The effects of that are visible the moment you try to rename or delete a file when the game is running.

- If you attempt file redirection through hooking, the game will receive the wrong size of file to load resulting in media files such as music being chopped off. This is because the file handles returned by CreateFileA and FindFirstFileA/FindNextFileA will not match. 

- *You are unable to load any media files not present in the dvdroot folder at game launch; making modding harder.
 
*This includes any `dvdroot` folder that may exist on your CD drive, the game supports streaming media from removable media.

### Some More Info.

The interesting fact that may perhaps be worth bringing is that the first of the two issues - the ones that would ultimately affect the end-user could have been entirely avoided.

The function that creates a FileTable that is used by the game takes a folder path. This function recursively (in the case of folders) finds all files within, queries their details, opens their handles and adds new files to the FileTable before returning. This is the operation that causes the long startup times.

As this FileTable is only used to load CRI files, the simple solution to this problem would have been simply to move all of the CRI files into their own respective folders; and surprisingly, the developers already did this. What they did not do, is point the FileTable generation functions to those folders.

This results in all files being added to the FileTable, of which less than 100 will ever be used and 3800+ files that will never be touched. Even worse, the individual FileEntries of the FileTable are in fact, a linked list. Therefore you can imagine, the function to get a FileEntry from a file path also being highly inefficient, having to often iterate over 1000s of entries that will never be accessed or retrieved in order to get the correct one each time. 

*NB. For those wondering about possible technical limitations, the game does already support multiple FileTables, they are in fact stored in an array and by default can be used to load files from the CD Drive. What the developers could have done is created two FileTables for the `dvdroot/BGM` and `dvdroot/movie` folders; and the same for removable media.*

*In addition, the function to return an individual file entry already checks all FileTables; there would not have been any programming complications in doing any of this*. 

## What This Project Does.

The CRI FileTable Hook provides a simple hook that latches onto the function used to retrieve a file entry from the filetable. For a requested file path the function takes, the hook opens the file with CreateFileA and generates a new FileEntry on the fly. The file entry is then cached in a string to FileEntry dictionary for later re-use, before being written to native memory and returned to the game.

In addition, the function to build a file table is hooked to perform nothing and simply return a success error code (0) as we will never be needing the file table. This prevents handles being made to every game file and slow initial startup times.
