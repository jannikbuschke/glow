---
title: Quick Start
root: "/docs"
parents: ["Get Started"]
---

## Prerequisites

- .NET 6+: [.NET Core](https://dotnet.microsoft.com/download) (SDK)
- nodejs 16+: [Node.js](https://nodejs.org/en/download/)
- editor: [VS Code](https://code.visualstudio.com/), [Visual Studio](https://visualstudio.microsoft.com/), [Rider](https://www.jetbrains.com/rider/) or similar

## Create and initialize a project

From a commandline run

`dotnet new -i Glow.App.Template::*` to install the main template. In a terminal run the following commands, to create a folder, initialise the application with the template. Also run `npm install` to install the npm dependencies.

```
mkdir my-app
cd my-app
dotnet new glow-app --name MyApp
cd MyApp
npm install web
```

## Start developing

Open a terminal and navigate to the frontend folder to start the frontend dev server:

```
cd MyApp\web
npm run start
```

The frontend dev server is running and watching for changes.

In a second terminal start the backend:

```
cd MyApp
dotnet watch run
```

Now the frontend is running on a developmet server at localhost:3000, the backend is running at localhost 5001. During development , the frontend is served from its dev server. in production the frontend will be compiled and its files be served by the backend. To make development easiert und catch bugs, we will also access the frontend during development via the backend.

So in development you usually will never go to http://localhost:3000. just got to to `https://localhost:5001`

## Open the source code and start editing!

When editing frontend files, the webapp should refresh automatically. Editing the backend should result in restarting the backend.
