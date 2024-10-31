---
title: Using the Semantic Kernel JDBC Vector Store connector in Java (Experimental)
description: Contains information on how to use a Semantic Kernel Vector store connector to access and manipulate data in JDBC for Java.
zone_pivot_groups: programming-languages
author: milderhc
ms.topic: conceptual
ms.author: milderhc
ms.date: 10/11/2024
ms.service: semantic-kernel
---
# Using the JDBC Vector Store connector

::: zone pivot="programming-language-csharp"

## Overview

JDBC vector store is a Java-specific feature, available only for Java applications.

::: zone-end
::: zone pivot="programming-language-python"

## Overview

JDBC vector store is a Java-specific feature, available only for Java applications.

::: zone-end
::: zone pivot="programming-language-java"

## Overview

The JDBC Vector Store connector can be used to access and manage data in SQL databases. The connector has the following characteristics.

| Feature Area                      | Support                                                                                                                          |
|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| Collection maps to                | SQL database table
| Supported SQL data sources        | <ul><li>PostgreSQL</li><li>MySQL</li><li>SQLite</li><li>HSQLDB</li></ul>                                                                                                           |
| Supported key property types      | String                                                                                                                           |
| Supported data property types     | <ul><li>String</li><li>int, Integer</li><li>long, Long</li><li>double, Double</li><li>float, Float</li><li>boolean, Boolean</li><li>OffsetDateTime</li></ul> |
| Supported vector property types   | List\<Float\>                                                                                                          |
| Supported index types             | <ul><li>PostgreSQL: Hnsw, IVFFlat, Flat</li><li>MySQL: Flat</li><li>SQLite: Flat</li><li>HSQLDB: Flat</li></ul>                                                                                              |
| Supported distance functions      | <ul><li>CosineDistance</li><li>DotProductSimilarity</li><li>EuclideanDistance</li></ul>                                        |
| Supports multiple vectors in a record | Yes                                                                                                                          |
| isFilterable supported?           | Yes                                                                                                                              |
| isFullTextSearchable supported?   | No                                                                                                                              |
| storageName supported?    | No, use `@JsonProperty` instead.               |

## Limitations

PostgreSQL leverages `pgvector` for vector indexing and search, uniquely offering approximate search capabilities. Other providers lack support for indexing, providing only exhaustive vector search.

## Getting started

Include the latest version of the Semantic Kernel JDBC connector in your Maven project by adding the following dependency to your `pom.xml`:

```xml
<dependency>
    <groupId>com.microsoft.semantic-kernel</groupId>
    <artifactId>semantickernel-data-jdbc</artifactId>
    <version>[LATEST]</version>
</dependency>
```

You can then create a vector store instance using the `JDBCVectorStore` class, having the data source as a parameter.

```java
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStore;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreOptions;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreRecordCollection;
import com.microsoft.semantickernel.data.jdbc.JDBCVectorStoreRecordCollectionOptions;
import com.microsoft.semantickernel.data.jdbc.mysql.MySQLVectorStoreQueryProvider;
import org.postgresql.ds.PGSimpleDataSource;

public class Main {
    public static void main(String[] args) {
        // Configure the data source
        PGSimpleDataSource dataSource = new PGSimpleDataSource();
        dataSource.setUrl("jdbc:postgresql://localhost:5432/sk");
        dataSource.setUser("postgres");
        dataSource.setPassword("root");

        // Build a query provider
        // Other available query providers are PostgreSQLVectorStoreQueryProvider, SQLiteVectorStoreQueryProvider
        // and HSQDBVectorStoreQueryProvider
        var queryProvider = MySQLVectorStoreQueryProvider.builder()
                .withDataSource(dataSource)
                .build();

        // Build a vector store
        var vectorStore = JDBCVectorStore.builder()
                .withDataSource(dataSource)
                .withOptions(JDBCVectorStoreOptions.builder()
                        .withQueryProvider(queryProvider)
                        .build())
                .build();
    }
}

```
You can also create a collection directly.

```java
var collection = new JDBCVectorStoreRecordCollection<>(
    dataSource,
    "skhotels",
    JDBCVectorStoreRecordCollectionOptions.<Hotel>builder()
        .withRecordClass(Hotel.class)
        .build()
);
```

::: zone-end