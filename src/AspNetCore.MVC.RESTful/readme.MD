# RESTful

This project offers code and conventions that can be used to easily create a MVC Controller for a RESTful resource. 

Resources themselves are accessed via Repository components with an EntityFrameworkResourceComponents implementation supplied.

Supports the following RESTful endpoints.

## HTTP GET /{collection}

Returns a collection of resources

## HTTP GET /{collection}/{id}

Returns a specific resource from a collection

## HTTP POST /{collection}

Adds a new resource to a collection (resource is contained in the BODY of the HttpRequest)

## HTTP PUT /{collection}/{id}

Upserts (creates if doesn't exist, updates otherwise) the specifed resource, (resource is contained in the BODY of the HttpRequest). Note that any fields not specified in the update resource data (in the BODY) will be set to it's default value

## HTTP PATCH /{collection}/{id}

Patches the specified document using .NET Cores JSON Patch Document support. Allows single fields to be updated without having to supply the other data. See (JSON Patch)[https://tools.ietf.org/html/rfc6902]

## HTTP DELETE /{collection}/{id}

Removes the specified document from the collection

# Other Features

* resource validation using .NET Data annotations and MVC model validation
* Support for JSON and XML input and output (`Accept` and `Content-type` headers)
* Filters
* Searches
* Order By multiple fields
* Data-shaping (customising output fields displayed)
* Pagination
* Hateoas Link support
* Standardised error handling using the proposed (Problem Details for HTTP APIs)[https://tools.ietf.org/html/rfc7807] schema available using the (ProblemDetails)[https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails?view=aspnetcore-3.0] class.

Configurable features are configured using query-string parameters (i.e. `?page-size=20&page-number=1&orderby=lastname desc, firstname&shape=id,firstname,lastname`) handled at an ActionFilter level and not needed to be exposed as arguments on the actual controller action.

Todos

* Caching
* Tests
* Swagger


# Implementing RESTful collection endpoints
RESTful allows you to implement a fully featured RESTful set of endpoints for a single entity in just a few steps. The controller can be implemented in just a few lines of code (less than 100, and still benicely formatted:)

Implementing a few lines of code for each of the following steps will give a fully featured set of endpoints that support creation, edit, delete and selection 
 of a single resource type with support for filtering, searching, ordering, pagination and customising the output (data-shapeing).

* Create bi-directional AutoMapper mappings from your Entity to and from your Resource DTO representation
	* Create additional mappings for Creation and Update DTO. These only require one-way mapping to the ENTITY.
* Create ResourceFilter and ResourceSearch implementations for your Entity
* Create ResourcesFilters object to contain filter values
	* Create bi-direction mapping between your ResourceFilters parameters object and ResourceFilter implementation
* Create a controller that inherits from `ResourceControllerBase<DTO, ENTITY>`
	* Implement any of the standard endpoints required delegating to the relevant `Resource` method found on `ResourceControllerBase<DTO, ENTITY>`
	* MVC `RouteNames` are used extensively to dynmically build URI's for HATEoas links  error . A RouteName should be set for each endpoint, RESTful assumes default RouteNames based on the ENTITY name, these can be over-ridden using `HateoasConfig` (see below TODO)

* Add `app.RestfulExceptionHandling(env)` & `app.UseRestful(env)` to your app startup configuration

* Add `services.AddRestful()` to your app startup services

* (Optional) Add `IOrderByPropertyMappingService`'s for any entities that have differences between resource and entity fields names or the resource value is derived from multiple field values, or the ordering between the resource and entity may be conceptually different (i.e. ordering by Age on a resource may require ordering by a DateTime in reverse order on the entity)

* (Optional) Use the `RestfulAutoMapperConventionsChecker` to check that required mappings are registered at startup

# Restful Endpoints

RESTful controllers can easily support one or more of the endpoints specified below for single Resource.

RESTful controllers inherit from `ResourceControllerBase` and use its `ResourceXXXX` methods to implement each indivudal REST endpoint. Most endpoints can be implemented with just a single line of code in the controller action.

The reusability is implemented using generics and (AutoMapper profiles)(AutoMapper Conventions)  to map objects from Entities to Data Transfer Objects and QueryString parameters to objects that the RESTful layer can use to determine filtering, searching, pagination and sorting requirements.

The following endpoints are supported;

## `GET /{resource}` - `ResourcesGet()`

By default retrieves all entries for a given resource, supports filters, searching, pagination and ordering.

## Expected Mappings

* A mapping is expected to exist to and from the entity and a resource DTO object.
* By default the entity DTO is expected to be named `{resource}Dto`.

### Options

* TODO: docs on filters, search, pagination, orderby etc. 

### Returns

* Ok (200) on success, body contains the resource array in request content format
* BadRequest (400) for invalid fields in the order by clause.
* Adds an X-Pagination header to successful responses containing pagination metadata (page size, number, next and previous links etc.) 

## `GET /{resource}/{id}` - `ResourceGet()`

Retrieves a single resource identified by the specified Id.

## Expected Mappings

* A mapping is expected to exist between the resource entity and a entity DTO object.
* By default the entity DTO is expected to be named `{resource}Dto`.
* A filter object inheriting from ResourceFilter should exist named `Get{resource}sResourceFilter`
* A search object implementing IResouceSearch should exist named `Get{resource}sSearch`

### Returns

* Ok (200) - body contains the resource in request content format.
* NotFound (404) - resource with specified Id could not be found.

## `POST /{resource}` - `ResourceCreate()`

Body contains a new resource (in JSON format by default). 

## Expected Mappings

* A mapping is expected to exist between the resource entity and a Creation DTO object.
* By default the Creation DTO is expected to be named `{resource}CreationDto`.

### Returns

* Created (201) - new resource is created and a `Location` header is added to the response, containing Uri of created resource

## `PUT /{resource}/{id}` - `ResourceUpsert()`

Replaces the specified resource with the one supplied in the body. Creates a new resource if the one specified doesn't exist.

Note. Any fields not supplied in the body, will be set to null in the modified/new resource

## Expected Mappings

* A mapping is expected to exist between the resource entity and a Update DTO object.
* By default the Creation DTO is expected to be named `{resource}UpdateDto`.

### Returns

* NoContent (204) - resource successfully modified
* Created (201) - new resource is created and a `Location` header is added to the response, containing Uri of created resource

## `PATCH /{resource}/{id}` - `ResourcePatch()`

Updates the specified resource using the JsonPatchDocument supplied in the body

## Expected Mappings

* A mapping is expected to exist between the resource entity and a Update DTO object.
* By default the Creation DTO is expected to be named `{resource}UpdateDto`.

### Returns

* NotFound (404) - specified resource doesn't not exist
* UnprocessableEntity (422) - patch changes failed model validation
* NoContent (204) - resource successfully modified

## `DELETE /{resource}/{id}` 

Removes the specified resource from the collection.

## Expected Mappings

* None required.

### Returns

* NotFound (404) - specified resource doesn't not exist
* NoContent (204) - resource successfully deleted

# AutoMapper Conventions

Extensive use is map of generics and AutoMapper to make the code as simple to use as possible.

Aside from standard mappings from Entities to Dto/Model objects we also use mappings to abstract querystring parameters to known objects that can used by RESTful to implement filtering, searching, sorting and pagination.

The individual end points described above each detail their expected mappings (most of which are very simple if you use the same names for entity and DTO fields).

A class to help check the required mappings also exists named `AutoMapperConventionsChecker`. This can be called specifing a resource (entity) and which endpoints are to be supported and it will throw an exception with details of any mappings missing for the required endpoints.


# Example RESTful controller