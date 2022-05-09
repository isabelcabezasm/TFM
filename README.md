# TFM

Data Science Master Final Project

## Graphs
https://isabelcabezasm.github.io/genderinequalityfilmindustry/

## Configuración

You can set the environment variables needed for running the application:

With the command "export" if you are in Linux:

```cmd
export Host="<nombre del servidor>.gremlin.cosmos.azure.com:443/"
```

or adding the section "env" in the launch.json file: 

```json
"env": {
    "Host": "<nombre del servidor>.gremlin.cosmos.azure.com:443/",
    "PrimaryKey": "<primary key de cosmos db>",
    "DatabaseName":"<nombre de la base de datos en cosmos db>",
    "ContainerName":"<nombre del contenedor en cosmos db>",
    "TMDBtoken":"<token>"
}
```

