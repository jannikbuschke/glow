dotnet nuget locals all --clear

dotnet new -i Glow.App.Template::*

dotnet new glow-app --name MyApp
cd MyApp
npm install web