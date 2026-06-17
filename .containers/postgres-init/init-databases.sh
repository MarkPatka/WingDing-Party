#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE DATABASE userdb;
    CREATE DATABASE authdb;
    CREATE DATABASE eventdb;
    CREATE DATABASE clubdb;
EOSQL
