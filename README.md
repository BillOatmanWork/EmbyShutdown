# EmbyShutdown
Application that waits for there to be no active users on the Emby server, then turns off Emby then the server.
<br/> <br/>
EmbyShutdown API_KEY
<br/>
To get Emby api key, as an admin in Emby go to dashboard > advanced > security and generate one
<br/> <br/>
<i>This program needs to be run on the Emby Server.</i>
<br/> <br/>
Logic Flow:<br/>
If one or more active users, wait for 10 minutes then check again <br/>
If no active users, then wait 5 minutes and check again, else wait for 10 minutes and check again. <br/>
If still no users after the additional 5 minutes, then shutdown Emby then the server.<br/>
