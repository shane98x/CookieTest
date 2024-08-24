# Blazor WebAssembly + Web API (both .NET)
A Blazor WASM application consuming a Web Api using authentication with cookies and authorization middleware without the identity package. Powered by an InMemory database.

## Installation

Clone the repository using Git (or download the ZIP package):
```
git clone https://github.com/shane98x/CookieTest.git
```
Navigate to the folder (modify path if needed:
```
cd CookieTest
```
## Run

Restore the packages:
```
dotnet restore
```

Build the solution:
```
dotnet build
```

Run the API:
```
cd CookieTest.API
dotnet run
```

Run Blazor WASM in second terminal:
```
cd CookieTest.Blazor
dotnet run
```

## Information
The authorization middleware used in the Blazor WASM app is purely for manipulating the UI. This can easily be overriden by people with malicious intents.
Not an issue if they don't have valid credentials (cookie) to use said pages. 






