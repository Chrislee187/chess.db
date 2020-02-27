# Chess.DB.WebAPI
A web api for my Chess Game database that acts as a template/reference for a RESTful API with Games, Players & Events as the Resources

## Overview

.NET Core has been moving forward very rapidly and I felt that I had gotten a little out of date and rusty with some of the core parts of getting an API setup from scratch and implementing a RESTful feature set to an enterprise standard. More and more stuff is either built-in to .NET/ASP.NET or available in a package and many processes are now more refined and no longer require custom code to be written.

Following with Pluralsite and Google to guide me, I am systematically going through the creation of a typical REST/Resource based WebAPI from scratch and filling in and/or updating/replacing my knowledge of ASP & EF Core features.

Depending on the type of endpoint, (posts, updates, item query, collection queries etc.) the following features are supported.

* Filtering
* Searching
* Pagination (pagination metadata is made available in the Response header)
* Ordering
* Standardised error and validation responses using the built-in ProblemDetails and ValidationProblemDetails objects. See also the [HTTP API Problem Details specification](https://tools.ietf.org/html/rfc7807).
* PATCH support with [JSON Patch](https://tools.ietf.org/html/rfc6902) using the built-in [JsonDocumentPatch](https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-3.0) object.
* JSON and XML input and output via Accept/Content-Type headers.
* Carefully considered usage of HTTP method types and response status-codes (POST vs PUT vs PATCH).
* Advanced model binding (for array/collection queries)

## Usage

By default the API is configured to use a sample inline SQLite database (SQLServer is also supported), found in the `.\SampleDatabase` sub-folder of this project.

Alternatively new data can be imported into an existing or new SQLite or SQLServer DB using the [PGN Importer tool](https://github.com/Chrislee187/chess.db/tree/master/src/chess.games.db.pgnimporter), see the importer's readme file for the one line sample DB build. A sample import file is also included.

Assuming a DB is available, the API can run in the standard .NET Core manner of `dotnet run` and the swagger interface can be found on [http://localhost:5000/swagger](http://localhost:5000/swagger) by default.

## TODOS

* Caching
* Logging
* API Integration tests (testing correct status codes for invalid data, unauthorised etc.)

