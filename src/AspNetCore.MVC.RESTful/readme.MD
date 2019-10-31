# RESTful

This project offers code and conventions that can be used to easily create a MVC Controller for a RESTful resource. 

Resources themseelves are accessed via Repository components with an EntityFrameworkResourceComponents implementation supplied.

Features include;

* .NET Data annotations used for model validation
* Support for JSON and XML input and output (`Accept` and `Content-type` headers)
* Filters
* Searches
* Order By multiple fields
* Pagination

Todos;

* Caching
* 'rel' links
* Test that expected status codes are returned for each condition in each end-point

# Restful Endpoints

RESTful controllers can easily support one or more of the endpoints specified below for single Resource.

RESTful controllers inherit from `ResourceControllerBase` and use its `ResourceXXXX` methods to implement each indivudal REST endpoint. Most endpoints can be implemented with just a single line of code in the controller action.

The reusability is implemented using generics and (AutoMapper profiles)(AutoMapper Conventions)  to map objects from Entities to Data Transfer Objects and QueryString parameters to objects that the RESTful layer can use to determine filtering, searching, pagination and sorting requirements.

The following endpoints are supported;

## `GET /{resource}` - `ResourcesGet()`

By default retrieves all entries for a given resource, supports filters, searching, pagination and ordering.

## Expected Mappings

* A mapping is expected to exist between the resource entity and a entity DTO object.
* By default the entity DTO is expected to be named `{resource}Dto`.

### Options

### Returns

* Ok (200) on success, body contains the resource array in request content format
* BadRequest (400) for invalid fields in the order by clause.
* Adds an X-Pagination header to successful responses containing pagination metadata (page size, number, next and previous links etc.) 

## `GET /{resource}/{id}` - `ResourceGet()`

Retrieves a single resource identified by the specified Id.

## Expected Mappings

* By default the entity DTO is expected to be named `{resource}Dto`.
* A mapping is expected to exist between the resource entity and a entity DTO object.
* A parameter object should exist named `Get{resource}sParameters`
* A bi-directional mapping should exist between the parameters object and `PaginationParameters`, `OrderByParameters`
* Resource specific filtering and searching parameter object should exist named `Get{resource}sFilters` & `Get{resource}sSearchQuery` this should both inherit from a `Query<{entity}> implementation.
* Bi-directional mappings should exist between the parameters object and Filters and SearchQuery objects.

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


