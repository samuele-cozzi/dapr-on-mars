# dapr-on-mars

## Topics

- Architecture Development

## Architecture Development

### 1. Code

Create app

```shell
dotnet new console --framework net6.0
```

then add dockerfile from vscode

### 2. Docker

Build Dockerfile & run Image

```shell
docker build -t samuelecozzi/rover .
docker run samuelecozzi/rover
```

### 3. Helm

Create helm chart

```shell
helm create rover
```

change the image repository

### 4. Skaffold

Create skaffold.yml

```shell
cd src

skaffold dev --port-forward --default-repo=docker.io/samuelecozzi

skaffold run --default-repo=docker.io/samuelecozzi

```

## Prerequisites

- [Dapr](https://docs.dapr.io/getting-started/)

