# uFrame-Porter
A tiny tool for porting from uFrame 1.6 (no longer support) to new version of uFrame
- uFrame new version at: https://github.com/uFrame

Steps to port project:

1. Change project setting: 
Exit > Project Settings > Editor > Asset Serialization > Force Text.

2. Menu uFramePort -> StartPort: 
Wait for seconds, arcoding to project's size. 
After completed, you have a folder: ProjectName.db out side Asssets folder. 

3. uFramePort -> Organize Code: 
Change folder and code's structure , similar with new version. 

4. Check base class: 
Check code in [*.designer] folder & [*.designer.cs] files (base class). 
Remove them if they doesn't has your code. 

5. Add new uFrame's lib, remove old uFrame's lib. 
New uFrame: https://github.com/uFrame. 

6. Fix error in code: 
Comment or do anything to fix error. 
Make Unity can build & run new uFrame's lib. 

6. Building new uFrame's code: 
Open new uFrame's editor, click Build All. 

Good luck to you!
