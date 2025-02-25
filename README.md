# Transfer data from A to B

> "Extensible" C# Project to transfer tabluar data from a to b


## Pipeline Definition

```json
{
  "$schema": "../Schema/pipeline.schema.json",
  "source": [
    {
      "Type": "csv",
      "FilePath": "<folder>",
      "FilePrefix": "process",
      "Delimiter": ","
    }
  ],
  "target": [
    {
      "Type": "sql",
      "ConnectionName": "sql",
      "TableName": "data",
      "UpdateStrategy": "Append"
    }
  ]
}
```

## Pipeline with explicit mapping

```json 
{
  "$schema": "../Schema/pipeline.schema.json",
  "source": [
    {
      "Type": "sql",
      "ConnectionName": "sql",
      "TableName": "data",
      "BatchSize": 1000
    }
  ],
  "target": [
    {
      "Type": "csv",
      "FilePath": "filepath",
      "FilePrefix": "prefix",
      "Delimiter": ",",
      "UpdateStrategy": "ReplaceComplete"
    }
  ],
  "mapping": {
    "Mappings": [
      {
        "SourceName": "Column_1",
        "TargetColumn": "Column_One",
        "Transform": [
          "replace_ones"
        ]
      }
    ]
  },
  "transformer": [
    {
      "Type": "regex_replace",
      "Name": "replace_ones",
      "ReplaceRegex": "1",
      "SelectRegex": "One"
    }
  ]
}
```