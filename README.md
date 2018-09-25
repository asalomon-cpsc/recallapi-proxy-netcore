## TO RUN WITH DOCKER

### Build
```bash
docker build -t recalls-microservice .
```

### Run
```

docker run -d -p 80:80 --name docker-recalls recalls-microservice

````

## TO HOST IN IIS



### Pre Req on Windows 2008R2 to 2016

#### Create IIS site:

1. On the hosting system, create a folder to contain the app's published folders and files. An app's deployment layout is described in the Directory Structure topic.

2. Within the new folder, create a logs folder to hold ASP.NET Core Module stdout logs when stdout logging is enabled. If the app is deployed with a logs folder in the payload, skip this step. For instructions on how to enable MSBuild to create the logs folder automatically when the project is built locally, see the Directory structure topic.

 Important

> Only use the stdout log to troubleshoot app startup failures. Never use stdout logging for routine app logging. 
> There's no limit on log file size or the number of log files created. The app pool must have write access to the 
> location where the logs are written. All of the folders on the path to the log location must exist. For more
> information on the stdout log, see Log creation and redirection. For information on logging in an ASP.NET Core app, > see the Logging    >  topic.

3. In IIS Manager, open the server's node in the Connections panel. Right-click the Sites folder. Select Add Website from the contextual menu.

4. Provide a Site name and set the Physical path to the app's deployment folder. Provide the Binding configuration and create the website by selecting OK:

5. Under the server's node, select Application Pools.

6. Right-click the site's app pool and select Basic Settings from the contextual menu.

7. In the Edit Application Pool window, set the .NET CLR version to No Managed Code:

#### Install .NET Core Runtime

1. Download [.NET Core Hosting Bundle](https://www.microsoft.com/net/download/thank-you/dotnet-runtime-2.1.4-windows-hosting-bundle-installer) 



2.  Install the dowloaded bundle, to prevent the installer from installing x86 packages on an x64 OS, run the installer from an administrator command prompt with the switch ```OPT_NO_X86=1```.


3. Restart the system or execute net stop was /y followed by net start w3svc from a command prompt. Restarting IIS picks up a change to the system PATH, which is an environment variable, made by the installer.


#### Deploy
4. Run `dotnet -publish -f netcoreapp2.0 -c release /verbosity:detailed` => should create a *publish* folder and the contents will be in the publish folder

5. This folder should be copied to IIS folder under wwwwroot\inetpub\{appName}

