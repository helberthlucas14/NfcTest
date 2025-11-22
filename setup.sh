#!/bin/bash
aws --endpoint-url=http://localstack:4566 s3api create-bucket --bucket my-dev-exports
# Mantém o container ativo (se for usado em entrypoint de container)
tail -f /dev/null
