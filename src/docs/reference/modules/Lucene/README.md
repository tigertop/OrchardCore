# Lucene (`OrchardCore.Search.Lucene`)

The Lucene module allows you to manage Lucene indexes.

## Recipe step

Lucene indexes can be created during recipe execution using the `lucene-index` step.  
Here is a sample step:

```json
{
  "steps":[
    {
      "name":"lucene-index",
      "Indices": [
        {
          "Search": {
            "AnalyzerName": "standard",
            "IndexLatest": false,
            "IndexedContentTypes": [
              "Article",
              "BlogPost"
            ]
          }
        }
      ]
    }
  ]
}
```

!!! note
    It's recommended to use the `CreateOrUpdateIndexProfile` recipe step instead as the `lucene-index` step is obsolete. 

### Queries recipe step

Here is an example for creating a Lucene query from a Queries recipe step:

```json
{
  "steps": [
    {
      "Source": "Lucene",
      "Name": "RecentBlogPosts",
      "Index": "Search",
      "Template":"...", // JSON encoded query template.
      "ReturnContentItems": true
    }
  ]
}
```

## Web APIs

### `api/lucene/content`

Executes a query with the specified name and returns the corresponding content items.

Verbs: `POST` and `GET`

| Parameter    | Example                                        | Description                                             |
|--------------|------------------------------------------------|---------------------------------------------------------|
| `indexName`  | `search`                                       | The name of the index to query.                         |
| `query`      | `{ "query": { "match_all": {} }, "size": 10 }` | A JSON object representing the query.                   |
| `parameters` | `{ size: 3}`                                   | A JSON object representing the parameters of the query. |

### `api/lucene/documents`

Executes a query with the specified name and returns the corresponding Lucene documents.
Only the stored fields are returned.

Verbs: `POST` and `GET`

| Parameter    | Example                                        | Description                                             |
|--------------|------------------------------------------------|---------------------------------------------------------|
| `indexName`  | `search`                                       | The name of the index to query.                         |
| `query`      | `{ "query": { "match_all": {} }, "size": 10 }` | A JSON object representing the query.                   |
| `parameters` | `{ size: 3}`                                   | A JSON object representing the parameters of the query. |

## Lucene Worker (`OrchardCore.Search.Lucene.Worker`)

This feature creates a background task that will keep the local file system index synchronized with
other instances that could have their own local index.  
It is recommended to use it only if you are running the same tenant on multiple instances (farm) and are using a Lucene file system index.

If you are running on Azure App Services or if you are using Elasticsearch, then you don't need this feature.

## Lucene Queries

The Lucene module provides a management UI and APIs for querying Lucene data using Elasticsearch Queries.
See: <https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl.html>

## Indexing custom data

The indexing module supports multiple sources for indexing. This allows you to create indexes based on different data sources, such as content items or custom data.

To register a new source, you can add the following code to your `Startup.cs` file:

```csharp
services.AddLuceneIndexingSource("CustomSource", o =>
{
    o.DisplayName = S["Custom Source in Provider"];
    o.Description = S["Create a Provider index based on custom source."];
});
```

## Recipe step

Lucene indices can be created during recipe execution using the `LuceneIndexSettings` step.  
Here is a sample step:

```json
{
  "steps":[
    {
      "name":"LuceneIndexSettings",
      "Indices":[
        {
          "Search":{
            "AnalyzerName":"standardanalyzer",
            "IndexLatest":false,
            "Culture":"",
            "StoreSourceData":false,
            "IndexedContentTypes":[
              "Article",
              "BlogPost"
            ]
          }
        }
      ]
    }
  ]
}
```

!!! note
    It's recommended to use the `CreateOrUpdateIndexProfile` recipe step instead as the `LuceneIndexSettings` step is obsolete. 

Here is an example of how to create `Lucene` index profile using the `IndexProfile` for Content items.

```json
{
  "steps":[
    {
      "name":"CreateOrUpdateIndexProfile",
      "indexes": [
	    {
		    "Name": "BlogPostsLucene",
            "IndexName": "blogposts",
		    "ProviderName": "Lucene",
		    "Type": "Content",
		    "Properties": {
			    "ContentIndexMetadata": {
				    "IndexLatest": false,
				    "IndexedContentTypes": ["BlogPosts"],
				    "Culture": "any"
			    },
                "LuceneIndexMetadata": {
                    "AnalyzerName": "standard",
                    "StoreSourceData": true,
                }
		    }
	    }
      ]
    }
  ]
}
```

### Reset Lucene Index Step

This Reset Index Step resets an Lucene index.
Restarts the indexing process from the beginning in order to update current content items.
It doesn't delete existing entries from the index.

```json
{
  "steps":[
    {
      "name":"lucene-index-reset",
      "Indices":[
        "IndexName1",
        "IndexName2"
      ]
    }
  ]
}
```

To reset all indices:

```json
{
  "steps":[
    {
      "name":"lucene-index-reset",
      "IncludeAll":true
    }
  ]
}
```

!!! note
    It's recommended to use the `ResetIndex` recipe step instead as the `lucene-index-reset` step is obsolete. 

### Rebuild Lucene Index Step

This Rebuild Index Step rebuilds an Lucene index.
Deletes and recreates the full index content.

```json
{
  "steps":[
    {
      "name":"lucene-index-rebuild",
      "Indices":[
        "IndexName1",
        "IndexName2"
      ]
    }
  ]
}
```

To rebuild all indices:

```json
{
  "steps":[
    {
      "name":"lucene-index-rebuild",
      "IncludeAll":true
    }
  ]
}
```

!!! note
    It's recommended to use the `RebuildIndex` recipe step instead as the `lucene-index-rebuild` step is obsolete. 

### Query Filters

Query filters are used to retrieve records from Lucene without taking care of the boost values on them. So, it is retrieving records just like a SQL database would do.

Here is an example of a filtered query:

```json
{
  "query":{
    "bool":{
      "filter":[
        {
          "term":{
            "Content.ContentItem.Published":"true"
          }
        },
        {
          "wildcard":{
            "Content.ContentItem.DisplayText":"Main*"
          }
        }
      ]
    }
  }
}
```

With a must query in the bool Query. "finding specific content type(s)"

```json
{
  "query":{
    "bool":{
      "must":{
        "term":{
          "Content.ContentItem.ContentType.keyword":"Menu"
        }
      },
      "filter":[
        {
          "term":{
            "Content.ContentItem.Published":"true"
          }
        },
        {
          "wildcard":{
            "Content.ContentItem.DisplayText":"Main*"
          }
        }
      ]
    }
  }
}
```

Using the [`query_string` Lucene query](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html) with the [Query Parser Syntax](https://lucene.apache.org/core/2_9_4/queryparsersyntax.html) (with syntax like `"exact match"` and `should AND contain`):

```json
{
    "query": {
        "query_string": {
            "query": "Content.ContentItem.FullText:\"exploration\""
        }
    }
}
```

Or in a way that you don't have to select the fields in the query (to allow users to do simpler search):

```json
{
  "query":{
    "query_string":{
      "query":"\"exploration\"",
      "default_field":"Content.ContentItem.FullText"
    }
  }
}
```

An alternative to the previous one with [`simple_query_string`](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-simple-query-string-query.html):

```json
{
  "query":{
    "simple_query_string":{
      "query":"\"exploration\"",
      "fields":[
        "Content.ContentItem.FullText"
      ]
    }
  }
}
```

As you can see it allows to filter on multiple query types. All of the Query types that are available in Lucene are also filters.

So you can use:

- `bool`
- `geo_distance`
- `geo_bounding_box`
- `fuzzy`
- `match`
- `match_all`
- `match_phrase`
- `prefix`
- `query_string`
- `range`
- `regexp`
- `simple_query_string`
- `term`
- `terms`
- `wildcard`

See Elasticsearch documentation for more details:
<https://www.elastic.co/guide/en/elasticsearch/reference/current/query-filter-context.html>

## Automatic mapping

Starting from OC version 1.5 the Lucene module will automatically map text fields with a  `.keyword` suffix as a `stored` value in the index unless the document is already set to be `stored` explicitly. It will ignore any value that has a length higher than 256 chars. This way, any TextField can be used as a technical value and searched by using a term query.

## Video

<iframe width="560" height="315" src="https://www.youtube-nocookie.com/embed/9EgZ_J1npw4" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

<iframe width="560" height="315" src="https://www.youtube-nocookie.com/embed/6jJH9ntqi_A" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
