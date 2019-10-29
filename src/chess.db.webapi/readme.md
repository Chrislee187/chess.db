# Chess.DB.WebAPI
A web api for my Chess Game database that acts as a template/reference for a RESTful API with Games, Players & Events as the Resources

# Overview

.NET Core has been moving forward very rapidly and I felt that I had gotten a little out of date and rusty with some of the core parts of getting an API setup from scratch and implementing a RESTful feature set to an enterprise standard. More and more stuff is either built-in to .NET/ASP.NET or available in a package and many processes are now more refined and no longer require custom code to be written.

So, following some formal guides (I mainly used a combination of MSDN, Pluralsight and of course Stackoverflow) I am systematically going through the creation of a WebAPI from scratch and filling in and/or updating/replacing my knowledge of .NET Core/MVC/RESTful applications with enterprise level features.

Where appropriate the API supports;

* Filtering
* Searching
* Pagination (pagination metadata is made available in the Response headeer)
* Ordering
* Standardised error and validation responses using the built-in ProblemDetails and ValidationProblemDetails objects. See also the (HTTP API Problem Details specification)[https://tools.ietf.org/html/rfc7807].
* PATCH support with (JSON Patch)[https://tools.ietf.org/html/rfc6902] using the built-in (JsonDocumentPatch)[https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-3.0]
* JSON and XML input and output via Accept/Content-Type headers.
* Carefully considered usage of HTTP method types and response status-codes (POST vs PUT vs PATCH).
* Advanced model binding (for array/collection queries)

# TODOS

* Caching
* Logging
* API Integration tests (testing correct status codes for invalid data, unauthorised etc.)

