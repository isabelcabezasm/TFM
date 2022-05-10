# TFM

Data Science Master Final Project

## Graphs
https://isabelcabezasm.github.io/genderinequalityfilmindustry/

## Configuración

You can set the environment variables needed for running the application:

With the command "export" if you are in Linux:

```cmd
export Host="<server name>.gremlin.cosmos.azure.com:443/"
```

or adding the section "env" in the launch.json file: 

```json
"env": {
    "Host": "<server name>.gremlin.cosmos.azure.com:443/",
    "PrimaryKey": "<primary key - cosmos db>",
    "DatabaseName":"<db anme of cosmos db>",
    "ContainerName":"<container name of cosmos db>",
    "TMDBtoken":"<token>"
}
```

